using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DevCamper.Data;
using DevCamper.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevCamper.Models;
using System.IO;
using Slugify;

namespace DevCamper.Areas.Bootcamp.Controllers
{
    [Area("Bootcamp")]
    public class BootcampsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private SlugHelper slugHelper;

        public BootcampsController(ApplicationDbContext context)
        {
            _context = context;
            slugHelper = new SlugHelper();
        }

        [HttpGet]
        [Route("bootcamps")]
        public async Task<IActionResult> Index(int budget = 0, int rating = 0)
        {
            List<Models.Bootcamp> bootcamps = new List<Models.Bootcamp>();

            if (budget == 0 && rating == 0)
                bootcamps = await _context.Bootcamps.Include(b => b.Career).ToListAsync();
            if (budget != 0 || rating != 0)
            {
                bootcamps = await filterBootcamps(budget, rating);
            }

            return View(bootcamps);
        }

        private async Task<List<Models.Bootcamp>> filterBootcamps(int budget, int rating)
        {
            var bootcamps = new List<Models.Bootcamp>();

            var bootcampsWithBudget = new List<Models.Bootcamp>();
            var bootcampsFromDb = await _context.Bootcamps.Include(b => b.Career).ToListAsync();

            //Filter by budget
            if (budget != 0)
            {
                double average = 0;
                foreach (var bootcamp in bootcampsFromDb)
                {
                    var courses = await _context.Courses.Where(c => c.BootcampId == bootcamp.Id).ToListAsync();
                    if (courses.Count > 0)
                    {
                        average = courses.Average(c => c.Tuition);
                        if (average <= budget)
                            bootcampsWithBudget.Add(bootcamp);
                    }
                }
                bootcamps.AddRange(bootcampsWithBudget);
            }

            //Filter by rating
            if (rating != 0)
            {
                List<Models.Bootcamp> filteredBootcamps = new List<Models.Bootcamp>();

                //If there is already a list with bootcamps filtered by budget, copy that list into a new list and filter again using this new list
                if (bootcampsWithBudget.Count > 0)
                {
                    filteredBootcamps.AddRange(bootcampsWithBudget);
                    foreach (var bootcamp in bootcampsWithBudget)
                    {
                        var average = await _context.Reviews.Where(r => r.BootcampId == bootcamp.Id).AverageAsync(r => (int?)r.Rating);
                        if (average != null && average >= rating)
                        {
                            if (!filteredBootcamps.Contains(bootcamp)) filteredBootcamps.Add(bootcamp);
                        }
                        else
                        {
                            if (filteredBootcamps.Contains(bootcamp)) filteredBootcamps.Remove(bootcamp);
                        }
                    }
                    bootcamps.Clear();
                    bootcamps.AddRange(filteredBootcamps);
                }
                else //There is no list already filtere by budget, just take items from db and filter
                {
                    foreach (var bootcamp in bootcampsFromDb)
                    {
                        var average = await _context.Reviews.Where(r => r.BootcampId == bootcamp.Id).AverageAsync(r => (int?)r.Rating);
                        if (average != null && average >= rating) filteredBootcamps.Add(bootcamp);
                    }
                    bootcamps.AddRange(filteredBootcamps);
                }
            }

            return bootcamps;
        }

        [Route("bootcamps/{slug}/details")]
        public async Task<IActionResult> Details(string slug)
        {
            if (slug == null) return NotFound();

            var bootcamp = await _context.Bootcamps.Include(b => b.Career).FirstOrDefaultAsync(b => b.Slug == slug);

            if (bootcamp == null) return NotFound();

            var courses = await _context.Courses.Include(c => c.Skill).Where(c => c.BootcampId == bootcamp.Id).ToListAsync();

            var average = await _context.Reviews.Where(r => r.BootcampId == bootcamp.Id).AverageAsync(r => (int?)r.Rating);

            BootcampViewModel viewModel = new BootcampViewModel()
            {
                Bootcamp = bootcamp,
                Courses = courses,
                BootcampAverage = average ?? 0
            };

            return View(viewModel);
        }

        [Route("bootcamp/manage")]
        public async Task<IActionResult> Manage()
        {
            var claimIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var userId = claim.Value;
            var bootcamp = await _context.Bootcamps.Include(b => b.ApplicationUser).Include(b => b.Career).SingleOrDefaultAsync(b => b.UserId == userId);

            return View(bootcamp);
        }

