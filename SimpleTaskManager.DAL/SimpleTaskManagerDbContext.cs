using Microsoft.EntityFrameworkCore;
using SimpleTaskManager.DAL.Models;

namespace SimpleTaskManager.DAL
{
    public  class SimpleTaskManagerDbContext: DbContext
    {
        public SimpleTaskManagerDbContext(DbContextOptions<SimpleTaskManagerDbContext> options) : base(options)
        {
        }

        public SimpleTaskManagerDbContext() : base()
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Models.Task> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
