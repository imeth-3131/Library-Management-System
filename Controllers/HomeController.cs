using Library_Management_System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Library_Management_System.Models;
using System.Diagnostics;

namespace Library_Management_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(
            ILogger<HomeController> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Total books
            var totalBooks = await _context.Books.CountAsync();

            // Total members
            var totalMembers = await _context.Members.CountAsync();

            // Currently borrowed books
            var currentlyBorrowed = await _context.Borrowings
                .CountAsync(b => b.ReturnDate == null);

            // Returned books
            var returnedBooks = await _context.Borrowings
                .CountAsync(b => b.ReturnDate != null);

            // Overdue books
            var overdueBooks = await _context.Borrowings
                .CountAsync(b =>
                    b.ReturnDate == null &&
                    b.DueDate < DateTime.Today);

            // Available copies
            var availableBooks = await _context.Books
                .SumAsync(b => b.AvailableQuantity);

            // Send values to the View
            ViewBag.TotalBooks = totalBooks;
            ViewBag.TotalMembers = totalMembers;
            ViewBag.CurrentlyBorrowed = currentlyBorrowed;
            ViewBag.ReturnedBooks = returnedBooks;
            ViewBag.OverdueBooks = overdueBooks;
            ViewBag.AvailableBooks = availableBooks;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id
                    ?? HttpContext.TraceIdentifier
            });
        }
    }
}