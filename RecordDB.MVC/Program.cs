using RecordDB.MVC.Services;
using RecordDB.Shared.DTOs;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------
// Services
// -----------------------------------------------------------------------

builder.Services.AddControllersWithViews();

// Typed HttpClient — one per entity service
var apiBase = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!);

builder.Services.AddHttpClient<IArtistService, ArtistService>(client =>
    client.BaseAddress = apiBase);

builder.Services.AddHttpClient<IRecordService, RecordService>(client =>
    client.BaseAddress = apiBase);

builder.Services.AddHttpClient<IDiscService, DiscService>(client =>
    client.BaseAddress = apiBase);

builder.Services.AddHttpClient<ITrackService, TrackService>(client =>
    client.BaseAddress = apiBase);

builder.Services.AddHttpClient<IStatisticService, StatisticService>(client =>
    client.BaseAddress = apiBase);

builder.Services.AddHttpClient<ITotalService, TotalService>(client =>
    client.BaseAddress = apiBase);

// -----------------------------------------------------------------------
// Pipeline
// -----------------------------------------------------------------------

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseStaticFiles();
app.UseAuthorization();

app.MapControllerRoute(
    name: "records-plural",
    pattern: "Records/{action=Index}/{id?}",
    defaults: new { controller = "Record" });

app.MapControllerRoute(
    name: "artists-plural",
    pattern: "Artists/{action=Index}/{id?}",
    defaults: new { controller = "Artist" });

app.MapControllerRoute(
    name: "discs-plural",
    pattern: "Discs/{action=Index}/{id?}",
    defaults: new { controller = "Disc" });

app.MapControllerRoute(
    name: "tracks-plural",
    pattern: "Tracks/{action=Index}/{id?}",
    defaults: new { controller = "Track" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
