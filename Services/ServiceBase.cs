using lampbot.Data;
using Microsoft.Extensions.Configuration;

namespace lampbot.Services
{
    public class ServiceBase
    {
        protected readonly DataContext _context;
        protected readonly IConfiguration _config;

        public ServiceBase(
            DataContext context,
            IConfiguration config
        )
        {
            _context = context;
            _config = config;
        }

        protected IConfigurationSection GetMessage(string section)
        {
            return _config.GetSection("Messages").GetSection(section);
        }
    }
}