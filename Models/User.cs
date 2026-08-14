using System.ComponentModel.DataAnnotations;

namespace Library_Management_System.Models
{
    public class user
    {
        public int UserID { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}