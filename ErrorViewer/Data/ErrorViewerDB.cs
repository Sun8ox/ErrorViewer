using ErrorViewer.Functions;
using ErrorViewer.Models;
using Microsoft.EntityFrameworkCore;

namespace ErrorViewer.Data
{
    public class ErrorViewerDB : DbContext
    {
        
        public ErrorViewerDB (DbContextOptions<ErrorViewerDB> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = default!;
        public DbSet<Source> Sources { get; set; } = default!;
        

    }
}

