using Microsoft.EntityFrameworkCore;
using Movies.Shared.Models;

namespace Movies.Persistence.Ef;
public class MoviesDbContext(DbContextOptions<MoviesDbContext> options) : DbContext(options)
{

    public DbSet<Movie> Movies { get; set; }


    //--------------------------// 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("Mvs");
        // Set the schema for the migrations history table
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MoviesDbContext).Assembly);

    }

    //--------------------------// 

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.LogTo(info => Console.WriteLine(info));
    }


}//Cls