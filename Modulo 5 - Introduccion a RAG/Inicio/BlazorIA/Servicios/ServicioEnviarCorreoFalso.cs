using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BlazorIA.Servicios;

internal class ServicioEnviarCorreoFalso
{
    [Description("Envia un correo a un destinatario. ")]
    public Task EnviarCorreo(
        [Description("Cuerpo del correo")] string cuerpo, 
        [Description("Asunto del correo")] string asunto, 
        [Description("Correo del destinatario")] string destinatario)
    {

        if (!string.IsNullOrWhiteSpace(asunto) && asunto.Length > 0)
        {
            var primeraLetra = asunto[0].ToString();

            if (primeraLetra != primeraLetra.ToUpper())
            {
                throw new Exception("Error con el asunto del correo. La primera letra de este debe ser mayúscula");
            }

        }

        Console.WriteLine("Enviando el correo...");

        Console.WriteLine($"""
            
            Destinatario: {destinatario}
            Asunto: {asunto}

            Cuerpo: 
            
            {cuerpo}

            """);

        Console.WriteLine("Correo enviado...");

        return Task.CompletedTask;
    }
}
