using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorIA.Servicios;

internal class ServicioClimaFalso : IServicioClima
{
    public async Task<string> ObtenerClima(string ciudad)
    {
        return ciudad.ToLower() switch
        {
            "santo domingo" => "Soleado, 32°C",
            "madrid" => "Nublado, 18°C",
            "new york" => "Lluvia ligera, 12°C",
            _ => "No tengo información del clima para esa ciudad"
        };
    }
}
