using Villa_Web.AutoMapper;
using Villa_Web.Services;
using Villa_Web.Services.IServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// AutoMaper config...
builder.Services.AddAutoMapper(typeof(MappingConfig));

// Adding HttpClient and service ...
builder.Services.AddHttpClient<IVillaService, VillaService>();
builder.Services.AddScoped<IVillaService, VillaService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();