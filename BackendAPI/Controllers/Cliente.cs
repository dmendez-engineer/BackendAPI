using BackendAPI.Entity;
using DataDB;
using DataDB.Model;
using Logic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using System.Data.SqlTypes;
using System.Security.Cryptography.Xml;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace BackendAPI.Controllers
{
    [Route("api/cliente")]
    [ApiController]
    public class Cliente : ControllerBase
    {
        private IConfiguration config;

        public Cliente(IConfiguration config)
        {
            this.config = config;
        }
        private string GenerateJWT(UserEntity c)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name,c.Password),
                new Claim(ClaimTypes.Email,c.Email),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("sdgsdgsdgsdgsdgsdgsdgsecretKeysdfsdfsdgsdgsdgsdgsdgsdgsdgsdgsdgsd"));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var securityToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMonths(1),
                signingCredentials: creds);
            string token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return token;
        }
        [HttpGet("GetAllClientes")]
        public Collection<ClienteEntity> GetClientes()
        {
           var lista= new Collection<ClienteEntity>();

            using (var conn= new SqlConnection(UI.cadenaSql))
            {
                // using (var adaptador = new SqlDataAdapter("Select Id,Nombre,Email from CLiente", conn))
                using (SqlCommand cmd= new SqlCommand("dbo.getCliente",conn))
                {
                    
                    conn.Open();
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                   // cmd.CommandText = "dbo.getCliente";
                    //  var reader = adaptador.SelectCommand.ExecuteReader();
                    var adaptador = new SqlDataAdapter(cmd);
                    var reader = adaptador.SelectCommand.ExecuteReader();
                    
                    while(reader.Read())
                    {
                        var objE = new ClienteEntity();
                        objE.Id = reader.GetInt32(0);
                        objE.Nombre = reader.GetString(1);
                        objE.Email = reader.GetString(2);
                        objE.FechaAlta = reader.GetDateTime(3);
                        lista.Add(objE);
                    }
                }

            }
            
            
            return lista;
        }
    
        
        [HttpPost("login")]
        public async Task<TransaccionEntidad> Login(UserEntity user)
        {
            var result = new TransaccionEntidad();
            ClienteEntity c = new ClienteEntity();
           // var lista = new List<ClienteEntity>();
            var response = new TransaccionEntidad();
            string token = GenerateJWT(user);
            string ePassword = "";
            if (user.TipoUsuario == "Costumer")
            {
                 ePassword = Encrypt.HASH_SHA1(user.Password);
            }
            else
            {
                 ePassword = user.Password;
            }
          
            

            using (var conn = new SqlConnection(UI.cadenaSql))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.login", conn))
                {
                    conn.Open();
                    
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@email", System.Data.SqlDbType.VarChar, 500).Value = user.Email;
                    cmd.Parameters.Add("@password", System.Data.SqlDbType.VarChar, 500).Value = ePassword;
                    cmd.Parameters.Add("@tipoUsuario", System.Data.SqlDbType.VarChar, 500).Value = user.TipoUsuario;
                    var adapter = new SqlDataAdapter(cmd);
                    var reader = adapter.SelectCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        c.Nombre = reader.GetString(0);
                        c.Email = reader.GetString(1);
                        c.Id=reader.GetInt32(2);
                        c.FechaAlta = reader.GetDateTime(3);
                        c.FechaBaja=reader.GetDateTime(4);
                        //lista.Add(c);
                        response.cliente = c;
                        response.token = token;
                        response.success = 202;
                        response.mensaje = "Usuario autenticado";
                    }

                }
            }
           
            if (c.Nombre != null)
            {
               // return Ok( new {res=response }); //It works if the return value is async Task<IActionResult>
                return response;
            }
            else
            {
                //ista.Add(new ClienteEntity("Las credenciales son incorrectas",404));
                response.mensaje = "Credenciales incorrectas";
                response.success = 400;
               // return Ok(new {res?})
                return response;
            }
            
            
            
        }
        [HttpGet("validarEmail")]
        public int ValidarEmail(string email)
        {
            //[validarEmail]


            int resultado = 0;
            using (var conn = new SqlConnection(UI.cadenaSql))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.validarEmail", conn))
                {
                    conn.Open();
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@email", System.Data.SqlDbType.VarChar, 500).Value = email;
                    var adapter = new SqlDataAdapter(cmd);
                    var reader = adapter.SelectCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        resultado = reader.GetInt32(0);
                    }
                    conn.Close();
                }
            }
            return resultado;

        }

        [HttpPost("postCliente")]
        //[HttpPost("postCliente/${token}")]
        public async Task<TransaccionEntidad> PostCliente(ClienteEntity cliente)
        {
            var result = new TransaccionEntidad();
            bool emailRepetido = false;
            using (var conn = new SqlConnection(UI.cadenaSql))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.PostCliente", conn))
                {

                    emailRepetido = ValidarEmail(cliente.Email) == 1 ? true : false;

                    if (emailRepetido == false)
                    {
                        cliente.Password = Encrypt.HASH_SHA1(cliente.Password); 
                        
                        conn.Open();
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Nombre", System.Data.SqlDbType.VarChar, 100).Value = cliente.Nombre;
                        cmd.Parameters.Add("@Email", System.Data.SqlDbType.VarChar, 100).Value = cliente.Email;
                        cmd.Parameters.Add("@Password", System.Data.SqlDbType.VarChar, 500).Value = cliente.Password;
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        result.success = 200;
                        result.mensaje = "This costumer was added!";
                    }
                    else
                    {
                        result.success = 404;
                        result.mensaje = "This email already exist!";
                    }

                    
                }
            }

                return result;
        }
        [HttpDelete("deleteCliente/{id}")]
        public async Task<TransaccionEntidad>DeleteCliente(int id)
        {
            var result = new TransaccionEntidad();

            using (var conn = new SqlConnection(UI.cadenaSql))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.sp_DeleteCliente", conn))
                {
                    conn.Open();
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id",System.Data.SqlDbType.Int,100).Value=id;
                    cmd.ExecuteNonQuery();
                    result.success = 200;
                    result.mensaje = "The customer was removed succesfully";
                    conn.Close();
                }

            }
           // return Ok(new { res = result });
            return result;
        }
        [HttpPut("updateCliente/{id}")]
        public async Task<TransaccionEntidad>PutCliente(int id,ClienteEntity c)
        {
            var result = new TransaccionEntidad();

            using(var conn=new SqlConnection(UI.cadenaSql))
            {
                using(SqlCommand cmd= new SqlCommand("dbo.sp_updateCliente", conn))
                {
                    conn.Open();
                    cmd.CommandType=System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@nombre", System.Data.SqlDbType.VarChar, 100).Value = c.Nombre;
                    cmd.Parameters.Add("@email", System.Data.SqlDbType.VarChar, 100).Value = c.Email;
                    cmd.Parameters.Add("@id", System.Data.SqlDbType.Int, 100).Value = id;
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    result.success = 200;
                }
            }
            return result;
        }
    }
    public static class UI
    {
        public static String cadenaSql { get; set; } = string.Empty;
    }
}
