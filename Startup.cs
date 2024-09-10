using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using LibraryManagement.Interfaces;
using LibraryManagement.Repositories;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Registruokite visus repozitorijus
        services.AddSingleton<IBookRepository>(provider =>
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            return new BookRepository(connectionString);
        });

        services.AddSingleton<IMemberRepository>(provider =>
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            return new MemberRepository(connectionString);
        });

        services.AddSingleton<ILoanRepository>(provider =>
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            return new LoanRepository(connectionString);
        });

        // Registruokite kitus paslaugų tipus
        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
