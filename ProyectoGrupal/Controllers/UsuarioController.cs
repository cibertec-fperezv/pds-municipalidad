using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using ProyectoGrupal.Models;
using ProyectoGrupal.Utils;
using ProyectoGrupal.Utils.Response;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace ProyectoGrupal.Controllers
{
    [Authorize]
    public class UsuarioController : Controller
    {

        private readonly IConfiguration _configuration;

        public UsuarioController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //-- METODOS PARA REALIZAR LAS OPERACIONES
        //LISTAR ROLES
        IEnumerable<Rol> listadoRoles()
        {
            List<Rol> lista = new List<Rol>();
            using (SqlConnection cn = new SqlConnection(_configuration["ConnectionStrings:cnDB"]))
            {
                SqlCommand cmd = new SqlCommand("usp_RolListar", cn);
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lista.Add(new Rol
                    {
                        idRol = dr.GetInt32(0),
                        descripcion = dr.GetString(1)
                    });
                }
                cn.Close();
            }
            return lista;
        }

        //LISTAR USUARIOS
        IEnumerable<Usuario> listadoUsuarios()
        {
            List<Usuario> lista = new List<Usuario>();
            using (SqlConnection cn = new SqlConnection(_configuration["ConnectionStrings:cnDB"]))
            {
                SqlCommand cmd = new SqlCommand("usp_UsuarioListar", cn);
                cn.Open();
                SqlDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    lista.Add(new Usuario
                    {
                        idUsuario = rd.GetInt32(0),
                        nombre = rd.GetString(1),
                        apePaterno = rd.GetString(2),
                        apeMaterno = rd.GetString(3),
                        correo = rd.GetString(4),
                        telefono = rd.GetString(5),
                        nombreRol = rd.GetString(6),
                        esActivo = rd.GetInt32(7),
                        idRol = rd.GetInt32(8)
                    });
                }
                cn.Close();
            }
            return lista;
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
                    u.esActivo = dr.GetInt32(8);
                }
                else
                {
                    u = null;
                }
                cn.Close();
            }
            return u;
        }

        //BUSCAR POR CORREO - VALIDAR NO EXISTENTES
        public Usuario buscarPorCorreo(string correo)
        {
            Usuario? u;
            using (SqlConnection cn = new SqlConnection(_configuration["ConnectionStrings:cnDB"]))
            {
                SqlCommand cmd = new SqlCommand("usp_UsuarioXCorreo", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@correo", correo);
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


        //INSERTAR USUARIO MEDIANTE PROCEDIMIENTO ALMACENADO
        public Usuario insertarUsuario(Usuario usu)
        {

            using (SqlConnection cn = new SqlConnection(_configuration["ConnectionStrings:cnDB"]))
            {
                SqlCommand cmd = new SqlCommand("usp_UsuarioInsertar", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@nombre", usu.nombre);
                cmd.Parameters.AddWithValue("@apePaterno", usu.apePaterno);
                cmd.Parameters.AddWithValue("@apeMaterno", usu.apeMaterno);
                cmd.Parameters.AddWithValue("@correo", usu.correo);
                cmd.Parameters.AddWithValue("@telefono", usu.telefono);
                cmd.Parameters.AddWithValue("@idRol", usu.idRol);
                cmd.Parameters.AddWithValue("@clave", usu.clave);
                cmd.Parameters.AddWithValue("@esActivo", usu.esActivo);
                cn.Open();
                int valor = cmd.ExecuteNonQuery();
                cn.Close();
            }

            Usuario usuario = buscarPorCorreo(usu.correo);

            return usuario;
        }

        //ELIMINAR USUARIO MEDIANTE PROCEDIMIENTO ALMACENADO

        public int eliminarUsuario(int idUsuario)
        {
            int respuesta=-1;
            
            using (SqlConnection cn = new SqlConnection(_configuration["ConnectionStrings:cnDB"]))
            {
                SqlCommand cmd = new SqlCommand("usp_UsuarioEliminar", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
                cn.Open();
                respuesta = cmd.ExecuteNonQuery();
                cn.Close();
            }
            return respuesta;
        }

        //METODO ASYNCRONO PARA ELIMINAR USUARIO
        public async Task<bool> borrarUsuario(int idUsuario)
        {
            try
            {
                Usuario usuario_encontrado = await Task.Run(() => buscarPorId(idUsuario));
                if (usuario_encontrado == null)
                    throw new TaskCanceledException("El usuario no existe");

                int respuesta = eliminarUsuario(usuario_encontrado.idUsuario);

                return true;
            }
            catch
            {
                throw;
            }
        }



        //ENVIAR CORREO LUEGO DE REGISTRO
        public async Task<bool> enviarCorreo(string CorreoDestino, string Asunto, string Mensaje)
        {

            try
            {
                //IEnumerable<Configuracion> lstConfig = await Task.Run(()=> listaConfigu());
                //Dictionary<string, string> config = lstConfig.ToDictionary(keySelector: c => c.propiedad, elementSelector: c => c.valor);

                var credenciales = new NetworkCredential("municipalidadpuentepiedraule@gmail.com", "ejbzxpbkixnnzmim");
                var correo = new MailMessage()
                {
                    From = new MailAddress("municipalidadpuentepiedraule@gmail.com", "UlePuentePiedra.com"),
                    Subject = Asunto,
                    Body = Mensaje,
                    IsBodyHtml = true
                };

                correo.To.Add(new MailAddress(CorreoDestino));

                var clienteServidor = new SmtpClient()
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    Credentials = credenciales,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    EnableSsl = true

                };

                clienteServidor.Send(correo);
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }

        }


        //REGISTRO CON ENVIADO DE CORREO
        public async Task<Usuario> crearUsuario(Usuario entidad, string UrlPlantillaCorreo = "")
        {
            Usuario usuario_existe = await Task.Run(() => buscarPorCorreo(entidad.correo));

            if (usuario_existe != null)
                throw new TaskCanceledException("El correo existe");
            try
            {
                Utilidades u = new Utilidades();

                string clave_generada = u.generarClave();
                entidad.clave = u.ConvertirSha256(clave_generada);

                Usuario usuario_creado = insertarUsuario(entidad);

                if (usuario_creado.idUsuario == 0)
                    throw new TaskCanceledException("no se creo al usuario");

                if (UrlPlantillaCorreo != "")
                {
                    UrlPlantillaCorreo = UrlPlantillaCorreo.Replace("[correo]", usuario_creado.correo).Replace("[clave]", clave_generada);

                    string htmlCorreo = "";

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UrlPlantillaCorreo);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream dataStream = response.GetResponseStream())
                        {
                            StreamReader readerStream = null;

                            if (response.CharacterSet == null)
                                readerStream = new StreamReader(dataStream);
                            else
                                readerStream = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));

                            htmlCorreo = readerStream.ReadToEnd();
                            response.Close();
                            readerStream.Close();

                        }
                    }
                    if (htmlCorreo != "")
                        await Task.Run(() => enviarCorreo(usuario_creado.correo, "Cuenta Creada", htmlCorreo));

                }
                return usuario_creado;

            }
            catch (Exception e)
            {
                throw;
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> listaRoles()
        {
            var lista = await Task.Run(() => listadoRoles());
            IEnumerable<Rol> listaRoles = lista;
            return StatusCode(StatusCodes.Status200OK, listaRoles);

        }

       [HttpGet]
        public async Task<IActionResult> listaUsuarios()
        {
            var lista = await Task.Run(() => listadoUsuarios());
            IEnumerable<Usuario> listaUsu = lista;
            return StatusCode(StatusCodes.Status200OK, new { data = listaUsu });
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] string modelo)
        {
            GenericResponse<Usuario> gResponse = new GenericResponse<Usuario>();

            try
            {
                Usuario? userGson = JsonConvert.DeserializeObject<Usuario>(modelo);

                string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/EnviarClave?correo=[correo]&clave=[clave]";

                Usuario usuario_creado = await crearUsuario(userGson, urlPlantillaCorreo);
                gResponse.Estado = true;
                gResponse.Objeto = usuario_creado;

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);

        }


        [HttpDelete]
        public async Task<IActionResult> Eliminar(int idUsuario)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();

            try
            {
                gResponse.Estado = await borrarUsuario(idUsuario);
            }
            catch(Exception ex)
            {
                gResponse.Estado= false;
                gResponse.Mensaje=ex.Message;

            }

            return StatusCode(StatusCodes.Status200OK, gResponse);

        }


    }
}
