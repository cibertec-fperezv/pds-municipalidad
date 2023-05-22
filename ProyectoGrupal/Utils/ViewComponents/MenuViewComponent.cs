using ProyectoGrupal.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace ProyectoGrupal.Utils.ViewComponents
{
    public class MenuViewComponent:ViewComponent
    {
        private readonly IConfiguration _configuration;

        public MenuViewComponent(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        //METODOS PARA RECIBIR DATOS DEL SQL ROLES
        IEnumerable<Menu>listaMenuPadreXId(int idUsuario)
        {
            List<Menu> lista = new List<Menu>();
            using(SqlConnection cn = new SqlConnection(_configuration["ConnectionStrings:cnDB"]))
            {
                SqlCommand cmd = new SqlCommand("usp_ListaMenuPadre", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
                cn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new Menu
                    {
                        idMenu=reader.GetInt32(0),
                        descripcion = reader.GetString(1),
                        idMenuPadre = reader.GetInt32(2),
                        icono = reader.GetString(3),
                        controlador = reader.GetString(4),
                        paginaAccion = reader.GetString(5)

                    });
                }
                cn.Close();
            }
            return lista;
        }

        //METODO PARA LISTAR LOS MENUHIJOS
        IEnumerable<Menu> listaMenuHijoXId(int idUsuario)
        {
            List<Menu> lista = new List<Menu>();
            using (SqlConnection cn = new SqlConnection(_configuration["ConnectionStrings:cnDB"]))
            {
                SqlCommand cmd = new SqlCommand("usp_ListaMenuHijo", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
                cn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new Menu
                    {
                        idMenu = reader.GetInt32(0),
                        descripcion = reader.GetString(1),
                        idMenuPadre = reader.GetInt32(2),
                        icono = reader.GetString(3),
                        controlador = reader.GetString(4),
                        paginaAccion = reader.GetString(5),
                        fechaRegistro = reader.GetDateTime(7)

                    });
                }
                cn.Close();
            }
            return lista;
        }

        public async Task<List<Menu>> ObtenerMenus(int idUsu)
        {

            IEnumerable<Menu> menuPadre = await Task.Run(()=> listaMenuPadreXId(idUsu));
            IEnumerable<Menu> menuHijo = await Task.Run(() => listaMenuHijoXId(idUsu));

            List<Menu> listaMenu = (from mpadre in menuPadre select new Menu(){
                descripcion = mpadre.descripcion,
                icono = mpadre.icono,
                controlador = mpadre.controlador,
                paginaAccion = mpadre.paginaAccion,
                InverseIdMenuPadreNavigation = (from mhijo in menuHijo
                                                where mhijo.idMenuPadre==mpadre.idMenu
                                                select mhijo).ToList()
            }).ToList();

            return listaMenu;
        }


        public async Task<IViewComponentResult> InvokeAsync()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            List<Menu>listaMenus;

            if (claimUser.Identity.IsAuthenticated)
            {
                string? idUsuario = claimUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();

                listaMenus = await ObtenerMenus(int.Parse(idUsuario));
            }
            else
            {
                listaMenus = new List<Menu>{};
            }
            return View(listaMenus);
        }

    }
}
