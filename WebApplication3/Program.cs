using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Identity;
using WebApplication3.Model;
using WebApplication3.ViewModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AuthDbContext>();
builder.Services.AddIdentity<Register, IdentityRole>().AddEntityFrameworkStores<AuthDbContext>();
builder.Services.AddDataProtection();
builder.Services.AddReCaptcha(builder.Configuration.GetSection("GoogleReCaptcha"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseStatusCodePagesWithReExecute("/Errors/{0}");

app.MapRazorPages();

app.Run();
