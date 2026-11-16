using IdentityApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp.Controllers
{
    public class UsersController : Controller
    {
        private UserManager<AppUser> _userManager;
        private RoleManager<AppRole> _roleManager;
        public UsersController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
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

        public async Task<IActionResult> Edit(string id)
        {
            if(id == null)
            {
                return RedirectToAction("Index");
            }
            
            var user = await _userManager.FindByIdAsync(id);

            if(user != null)
            {
                ViewBag.Roles = await _roleManager.Roles.Select(i => i.Name).ToListAsync(); // ViewBag.Roles özelliğine, _roleManager.Roles koleksiyonundaki her bir rolün adını içeren bir listeyi atıyoruz. Bu listeyi, kullanıcı düzenleme sayfasında rollerin seçilebilmesi için kullanacağız. ToListAsync() metodu, koleksiyonu bir listeye dönüştürür ve veritabanından asenkron olarak verileri alır.
                return View(new EditViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    SelectedRoles = await _userManager.GetRolesAsync(user) // Kullanıcının rollerini almak için _userManager.GetRolesAsync(user) metodunu çağırıyoruz. Bu metod, kullanıcının sahip olduğu rolleri içeren bir liste döndürür. Bu listeyi EditViewModel'in SelectedRoles özelliğine atıyoruz.
                });
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, EditViewModel model)
        {
            if (id != model.Id)
            {
                return RedirectToAction("Index");
            }

            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id); // Kullanıcıyı veritabanında bulmak için _userManager.FindByIdAsync(id) metodunu çağırıyoruz. Bu metod, belirtilen id'ye sahip kullanıcıyı bulur ve bir AppUser nesnesi döndürür. Eğer kullanıcı bulunamazsa null döner.

                if(user != null)
                {
                    user.FullName = model.FullName;
                    user.Email = model.Email;

                    var result = await _userManager.UpdateAsync(user); // Kullanıcıyı güncellemek için _userManager.UpdateAsync(user) metodunu çağırıyoruz. Bu metod, kullanıcıyı veritabanında günceller ve bir IdentityResult nesnesi döndürür. Bu nesne, işlemin başarılı olup olmadığını ve varsa hata mesajlarını içerir.

                    if(result.Succeeded && !string.IsNullOrEmpty(model.Password))
                    {
                        await _userManager.RemovePasswordAsync(user); // Kullanıcının mevcut şifresini kaldırmak için _userManager.RemovePasswordAsync(user) metodunu çağırıyoruz. Bu metod, kullanıcının mevcut şifresini kaldırır ve bir IdentityResult nesnesi döndürür. Bu nesne, işlemin başarılı olup olmadığını ve varsa hata mesajlarını içerir.
                        await _userManager.AddPasswordAsync(user, model.Password); // Kullanıcıya yeni şifre eklemek için _userManager.AddPasswordAsync(user, model.Password) metodunu çağırıyoruz. Bu metod, kullanıcının şifresini belirler ve bir IdentityResult nesnesi döndürür. Bu nesne, işlemin başarılı olup olmadığını ve varsa hata mesajlarını içerir.
                    }

                    if(result.Succeeded)
                    {
                        await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user)); // Kullanıcının mevcut rollerini kaldırmak için _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user)) metodunu çağırıyoruz. Bu metod, kullanıcının sahip olduğu tüm rolleri kaldırır ve bir IdentityResult nesnesi döndürür. Bu nesne, işlemin başarılı olup olmadığını ve varsa hata mesajlarını içerir.
                        if(model.SelectedRoles != null)
                        {
                            await _userManager.AddToRolesAsync(user, model.SelectedRoles); // Kullanıcıya yeni roller eklemek için _userManager.AddToRolesAsync(user, model.SelectedRoles) metodunu çağırıyoruz. Bu metod, kullanıcının sahip olacağı rolleri belirler ve bir IdentityResult nesnesi döndürür. Bu nesne, işlemin başarılı olup olmadığını ve varsa hata mesajlarını içerir.
                        }
                        return RedirectToAction("Index");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    // foreach döngüsü, result.Errors koleksiyonundaki her bir hatayı ModelState'e ekler. Bu sayede, kullanıcıya hata mesajları gösterilebilir. ModelState.AddModelError("", error.Description) metodu, hata mesajını ModelState'e ekler ve boş bir anahtar kullanır. Bu, hata mesajlarının genel bir hata olarak görüntülenmesini sağlar.
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if(user != null)
            {
                await _userManager.DeleteAsync(user); // Kullanıcıyı silmek için _userManager.DeleteAsync(user) metodunu çağırıyoruz. Bu metod, kullanıcıyı veritabanından siler ve bir IdentityResult nesnesi döndürür. Bu nesne, işlemin başarılı olup olmadığını ve varsa hata mesajlarını içerir.
            }
            return RedirectToAction("Index");
        }   
    }
}
