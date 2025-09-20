using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlexSSO.Attributes;
using PlexSSO.Data;
using PlexSSO.Entities;
using PlexSSO.Services;
using PlexSSO.ViewModels.Admin;

namespace PlexSSO.Controllers
{
    [Route("[controller]")]
    [AdminAuthorize]
    public class AdminController(PlexBridgeDb db) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new AdminIndexModel()
            {
                Clients = await db.Clients.OrderBy(c => c.ClientId).ToListAsync()
            };
            return View(model); // Views/Admin/Index.cshtml
        }

        [HttpPost]
        public async Task<IActionResult> OnPostIndexCreate(AdminIndexModel model)
        {
            var newModel = new AdminCreateClientModel()
            {
                ClientSecret = model.Post.ClientSecret,
                ClientId = model.Post.ClientId,
                RedirectUri = model.Post.RedirectUri
            };

            return await OnPostCreateAsync(newModel);
        }

        [HttpGet("Login")]
        [AllowAnonymous] // explicitly allow this one
        public IActionResult Login()
        {
            var model = new AdminLoginModel();
            return View(model); // Views/Admin/Login.cshtml
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginPostAsync(AdminLoginModel model)
        {
            var user = await db.AdminUsers.FirstOrDefaultAsync(u => u.Username == (model.Username ?? ""));

            if (user == null || !SimplePasswordHasher.Verify(model.Password ?? "", user.PasswordHash, user.PasswordSalt))
            {
                model.Error = "Invalid credentials";
                return View("Login", model); // Views/Admin/Login.cshtml
            }

            HttpContext.Session.SetInt32("admin-id", user.Id);
            return Redirect("/Admin");
        }

        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("admin-id");
            return RedirectToAction("Login");
        }

        [HttpPost("Create")]
        public async Task<IActionResult> OnPostCreateAsync(AdminCreateClientModel input)
        {
            if (string.IsNullOrWhiteSpace(input.ClientId) || string.IsNullOrWhiteSpace(input.RedirectUri))
                return RedirectToPage("");

            db.Clients.Add(new OidcClientApp
            {
                ClientId = input.ClientId.Trim(),
                ClientSecret = string.IsNullOrWhiteSpace(input.ClientSecret) ? null : input.ClientSecret.Trim(),
                RedirectUri = input.RedirectUri.Trim(),
                Enabled = true
            });

            await db.SaveChangesAsync();

            return RedirectToPage("");
        }

        [HttpGet("Edit/{id:int}")]
        public async Task<IActionResult> EditAsync(int id)
        {
            var item = await db.Clients.FindAsync(id);
            if (item is null)
            {
                return Redirect("/Admin");
            }

            var model = new AdminEditClientModel
            {
                Item = item
            };

            return View(model); // Views/Admin/Edit.cshtml
        }

        [HttpPost("Edit/{id:int}")]
        public async Task<IActionResult> OnPostEditAsync([FromForm] AdminEditClientModel model, int id)
        {
            var item = await db.Clients.FindAsync(id);
            if (item is null)
            {
                return Redirect("/Admin");
            }

            item.ClientId = model.Item.ClientId;
            item.ClientSecret = model.Item.ClientSecret;
            item.RedirectUri = model.Item.RedirectUri;
            item.Enabled = model.Item.Enabled;

            await db.SaveChangesAsync();

            return Redirect("/Admin");
        }

        [HttpPost("Delete/{id:int}")]
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var item = await db.Clients.FindAsync(id);

            if (item != null)
            {
                db.Clients.Remove(item);
                await db.SaveChangesAsync();
            }

            return Redirect("/Admin");
        }

        [HttpGet("Delete/{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var model = new AdminDeleteClientModel
            {
                Item = await db.Clients.FindAsync(id)
            };

            return View(model);
        }

    }
}
