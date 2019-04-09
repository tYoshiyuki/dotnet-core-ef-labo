using DotNetCoreEfLabo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace DotNetCoreEfLabo
{
    public class Application
    {
        private readonly ILogger _logger;
        private readonly DotNetCorEefLaboContext _context;

        public Application(ILogger<Application> logger, DotNetCorEefLaboContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// エントリポイント
        /// </summary>
        public void Run()
        {
            try
            {
                Before();

                Main();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "処理失敗");
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
            }
            finally
            {
                After();
            }
        }

        /// <summary>
        /// 事前処理
        /// </summary>
        private void Before()
        {
            _logger.LogInformation("処理開始");
        }

        /// <summary>
        /// メイン処理
        /// </summary>
        private void Main()
        {
            var blog = _context.Blog.Find(1);
            var post = _context.Post.Include(_ => _.Blog)
                .Where(_ => _.PostId == 3).ToList();

            var key = _context.Entry(blog).Metadata.FindPrimaryKey()
                .Properties.Select(_ => _.PropertyInfo.GetValue(blog)).ToArray();

            _context.Post.Add(new Post { BlogId = 3, Title = "title", Content = "content"});
            _context.SaveChanges();
        }

        /// <summary>
        /// 事後処理
        /// </summary>
        private void After()
        {
            _logger.LogInformation("処理完了");
        }
    }
}
