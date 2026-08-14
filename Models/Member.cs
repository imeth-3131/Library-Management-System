using System.ComponentModel.DataAnnotations;

namespace Library_Management_System.Models
{
    public class Member
    {
        public int MemberID { get; set; }

        [Required]
        public string FullName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public DateTime RegistrationDate { get; set; }
    }
}