using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp.Models
{
    public class IdentityContext : IdentityDbContext<IdentityUser>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options) // burda ne oluyor? options parametresi ile IdentityContext sınıfının yapılandırma seçeneklerini alıyoruz. Bu seçenekler, veritabanı bağlantısı, model yapılandırması ve diğer Entity Framework Core ayarlarını içerir. base(options) ifadesi, bu seçenekleri üst sınıf olan IdentityDbContext'e geçirir, böylece IdentityDbContext bu seçenekleri kullanarak veritabanı işlemlerini gerçekleştirebilir.
        {
            
        }
    }
}
