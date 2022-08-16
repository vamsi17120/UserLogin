using System.ComponentModel.DataAnnotations;

namespace UserLogin.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string? Username { get; set; }
        public int Password { get; set; }
    }
}
