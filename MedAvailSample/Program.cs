using Microsoft.EntityFrameworkCore;
using MedAvailSample.Data;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MedAvailDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("MedAvailDb")));

builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Error");

app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();
app.Run();
