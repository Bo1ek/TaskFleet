using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskFleet.Models;

namespace TaskFleet.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public DbSet<Ticket> Tickets { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        List<IdentityRole> roles = new()
        {
            new IdentityRole
            {
                Id = "1",
                Name = "Admin",
                NormalizedName = "ADMIN"
            },
            new IdentityRole
            {
                Id = "2",
                Name = "User",
                NormalizedName = "USER"
            },
        };
        builder.Entity<IdentityRole>().HasData(roles);
        builder.Entity<User>().Property(u => u.Id).ValueGeneratedOnAdd();

        builder.Entity<Ticket>()
            .HasOne(t => t.AssignedUser)
            .WithMany()
            .HasForeignKey(t => t.AssignedUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
    
}