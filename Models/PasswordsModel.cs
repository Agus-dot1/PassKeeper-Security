namespace PassKeeper.Models
{
    public class PasswordsModel
    {
        public bool UsernameIsNotEmpty => !string.IsNullOrEmpty(Username);
        public string Id { get; set; }
        public Guid UserId { get; set; }
        public string? Icon { get; set; }
        public string? Name { get; set; } = string.Empty;
        public string? Username { get; set; }
        public string? Password { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? Url { get; set; }
        public string? Notes { get; set; }
        public string? Strength { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModified { get; set; }

        public PasswordsModel()
        {
            CreationDate = DateTime.Now;
            LastModified = DateTime.Now;
        }
    }
}
