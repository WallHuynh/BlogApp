using BlogApp.Data;
using BlogApp.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BlogApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BlogAppContext _context;
        private readonly IEmailSender _emailSender;

        public HomeController(
            ILogger<HomeController> logger,
            BlogAppContext context,
            IEmailSender emailSender
        )
        {
            _emailSender = emailSender;
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var query = from p in _context.Posts select p;
            query = query
                .Include(p => p.BlogAppUser)
                .Include(p => p.Category)
                .Where(p => p.IsPublished);

            var lastestPost = await query
                .OrderByDescending(p => p.PublishedDate)
                .FirstOrDefaultAsync();

            var featuredPosts = query
                .Where(p => p.PostID != lastestPost.PostID)
                .OrderByDescending(p => p.PublishedDate);

            if (!featuredPosts.Any() && lastestPost == null)
                return NotFound();

            PostHome postData = new PostHome
            {
                FeaturedPosts = await featuredPosts.ToListAsync(),
                LastestPost = lastestPost
            };
            return View(postData);
        }

        public async Task<IActionResult> Portfolio()
        {
            var postData = _context.Posts
                .Include(p => p.Category)
                .Include(p => p.BlogAppUser)
                .OrderByDescending(p => p.PublishedDate)
                .Take(2);
            ViewBag.BlogPosts = await postData.ToListAsync();
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Contact(ContactViewModel contact)
        {
            if (ModelState.IsValid)
            {
                var AddedContact = await _context.Contacts.FirstOrDefaultAsync(
                    c => c.Email == contact.Email
                );

                if (AddedContact == null)
                {
                    var data = new Contact
                    {
                        Id = Guid.NewGuid().ToString(),
                        FirstName = contact.FirstName,
                        LastName = contact.LastName,
                        Email = contact.Email,
                        Message = contact.Message
                    };
                    _context.Add(data);
                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException)
                    {
                        // Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "Something went wrong, try again.");
                        return View();
                    }
                }
                else
                {
                    AddedContact.Message = contact.Message;
                    AddedContact.FirstName = contact.FirstName;
                    AddedContact.LastName = contact.LastName;
                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException)
                    {
                        // Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "Something went wrong, try again.");
                        return View();
                    }
                }

                string htmlBody =
                    $"<p>Hi {contact.FirstName},</p>"
                    + "<p>I am Huynh Phuc Tuong, Wall'Blog Admin.<br>"
                    + "Thank you for getting in touch with me!<br>"
                    + "I contact to you as soon as possible.</p>"
                    + "<p>Kind regards,</p>"
                    + "<p>Phuc Tuong</p>";
                await _emailSender.SendEmailAsync(
                    contact.Email,
                    "Contact Form Submission",
                    htmlBody
                );
                TempData["SuccessMessage"] = "Your message has been sent successfully.";
                return RedirectToAction(nameof(Contact));
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(
                new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                }
            );
        }
    }
}
