using CustomerCommandService.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerCommandService.Data
{
    public class DataHelper
    {
        public static void CreateDbIfNotExists(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<CustomerContext>();
                    context.Database.EnsureCreated();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<DataHelper>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
        }
    }
}
    