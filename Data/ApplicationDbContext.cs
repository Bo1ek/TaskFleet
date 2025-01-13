using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskFleet.Models;
using License = TaskFleet.Models.License;
namespace TaskFleet.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Location> Locations {get; set;}
    public DbSet<Vehicle> Vehicles {get; set;}
    public DbSet<License> Licenses { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        List<IdentityRole> roles = new()
        {
            new IdentityRole { Id = "1", Name = "SuperAdmin", NormalizedName = "SUPERADMIN" },
            new IdentityRole { Id = "2", Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole { Id = "3", Name = "Client", NormalizedName = "CLIENT" },
            new IdentityRole { Id = "4", Name = "Hr", NormalizedName = "HR" },
            new IdentityRole { Id = "5", Name = "Driver", NormalizedName = "Driver" },
        };
        builder.Entity<IdentityRole>().HasData(roles);
        
        builder.Entity<License>().HasData(
            new License { LicenseId = 1, Category = "AM" },
            new License { LicenseId = 2, Category = "A1" },
            new License { LicenseId = 3, Category = "A2" },
            new License { LicenseId = 4, Category = "A" },
            new License { LicenseId = 5, Category = "B1" },
            new License { LicenseId = 6, Category = "B" },
            new License { LicenseId = 7, Category = "B+E" },
            new License { LicenseId = 8, Category = "C" },
            new License { LicenseId = 9, Category = "C1" },
            new License { LicenseId = 10, Category = "C1+E" },
            new License { LicenseId = 11, Category = "C+E" },
            new License { LicenseId = 12, Category = "D" },
            new License { LicenseId = 13, Category = "D1" },
            new License { LicenseId = 14, Category = "D1+E" },
            new License { LicenseId = 15, Category = "D+E" }
        );
        builder.Entity<User>().Property(u => u.Id).ValueGeneratedOnAdd();

        builder.Entity<Ticket>()
            .HasOne(t => t.AssignedUser)
            .WithMany()
            .HasForeignKey(t => t.AssignedUserId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.Entity<Ticket>()
            .HasOne(t => t.StartLocation)
            .WithMany()
            .HasForeignKey(t => t.StartLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Ticket>()
            .HasOne(t => t.EndLocation)
            .WithMany()
            .HasForeignKey(t => t.EndLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Ticket>().Property(t => t.Status)
            .HasConversion<string>();
        
        builder.Entity<Vehicle>()
            .HasOne(v => v.AssignedTicket)
            .WithOne(t => t.AssignedVehicle)
            .HasForeignKey<Ticket>(t => t.AssignedVehicleId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<License>()
            .HasMany(l => l.Users)
            .WithMany(u => u.Licenses)
            .UsingEntity<Dictionary<string, object>>(
                "UserLicenses",
                j => j.HasOne<User>().WithMany().HasForeignKey("UserId"),
                j => j.HasOne<License>().WithMany().HasForeignKey("LicenseId")
            );

    }
    
}