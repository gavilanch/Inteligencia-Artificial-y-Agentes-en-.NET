using Anthropic;
using BlazorIA.Utilidades;
using Microsoft.Extensions.AI;

namespace BlazorIA.Servicios
{
    public class ChatClientFactory(IConfiguration configuration, IServiceProvider sp) : IChatClientFactory
    {
        public IChatClient Crear(string modelo)
        {
            var llaveOpenAI = configuration.GetValue<string>("OPENAI_LLAVE");
            var llaveAnthropic = configuration.GetValue<string>("ANTHROPIC_LLAVE");

            var proveedor = ModelosIA.ObtenerProveedor(modelo);

            var cliente = proveedor switch
            {
                "openai" => new OpenAI.Chat.ChatClient(modelo ?? "gpt-5.4-nano", llaveOpenAI).AsIChatClient(),
                "claude" => new AnthropicClient()
                {
                    ApiKey = llaveAnthropic
                }.AsIChatClient().AsBuilder().ConfigureOptions(c => c.ModelId = modelo ?? "claude-haiku-4-5").Build(),
                _ => throw new ArgumentException($"Proveedor desconocido: {proveedor}")
            };

            return cliente.AsBuilder()
            .UseFunctionInvocation(null, c =>
            {
                c.IncludeDetailedErrors = true;
            })
            .Build(sp);
        }
    }
}
