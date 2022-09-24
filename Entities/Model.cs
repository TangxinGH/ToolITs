using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ToolITs.Entities
{
    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        public string DbPath { get; }
        public string Connstr { get; }

        public BloggingContext()
        {
           
            MyService myService = ServiceProviderFactory.ServiceProvider.GetRequiredService<MyService>();
            Connstr = myService.getConnstr();

            //Connstr = myService.getConnstr("Connstr");
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        //=> options.UseOracle(Connstr);
        => options.UseSqlite(Connstr);
        //=> options.UseOracle(Connstr);
        //=> options.UseOracle($"Data Source={DbPath}");

        //https://docs.microsoft.com/zh-cn/ef/core/cli/powershell
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }

        public List<Post> Posts { get; } = new();
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}

