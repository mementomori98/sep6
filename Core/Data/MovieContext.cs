using System.ComponentModel.DataAnnotations.Schema;
using Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace Core.Data
{
    public class MovieContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(new MySqlConnection("server=mysql35.unoeuro.com;database=arongk_dk_db_sep6_1;user=arongk_dk;password=5Lx3xT9M9Hb3;persistsecurityinfo=True;SslMode=None;"));
        }

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<Question>()
                .HasKey(q => q.Id);

            b.Entity<Choice>()
                .HasKey(c => new {c.QuestionId, c.Text});
            b.Entity<Choice>()
                .HasOne(c => c.Question)
                .WithMany(q => q.Choices)
                .HasForeignKey(c => c.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}