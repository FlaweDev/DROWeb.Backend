using DROWeb.Application;
using DROWeb.Auth;
using DROWeb.Persistence;
using FastEndpoints;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration) => Configuration = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMemoryCache(); // TODO: replace to Redis

        services.AddFastEndpoints();
        services.AddOpenApi();

        services.AddApplication();
        services.AddPersistence(Configuration);
        services.AddAuth(Configuration);
        services.AddControllers();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
                policy.AllowAnyOrigin();
            });
        });

        /*services.AddAuthentication(config =>
        {
            config.DefaultAuthenticateScheme =
                JwtBearerDefaults.AuthenticationScheme;
            config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = "https://localhost:44386/";
                options.Audience = "NotesWebAPI";
                options.RequireHttpsMetadata = false;
            });*/

        /*services.AddVersionedApiExplorer(options =>
            options.GroupNameFormat = "'v'VVV");*/
        /*services.AddTransient<IConfigureOptions<SwaggerGenOptions>,
                ConfigureSwaggerOptions>();
        services.AddSwaggerGen();
        services.AddApiVersioning();*/

        //services.AddSingleton<ICurrentUserService, CurrentUserService>();
        services.AddHttpContextAccessor();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) //IApiVersionDescriptionProvider provider
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseHsts();
        }
        /*app.UseSwagger();
        app.UseSwaggerUI(config =>
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                config.SwaggerEndpoint(
                    $"/swagger/{description.GroupName}/swagger.json",
                    description.GroupName.ToUpperInvariant());
                config.RoutePrefix = string.Empty;
            }
        });
        app.UseCustomExceptionHandler();*/
        app.UseRouting();
        app.UseHttpsRedirection();
        app.UseCors("AllowAll");
        app.UseStaticFiles(); // Статические файлы из wwwroot


        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
            // Явно указываем, что мы за reverse proxy
            ForwardLimit = null
        });

        // Принудительно заменяем схему на https, если пришел заголовок X-Forwarded-Proto
        app.Use((context, next) =>
        {
            var proto = context.Request.Headers["X-Forwarded-Proto"].FirstOrDefault();
            if (!string.IsNullOrEmpty(proto))
            {
                context.Request.Scheme = proto;
            }
            return next();
        });

        app.UseAuthentication();
        app.UseTokenIntrospection();
        app.UseAuthorization();
        app.UseWebSockets();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapFastEndpoints();
            if (env.IsDevelopment())
                endpoints.MapOpenApi();
        });

    }
}