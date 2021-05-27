using System.ComponentModel.DataAnnotations.Schema;
using Core.Data.Models;
using Core.Data.Models.DiscussionItems;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace Core.Data
{
    public class MovieContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(new MySqlConnection("server=mysql35.unoeuro.com;database=arongk_dk_db_sep6_1;user=arongk_dk;password=5Lx3xT9M9Hb3;persistsecurityinfo=True;SslMode=None;"));
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
        }

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Configure<DiscussableDao>(x =>
                x.HasKey(d => d.Id), x =>
                x.ToTable("Discussable"));

            b.Configure<MovieDao>(x =>
                x.HasIndex(m => m.ImdbId).IsUnique(), x =>
                x.Property(m => m.ImdbId).IsRequired(), x =>
                x.ToTable("Movie"));

            b.Configure<ToplistDao>(x =>
                x.ToTable("Toplist"));

            b.Configure<ActorDao>(x =>
                x.ToTable("Actor"));

            b.Configure<UserDao>(x =>
                x.HasKey(u => u.Id), x =>
                x.ToTable("User"), x =>
                x.HasIndex(u => u.Username).IsUnique(), x =>
                x.Property(u => u.Username).IsRequired());

            b.Configure<LoginSessionDao>(x =>
                x.HasKey(ls => ls.Id), x =>
                x.ToTable("LoginSession"), x =>
                x.HasIndex(ls => ls.Token).IsUnique(), x =>
                x.HasOne(ls => ls.User)
                    .WithMany()
                    .HasForeignKey(ls => ls.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade));

            b.Configure<ToplistMovieDao>(x =>
                x.HasKey(tlm => new {tlm.MovieId, tlm.ToplistId}), x =>
                x.ToTable("ToplistMovie"), x =>
                x.HasOne<ToplistDao>()
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

            b.Configure<DiscussionItemDao>(x =>
                x.HasKey(di => di.Id), x =>
                x.Property(di => di.Text).IsRequired(), x =>
                x.ToTable("DiscussionItem"), x =>
                x.HasOne(di => di.Author)
                    .WithMany()
                    .HasForeignKey(di => di.AuthorId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade), x =>
                x.HasOne<DiscussableDao>()
                    .WithMany()
                    .HasForeignKey(di => di.DiscussableId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Cascade));

            b.Configure<CommentDao>(x =>
                x.HasOne<DiscussionItemDao>()
                    .WithMany()
                    .HasForeignKey(c => c.DiscussionItemId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Cascade));

            b.Configure<ReviewDao>();

            b.Configure<FunFactDao>();

            b.Configure<UserDiscussionItemInteractionDao>(x =>
                x.HasKey(i => new {i.DiscussionItemId, i.UserId}), x =>
                x.ToTable("UserDiscussionItemInteraction"), x =>
                x.HasOne<UserDao>()
                    .WithMany()
                    .HasForeignKey(i => i.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade), x =>
                x.HasOne<DiscussionItemDao>()
                    .WithMany()
                    .HasForeignKey(i => i.DiscussionItemId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade));
        }
    }
}