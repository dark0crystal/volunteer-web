using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

// Enable MVC with controllers and views
builder.Services.AddControllersWithViews();

// Add Razor Pages support
builder.Services.AddRazorPages(); // Add this line

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("https://localhost:5173") // Replace with your frontend URL
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials(); // Important for cookies
    });
});

// Add distributed memory cache and session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".AdventureWorks.Session";
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.IsEssential = true;
});


// Add services for API documentation and endpoints
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Use CORS policy
app.UseCors("AllowSpecificOrigin");

// Enable default files and static files
app.UseDefaultFiles();
app.UseStaticFiles();



// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // Always redirect HTTP requests to HTTPS
app.UseRouting(); // Ensure routing is enabled

app.UseAuthorization(); // Authorization middleware should come after UseRouting
// Use session middleware
app.UseSession();

// Set up endpoint routing

    app.MapControllers();
    app.MapRazorPages(); // Ensure Razor Pages are mapped
    app.MapDefaultControllerRoute(); // Enable default route


app.Run();



















app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();

