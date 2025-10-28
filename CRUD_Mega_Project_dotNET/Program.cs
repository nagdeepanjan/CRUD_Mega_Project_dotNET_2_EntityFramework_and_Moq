using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

//Add services to IoC container
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPersonsService, PersonsService>();
builder.Services.AddDbContext<PersonsDbContext>((options =>options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))));  //Scoped automatically. This above lines cannot be singleton...they too have to be scoped

var app = builder.Build();
app.UseStaticFiles();


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

//app.MapGet("/", () => "Hello World!");
app.MapControllers();

app.Run();
