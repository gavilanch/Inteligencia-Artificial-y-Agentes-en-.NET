using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BlazorIA.Servicios;

internal class ServicioObtenerCorreoFalso
{
    [Description("Obtiene el correo de una persona")]
    public string ObtenerCorreo([Description("Nombre de la persona")]string nombre) => $"{nombre}@ejemplo.com";
}
