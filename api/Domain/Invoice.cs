namespace api.Domain;

public enum Status
{
    Draft,
    Sent,
    Paid,
    Overdue
}

public class Invoice
{
    public string Id { get; set; }
    public string CustomerId { get; set; }
    public int AmountCents { get; set; }
    public Status Status { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string PublicId { get; set; }
}

public class CreateInvoiceDto
{
    public string CustomerId { get; set; }
    public int AmountCents { get; set; }
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
}