using BlogApp.Data;
using BlogApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogApp.Services;

namespace BlogApp.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
    public class PostsController : Controller
    {
        private readonly BlogAppContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly AzureStorge _storge;

        public PostsController(
            BlogAppContext context,
            IWebHostEnvironment webHostEnvironment,
            AzureStorge storge
        )
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _storge = storge;
        }

        private string SearchString = "";
        private string SelectedCategoryID = "";

        [AllowAnonymous]
        public async Task<IActionResult> Index(string searchString, string selectedCategoryID)
        {
            if (_context.Posts == null)
            {
                return Problem("Entity set database is null.");
            }

            SearchString = searchString;
            SelectedCategoryID = selectedCategoryID;

            IQueryable<Category> categorieQuery = from c in _context.Categories select c;

            var postQuery = from p in _context.Posts select p;
            postQuery.Include(p => p.Category).Include(p => p.BlogAppUser);

            if (!string.IsNullOrEmpty(searchString))
            {
                postQuery = postQuery.Where(p => p.Title.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(selectedCategoryID))
            {
                postQuery = postQuery.Where(c => c.CategoryID == selectedCategoryID);
            }
            postQuery = postQuery.Where(p => p.IsPublished).Take(6);

            await GenerateCategoriesData();

            var viewModel = new PostViewModel
            {
                PostData = await postQuery.ToListAsync(),
                Categories = new SelectList(
                    await categorieQuery.Distinct().ToListAsync(),
                    "CategoryID",
                    "Name"
                )
            };
            return View(viewModel);
        }

        [AllowAnonymous]
        public async Task<IActionResult> GetMorePosts(int pageNumber, int pageSize)
        {
            int skip = (pageNumber - 1) * pageSize;
            var postQuery = from p in _context.Posts select p;
            postQuery = postQuery.Include(p => p.BlogAppUser).Include(p => p.Category);

            if (!string.IsNullOrEmpty(SearchString))
            {
                postQuery = postQuery.Where(p => p.Title.Contains(SearchString));
            }
            if (!string.IsNullOrEmpty(SelectedCategoryID))
            {
                postQuery = postQuery.Where(c => c.CategoryID == SelectedCategoryID);
            }

            var posts = await postQuery.Skip(skip).Take(pageSize).ToListAsync();

            bool hasMorePosts = posts.Any() ? true : false;

            if (hasMorePosts)
            {
                return PartialView("_PostsPartial", posts);
            }
            else
            {
                return NotFound();
            }
        }

        private async Task GenerateCategoriesData()
        {
            var query = from c in _context.Categories select c;
            var categories = await query.ToListAsync();
            ViewData["CategoriesData"] = categories;
        }

        // GET: Manage
        public async Task<IActionResult> Manage()
        {
            var blogAppContext = _context.Posts
                .Include(p => p.BlogAppUser)
                .Include(p => p.Category);
            return View(await blogAppContext.ToListAsync());
        }

        // GET: Posts/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.BlogAppUser)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.PostID == id);
            if (post == null)
            {
                return NotFound();
            }
            string[] postParagraphs = post.Content.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.None
            );
            ViewBag.PostParagraphs = postParagraphs;

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            ViewData["BlogUserID"] = new SelectList(_context.Users, "Id", "UserName");
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "Name");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostCreateViewModel post)
        {
            if (ModelState.IsValid)
            {
                Post data = new Post
                {
                    Title = post.Title,
                    Content = post.Content,
                    CategoryID = post.CategoryID,
                    BlogUserID = post.BlogUserID,
                    PublishedDate = DateTime.Now,
                    IsPublished = post.IsPublished,
                    Introduction = post.Introduction
                };

                if (post.Image != null && post.Image.Length != 0)
                {
                    // string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "imgs");
                    // Directory.CreateDirectory(uploadsFolder);

                    // string filePath = Path.Combine(uploadsFolder, post.Image.FileName);
                    // using (var stream = new FileStream(filePath, FileMode.Create))
                    // {
                    //     await post.Image.CopyToAsync(stream);
                    // }
                    // data.ImgUrl = post.Image.FileName;

                    string fileName = post.Image.FileName;
                    using (var stream = new MemoryStream())
                    {
                        await post.Image.CopyToAsync(stream);
                        string imageName = post.Image.FileName;
                        string imageUrl = await _storge.UploadImageAsync(stream, imageName);
                        data.ImgUrl = imageUrl;
                    }
                }
                _context.Add(data);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Manage));
            }
            ViewData["BlogUserID"] = new SelectList(
                _context.Users,
                "Id",
                "UserName",
                post.BlogUserID
            );
            ViewData["CategoryID"] = new SelectList(
                _context.Categories,
                "CategoryID",
                "Name",
                post.CategoryID
            );
            return View(post);
        }

        private void PopulateCategoriesDropDownList(object selectedCategory = null)
        {
            var categoriesQuery = from d in _context.Categories orderby d.Name select d;
            ViewBag.CategoryID = new SelectList(
                categoriesQuery.AsNoTracking(),
                "CategoryID",
                "Name",
                selectedCategory
            );
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var postData = await _context.Posts
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PostID == id);

            if (postData == null)
            {
                return NotFound();
            }
            PostUpdateViewModel data = new PostUpdateViewModel
            {
                Title = postData.Title,
                Content = postData.Content,
                IsPublished = postData.IsPublished,
                CategoryID = postData.CategoryID,
                Introduction = postData.Introduction
            };
            ViewBag.Img = postData.ImgUrl;
            PopulateCategoriesDropDownList(postData.CategoryID);
            return View(data);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, PostUpdateViewModel post)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postToUpdate = await _context.Posts.FirstOrDefaultAsync(p => p.PostID == id);
            if (postToUpdate == null)
            {
                return NotFound();
            }
            postToUpdate.Title = post.Title;
            postToUpdate.Content = post.Content;
            postToUpdate.IsPublished = post.IsPublished;
            postToUpdate.CategoryID = post.CategoryID;
            postToUpdate.Introduction = post.Introduction;

            if (post.Image != null && post.Image.Length != 0)
            {
                // string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "imgs");
                // Directory.CreateDirectory(uploadsFolder);

                // string filePath = Path.Combine(uploadsFolder, post.Image.FileName);
                // using (var stream = new FileStream(filePath, FileMode.Create))
                // {
                //     await post.Image.CopyToAsync(stream);
                // }

                // string oldFile = Path.Combine(uploadsFolder, postToUpdate.ImgUrl);
                // System.IO.File.Delete(oldFile);
                // postToUpdate.ImgUrl = post.Image.FileName;

                string fileName = post.Image.FileName;
                using (var stream = new MemoryStream())
                {
                    await post.Image.CopyToAsync(stream);
                    string imageName = post.Image.FileName;
                    string imageUrl = await _storge.UploadImageAsync(stream, imageName);

                    if (await _storge.DeletaImageAsync(postToUpdate.ImgUrl))
                        Console.WriteLine("deleted old Image");

                    postToUpdate.ImgUrl = imageUrl;
                }
            }

            try
            {
                // Save the changes to the database
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Manage));
            }
            catch (DbUpdateException)
            {
                // Log the error (uncomment ex variable name and write a log.)
                ModelState.AddModelError(
                    "",
                    "Unable to save changes. Try again, and if the problem persists, see your system administrator."
                );
            }
            PopulateCategoriesDropDownList(postToUpdate.CategoryID);
            return View(postToUpdate);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.BlogAppUser)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.PostID == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Posts == null)
            {
                return Problem("Entity set 'BlogAppContext.Posts'  is null.");
            }
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Manage));
        }

        private bool PostExists(string id)
        {
            return (_context.Posts?.Any(e => e.PostID == id)).GetValueOrDefault();
        }
    }
}
