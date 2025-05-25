using Microsoft.EntityFrameworkCore;
using Tutorial11.Data;
using Tutorial11.Services;

namespace Tutorial11;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddDbContext<DatabaseContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddScoped<IHospitalService, HospitalService>();
        builder.Services.AddControllers();
        
        var app = builder.Build();

        app.MapControllers();
        app.UseAuthentication();
        app.UseAuthorization();
        app.Run();
    }
}