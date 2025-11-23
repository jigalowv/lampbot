using Microsoft.Extensions.Configuration;

namespace lampbot.Extensions
{
    public static class ResponseExtensions
    {
        public static string GetResponse(
            this IConfiguration configuration,
            string name, 
            object arg0,
            object arg1)
        {
            return string
                .Format(configuration
                    .GetSection(Constants.Responses)[name]!, arg0, arg1);
        }
    }
}