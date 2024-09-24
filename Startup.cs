using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using LibraryManagement.Interfaces;
using LibraryManagement.Repositories;
using System.Text;
using Microsoft.Extensions.Logging;
using LibraryManagement.Services;

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
            configure.SetMinimumLevel(LogLevel.Debug); 
        });

        var connectionString = Configuration.GetConnectionString("DefaultConnection");

 
        services.AddSingleton<ILoanRepository>(provider => new LoanRepository(connectionString));
        services.AddSingleton<IMemberRepository>(provider =>
        {
            var loanRepository = provider.GetRequiredService<ILoanRepository>();
            return new MemberRepository(connectionString, loanRepository);
        });
        services.AddSingleton<IBookRepository>(provider => new BookRepository(connectionString));


        var jwtSecret = Configuration["Jwt:Secret"]; 
        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

        services.AddControllers();
        services.AddScoped<AuthenticationService>();

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
        app.UseCors("AllowAll"); 
        app.UseAuthentication(); 
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
