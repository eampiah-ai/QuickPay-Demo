using api.Domain;
using api.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoicesController(InvoiceDb db) : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Invoice>>> GetAll()
    {
        return await db.Invoices.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IEnumerable<Invoice>>> Get(string id)
    {
        return await db.Invoices.FindAsync(id) is Invoice invoice ? Ok(invoice) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult> Create(Invoice invoice)
    {
        // explicitly set id
        invoice.Id = Guid.NewGuid().ToString();
        db.Invoices.Add(invoice);
        await db.SaveChangesAsync();

        return Created($"/invoices/{invoice.Id}", invoice);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(string id, Invoice updatedInvoice)
    {
        if (!string.Equals(id, updatedInvoice.Id, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Mismatched Ids");
        }

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