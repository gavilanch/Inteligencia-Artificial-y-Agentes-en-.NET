using Anthropic;
using BlazorIA.Components;
using BlazorIA.Datos;
using BlazorIA.Servicios;
using BlazorIA.Servicios.Chatbots;
using BlazorIA.Utilidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContextFactory<ApplicationDbContext>(opciones =>
    opciones.UseSqlite("Data Source=midb.db"));


builder.Services.AddScoped<IServicioPersonas, ServicioPersonas>();

builder.Services.AddScoped<IChatbot, ChatbotReal>();


builder.Services.AddTransient<IServicioClima, ServicioClimaOpenWeather>();
builder.Services.AddTransient<ServicioEvaluaCondiciones>();
builder.Services.AddTransient<ServicioEnviarCorreoFalso>();
builder.Services.AddTransient<ServicioObtenerCorreoFalso>();
builder.Services.AddHttpClient();

builder.Services.AddTransient<IChatClientFactory, ChatClientFactory>();

//var proveedor = "openai";
//var modelo = "gpt-5.4-nano";

//builder.Services.AddSingleton<IChatClient>(sp =>
//{
//    var configuration = sp.GetRequiredService<IConfiguration>();
   
//    var llaveOpenAI = configuration.GetValue<string>("OPENAI_LLAVE");
//    var llaveAnthropic = configuration.GetValue<string>("ANTHROPIC_LLAVE");

//    var cliente = proveedor switch
//    {
//        "openai" => new OpenAI.Chat.ChatClient(modelo ?? "gpt-5.4-nano", llaveOpenAI).AsIChatClient(),
//        "claude" => new AnthropicClient()
//        {
//            ApiKey = llaveAnthropic
//        }.AsIChatClient().AsBuilder().ConfigureOptions(c => c.ModelId = modelo ?? "claude-haiku-4-5").Build(),
//        _ => throw new ArgumentException($"Proveedor desconocido: {proveedor}")
//    };

//    return cliente.AsBuilder()
//    .UseFunctionInvocation(null, c =>
//    {
//        c.IncludeDetailedErrors = true;
//    })
//    .Build(sp);
//});

builder.Services.AddTransient<ChatOptions>(sp => new ChatOptions
{
    Tools = [.. Tools.ObtenerTools(sp)],
    Temperature = 0.7f,
    MaxOutputTokens = 2000
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
