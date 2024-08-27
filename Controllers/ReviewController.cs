using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReviewSystem.Data;
using ReviewSystem.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ReviewSystem.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Review
        public async Task<IActionResult> Index(string searchString)
        {
            var reviews = from r in _context.Reviews.Include(r => r.User)
                          select r;

            if (!string.IsNullOrEmpty(searchString))
            {
                reviews = reviews.Where(r => r.User.Username.Contains(searchString));
            }

            return View(await reviews.ToListAsync());
        }

        // POST: Review/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string username, string email, int rating, string comment)
        {
            if (ModelState.IsValid)
            {
                // Check if user already exists
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username && u.Email == email);

                if (user == null)
                {
                    user = new User
                    {
                        Username = username,
                        Email = email
                    };
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                }

                var review = new Review
                {
                    UserId = user.UserId,
                    Rating = rating,
                    Comment = comment
                };

                _context.Add(review);
                await _context.SaveChangesAsync();

                // Redirect to the Index action (same page)
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        // GET: Review/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews.Include(r => r.User)
                                               .FirstOrDefaultAsync(m => m.ReviewId == id);

            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Review/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReviewId,UserId,Rating,Comment,CreatedAt")] Review review)
        {
            if (id != review.ReviewId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(review);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.ReviewId))
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
            return View(review);
        }

        private bool ReviewExists(int id)
        {
            return _context.Reviews.Any(e => e.ReviewId == id);
        }
        // GET: Review/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews.Include(r => r.User)
                                               .FirstOrDefaultAsync(m => m.ReviewId == id);

            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Review/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }




    }
}
