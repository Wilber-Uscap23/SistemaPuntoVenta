
using PuntoVentaWeb.Components;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// CONFIGURACIÓN DE BLAZOR
// ==========================================

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ==========================================
// CONEXIÓN CON NUESTRA API
// ==========================================

builder.Services.AddHttpClient("PuntoVentaAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:5235/");
});

// Permitir inyectar HttpClient en las páginas Blazor
builder.Services.AddScoped(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();

    return factory.CreateClient("PuntoVentaAPI");
});

// ==========================================
// CREAR LA APLICACIÓN WEB
// ==========================================

var app = builder.Build();

// ==========================================
// CONFIGURACIÓN HTTP
// ==========================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute(
    "/not-found",
    createScopeForStatusCodePages: true);

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// ==========================================
// INICIAR BLAZOR
// ==========================================

app.Run();
