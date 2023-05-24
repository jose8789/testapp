var builder = WebApplication.CreateBuilder(args);
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
var app = builder.Build();



app.MapReverseProxy(proxyPipeline =>
{
    proxyPipeline.Use((context, next) =>
    {
        if (!CheckAllowedRequest(context, out var reason))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return context.Response.WriteAsync(reason);
        }

        return next();
    });
    proxyPipeline.Use(MyCustomProxyStep);
    // Don't forget to include these two middleware when you make a custom proxy pipeline (if you need them).
    proxyPipeline.UseSessionAffinity();
    proxyPipeline.UseLoadBalancing();
});

bool CheckAllowedRequest(HttpContext context, out string reason)
{
    reason = "No reason to report";
    return true;
}

app.Use(async (context, next) =>
{
    // Do work that can write to the Response.
    await next.Invoke();
    // Do logging or other work that doesn't write to the Response.
});

static Task MyCustomProxyStep(HttpContext context, Func<Task> next)
{
    // Can read data from the request via the context
    foreach (var header in context.Request.Headers)
    {
        Console.WriteLine($"{header.Key}: {header.Value}");
    }

    // The context also stores a ReverseProxyFeature which holds proxy specific data such as the cluster, route and destinations
    var proxyFeature = context.GetReverseProxyFeature();
    Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(proxyFeature.Route.Config));

    // Important - required to move to the next step in the proxy pipeline
    return next();
}
app.Run();