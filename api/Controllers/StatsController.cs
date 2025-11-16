using api.Domain;
using api.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatsController(CustomerDb customerDb, InvoiceDb invoiceDb) : ControllerBase
{
    [HttpGet()]
    public async Task<ActionResult> GetStats()
    {
        var stats = new List<StatsDto>
        {
            await GetNumberOfCustomers(),
            await GetInvoiceTotals(Status.Pending),
            await GetInvoiceTotals(Status.Sent),
            await GetInvoiceTotals(Status.Overdue),
            await GetInvoiceTotals(Status.Paid),
        };

        return Ok(stats);
    }

    private async Task<StatsDto> GetNumberOfCustomers()
    {

        var count = await customerDb.Customers.CountAsync();
        return new StatsDto
        {
            Title = "Number of customers",
            Value = count.ToString()
        };
    }

    private async Task<StatsDto> GetInvoiceTotals(Status status)
    {
        var amount = await invoiceDb.Invoices
        .Where(invoice => invoice.Status == status)
        .SumAsync(invoice => invoice.AmountCents);
        return new StatsDto
        {
            Title = string.Format("Total {0} invoice amount", status),
            Value = string.Format("${0:N0}", amount)
        };
    }

    public class StatsDto
    {
        public string Title { get; set; }
        public string Value { get; set; }
    }
}