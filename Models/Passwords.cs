namespace PassKeeper.Models
{
    public class Passwords
    {
        public required string Name { get; set; }
        public required string Password { get; set; }
        public string? Category { get; set; }
        public string? Url { get; set; }
        public string? Notes { get; set; }
        public string? Strength { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
