using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DevCamper.Data;
using DevCamper.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevCamper.Areas.Reviews.Controllers
{
    [Area("Reviews")]
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("reviews")]
        public async Task<IActionResult> Index()
        {
            var claimIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var userId = claim.Value;
            var reviews = await _context.Reviews.Include(r => r.Bootcamp).Include(r => r.ApplicationUser).Where(r => r.UserId == userId).ToListAsync();

            return View(reviews);
        }

        [Route("reviews/{id}/edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var reviewFromDb = await _context.Reviews.Include(r => r.Bootcamp).FirstOrDefaultAsync(r => r.Id == id);

            if (reviewFromDb == null) return NotFound();

            return View(reviewFromDb);
        }

        [Route("reviews/{id}/update")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Review review)
        {
            if (id == null) return NotFound();
            var reviewFromDb = await _context.Reviews.Include(r => r.Bootcamp).FirstOrDefaultAsync(r => r.Id == id);

            if (reviewFromDb == null) return NotFound();

            if(ModelState.IsValid)
            {
                reviewFromDb.Title = review.Title;
                reviewFromDb.Rating = review.Rating;
                reviewFromDb.Text = review.Text;

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            review.Bootcamp = reviewFromDb.Bootcamp;
            return View(review);
        }

        public async Task<IActionResult> Remove(int? id)
        {
            if (id == null) return NotFound();
            var reviewFromDb = await _context.Reviews.Include(r => r.Bootcamp).FirstOrDefaultAsync(r => r.Id == id);

            if (reviewFromDb == null) return NotFound();

            _context.Reviews.Remove(reviewFromDb);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}