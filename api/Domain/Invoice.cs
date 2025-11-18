using System.Text.Json.Serialization;

namespace api.Domain;

public enum Status
{
    Pending,
    Sent,
    Paid,
    Overdue
}

public class Invoice
{
    public string Id { get; set; }
    public Customer Customer { get; set; }
    public int AmountCents { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Status Status { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public string PublicId { get; set; }
}

public class CreateInvoiceDto
{
    public string CustomerId { get; set; }
    public int AmountCents { get; set; }
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
}


public class InvoiceDto
{
    public string Id { get; set; }
    public string CustomerId { get; set; }
    public int AmountCents { get; set; }
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public string PublicId { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Status Status { get; set; }

}