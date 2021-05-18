using System.ComponentModel.DataAnnotations.Schema;
using Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Ubiety.Dns.Core.Records;

namespace Core.Data
{
    public class MovieContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(new MySqlConnection("server=mysql35.unoeuro.com;database=arongk_dk_db_sep6_1;user=arongk_dk;password=5Lx3xT9M9Hb3;persistsecurityinfo=True;SslMode=None;"));
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Configure<User>(x =>
                x.HasKey(u => u.Id), x =>
                x.HasIndex(u => u.Username).IsUnique());

            b.Configure<LoginSession>(x =>
                x.HasKey(ls => ls.Id), x =>
                x.HasIndex(ls => ls.Token).IsUnique(), x =>
                x.HasOne(ls => ls.User)
                    .WithMany()
                    .HasForeignKey(ls => ls.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade));

            b.Configure<Movie>(x =>
                x.HasKey(m => m.Id), x =>
                x.HasIndex(m => m.ImdbId).IsUnique(), x =>
                x.Property(m => m.ImdbId).IsRequired());

            b.Configure<Toplist>(x =>
                x.HasKey(tl => tl.Id));

            b.Configure<ToplistMovie>(x =>
                x.HasKey(tlm => new {tlm.MovieId, tlm.ToplistId}), x =>
                x.HasOne<Toplist>()
                    .WithMany(tl => tl.ToplistMovies)
                    .HasForeignKey(tlm => tlm.ToplistId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade), x =>
                x.HasOne(tlm => tlm.Movie)
                    .WithMany()
                    .HasForeignKey(tlm => tlm.MovieId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade), x =>
                x.HasIndex(tlm => new {tlm.ToplistId, tlm.Position}).IsUnique());
        }
    }
}