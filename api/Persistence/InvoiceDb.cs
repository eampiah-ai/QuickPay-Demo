namespace api.Persistence;

using api.Domain;
using Microsoft.EntityFrameworkCore;

public class InvoiceDb(DbContextOptions<InvoiceDb> options) : DbContext(options)
{
    public DbSet<Invoice> Invoices => Set<Invoice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Invoice>()
            .HasIndex(i => i.PublicId)
            .IsUnique();
    }
}