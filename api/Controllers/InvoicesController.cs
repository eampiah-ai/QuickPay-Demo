using api.Domain;
using api.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoicesController(InvoiceDb db, CustomerDb customerDb) : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetAll()
    {
        return await db.Invoices.Include(invoice => invoice.Customer).Select(invoice => new InvoiceDto
        {
            Id = invoice.Id,
            CustomerId = invoice.Customer.Id,
            AmountCents = invoice.AmountCents,
            Description = invoice.Description,
            DueDate = invoice.DueDate,
            PublicId = invoice.PublicId,
            Status = invoice.Status
        }).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<InvoiceDto>> Get(string id)
    {
        return await db.Invoices
        .Include(invoice => invoice.Customer)
        .Where(invoice => string.Equals(id, invoice.Id))
        .Select(invoice => new InvoiceDto
        {
            Id = invoice.Id,
            CustomerId = invoice.Customer.Id,
            AmountCents = invoice.AmountCents,
            Description = invoice.Description,
            DueDate = invoice.DueDate,
            PublicId = invoice.PublicId
        })
        .FirstOrDefaultAsync() is InvoiceDto invoice ? Ok(invoice) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreateInvoiceDto dto)
    {
        var customer = await customerDb.Customers.FirstOrDefaultAsync(customer => string.Equals(customer.Id, dto.CustomerId));

        if (customer == null)
        {
            return NotFound("Customer not found");
        }
        db.Entry(customer).State = EntityState.Unchanged;
        var invoice = new Invoice
        {
            Id = Guid.NewGuid().ToString(),
            Customer = customer,
            AmountCents = dto.AmountCents,
            DueDate = dto.DueDate,
            Description = dto.Description!,
            PublicId = InvoiceIdGenerator.GeneratePublicId()
        };
        db.Invoices.Add(invoice);
        await db.SaveChangesAsync();

        return Created($"/invoices/{invoice.Id}", invoice);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(string id, [FromBody] InvoiceDto dto)
    {
        if (!string.Equals(id, dto.Id, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Mismatched Ids");
        }
        var customer = await customerDb.Customers.FirstOrDefaultAsync(customer => string.Equals(customer.Id, dto.CustomerId));

        if (customer == null)
        {
            return NotFound("Customer not found");
        }

        var updatedInvoice = new Invoice
        {
            Id = id,
            Customer = customer,
            AmountCents = dto.AmountCents,
            DueDate = dto.DueDate,
            Description = dto.Description!,
            PublicId = dto.PublicId
        };

        db.Invoices.Update(updatedInvoice);
        await db.SaveChangesAsync();
        return Accepted();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        var entity = await db.Invoices.FindAsync(id);
        if (entity != null)
        {
            db.Invoices.Remove(entity);
            await db.SaveChangesAsync();
        }
        return Ok();
    }
}