using api.Domain;
using api.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController(CustomerDb db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Customer>>> GetAll()
    {
        return await db.Customers.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult> Create(Customer customer)
    {
        // explicitly set id
        customer.Id = Guid.NewGuid().ToString();
        db.Customers.Add(customer);
        await db.SaveChangesAsync();

        return Created($"/customers/{customer.Id}", customer);
    }

    [HttpPut("/{id}")]
    public async Task<ActionResult> Update(string id, Customer updatedCustomer)
    {
        if (!string.Equals(id, updatedCustomer.Id, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Mismatched Ids");
        }

        db.Customers.Update(updatedCustomer);
        await db.SaveChangesAsync();
        return Accepted();
    }
}