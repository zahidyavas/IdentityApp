using System;
using IdentityApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace IdentityApp.TagHelpers
{
    [HtmlTargetElement("td", Attributes = "asp-role-users")] // Bu TagHelper, <td> elementine uygulanacak ve "asp-role-users" attribute'u ile kullanılacak.
    public class RoleUsersTagHelper : TagHelper
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        public RoleUsersTagHelper(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager) // RoleManager ve UserManager bağımlılıklarını alır.
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        [HtmlAttributeName("asp-role-users")] // "asp-role-users" attribute'unun değerini Role property'ye bağlar.
        public string RoleId { get; set; } = null!; // Role property'si null olamaz, bu yüzden null-forgiving operator (!) kullanılır.

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output) // TagHelper'ın işleme mantığını tanımlar.
        {
            var userNames = new List<string>();
            var role = await _roleManager.FindByIdAsync(RoleId); // RoleId ile role'ü bulur.

            if(role != null && role.Name != null) // Role ve role adı null değilse
            {
                foreach (var user in _userManager.Users)
                {
                    if(await _userManager.IsInRoleAsync(user, role.Name)) // Kullanıcının role'de olup olmadığını kontrol eder.
                    {
                        userNames.Add(user.UserName ?? ""); // Eğer kullanıcı role'de ise, kullanıcı adını listeye ekler.
                    }
                }
                output.Content.SetHtmlContent(userNames.Count == 0 ? "User not found" : setHtml(userNames)); // Eğer kullanıcı bulunamazsa "User not found" mesajını, aksi halde kullanıcı adlarını virgülle ayırarak içerik olarak ayarlar.
            }
        }

        private string setHtml(List<string> userNames)
        {
            var html = "<ul>";
            foreach (var item in userNames)
            {
                html += $"<li>{item}</li>";
            }
            html += "</ul>";
            return html;
        }
    }
}
