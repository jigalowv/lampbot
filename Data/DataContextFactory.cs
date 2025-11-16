using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace lampbot.Data
{
    public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        private readonly IConfiguration _configuration;
        
        public DataContextFactory(IConfiguration configuration)
        {
            _configuration = configuration;   
        }

        public DataContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<DataContext>();
            builder.UseSqlite(_configuration.GetConnectionString("DefaultConnection"));

            return new DataContext(builder.Options);
        }
    }
}