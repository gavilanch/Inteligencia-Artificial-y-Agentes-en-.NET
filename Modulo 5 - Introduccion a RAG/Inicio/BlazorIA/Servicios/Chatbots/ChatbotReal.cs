using BlazorIA.DTOs;
using BlazorIA.Utilidades;
using Microsoft.Extensions.AI;

namespace BlazorIA.Servicios.Chatbots
{
    public class ChatbotReal : IChatbot
    {
        private string modelo;
        private readonly IChatClientFactory chatClientFactory;
        private readonly ChatOptions chatOptions;
        private readonly List<ChatMessage> mensajes = [];
        private readonly Queue<ToolApprovalRequestContent> aprobacionesPendientes = new();
        private CancellationTokenSource? _ctsActual;

        public List<MensajeChatUI> Conversacion { get; } = [];
        public bool EstaProcesando { get; private set; }
        public event Action? OnChange;
        public SolicitudAprobacionUI? AprobacionPendiente { get; private set; }

        public ChatbotReal(IChatClientFactory chatClientFactory, ChatOptions chatOptions)
        {
            modelo = ModelosIA.ObtenerModeloPorDefecto;
            this.chatClientFactory = chatClientFactory;
            this.chatOptions = chatOptions;
            var systemPromptGeneral = """
            Eres un asistente que responde preguntas generales.
            Debes responder en español.
            Las respuestas deben ser en texto plano, no usar formatos como markdown.
            Las respuestas deben ser concisas a menos que te indiquen lo contrario.

            Si un tool falla, lee el mensaje de la excepción para ver si puedes arreglarlo haciendo algún ajuste. Comunícale al usuario cualquier ajuste que vayas a hacer.
            """;

            mensajes.Add(new ChatMessage(ChatRole.System, systemPromptGeneral));
        }

        public void CancelarRespuestaActual()
        {
            if (EstaProcesando)
            {
                _ctsActual?.Cancel();
            }
        }

        public async Task EnviarMensajeAsync(string textoUsuario, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(textoUsuario))
            {
                return;
            }

            if (EstaProcesando || AprobacionPendiente is not null)
            {
                return;
            }

            try
            {
                EstaProcesando = true;
                _ctsActual = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

                Conversacion.Add(new MensajeChatUI
                {
                    Rol = RolMensaje.Usuario,
                    Texto = textoUsuario
                });

                mensajes.Add(new ChatMessage(ChatRole.User, textoUsuario));

                Conversacion.Add(new MensajeChatUI
                {
                    Rol = RolMensaje.IA,
                    Texto = string.Empty
                });

                NotificarCambio();
                await ProcesarRespuesta(_ctsActual.Token);
            }
            catch (OperationCanceledException)
            {
                ManejarOperacionCancelada();
            }
            finally
            {
                ManejarFinally();
            }
        }

        private void ManejarOperacionCancelada()
        {
            if (Conversacion.Count > 0 && Conversacion[^1].Rol == RolMensaje.IA)
            {
                if (string.IsNullOrWhiteSpace(Conversacion[^1].Texto))
                {
                    Conversacion[^1].Texto = "[Respuesta cancelada]";
                } else
                {
                    Conversacion[^1].Texto += " [cancelado]";
                }
            }
        }

        private void ManejarFinally()
        {
            _ctsActual?.Dispose();
            _ctsActual = null;
            EstaProcesando = false;
            NotificarCambio();
        }


        private async Task ProcesarRespuesta(CancellationToken cancellationToken)
        {
            var updates = new List<ChatResponseUpdate>();

            var cliente = chatClientFactory.Crear(modelo);

            await foreach (var update in cliente.GetStreamingResponseAsync(mensajes, chatOptions,
                                            cancellationToken: cancellationToken))
            {
                updates.Add(update);

                foreach (var content in update.Contents)
                {
                    if (content is TextContent textContent)
                    {
                        Conversacion[^1].Texto += textContent.Text;
                        NotificarCambio();
                    }
                }
            }

            var respuesta = updates.ToChatResponse();
            mensajes.AddMessages(respuesta);

            var solicitudesAprobacion = respuesta.Messages
            .SelectMany(m => m.Contents)
            .OfType<ToolApprovalRequestContent>()
            .ToList();

            if (solicitudesAprobacion.Count > 0)
            {
                foreach (var solicitud in solicitudesAprobacion)
                {
                    aprobacionesPendientes.Enqueue(solicitud);
                }

                // Removemos el mensaje vacío de la IA.
                if (string.IsNullOrWhiteSpace(Conversacion[^1].Texto))
                {
                    Conversacion.RemoveAt(Conversacion.Count - 1);
                }

                MostrarSiguienteAprobacionPendiente();
                NotificarCambio();
                return;
            }
        }

        private void MostrarSiguienteAprobacionPendiente()
        {
            if (aprobacionesPendientes.Count == 0)
            {
                AprobacionPendiente = null;
                return;
            }

            var solicitudAprobacion = aprobacionesPendientes.Dequeue();

            if (solicitudAprobacion.ToolCall is FunctionCallContent functionCall)
            {
                AprobacionPendiente = new SolicitudAprobacionUI
                {
                    SolicitudAprobacion = solicitudAprobacion,
                    NombreTool = ConvertirNombreDeFuncion(functionCall.Name),
                    Argumentos = functionCall.Arguments?.ToDictionary(x => x.Key, x => x.Value) ?? []
                };
            }

        }

        private void NotificarCambio() => OnChange?.Invoke();

        public async Task ResolverAprobacionAsync(bool aprobada, CancellationToken cancellationToken = default)
        {
            if (AprobacionPendiente is null || EstaProcesando)
            {
                return;
            }

            try
            {
                EstaProcesando = true;
                _ctsActual = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                var respuestaAprobacion = AprobacionPendiente.SolicitudAprobacion.CreateResponse(aprobada);
                mensajes.Add(new ChatMessage(ChatRole.User, [respuestaAprobacion]));
                AprobacionPendiente = null;

                Conversacion.Add(new MensajeChatUI
                {
                    Rol = RolMensaje.Sistema,
                    Texto = aprobada ? "Acción aprobada por el usuario." : "Acción rechazada por el usuario."
                });

                AprobacionPendiente = null;
                MostrarSiguienteAprobacionPendiente();

                if (AprobacionPendiente is not null)
                {
                    EstaProcesando = false;
                    NotificarCambio();
                    return;
                }

                Conversacion.Add(new MensajeChatUI
                {
                    Rol = RolMensaje.IA,
                    Texto = string.Empty
                });

                NotificarCambio();
                await ProcesarRespuesta(_ctsActual.Token);
            }
            catch (OperationCanceledException)
            {
                ManejarOperacionCancelada();
            }
            finally
            {
                ManejarFinally();
            }
        }

        private static string ConvertirNombreDeFuncion(string nombre)
        {
            return nombre switch
            {
                "EnviarCorreo" => "Enviar correo",
                _ => nombre
            };
        }

        public void SetearModelo(string modelo)
        {
            this.modelo = modelo;
        }
    }
}
