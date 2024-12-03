using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;
using Puzzle.Models;

namespace Puzzle.Data
{
    public class PuzzleDbContext : IdentityDbContext<User>
    {
        public PuzzleDbContext(DbContextOptions<PuzzleDbContext> options)
            : base(options)
        {
        }

        public PuzzleDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (Setting.IsDevelopment)
            {
                optionsBuilder.UseSqlServer(@"Server=.; DataBase=Puzzle Stage; Trusted_Connection=True;");
            }
            else
            {
                optionsBuilder.UseSqlServer(@"Server=.; DataBase=Puzzle Stage; Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("Puzzle");
            builder.Entity<Customer>().HasIndex(c => c.Mobile).IsUnique();
            builder.Entity<Rating>().HasIndex(c => new { c.DateTime, c.CustomerId }).IsUnique();

            base.OnModelCreating(builder);
        }

        public DbSet<Customer> Customer { get; set; }

        public DbSet<CustomerChat> CustomerChat { get; set; }

        public DbSet<Project> Project { get; set; }

        public DbSet<ProjectAccounting> ProjectAccounting { get; set; }

        public DbSet<ProjectReport> ProjectReport { get; set; }

        public DbSet<Favorite> Favorite { get; set; }

        //public DbSet<Media> Media { get; set; }

        public DbSet<Discount> Discount { get; set; }

        public DbSet<Error> Error { get; set; }

        public DbSet<Rating> Rating { get; set; }

        public DbSet<SMS> SMS { get; set; }

        public DbSet<Survey> Survey { get; set; }

        public DbSet<Deposit> Deposit { get; set; }

        public DbSet<Wage> Wage { get; set; }

        public DbSet<Payment> Payment { get; set; }

        public DbSet<History> History { get; set; }

        public DbSet<Conversation> Conversation { get; set; }

        public DbSet<Token> Token { get; set; }

        public DbSet<SmsAlarm> SmsAlarm { get; set; }

        public DbSet<Transaction> Transaction { get; set; }

        public DbSet<Firebase> Firebase { get; set; }

        public DbSet<ConversationFavorite> ConversationFavorite { get; set; }
    }
}
