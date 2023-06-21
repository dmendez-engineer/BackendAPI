using BackendAPI.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;

namespace BackendAPI.Controllers
{
    [Route("api/producto")]
    [ApiController]
    public class Producto : ControllerBase
    {
        [HttpPost("postProduct")]
        public async Task<TransaccionEntidad> PostProducto(ProductoEntity p)
        {
            var response= new TransaccionEntidad();
                using(var conn= new SqlConnection(UI.cadenaSql))
            {
                using(SqlCommand cmd= new SqlCommand("dbo.PostProduct", conn))
                {
                    conn.Open();
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@nombre", System.Data.SqlDbType.VarChar, 500).Value = p.Nombre;
                    cmd.Parameters.Add("@price", System.Data.SqlDbType.Decimal).Value = p.Precio;
                    cmd.Parameters.Add("@descripcion", System.Data.SqlDbType.VarChar, 500).Value = p.Descripcion;
                    var adapter = new SqlDataAdapter(cmd);
                    
                    SqlParameter varOutput = new SqlParameter("@resultado", System.Data.SqlDbType.Int);
                    varOutput.Direction = ParameterDirection.Output;

                    cmd.Parameters.Add(varOutput);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        int resultado = Convert.ToInt32(cmd.Parameters["@resultado"].Value);
                        if (resultado == 1)
                        {
                            response.success = 200;
                            response.mensaje = "The product was added succesfully";
                        }

                    }
                    catch(SqlException ex)
                    {
                        response.success = 404;
                        response.mensaje = "There was an error while adding the product";
                    }

                }
            }
            
            return response;
        }

        [HttpGet("getProducts")]
    public Collection<ProductoEntity>GetProductoEntities()
        {
            var lista = new Collection<ProductoEntity>();
            
            using (var conn = new SqlConnection(UI.cadenaSql))
            {
                    using(SqlCommand cmd = new SqlCommand("dbo.getProducts",conn)) {
                    conn.Open();
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    var adaptador = new SqlDataAdapter(cmd);
                    var reader = adaptador.SelectCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        var oProducto = new ProductoEntity();
                        oProducto.Id= reader.GetInt32(0);
                        oProducto.Nombre= reader.GetString(1);
                        oProducto.Precio = reader.GetDecimal(2);
                        oProducto.Descripcion=reader.GetString(3);
                        lista.Add(oProducto);
                    }
                
                    }
            }

            return lista;
        }
    
    }
}
