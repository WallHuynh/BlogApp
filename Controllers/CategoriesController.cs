using BlogApp.Data;
using BlogApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
    public class CategoriesController : Controller
    {
        private readonly BlogAppContext _context;

        public CategoriesController(BlogAppContext context)
        {
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            return _context.Categories != null
                ? View(await _context.Categories.ToListAsync())
                : Problem("Entity set 'BlogAppContext.Categories'  is null.");
        }

        // GET: Categories/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var queryPost = from p in _context.Posts select p;
            queryPost = queryPost
                .Include(p => p.Category)
                .Include(p => p.BlogAppUser)
                .Where(p => p.CategoryID == id)
                .Where(p => p.IsPublished)
                .OrderByDescending(p => p.PublishedDate);

            var queryCategories = from c in _context.Categories select c;
            var categories = await queryCategories.ToListAsync();
            CategoryPostViewModel data = new CategoryPostViewModel
            {
                PostData = await queryPost.ToListAsync(),
                SelectedCategory = queryCategories.FirstOrDefault(c => c.CategoryID == id)
            };
            ViewData["CategoriesData"] = categories;

            return View(data);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatecategoryViewModel category)
        {
            if (ModelState.IsValid)
            {
                Category data = new Category
                {
                    Name = category.Name,
                    Description = category.Description
                };
                _context.Add(data);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            string id,
            [Bind("CategoryID,Name,Description")] Category category
        )
        {
            if (id != category.CategoryID)
            {
                return NotFound();
            }

            // if (ModelState.IsValid)
            // {
            try
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.CategoryID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
            // }
            // return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FirstOrDefaultAsync(m => m.CategoryID == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'BlogAppContext.Categories'  is null.");
            }
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(string id)
        {
            return (_context.Categories?.Any(e => e.CategoryID == id)).GetValueOrDefault();
        }
    }
}
