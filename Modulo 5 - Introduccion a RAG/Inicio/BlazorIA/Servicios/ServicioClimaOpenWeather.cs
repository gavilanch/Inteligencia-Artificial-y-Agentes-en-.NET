using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace BlazorIA.Servicios
{
    internal class ServicioClimaOpenWeather(HttpClient httpClient, IConfiguration configuration) : IServicioClima
    {
        public async Task<string> ObtenerClima(string ciudad)
        {
            var apiKey = configuration.GetValue<string>("CLIMA_API_KEY");
            var ciudadURL = Uri.EscapeDataString(ciudad);
            var url = $"http://api.weatherapi.com/v1/current.json?key={apiKey}&q={ciudadURL}&aqi=no&lang=es";
            var weatherResponse = await httpClient.GetFromJsonAsync<WeatherResponse>(url);
            return weatherResponse!.Current.Condition.Text;
        }

        public class WeatherResponse
        {
            [JsonPropertyName("current")]
            public Current Current { get; set; } = default!;
        }

        public class Current
        {
            [JsonPropertyName("condition")]
            public Condition Condition { get; set; } = default!;
        }

        public class Condition
        {
            [JsonPropertyName("text")]
            public string Text { get; set; } = default!;
        }

    }
}
