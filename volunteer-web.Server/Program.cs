
using Microsoft.EntityFrameworkCore;
using volunteer_web.Server.Data;
var builder = WebApplication.CreateBuilder(args);



//builder.Services.AddDbContext<DbMydataContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DB1")));
builder.Services.AddDbContext<MasterContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DB1")));

Console.WriteLine($"Connection String: {builder.Configuration.GetConnectionString("DB1")}");
// Add services to the container
builder.Services.AddControllersWithViews();  // Change this line to include view support
////builder.Services.AddDbContext<VolunteerDbContext>(options =>
////options.useimmem
//);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Enable CORS with specific origins and settings
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policyBuilder =>
    {
        policyBuilder.WithOrigins("https://localhost:5173") 
                     .AllowAnyHeader()
                     .AllowAnyMethod()
                     .AllowCredentials(); 
    });
});

// Configure session management
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

string db1 = app.Configuration.GetConnectionString("DB1");
// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS policy
app.UseCors("AllowSpecificOrigin");

// Use session before authorization
app.UseSession();

// Set up authorization
app.UseAuthorization();

// Map controllers and enable default route to support MVC actions if needed
app.MapControllers();
app.MapDefaultControllerRoute(); // Enable MVC default routing for views

app.Run();
