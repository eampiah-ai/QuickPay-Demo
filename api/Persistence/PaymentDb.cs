namespace api.Persistence;

using api.Domain;
using Microsoft.EntityFrameworkCore;

public class PaymentDb(DbContextOptions<PaymentDb> options) : DbContext(options)
{
    public DbSet<Payment> Payments => Set<Payment>();
}