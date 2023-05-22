using Microsoft.AspNetCore.Mvc;
using ProyectoGrupal.Models;
using Microsoft.Data.SqlClient;
using ProyectoGrupal.Utils;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;



namespace ProyectoGrupal.Controllers
{
    public class AccesoController : Controller
    {
        private readonly IConfiguration _configuration;

        public AccesoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //METODO PARA BUSCAR POR CREDENCIALES DE USUARIO
        public Usuario buscarXCredenciales(string correo, string clave)
        {
            Utilidades utils = new Utilidades();
            string clave_encriptada = utils.ConvertirSha256(clave);

            Usuario? u;
            using (SqlConnection cn = new SqlConnection(_configuration["ConnectionStrings:cnDB"]))
            {
                SqlCommand cmd = new SqlCommand("usp_Iniciar_Sesion", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Correo", correo);
                cmd.Parameters.AddWithValue("@Clave", clave_encriptada);
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    u = new Usuario();
                    u.idUsuario = dr.GetInt32(0);
                    u.nombre = dr.GetString(1);
                    u.apePaterno = dr.GetString(2);
                    u.apeMaterno = dr.GetString(3);
                    u.correo = dr.GetString(4);
                    u.telefono = dr.GetString(5);
                    u.idRol = dr.GetInt32(6);

                }
                else
                {
                    u = null;
                }
                cn.Close();
            }
            return u;
        }

        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            if (claimUser.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Usuario usu)
        {
            Usuario usuario_encontrado = await Task.Run(() => buscarXCredenciales(usu.correo, usu.clave));
            if (usuario_encontrado == null)
            {
                ViewBag.mensaje = "Credencial incorrecta";
                return View();
            }
            ViewBag.mensaje = null;
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,usuario_encontrado.nombre),
                new Claim(ClaimTypes.NameIdentifier,usuario_encontrado.idUsuario.ToString()),
                new Claim(ClaimTypes.Role,usuario_encontrado.idRol.ToString()),
            };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
                );
            return RedirectToAction("Index", "Home");
        }
    }
}
