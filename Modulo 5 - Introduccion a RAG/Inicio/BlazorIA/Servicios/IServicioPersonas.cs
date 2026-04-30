using BlazorIA.Entidades;
using System.ComponentModel;

namespace BlazorIA.Servicios
{
    [Description("Servicio para interactuar con personas")]
    public interface IServicioPersonas
    {
        [Description("Obtiene un listado de todas las personas")]
        Task<IEnumerable<Persona>> ObtenerTodas();
    }
}