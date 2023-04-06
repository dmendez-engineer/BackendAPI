using BackendAPI.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using System.Data.SqlTypes;

namespace BackendAPI.Controllers
{
    [Route("api/cliente")]
    [ApiController]
    public class Cliente : ControllerBase
    {

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
                        lista.Add(objE);
                    }
                }

            }
            
            
            return lista;
        }
        [HttpPost("postCliente")]
        public async Task<TransaccionEntidad> PostCliente(ClienteEntity cliente)
        {
            var result = new TransaccionEntidad();

            using (var conn = new SqlConnection(UI.cadenaSql))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.sp_postCliente", conn))
                {
                    conn.Open();
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Nombre", System.Data.SqlDbType.VarChar,100).Value=cliente.Nombre;
                    cmd.Parameters.Add("@Email", System.Data.SqlDbType.VarChar, 100).Value = cliente.Email;
                    cmd.ExecuteNonQuery();
                    conn.Close();

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
                    conn.Close();
                }

            }

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