        [HttpGet]
        [Route("bootcamp/create")]
        public async Task<IActionResult> Create()
        {
            CreateEditBootcampViewModel viewModel = new CreateEditBootcampViewModel()
            {
                Bootcamp = new Models.Bootcamp(),
                Careers = await _context.Careers.ToListAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("bootcamp/create")]
        public async Task<IActionResult> Create(CreateEditBootcampViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var claimIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

                var userId = claim.Value;
                viewModel.Bootcamp.UserId = userId;
                viewModel.Bootcamp.Slug = slugHelper.GenerateSlug(viewModel.Bootcamp.Name);

                await _context.Bootcamps.AddAsync(viewModel.Bootcamp);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Manage));
            }

            viewModel.Careers = await _context.Careers.ToListAsync();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("bootcamp/loadImage")]
        public async Task<IActionResult> LoadImage(Models.Bootcamp bootcamp)
        {
            var files = HttpContext.Request.Form.Files;

            if (files.Count > 0)
            {
                byte[] picture = null;
                using (var fs = files[0].OpenReadStream())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        fs.CopyTo(ms);
                        picture = ms.ToArray();
                    }
                }
                var bootcampFromDb = await _context.Bootcamps.FindAsync(bootcamp.Id);

                if (bootcampFromDb == null) return RedirectToAction(nameof(Manage));

                bootcampFromDb.Picture = picture;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Manage));
        }

        [Route("bootcamp/edit/{slug}")]
        public async Task<IActionResult> Edit(string slug)
        {
            if (slug == null) return NotFound();

            var bootCampFromDb = await _context.Bootcamps.Include(b => b.Career).FirstOrDefaultAsync(b => b.Slug == slug);

            if (bootCampFromDb == null) return NotFound();

            CreateEditBootcampViewModel viewModel = new CreateEditBootcampViewModel()
            {
                Bootcamp = bootCampFromDb,
                Careers = await _context.Careers.ToListAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("bootcamp/update")]
        public async Task<IActionResult> Update(CreateEditBootcampViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var bootcampFromDb = await _context.Bootcamps.Include(b => b.Career).FirstOrDefaultAsync(b => b.Slug == viewModel.Bootcamp.Slug);
                if (bootcampFromDb == null) return NotFound();

                bootcampFromDb.Name = viewModel.Bootcamp.Name;
                bootcampFromDb.Slug = slugHelper.GenerateSlug(viewModel.Bootcamp.Name);
                bootcampFromDb.Address = viewModel.Bootcamp.Address;
                bootcampFromDb.Phone = viewModel.Bootcamp.Phone;
                bootcampFromDb.Email = viewModel.Bootcamp.Email;
                bootcampFromDb.Website = viewModel.Bootcamp.Website;
                bootcampFromDb.Description = viewModel.Bootcamp.Description;
                bootcampFromDb.CareerId = viewModel.Bootcamp.CareerId;
                bootcampFromDb.Housing = viewModel.Bootcamp.Housing;
                bootcampFromDb.JobAssistance = viewModel.Bootcamp.JobAssistance;
                bootcampFromDb.JobGuarantee = viewModel.Bootcamp.JobGuarantee;
                bootcampFromDb.AcceptGi = viewModel.Bootcamp.AcceptGi;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Manage));
            }

            viewModel.Careers = await _context.Careers.ToListAsync();

            return View(viewModel);
        }


        [Route("bootcamp/remove/{slug}")]
        public async Task<IActionResult> Remove(string slug)
        {
            if (slug == null) return NotFound();

            var bootCampFromDb = await _context.Bootcamps.Include(b => b.Career).FirstOrDefaultAsync(b => b.Slug == slug);

            if (bootCampFromDb == null) return NotFound();

            _context.Bootcamps.Remove(bootCampFromDb);
            await _context.SaveChangesAsync();

            return PartialView("_AddBootcampPartial");
        }

        [HttpGet]
        [Route("bootcamp/{slug}/reviews")]
        public async Task<IActionResult> Reviews(string slug)
        {
            if (slug == null) return NotFound();

            var bootCampFromDb = await _context.Bootcamps.Include(b => b.Career).FirstOrDefaultAsync(b => b.Slug == slug);

            if (bootCampFromDb == null) return NotFound();

            var reviews = await _context.Reviews.Include(r => r.Bootcamp).Include(r => r.ApplicationUser).Where(r => r.BootcampId == bootCampFromDb.Id).ToListAsync();

            IndexBootcampReviewsViewModel viewModel = new IndexBootcampReviewsViewModel()
            {
                Reviews = reviews,
                Slug = bootCampFromDb.Slug,
                BootcampTitle = bootCampFromDb.Name
            };

            return View(viewModel);
        }

        [Route("bootcamp/{slug}/reviews/create")]
        public async Task<IActionResult> CreateReview(string slug)
        {
            if (slug == null) return NotFound();

            var bootCampFromDb = await _context.Bootcamps.Include(b => b.Career).FirstOrDefaultAsync(b => b.Slug == slug);

            if (bootCampFromDb == null) return NotFound();

            CreateEditReviewViewModel viewModel = new CreateEditReviewViewModel()
            {
                Review = new Review(),
                Slug = bootCampFromDb.Slug,
                BootcampTitle = bootCampFromDb.Name
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("bootcamp/{slug}/reviews")]
        public async Task<IActionResult> InsertReview(string slug, CreateEditReviewViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (slug == null)
                {
                    ModelState.AddModelError(string.Empty, "Error: Bootcamp not found. Try again");
                    return View("CreateReview", viewModel);
                }

                var bootCampFromDb = await _context.Bootcamps.FirstOrDefaultAsync(b => b.Slug == slug);

                if (bootCampFromDb == null)
                {
                    ModelState.AddModelError(string.Empty, "Error: Bootcamp not found. Try again");
                    return View("CreateReview", viewModel);
                }

                var claimIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

                var userId = claim.Value;
                viewModel.Review.BootcampId = bootCampFromDb.Id;
                viewModel.Review.UserId = userId;

                await _context.Reviews.AddAsync(viewModel.Review);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Bootcamps", new { slug = bootCampFromDb.Slug });
            }

            return View("CreateReview", viewModel);
        }
    }
}