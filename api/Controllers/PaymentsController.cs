using api.Domain;
using api.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController(IOptions<StripeConfig> stripeConfig, InvoiceDb invoiceDb, PaymentDb paymentDb) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        return Ok(await paymentDb.Payments.ToListAsync());
    }

    [HttpGet("{invoiceId}/create-checkout-session")]
    public async Task<ActionResult> MakePayment(string invoiceId)
    {
        var invoice = await invoiceDb.Invoices
            .Include(i => i.Customer)
            .SingleOrDefaultAsync(i => string.Equals(i.PublicId, invoiceId));

        if (invoice is null)
            return NotFound();

        StripeConfiguration.ApiKey = stripeConfig.Value.SecretKey;

        var domain = "http://localhost:5173";

        var options = new SessionCreateOptions
        {
            Mode = "payment",
            SuccessUrl = $"{domain}/dashboard?toast=success&message=Invoice+processed+successfully",
            CancelUrl = $"{domain}/dashboard?toast=error&message=Unable+to+process+invoice",
            LineItems =
            [
                new()
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = invoice.AmountCents,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"Invoice #{invoice.PublicId}"
                        }
                    },
                    Quantity = 1
                }
            ],
            Metadata = new Dictionary<string, string>
            {
                ["invoiceId"] = invoice.PublicId
            },
            CustomerEmail = invoice.Customer.Email
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);

        return Ok(session.Url);
    }

    [HttpPost("webhook")]
    public async Task<ActionResult> StripeWebook()
    {
        var stripeSignature = Request.Headers["Stripe-Signature"];
        Event stripeEvent;
        try
        {
            using var reader = new StreamReader(Request.Body);
            var json = await reader.ReadToEndAsync();
            stripeEvent = EventUtility.ConstructEvent(
                json,
                stripeSignature,
                stripeConfig.Value.WebhookSecret
            );
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }

        if (string.Equals(stripeEvent.Type, "checkout.session.completed", StringComparison.OrdinalIgnoreCase))
        {
            var session = stripeEvent.Data.Object as Session;
            if (session?.Metadata == null ||
                !session.Metadata.TryGetValue("invoiceId", out var invoiceId))
            {
                return Ok(); // nothing to do
            }

            var invoice = await invoiceDb.Invoices
            .Where(invoice => string.Equals(invoice.PublicId, invoiceId))
            .FirstOrDefaultAsync();

            if (invoice is null)
            {
                return Ok();
            }

            if (invoice.Status == Status.Paid)
            {
                return Ok();
            }

            var amountCents = (long?)session.AmountTotal ?? invoice.AmountCents;

            invoice.Status = Status.Paid;
            invoice.UpdatedAt = DateTime.UtcNow;

            paymentDb.Payments.Add(new Payment
            {
                Id = Guid.NewGuid().ToString(),
                InvoiceId = invoice.Id,
                AmountCents = (int)amountCents,
                StripeSessionId = session.Id,
                CreatedAt = DateTime.UtcNow
            });

            await paymentDb.SaveChangesAsync();

            invoiceDb.Invoices.Update(invoice);
            await invoiceDb.SaveChangesAsync();
        }

        return Ok();
    }

}