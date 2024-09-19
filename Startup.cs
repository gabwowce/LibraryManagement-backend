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

        services.AddLogging(configure =>
        {
            configure.AddConsole();
            configure.AddFile("logs/LMA-{Date}.txt"); // Configure file logging
            configure.SetMinimumLevel(LogLevel.Debug); // Ensure Debug level logs are captured
        });

        var connectionString = Configuration.GetConnectionString("DefaultConnection");

        services.AddSingleton<ILoanRepository>(provider =>
         new LoanRepository(connectionString));

        services.AddSingleton<IMemberRepository>(provider =>
        {
            var loanRepository = provider.GetRequiredService<ILoanRepository>();
            return new MemberRepository(connectionString, loanRepository);
        });

        services.AddSingleton<IBookRepository>(provider =>
            new BookRepository(connectionString));

        services.AddControllers();
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                builder => builder.AllowAnyOrigin()
                                  .AllowAnyMethod()
                                  .AllowAnyHeader());
        });
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
        app.UseCors("AllowAll"); // Enable CORS
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
