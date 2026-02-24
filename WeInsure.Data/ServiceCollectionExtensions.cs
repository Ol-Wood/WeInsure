using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace WeInsure.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, string? connectionString)
    {

        if (string.IsNullOrEmpty(connectionString))
        {
            const Environment.SpecialFolder folder = Environment.SpecialFolder.LocalApplicationData;
            connectionString = $"Data Source={Path.Join(Environment.GetFolderPath(folder), "weinsure.db")}";
        }
        
        services.AddDbContext<WeInsureDbContext>(options => options.UseSqlite(connectionString));
        return services;
    }
    
    
}