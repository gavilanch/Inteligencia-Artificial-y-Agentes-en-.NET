using BlazorIA.DTOs;

namespace BlazorIA.Servicios.Chatbots
{
    public interface IChatbot
    {
        List<MensajeChatUI> Conversacion { get; }
        bool EstaProcesando { get; }
        SolicitudAprobacionUI? AprobacionPendiente { get; }

        event Action? OnChange;

        void SetearModelo(string modelo);
        void CancelarRespuestaActual();
        Task EnviarMensajeAsync(string textoUsuario, CancellationToken cancellationToken = default);
        Task ResolverAprobacionAsync(bool aprobada, CancellationToken cancellationToken = default);
    }
}
