using System.Net;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using URAuth.Data;
using URAuth.Model;
using Microsoft.AspNetCore.Http;
using URAuth.Helpers;
using URAuth.Data.Repository;

var builder = WebApplication.CreateBuilder(args);


if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<URDBContext>(x =>
    {
        x.UseLazyLoadingProxies();
        // x.UseSqlServer(Configuration.GetConnectionString("DevelopmentConnection"));
        x.UseSqlServer(builder.Configuration.GetConnectionString("DevelopmentConnection")
        // sqlServerOptionsAction: sqlOptions =>
        //         {
        //             sqlOptions.EnableRetryOnFailure(
        //                 maxRetryCount: 5, // Maximum number of retry attempts
        //                 maxRetryDelay: TimeSpan.FromSeconds(30), // Maximum delay between retries
        //                 errorNumbersToAdd: null // Additional error numbers to consider transient
        //             );
        //         }
        );
    });

}
else
{
    builder.Services.AddDbContext<URDBContext>(x =>
{
    x.UseSqlServer(builder.Configuration.GetConnectionString("ProductionConnection")
    // , sqlServerOptionsAction: sqlOptions =>
    //             {
    //                 sqlOptions.EnableRetryOnFailure(
    //                     maxRetryCount: 5, // Maximum number of retry attempts
    //                     maxRetryDelay: TimeSpan.FromSeconds(30), // Maximum delay between retries
    //                     errorNumbersToAdd: null // Additional error numbers to consider transient
    //                 );
    //             }
    );
    x.UseLazyLoadingProxies();

    // o => o.UseNetTopologySuite());
});
}

IdentityBuilder builder1 = builder.Services.AddIdentityCore<User>(
    opt =>
    {
        opt.Password.RequireDigit = false;
        opt.Password.RequiredLength = 8;
        opt.Password.RequireNonAlphanumeric = false;
        opt.Password.RequireUppercase = false;
        opt.Password.RequireLowercase = false;
        opt.Password.RequiredUniqueChars = 0;
    }
);
// Role Access Setting
builder1 = new IdentityBuilder(builder1.UserType, typeof(Role), builder.Services);
builder1.AddEntityFrameworkStores<URDBContext>();
builder1.AddRoleValidator<RoleValidator<Role>>();
builder1.AddRoleManager<RoleManager<Role>>();
builder1.AddSignInManager<SignInManager<User>>();
builder1.AddDefaultTokenProviders();
builder.Services.Configure<DataProtectionTokenProviderOptions>(opt => opt.TokenLifespan = TimeSpan.FromHours(2));
// Token Decoding
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
        .GetBytes(builder.Configuration.GetSection(builder.Environment.IsDevelopment() ? "AppSettings:DevelopmentToken" : "AppSettings:ProductionToken").Value)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
});

builder.Services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();

    options.Filters.Add(new AuthorizeFilter(policy));
})
.AddNewtonsoftJson(opt =>
{
    opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
         {
             options.ForwardedHeaders =
                 ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
         });

builder.Services.AddCors();//Enable Postman
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Repositories
builder.Services.AddScoped<IAuthRepo, AuthRepo>();




builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "auth", Version = "v1" });
});
// }

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "auth v1"));
}
else
{
    app.UseExceptionHandler(builder =>
    {
        builder.Run(async context =>
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var error = context.Features.Get<IExceptionHandlerFeature>();
            if (error != null)
            {
                context.Response.AddApplicationError(error.Error.Message);
                await context.Response.WriteAsync(error.Error.Message);
            }
        });
    });
    app.UseHsts();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
);
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");

app.UseStaticFiles();


AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var scope = app.Services.CreateScope();

var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<URDBContext>();
    context.Database.Migrate();
    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<Role>>();
    DataSeeder.UrSetting(context, userManager, roleManager);
}
catch (System.Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occured during migration!");
}

await app.RunAsync();