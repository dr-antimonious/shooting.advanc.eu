using Microsoft.AspNetCore.CookiePolicy;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
    options.Secure = CookieSecurePolicy.SameAsRequest;
    options.HttpOnly = HttpOnlyPolicy.Always;
    options.OnAppendCookie = cookieContext =>
        CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
    options.OnDeleteCookie = cookieContext =>
        CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
});

void CheckSameSite(HttpContext httpContext, CookieOptions options)
{
    if (options.SameSite == SameSiteMode.None)
    {
        var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
        if (DisallowsSameSiteNone(userAgent))
        {
            options.SameSite = SameSiteMode.Unspecified;
        }
    }
}

bool DisallowsSameSiteNone(String userAgent)
{
    if (String.IsNullOrWhiteSpace(userAgent))
        return false;

    if (userAgent.Contains("CPU iPhone OS 12") ||
        userAgent.Contains("iPad; CPU OS 12"))
    {
        return true;
    }

    if (userAgent.Contains("Macintosh; Intel Mac OS X 10_14") &&
        userAgent.Contains("Version/") && userAgent.Contains("Safari"))
    {
        return true;
    }

    if (userAgent.Contains("Chrome/5") ||
        userAgent.Contains("Chrome/6"))
    {
        return true;
    }

    return false;
}

builder.Services.AddRazorPages();

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

app.UseCookiePolicy();
app.UseAuthorization();

app.MapRazorPages();

app.Run();