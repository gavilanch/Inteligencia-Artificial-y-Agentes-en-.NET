using Microsoft.Extensions.AI;

namespace BlazorIA.DTOs
{
    public class SolicitudAprobacionUI
    {
        public required ToolApprovalRequestContent SolicitudAprobacion { get; set; }
        public required string NombreTool { get; set; }
        public Dictionary<string, object?> Argumentos { get; set; } = [];
    }
}
