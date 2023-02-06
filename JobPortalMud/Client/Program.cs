using JobPortalMud.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.ComponentModel;
using System;
using MudBlazor.Services;
using JobPortalMud.Client.Services.JobService;
using JobPortalMud.Client.Services.ContactService;
using JobPortalMud.Client.Services.UserService;
using JobPortalMud.Client.Services.AppliedJobService;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MudBlazor;
using JobPortalMud.Client.Services.BookmarkedJobService;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("JobPortalMud.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("JobPortalMud.ServerAPI"));
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAppliedJobService, AppliedJobService>();
builder.Services.AddScoped<IBookmarkedJobService, BookmarkedJobService>();

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter;

    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 10000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

builder.Services.AddApiAuthorization();
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();


await builder.Build().RunAsync();
