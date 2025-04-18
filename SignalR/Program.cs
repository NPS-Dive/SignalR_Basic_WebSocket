using Microsoft.AspNetCore.Authentication.Cookies;
using SignalR.Hubs;
using SignalR.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(options =>
{
    options.LoginPath = "/Login";
});

// Add SignalR
builder.Services.AddSignalR();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("ChatPolicy", builder =>
    {
        builder
            .WithOrigins("http://localhost:5094") // Specify client origins
            .AllowAnyHeader()
            .WithMethods("GET", "POST")
            .AllowCredentials(); // Required for SignalR with authentication
    });
});

// Dependency Injection
builder.Services.AddSingleton<IChatRoomService, InMemoryChatRoomService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();

// Apply CORS before authentication
app.UseCors("ChatPolicy");

app.UseAuthentication();
app.UseAuthorization();

// Map SignalR hubs and Razor pages
app.MapHub<ChatHub>("/chathub");
app.MapHub<AgentHub>("/agenthub");
app.MapRazorPages();

app.Run();