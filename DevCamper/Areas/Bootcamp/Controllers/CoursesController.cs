using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DevCamper.Data;
using DevCamper.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Slugify;

namespace DevCamper.Areas.Bootcamp.Controllers
{
    [Area("Bootcamp")]
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private SlugHelper _slugHelper;

        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
            _slugHelper = new SlugHelper();
        }

        [Route("bootcamp/{slug}/courses")]
        public async Task<IActionResult> Index(string slug)
        {
            if (slug == null) return NotFound();

            var bootCampFromDb = await _context.Bootcamps.Include(b => b.Career).FirstOrDefaultAsync(b => b.Slug == slug);

            if (bootCampFromDb == null) return NotFound();

            var courses = await _context.Courses.Include(c => c.Skill).Where(c => c.BootcampId == bootCampFromDb.Id).ToListAsync();

            CoursesIndexViewModel viewModel = new CoursesIndexViewModel()
            {
                Bootcamp = bootCampFromDb,
                Courses = courses
            };

            return View(viewModel);
        }

        [Route("bootcamp/{slug}/courses/create")]
        public async Task<IActionResult> Create(string slug)
        {
            if (slug == null) return NotFound();

            var bootCampFromDb = await _context.Bootcamps.FirstOrDefaultAsync(b => b.Slug == slug);

            if (bootCampFromDb == null) return NotFound();

            CreateEditCourseViewModel viewModel = new CreateEditCourseViewModel()
            {
                Course = new Models.Course(),
                Skills = await _context.Skills.ToListAsync(),
                BootcampSlug = bootCampFromDb.Slug,
                BootcampTitle = bootCampFromDb.Name
            };

            return View(viewModel);
        }

        [Route("bootcamp/courses/insert")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Insert(CreateEditCourseViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var claimIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

                var userId = claim.Value;

                viewModel.Course.UserId = userId;
                var bootcampFromDb = await _context.Bootcamps.FirstOrDefaultAsync(b => b.Slug == viewModel.BootcampSlug);
                
                if (bootcampFromDb == null) return NotFound();

                viewModel.Course.BootcampId = bootcampFromDb.Id;
                viewModel.Course.Slug = _slugHelper.GenerateSlug(viewModel.Course.Title);

                await _context.Courses.AddAsync(viewModel.Course);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Courses", new { slug = viewModel.BootcampSlug });
            }

            viewModel.Skills = await _context.Skills.ToListAsync();
            return View("Create", viewModel);
        }

        [Route("bootcamp/{bootcampSlug}/course/{courseSlug}/edit")]
        public async Task<IActionResult> Edit(string bootcampSlug, string courseSlug)
        {
            if (bootcampSlug == null) return NotFound();

            var bootCampFromDb = await _context.Bootcamps.FirstOrDefaultAsync(b => b.Slug == bootcampSlug);

            if (bootCampFromDb == null) return NotFound();

            if (courseSlug == null) return NotFound();

            var courseFromDb = await _context.Courses.Include(c => c.Skill).FirstOrDefaultAsync(c => c.Slug == courseSlug);

            if (courseFromDb == null) return NotFound();

            CreateEditCourseViewModel viewModel = new CreateEditCourseViewModel()
            {
                Course = courseFromDb,
                Skills = await _context.Skills.ToListAsync(),
                BootcampSlug = bootCampFromDb.Slug,
                BootcampTitle = bootCampFromDb.Name
            };

            return View(viewModel);
        }

        [Route("bootcamp/courses/update")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(CreateEditCourseViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var courseFromDb = await _context.Courses.Include(c => c.Skill).FirstOrDefaultAsync(c => c.Slug == viewModel.Course.Slug);
                if (courseFromDb == null) return NotFound();

                courseFromDb.Title = viewModel.Course.Title;
                courseFromDb.Slug = _slugHelper.GenerateSlug(viewModel.Course.Title);
                courseFromDb.Weeks = viewModel.Course.Weeks;
                courseFromDb.Tuition = viewModel.Course.Tuition;
                courseFromDb.SkillId = viewModel.Course.SkillId;
                courseFromDb.Description = viewModel.Course.Description;
                courseFromDb.ScholarshipsAvailable = viewModel.Course.ScholarshipsAvailable;

                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Courses", new { slug = viewModel.BootcampSlug });
            }

            viewModel.Skills = await _context.Skills.ToListAsync();
            return View(viewModel);
        }

        [Route("bootcamp/{bootcampSlug}/courses/{courseSlug}/remove")]
        public async Task<IActionResult> Remove(string bootcampSlug, string courseSlug)
        {
            if (bootcampSlug == null) return NotFound();

            var bootCampFromDb = await _context.Bootcamps.FirstOrDefaultAsync(b => b.Slug == bootcampSlug);

            if (bootCampFromDb == null) return NotFound();

            if (courseSlug == null) return NotFound();

            var courseFromDb = await _context.Courses.Include(c => c.Skill).FirstOrDefaultAsync(c => c.Slug == courseSlug);

            if (courseFromDb == null) return NotFound();

            _context.Courses.Remove(courseFromDb);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Courses", new { slug = bootcampSlug });
        }
    }
}