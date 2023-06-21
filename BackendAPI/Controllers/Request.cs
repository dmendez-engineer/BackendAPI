using BackendAPI.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;

namespace BackendAPI.Controllers
{
    [Route("api/request")]
    [ApiController]
    public class Request : ControllerBase
    {

        [HttpGet("getRequest/{id}")]
        public Collection<RequestEntity> GetRequests(int id)
        {
            Collection<RequestEntity> response = new Collection<RequestEntity>();

            using(var conn= new SqlConnection(UI.cadenaSql))
            {
                using(SqlCommand cmd = new SqlCommand("dbo.getRequests", conn))
                {
                    conn.Open();
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = id;
                    var adaptador = new SqlDataAdapter(cmd);
                    var reader = adaptador.SelectCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        RequestEntity r = new RequestEntity();
                        r.Name_Customer = reader.GetString(0);
                        r.Name_Product = reader.GetString(1);
                        r.Cantidad = reader.GetInt32(2);
                        r.Total = reader.GetDecimal(3);
                        r.FechaPedido = reader.GetDateTime(4);
                        response.Add(r);
                    }

                    conn.Close();
                }
            }
            
            return response;

        } 
    }
}
