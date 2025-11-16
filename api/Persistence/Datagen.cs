namespace api.Persistence;

using api.Domain;

public class Datagen(CustomerDb customerDb, InvoiceDb invoiceDb)
{
    public void GenerateCustomers(int count = 5)
    {
        var customers = Enumerable.Range(0, count).Select((val, index) =>
        {
            return new Customer
            {
                Id = Guid.NewGuid().ToString(),
                Name = $"Customer {index}",
                Email = $"customer{index}@example.com",
                CreatedAt = DateTime.Now.AddDays(-Random.Shared.Next(5))
            };
        });

        customerDb.Customers.AddRange(customers);
        customerDb.SaveChanges();
    }

    public void GenerateInvoices(int count = 3)
    {
        var customers = customerDb.Customers.ToList();
        if (customers.Count == 0) throw new Exception("Need to populate customer table prior to creating Invoices");

        var invoices = new List<Invoice>();
        foreach (var customer in customers)
        {
            for (var i = 0; i < count; i++)
            {
                var invoice = new Invoice()
                {
                    Id = Guid.NewGuid().ToString(),
                    CustomerId = customer.Id,
                    AmountCents = Random.Shared.Next(1000) * 10000,
                    Status = (Status)Enum.GetValues(typeof(Status)).GetValue(Random.Shared.Next(Enum.GetValues(typeof(Status)).Length)),
                    Description = "test description",
                    PublicId = Guid.NewGuid().ToString(),
                    DueDate = Random.Shared.Next(10) % 3 == 0 ? DateTime.Now.AddDays(Random.Shared.Next(100)) : DateTime.Now.AddDays(-Random.Shared.Next(100))
                };
                invoices.Add(invoice);
            }
        }

        invoiceDb.Invoices.AddRange(invoices);
        invoiceDb.SaveChanges();
    }
}