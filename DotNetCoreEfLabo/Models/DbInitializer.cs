using System.Linq;

namespace DotNetCoreEfLabo.Models
{
    public static class DbInitializer
    {
        public static void Initialize(DotNetCorEefLaboContext context)
        {
            if (context.Blog.Any())
            {
                var blogs = new Blog[]
                {
                new Blog { Url = "http://blogs.msdn.com/dotnet"},
                new Blog { Url = "http://blogs.msdn.com/webdev"},
                new Blog { Url = "http://blogs.msdn.com/visualstudio"},
                };

                foreach (var blog in blogs)
                {
                    context.Blog.Add(blog);
                }
                context.SaveChanges();
            }
        }
    }
}
