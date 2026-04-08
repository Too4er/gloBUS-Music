using System;
using Microsoft.EntityFrameworkCore;
using gloBUS_Music.MVVM.Model;
using System.Collections.Generic;
using System.Text;

namespace gloBUS_Music.Data
{
    public class gloBUS_MusicDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<Track> Tracks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost;Database=gloBUS_Music;Trusted_Connection=True;TrustServerCertificate=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 
        }
    }
}
