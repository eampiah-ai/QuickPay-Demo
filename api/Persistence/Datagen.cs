namespace api.Persistence;

using api.Domain;

public class Datagen(CustomerDb customerDb)
{
    public void GenerateCustomers(int count = 10)
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
}