using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Library_Management_System.Models;
using Library_Management_System.Data;

namespace Library_Management_System.Controllers
{
    public class BorrowingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BorrowingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Borrowings
        public async Task<IActionResult> Index()
        {
            var borrowings = await _context.Borrowings
                .Include(b => b.Book)
                .Include(b => b.Member)
                .ToListAsync();

            return View(borrowings);
        }

        // GET: Borrowings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var borrowing = await _context.Borrowings
                .Include(b => b.Book)
                .Include(b => b.Member)
                .FirstOrDefaultAsync(b => b.BorrowID == id);

            if (borrowing == null)
            {
                return NotFound();
            }

            return View(borrowing);
        }

        // GET: Borrowings/Create
        public async Task<IActionResult> Create()
        {
            // Automatically set today's date
            // and a default due date of 7 days later
            var borrowing = new Borrowing
            {
                BorrowDate = DateTime.Today,
                DueDate = DateTime.Today.AddDays(7)
            };

            // Only show books that are currently available
            var availableBooks = await _context.Books
                .Where(b => b.AvailableQuantity > 0)
                .ToListAsync();

            // Get all members
            var members = await _context.Members
                .ToListAsync();

            ViewData["BookID"] = new SelectList(
                availableBooks,
                "BookID",
                "Title"
            );

            ViewData["MemberID"] = new SelectList(
                members,
                "MemberID",
                "FullName"
            );

            return View(borrowing);
        }

        // POST: Borrowings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("BorrowID,BookID,MemberID,BorrowDate,DueDate,ReturnDate")]
            Borrowing borrowing)
        {
            // Check validation
            if (!ModelState.IsValid)
            {
                // Reload books
                var availableBooks = await _context.Books
                    .Where(b => b.AvailableQuantity > 0)
                    .ToListAsync();

                // Reload members
                var members = await _context.Members
                    .ToListAsync();

                ViewData["BookID"] = new SelectList(
                    availableBooks,
                    "BookID",
                    "Title",
                    borrowing.BookID
                );

                ViewData["MemberID"] = new SelectList(
                    members,
                    "MemberID",
                    "FullName",
                    borrowing.MemberID
                );

                return View(borrowing);
            }

            // Find the selected book
            var book = await _context.Books
                .FirstOrDefaultAsync(b => b.BookID == borrowing.BookID);

            if (book == null)
            {
                ModelState.AddModelError(
                    "",
                    "Book not found."
                );
            }
            else if (book.AvailableQuantity <= 0)
            {
                ModelState.AddModelError(
                    "",
                    "This book is currently unavailable."
                );
            }
            else
            {
                // Reduce available quantity by 1
                book.AvailableQuantity--;

                // Add borrowing record
                _context.Borrowings.Add(borrowing);

                // Save both changes
                await _context.SaveChangesAsync();

                // Return to borrowing list
                return RedirectToAction(nameof(Index));
            }

            // Reload dropdowns if there is an error
            var books = await _context.Books
                .Where(b => b.AvailableQuantity > 0)
                .ToListAsync();

            var allMembers = await _context.Members
                .ToListAsync();

            ViewData["BookID"] = new SelectList(
                books,
                "BookID",
                "Title",
                borrowing.BookID
            );

            ViewData["MemberID"] = new SelectList(
                allMembers,
                "MemberID",
                "FullName",
                borrowing.MemberID
            );

            return View(borrowing);
        }

        // GET: Borrowings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var borrowing = await _context.Borrowings
                .FindAsync(id);

            if (borrowing == null)
            {
                return NotFound();
            }

            ViewData["BookID"] = new SelectList(
                _context.Books,
                "BookID",
                "Title",
                borrowing.BookID
            );

            ViewData["MemberID"] = new SelectList(
                _context.Members,
                "MemberID",
                "FullName",
                borrowing.MemberID
            );

            return View(borrowing);
        }

        // POST: Borrowings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("BorrowID,BookID,MemberID,BorrowDate,DueDate,ReturnDate")]
            Borrowing borrowing)
        {
            if (id != borrowing.BorrowID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(borrowing);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BorrowingExists(borrowing.BorrowID))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["BookID"] = new SelectList(
                _context.Books,
                "BookID",
                "Title",
                borrowing.BookID
            );

            ViewData["MemberID"] = new SelectList(
                _context.Members,
                "MemberID",
                "FullName",
                borrowing.MemberID
            );

            return View(borrowing);
        }

        // GET: Borrowings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var borrowing = await _context.Borrowings
                .Include(b => b.Book)
                .Include(b => b.Member)
                .FirstOrDefaultAsync(b => b.BorrowID == id);

            if (borrowing == null)
            {
                return NotFound();
            }

            return View(borrowing);
        }

        // POST: Borrowings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var borrowing = await _context.Borrowings
                .Include(b => b.Book)
                .FirstOrDefaultAsync(b => b.BorrowID == id);

            if (borrowing != null)
            {
                // If the book has not been returned,
                // make it available again
                if (borrowing.ReturnDate == null &&
                    borrowing.Book != null)
                {
                    borrowing.Book.AvailableQuantity++;
                }

                _context.Borrowings.Remove(borrowing);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Borrowings/Return/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(int id)
        {
            var borrowing = await _context.Borrowings
                .Include(b => b.Book)
                .FirstOrDefaultAsync(b => b.BorrowID == id);

            if (borrowing == null)
            {
                return NotFound();
            }

            // Only return if it hasn't already been returned
            if (borrowing.ReturnDate == null)
            {
                borrowing.ReturnDate = DateTime.Now;

                // Increase available quantity
                if (borrowing.Book != null)
                {
                    borrowing.Book.AvailableQuantity++;
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // Check whether borrowing exists
        private bool BorrowingExists(int id)
        {
            return _context.Borrowings
                .Any(e => e.BorrowID == id);
        }
    }
}