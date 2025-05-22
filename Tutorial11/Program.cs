using Tutorial11.Services;

namespace Tutorial11;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddScoped<IHospitalService, HospitalService>();
        builder.Services.AddControllers();
        
        var app = builder.Build();

        app.MapControllers();
        app.UseAuthentication();
        app.UseAuthorization();
        app.Run();
    }
}