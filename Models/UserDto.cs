namespace Calculator.Models
{
    public class UserDto
    { 
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool KeepLoggedIn { get; set; }
        public string newPassword { get; set; }
        public string confPassword { get; set; }
        public int TotalArticles { get; set; }
        public int TotalComponents { get; set; }
        public string token { get; set; }
        public bool existaccount { get; set; }
    }
}
