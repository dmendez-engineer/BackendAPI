using BackendAPI.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Data;

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
                        r.Unit_Price=reader.GetDecimal(5);
                        response.Add(r);
                    }

                    conn.Close();
                }
            }
            
            return response;

        }
        [HttpPost("processingRequest")]
        public async Task<TransaccionEntidad> processRequest(RequestDone pedido)
        {
            TransaccionEntidad response = new TransaccionEntidad();
            int lastIndex = 0;
           

            using(var conn= new SqlConnection(UI.cadenaSql))
            {
                using(SqlCommand cmd= new SqlCommand("dbo.postRequest", conn))
                {
                    conn.Open();

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@idCustomer", System.Data.SqlDbType.Int).Value = pedido.IdCliente;
                    cmd.Parameters.Add("@total", System.Data.SqlDbType.Decimal).Value = pedido.Total;

                    var adaptador = new SqlDataAdapter(cmd);
                    try
                    {
                        var reader = adaptador.SelectCommand.ExecuteReader();

                        while (reader.Read())
                        {
                            lastIndex = reader.GetInt32(0);
                        }
                    }
                    catch(SqlException err)
                    {
                        response.success = 400;
                        response.mensaje = "There was an error in the proccesing: "+err.Message;
                        conn.Close();
                        return response;
                    }


                    // using(SqlCommand cmd2= new SqlCommand("dbo.postRequestLine", conn))
                    //{
                         
                        
                         for(int i = 0; i < pedido.LineaPedido.Count; i++)
                        {
                        
                            SqlCommand cmd2 = new SqlCommand("dbo.postRequestLine", conn);
                            cmd2.CommandType = System.Data.CommandType.StoredProcedure;
                        
                            cmd2.Parameters.Add("@idRequest", System.Data.SqlDbType.Int).Value = lastIndex;
                            cmd2.Parameters.Add("@idProduct", System.Data.SqlDbType.Int).Value = pedido.LineaPedido[i].idProducto;
                            cmd2.Parameters.Add("@quantity", System.Data.SqlDbType.Int).Value = pedido.LineaPedido[i].cant;
                            cmd2.Parameters.Add("@unitCost", System.Data.SqlDbType.Decimal).Value = pedido.LineaPedido[i].importeUnitario;
                            try
                            {
                                cmd2.ExecuteNonQuery();
                            }
                            catch(SqlException err)
                            {
                                response.success = 400;
                                response.mensaje = "There was an error in the proccesing: "+err.Message;
                                conn.Close();
                                return response;
                            }
                            

                        }
                        response.success = 200;
                        response.mensaje = "The request was proccesed succesfully";
                    //}
                }

                    conn.Close();
                }
               


            return response;
        }
    }
}
