using Microsoft.Extensions.AI;

namespace BlazorIA.Servicios
{
    public interface IChatClientFactory
    {
        IChatClient Crear(string modelo);
    }
}
