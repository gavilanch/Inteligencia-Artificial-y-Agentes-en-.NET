namespace BlazorIA.Entidades
{
    public class Persona
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public required string Email { get; set; }
        public decimal Salario { get; set; }
        public bool Activo { get; set; }
    }
}
