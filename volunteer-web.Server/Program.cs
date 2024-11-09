var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();  // Change this line to include view support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Enable CORS with specific origins and settings
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policyBuilder =>
    {
        policyBuilder.WithOrigins("https://localhost:5173") // Replace with your frontend URL
                     .AllowAnyHeader()
                     .AllowAnyMethod()
                     .AllowCredentials(); // Important for cookies
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
