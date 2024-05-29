using DonainModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net.Sockets;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data; 

public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.EnableSensitiveDataLogging();
    }

    public DbSet<Request> Requests { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Car> Cars { get; set; }
    
    //TODO: Seed data
    protected override void OnModelCreating(ModelBuilder model) {
        model.Entity<Car>()
            .HasIndex(p => new { p.licensePlate })
            .IsUnique(true);

        var cars = new List<Car> {
            new Car() {id=1,brand = "brand", imageUrl = "url", isAvailable = true, licensePlate = "999-XX-9", location = "location", model = "model", seats = 5}
        };

        model.Entity<Car>().HasData(cars);
        
        
        base.OnModelCreating(model);
    }
}