using Microsoft.AspNetCore.Mvc;
using ProyectoGrupal.Models;
using System.Diagnostics;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

using Microsoft.Data.SqlClient;
using ProyectoGrupal.Utils;
using ProyectoGrupal.Utils.Response;

namespace ProyectoGrupal.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //BUSCAR POR ID - USUARIO

        public Usuario buscarPorId(int idUsuario)
        {
            Usuario? u;
            using (SqlConnection cn = new SqlConnection(_configuration["ConnectionStrings:cnDB"]))
            {
                SqlCommand cmd = new SqlCommand("usp_UsuarioXId", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
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
                    u.clave = dr.GetString(7);
                    u.esActivo = dr.GetInt32(8);
                    u.nombreRol = dr.GetString(9);
                }
                else
                {
                    u = null;
                }
                cn.Close();
            }
            return u;
        }


        //ACTUALIZAR USUARIO DESDE EL SQL
        public int actualizarDatosUsuario(Usuario usuario)
        {
            int resu = -1;
            using (SqlConnection cn = new SqlConnection(_configuration["ConnectionStrings:cnDB"]))
            {
                SqlCommand cmd = new SqlCommand("usp_UsuarioEditarPersonal", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idUsuario", usuario.idUsuario);
                cmd.Parameters.AddWithValue("@correo", usuario.correo);
                cmd.Parameters.AddWithValue("@telefono", usuario.telefono);
                cn.Open();
                resu = cmd.ExecuteNonQuery();
                cn.Close();
            }
            
            return resu;

        }


        //ACTUALIZAR CONTRASÑEA DESDE SQL
        public int actualizarContraseñaUsuario(Usuario usuario)
        {
            int resu = -1;
            using (SqlConnection cn = new SqlConnection(_configuration["ConnectionStrings:cnDB"]))
            {
                SqlCommand cmd = new SqlCommand("usp_UsuarioEditarContraseña", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idUsuario", usuario.idUsuario);
                cmd.Parameters.AddWithValue("@clave", usuario.clave);
                cn.Open();
                resu = cmd.ExecuteNonQuery();
            }
            return resu;

        }

        //GUARDAR DATOS DEL PERFIL DE USUARIO SQL
        public async Task<bool> GuardarPerfilUsuario(Usuario entidad)
        {
            try
            {
                Usuario usuario_encontrado = await Task.Run(() => buscarPorId(entidad.idUsuario));

                if (usuario_encontrado == null)
                    throw new TaskCanceledException("El usuario no existe");

                usuario_encontrado.correo = entidad.correo;
                usuario_encontrado.telefono = entidad.telefono;

                int respuesta = await Task.Run(() => actualizarDatosUsuario(usuario_encontrado));

                return true;

            }
            catch (Exception e)
            {
                return false;
            }

        }

        //GUARDAR CONTRASEÑA USUARIO

        public async Task<bool> GuardarContraseñaUsuario(int idUsuario, string claveActual, string claveNueva)
        {
            Utilidades u = new Utilidades();
            try
            {
                Usuario usuario_encontrado = await Task.Run(() => buscarPorId(idUsuario));
                if (usuario_encontrado == null)
                    throw new TaskCanceledException("No existe el usuario");

                if (usuario_encontrado.clave != u.ConvertirSha256(claveActual))
                    throw new TaskCanceledException("La contraseña ingresada como actual no es correcta");

                usuario_encontrado.clave = u.ConvertirSha256(claveNueva);
                int respuesta = await Task.Run(() => actualizarContraseñaUsuario(usuario_encontrado));

                return true;

            }
            catch (Exception e)
            {
                return false;
            }

        }



        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Perfil()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerUsuario()
        {
            GenericResponse<Usuario> response = new GenericResponse<Usuario>();
            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;

                string idUsuario = claimUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();

                Usuario usuario = await Task.Run(()=> buscarPorId(int.Parse(idUsuario)));

                response.Estado = true;
                response.Objeto = usuario;

            }catch(Exception e)
            {
                response.Estado = false;
                response.Mensaje = e.Message;
            }

            return StatusCode(StatusCodes.Status200OK,response);
        }


        [HttpPost]
        public async Task<IActionResult> GuardarPerfil([FromBody]Usuario modelo)
        {
            GenericResponse<Usuario> response = new GenericResponse<Usuario>();
            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;

                string idUsuario = claimUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();

                Usuario entidad = modelo;
                entidad.idUsuario = int.Parse(idUsuario);

                bool resultado = await GuardarPerfilUsuario(entidad);

                response.Estado = resultado;

            }
            catch (Exception e)
            {
                response.Estado = false;
                response.Mensaje = e.Message;
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }


        [HttpPost]
        public async Task<IActionResult> CambiarClave([FromBody] CambiarClave modelo)
        {
            GenericResponse<bool> response = new GenericResponse<bool>();
            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;

                string idUsuario = claimUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();


                bool resultado = await GuardarContraseñaUsuario(
                int.Parse(idUsuario),modelo.claveActual,modelo.claveNueva);

                response.Estado = resultado;

            }
            catch (Exception e)
            {
                response.Estado = false;
                response.Mensaje = e.Message;
            }

            return StatusCode(StatusCodes.Status200OK, response);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Acceso");
        }


    }
}