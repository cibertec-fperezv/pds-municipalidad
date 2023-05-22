namespace ProyectoGrupal.Models
{
    public class Menu
    {
        public int idMenu { get; set; }
        public string? descripcion { get; set; }
        public int idMenuPadre { get; set; }
        public string? icono { get; set; }
        public string? controlador { get; set; }
        public string? paginaAccion { get; set; }
        public DateTime fechaRegistro { get; set; }
        public ICollection<Menu>?InverseIdMenuPadreNavigation { get; set; }
    }
}
