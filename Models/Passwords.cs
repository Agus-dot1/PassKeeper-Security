namespace PassKeeper.Models
{
    public class Passwords
    {
        public int Id { get; set; }
        public string? Icon { get; set; }
        public string? Name { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Category { get; set; }
        public string? Url { get; set; }
        public string? Notes { get; set; }
        public string? Strength { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModified { get; set; }

        public Passwords()
        {
            CreationDate = DateTime.Now;
            LastModified = DateTime.Now;
        }
    }
}
