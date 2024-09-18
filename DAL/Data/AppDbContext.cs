    using Microsoft.EntityFrameworkCore;

    namespace DAL.Data;
    using DAL.Models;
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ActivityLog> ActivityLog { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the many-to-many relationship
            modelBuilder.Entity<Task>()
                .HasMany(t => t.AssignedToUser) // or AssignedToUser if you kept that name
                .WithMany(u => u.AssignedTo)
                .UsingEntity(j => j.ToTable("UserTasks"));
            
            modelBuilder.Entity<Project>()
                .HasMany(p => p.Users)
                .WithMany(u => u.Projects)
                .UsingEntity(j => j.ToTable("UserProjects"));

            // Configure the one-to-many relationship for CreatedBy
            modelBuilder.Entity<Project>()
                .HasOne(p => p.CreatedBy)
                .WithMany() // Assuming User doesn't have a collection of created projects
                .OnDelete(DeleteBehavior.Restrict);

                
        }
        
    }