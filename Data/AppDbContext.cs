using CSharpCornerApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CSharpCornerApi.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<CSharpCornerArticle> Articles { get; set; }
        public DbSet<UserHistory> UserHistories { get; set; }
        public DbSet<Invoice> Invoices { get; set; } // Добавьте это
        public DbSet<Transaction> Transactions { get; set; } // и это
    }
}
