using FluentValidation;
using Hangfire;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.IdentityModel.Tokens;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using SurveyBasket.Api.Authentication.Filters;
using SurveyBasket.Api.Settings;
using System.Text;

namespace SurveyBasket.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();

        services.AddOpenApi();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        //Cache
        services.AddHybridCache();

        //CORS
        services.AddCors(option =>
            option.AddDefaultPolicy(policyBuilder =>
                policyBuilder
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>()!)
         ));

        // Exception Handler
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        // Private Extension Methods
        services
            .AddDataBaseConfig(configuration)
            .AddEmailConfig(configuration)
            .AddMapsterConfig()
            .AddBackgroundJobsConfig(configuration)
            .AddAuthenticationConfig(configuration)
            .AddFluntValidationConfig();


        services.AddScoped<IPollService, PollService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<IVoteService, VoteService>();
        services.AddScoped<IResultService, ResultService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();



        return services;
    }
    private static IServiceCollection AddEmailConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<MailSettings>()
            .BindConfiguration(MailSettings.SectionName)
            .ValidateOnStart()
            .ValidateDataAnnotations();

        services.AddScoped<IEmailSender, EmailService>();

        return services;
    }
    private static IServiceCollection AddDataBaseConfig(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("DefaultConnection") ??
           throw new InvalidOperationException("Connection string not found of DefaultConnection");

        return services.AddDbContext<ApplicationDbContext>(option =>
        {
            option.UseSqlServer(connectionString);
        });
    }
    private static IServiceCollection AddMapsterConfig(this IServiceCollection services)
    {
        TypeAdapterConfig config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());

        return services.AddSingleton<IMapper>(new Mapper(config));
    }
    private static IServiceCollection AddFluntValidationConfig(this IServiceCollection services)
    {
        return services
            .AddFluentValidationAutoValidation()
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    }
    private static IServiceCollection AddAuthenticationConfig(this IServiceCollection services, IConfiguration configuration)
    {
        // token provider
        services.AddScoped<IJwtProvider, JwtProvider>();

        // Permission Based Authorization
        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        // Jwt Option
        //services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateDataAnnotations().ValidateOnStart();

        // Identity Services
        services.AddIdentity<ApplicationUser, ApplicationRole>()
           .AddEntityFrameworkStores<ApplicationDbContext>()
           .AddDefaultTokenProviders();

        // Add Authentication
        var JwtSettings = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

        services.AddAuthentication(option =>
        {
            option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(option =>
        {
            option.SaveToken = true;
            option.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidIssuer = JwtSettings?.Issuer,
                ValidateAudience = true,
                ValidAudience = JwtSettings?.Audience,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettings?.SecretKey!))
            };

        });


        // Configure Identity Options
        services.Configure<IdentityOptions>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequiredLength = 8;
            options.SignIn.RequireConfirmedEmail = true;
        });

        return services;
    }
    private static IServiceCollection AddBackgroundJobsConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config => config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));

        services.AddHangfireServer();

        return services;
    }
}
