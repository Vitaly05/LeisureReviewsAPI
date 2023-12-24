using Algolia.Search.Clients;
using LeisureReviewsAPI;
using LeisureReviewsAPI.Models.Database;
using LeisureReviewsAPI.Repositories.Interfaces;
using LeisureReviewsAPI.Repositories;
using LeisureReviewsAPI.Services.Interfaces;
using LeisureReviewsAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddHsts(options =>
    {
        options.Preload = true;
        options.IncludeSubDomains = true;
        options.MaxAge = TimeSpan.FromDays(90);
    });
    builder.Services.AddHttpsRedirection(options =>
    {
        options.RedirectStatusCode = (int)HttpStatusCode.PermanentRedirect;
        options.HttpsPort = 443;
    });
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactClientPolicy", policyBuilder =>
    {
        policyBuilder.WithOrigins(builder.Configuration["ReactHost"])
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;

    options.Password.RequiredLength = 1;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
}).AddEntityFrameworkStores<ApplicationContext>();

builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration.GetSection("GoogleAuthSettings").GetValue<string>("ClientId");
        googleOptions.ClientSecret = builder.Configuration.GetSection("GoogleAuthSettings").GetValue<string>("ClientSecret");
        googleOptions.SignInScheme = IdentityConstants.ExternalScheme;
    }).AddVkontakte(vkOptions =>
    {
        vkOptions.ClientId = builder.Configuration.GetSection("VkontakteAuthSettings").GetValue<string>("ClientId");
        vkOptions.ClientSecret = builder.Configuration.GetSection("VkontakteAuthSettings").GetValue<string>("ClientSecret");
        vkOptions.SignInScheme = IdentityConstants.ExternalScheme;
        vkOptions.CallbackPath = "/Account/ExternalSignInResponse/";
    });

if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddHsts(options =>
    {
        options.Preload = true;
        options.IncludeSubDomains = true;
        options.MaxAge = TimeSpan.FromDays(90);
    });
    builder.Services.AddHttpsRedirection(options =>
    {
        options.RedirectStatusCode = (int)HttpStatusCode.PermanentRedirect;
        options.HttpsPort = 443;
    });
}

builder.Services.AddSignalR();

builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IReviewsRepository, ReviewsRepository>();
builder.Services.AddScoped<ITagsRepository, TagsRepository>();
builder.Services.AddScoped<ILikesRepository, LikesRepository>();
builder.Services.AddScoped<ICommentsRepository, CommentsRepository>();
builder.Services.AddScoped<IRatesRepository, RatesRepository>();
builder.Services.AddScoped<IIllustrationsRepository, IllustrationsRepository>();
builder.Services.AddScoped<ILeisuresRepository, LeisuresRepository>();
builder.Services.AddScoped<ICloudService, CloudinaryCloudService>();

builder.Services.AddSingleton<ISearchService, AlgoliaSearchService>();
builder.Services.AddSingleton<ISearchClient>(new SearchClient(
    builder.Configuration.GetSection("AlgoliaSearch").GetValue<string>("ApplicationId"),
    builder.Configuration.GetSection("AlgoliaSearch").GetValue<string>("ApiKey")));

using (var serviceProvider = builder.Services.BuildServiceProvider())
{
    var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    await new RolesInitializer(userManager, roleManager, configuration).InitializeAsync();
}


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
    app.UseHttpsRedirection();
    app.UseDefaultFiles();
    app.UseStaticFiles();
}

app.UseCors("ReactClientPolicy");

app.MapControllers();

app.Run();
