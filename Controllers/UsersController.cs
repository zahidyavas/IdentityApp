using IdentityApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Controllers
{
    public class UsersController : Controller
    {
        private UserManager<AppUser> _userManager;
        public UsersController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            
            return View(_userManager.Users); // Parantez içindeki view model olarak _userManager.Users'ı gönderiyoruz. Bu, kullanıcıların listesini içeren bir koleksiyonu temsil eder. Views/Users/Index.cshtml dosyasında bu veriyi kullanarak kullanıcıları listeleyebiliriz.
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = model.FullName,
                    Email = model.Email,
                    FullName = model.FullName,
                };

                var result = await _userManager.CreateAsync(user, model.Password ?? string.Empty);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

    }
}
