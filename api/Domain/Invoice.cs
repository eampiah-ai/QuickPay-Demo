using System.Text.Json.Serialization;

namespace api.Domain
{

    public enum Status
    {
        Pending,
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
        public DateTime CreatedAt { get; set; }
        public string PublicId { get; set; }
    }
}