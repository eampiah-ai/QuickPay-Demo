namespace api.Persistence;

using api.Domain;
using Microsoft.EntityFrameworkCore;

public class InvoiceDb(DbContextOptions<InvoiceDb> options) : DbContext(options)
{
    public DbSet<Invoice> Invoices => Set<Invoice>();
}