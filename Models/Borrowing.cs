using System.ComponentModel.DataAnnotations;

namespace Library_Management_System.Models
{
    public class Borrowing
    {
        [Key]
        public int BorrowID { get; set; }

        [Required]
        public int BookID { get; set; }

        [Required]
        public int MemberID { get; set; }

        [Required]
        public DateTime BorrowDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        // Navigation properties
        public Book? Book { get; set; }

        public Member? Member { get; set; }
    }
}