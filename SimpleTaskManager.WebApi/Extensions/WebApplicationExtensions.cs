using Microsoft.EntityFrameworkCore;
using SimpleTaskManager.DAL;

namespace SimpleTaskManager.WebApi.Extensions
{
    public static class WebApplicationExtensions
    {
        public static async Task ApplyMigrations(this WebApplication app)
        {
            try
            {
                using (var scope = app.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<SimpleTaskManagerDbContext>();

                    await dbContext.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"Error applying migration: {ex.Message}");
            }
        }
    }
}
