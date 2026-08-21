
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Library_Management_System.Models;
using Library_Management_System.Data;

public class MembersController : Controller
{
    private readonly ApplicationDbContext _context;

    public MembersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: MEMBERS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Members.ToListAsync());
    }

    // GET: MEMBERS/Details/5
    public async Task<IActionResult> Details(int? memberid)
    {
        if (memberid == null)
        {
            return NotFound();
        }

        var member = await _context.Members
            .FirstOrDefaultAsync(m => m.MemberID == memberid);
        if (member == null)
        {
            return NotFound();
        }

        return View(member);
    }

    // GET: MEMBERS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: MEMBERS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("MemberID,FullName,Email,Phone,Address,RegistrationDate")] Member member)
    {
        if (ModelState.IsValid)
        {
            _context.Add(member);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(member);
    }

    // GET: MEMBERS/Edit/5
    public async Task<IActionResult> Edit(int? memberid)
    {
        if (memberid == null)
        {
            return NotFound();
        }

        var member = await _context.Members.FindAsync(memberid);
        if (member == null)
        {
            return NotFound();
        }
        return View(member);
    }

    // POST: MEMBERS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? memberid, [Bind("MemberID,FullName,Email,Phone,Address,RegistrationDate")] Member member)
    {
        if (memberid != member.MemberID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(member);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberExists(member.MemberID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(member);
    }

    // GET: MEMBERS/Delete/5
    public async Task<IActionResult> Delete(int? memberid)
    {
        if (memberid == null)
        {
            return NotFound();
        }

        var member = await _context.Members
            .FirstOrDefaultAsync(m => m.MemberID == memberid);
        if (member == null)
        {
            return NotFound();
        }

        return View(member);
    }

    // POST: MEMBERS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? memberid)
    {
        var member = await _context.Members.FindAsync(memberid);
        if (member != null)
        {
            _context.Members.Remove(member);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool MemberExists(int? memberid)
    {
        return _context.Members.Any(e => e.MemberID == memberid);
    }
}
