namespace api.Persistence;

using api.Domain;
using Microsoft.EntityFrameworkCore;

public class CustomerDb(DbContextOptions<CustomerDb> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
}