namespace ProyectoGrupal.Models
{
    public class Rol
    {

        public int idRol { get; set; }
        public string descripcion { get; set; } = string.Empty;
        public int esActivo { get; set; }
        public DateTime fechaRegistro { get; set; }

    }
}
