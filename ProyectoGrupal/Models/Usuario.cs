namespace ProyectoGrupal.Models
{
    public class Usuario
    {
        public int idUsuario { get; set; }
        public string nombre { get; set; } = string.Empty;
        public string apePaterno { get; set; } = string.Empty;
        public string apeMaterno { get; set; } = string.Empty;
        public string? correo { get; set; }
        public string? telefono { get; set; }
        public int idRol { get; set; }
        public string? nombreRol { get; set; }
        public string? clave { get; set; }
        public int esActivo { get; set; }

        public DateTime FecRegistro { get; set; }

    }
}
