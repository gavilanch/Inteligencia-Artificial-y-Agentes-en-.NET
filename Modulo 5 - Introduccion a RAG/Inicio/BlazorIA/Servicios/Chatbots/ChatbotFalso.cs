using BlazorIA.DTOs;

namespace BlazorIA.Servicios.Chatbots
{
    public class ChatbotFalso: IChatbot
    {
        public List<MensajeChatUI> Conversacion { get; } = [];

        public bool EstaProcesando => false;

        public SolicitudAprobacionUI? AprobacionPendiente => throw new NotImplementedException();

        public event Action? OnChange;

        public void CancelarRespuestaActual()
        {

        }

        public async Task EnviarMensajeAsync(string textoUsuario, CancellationToken cancellationToken = default)
        {
            Conversacion.Add(new MensajeChatUI { Rol = RolMensaje.Usuario, Texto = textoUsuario });
            NotificarCambio();
            await Task.Delay(500);
            Conversacion.Add(new MensajeChatUI { Rol = RolMensaje.IA, Texto = "Este es un mensaje de prueba. Aún no te conectas con una IA" });
            NotificarCambio();
        }

        private void NotificarCambio() => OnChange?.Invoke();

        public Task ResolverAprobacionAsync(bool aprobada, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void SetearModelo(string modelo)
        {
        }
    }
}
