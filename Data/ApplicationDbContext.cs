using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GITA.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Comment>()
                .HasOne(c => c.Topic)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TopicId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete
        }
    }

    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsBanned { get; set; } = false;
    }

    public class Topic
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string State { get; set; } = "pending";
        public string Status { get; set; } = "active";
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public ICollection<Comment> Comments { get; set; }
    }
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int TopicId { get; set; }
        public Topic Topic { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
