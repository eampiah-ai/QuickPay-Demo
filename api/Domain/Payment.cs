using System.Text.Json.Serialization;

namespace api.Domain
{
    public class Payment
    {
        public string Id { get; set; }
        public string InvoiceId { get; set; }
        public int AmountCents { get; set; }
        public DateTime CreatedAt { get; set; }
        public string StripeSessionId { get; set; }
    }
}