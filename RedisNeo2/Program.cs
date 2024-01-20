using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;
using Neo4jClient;
using RedisNeo2.Hubs;
using RedisNeo2.Services.Implementation;
using RedisNeo2.Services.Usage;
using StackExchange.Redis;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RedisNeo2_sol", Version = "v1" });
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "RedisNeo2_";
});

var clientNeo4j = new BoltGraphClient(new Uri("bolt://localhost:7687"), username: "neo4j", password: "svetomir132");
await clientNeo4j.ConnectAsync();

builder.Services.AddSingleton<IGraphClient>(clientNeo4j);

builder.Services.AddScoped<IBibliotekaService, BibliotekaService>();
builder.Services.AddScoped<IChatServices, ChatService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IKorisnikService, KorisnikService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/LoginPage";
        options.Cookie.Name = "MojKolacXD";
    });

builder.Services.AddMvc();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSignalR();
builder.Services.AddResponseCompression(options =>
{
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] {
        "application/octet-stream"
    });
});
Debug.WriteLine("Redis connection string = " + builder.Configuration.GetConnectionString("Redis"));
var redisClient = ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis"));
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("CORS", o =>
    {
        o.WithOrigins(new string[] {
                 "http://localhost:8080",
                 "https://localhost:8080",
                 "http://127.0.0.1:8080",
                 "https://127.0.0.1:8080",
                 "http://127.0.0.1:7241",
                 "https://127.0.0.1:7241",
                 "https://localhost:7241",
                 "http://localhost:7241"
             }).AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseResponseCompression();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RedisNeo2_sol v1"));
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseCors("CORS");

app.UseAuthentication();

app.UseAuthorization();

app.UseCookiePolicy();

app.MapRazorPages();

app.MapBlazorHub();

app.MapHub<ChatHub>("/chatHub");

app.UseEndpoints(e =>
{
    e.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();