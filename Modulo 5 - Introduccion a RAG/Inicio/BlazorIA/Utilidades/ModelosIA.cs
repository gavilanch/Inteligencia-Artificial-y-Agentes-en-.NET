namespace BlazorIA.Utilidades
{
    public static class ModelosIA
    {
        private static readonly Dictionary<string, string> Modelos = new(StringComparer.OrdinalIgnoreCase)
        {
            ["gpt-5.4-nano"] = "openai",
            ["gpt-5.4"] = "openai",
            ["claude-haiku-4-5"] = "claude",
            ["claude-sonnet-4-5"] = "claude"
        };

        public static string ObtenerProveedor(string modelo)
        {
            if (Modelos.TryGetValue(modelo, out var proveedor))
            {
                return proveedor;
            }

            throw new ArgumentException($"Modelo no soportado: {modelo}");
        }

        public static IEnumerable<string> ObtenerModelosDisponibles() => Modelos.Keys;
        public static string ObtenerModeloPorDefecto => "gpt-5.4-nano";
    }
}
