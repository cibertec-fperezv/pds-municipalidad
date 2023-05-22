namespace ProyectoGrupal.Models
{
    public class CambiarClave
    {
        public int idUsuario { get; set; }
        public string claveActual { get; set; } = string.Empty;
        public string claveNueva { get; set; } = string.Empty;

    }
}
