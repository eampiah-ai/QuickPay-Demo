namespace api;

using System.Security.Cryptography;

/// <summary>
/// Utility class to generate a unique, public-facing invoice ID based on date/time.
/// </summary>
public static class InvoiceIdGenerator
{
    /// <summary>
    /// Generates a public invoice ID using a fixed prefix, a precise UTC timestamp,
    /// and a small random suffix to ensure high uniqueness.
    /// </summary>
    /// <returns>A unique, formatted invoice ID (e.g., INV-20251116141025789-a3b4).</returns>
    public static string GeneratePublicId()
    {
        const string prefix = "INV-";
        string timestamp = DateTime.UtcNow.ToString("yyyyMMdd");
        try
        {
            byte[] randomBytes = new byte[2]; 
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            string randomSuffix = BitConverter.ToString(randomBytes).Replace("-", "").ToLowerInvariant();
            return $"{prefix}{timestamp}-{randomSuffix}";
        }
        catch (Exception ex)
        {
            // Fallback in case the RNG fails (e.g., environment restrictions)
            Console.WriteLine($"Error generating random component: {ex.Message}. Falling back to timestamp only.");
            return $"{prefix}{timestamp}-FALLBACK-{Random.Shared.Next(10000)}";
        }
    }
}