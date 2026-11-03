using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Controllers
{
    public class UsersController : Controller
    {
        private UserManager<IdentityUser> _userManager;
        public UsersController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            
            return View(_userManager.Users); // Parantez içindeki view model olarak _userManager.Users'ı gönderiyoruz. Bu, kullanıcıların listesini içeren bir koleksiyonu temsil eder. Views/Users/Index.cshtml dosyasında bu veriyi kullanarak kullanıcıları listeleyebiliriz.
        }

    }
}
