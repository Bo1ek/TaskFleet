using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskFleet.Models;

namespace TaskFleet.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Location> Locations {get; set;}
    public DbSet<Vehicle> Vehicles {get; set;}
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
                Name = "SuperAdmin",
                NormalizedName = "SUPERADMIN"
            },
            new IdentityRole
            {
                Id = "2",
                Name = "Admin",
                NormalizedName = "ADMIN"
            },
            new IdentityRole
            {
                Id = "3",
                Name = "Client",
                NormalizedName = "CLIENT"
            },
            new IdentityRole
            {
                Id = "4",
                Name = "Hr",
                NormalizedName = "HR"
            },
            new IdentityRole
            {
                Id = "5",
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
    }
    
}