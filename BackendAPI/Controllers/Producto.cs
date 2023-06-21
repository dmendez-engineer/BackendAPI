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
                conn.Close();
            }
            
            return response;
        }
        //[sp_DeleteProduct]
        [HttpDelete("deleteProduct/{id}")]
        public async Task<TransaccionEntidad>DeleteProduct(int id)
        {
            TransaccionEntidad response = new TransaccionEntidad();
            using(var conn= new SqlConnection(UI.cadenaSql))
            {
                using(SqlCommand cmd= new SqlCommand("dbo.sp_DeleteProduct", conn))
                {
                    conn.Open();
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = id;
                    SqlParameter output= new SqlParameter("@response", SqlDbType.Int);
                    output.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(output);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        response.success = Convert.ToInt32(cmd.Parameters["@response"].Value);
                        response.mensaje = "The product was deleted";
                    }catch(SqlException ex)
                    {
                        response.success = 404;
                        response.mensaje = "There was an error while deleting this producto";

                    }

                    conn.Close();
                }
            }
            return response;
        }
        [HttpPut("putProducto/{id}")]
        public async Task<TransaccionEntidad> PutProducto(int id,ProductoEntity p)
        {
            TransaccionEntidad response = new TransaccionEntidad();

            using(var conn= new SqlConnection(UI.cadenaSql))
            {
                using(SqlCommand cmd = new SqlCommand("dbo.sp_updateProduct", conn))
                {
                    conn.Open();

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", System.Data.SqlDbType.Int, 100).Value = id;
                    cmd.Parameters.Add("@name", System.Data.SqlDbType.VarChar, 100).Value = p.Nombre;
                    cmd.Parameters.Add("@price", System.Data.SqlDbType.Int, 100).Value = p.Precio;
                    cmd.Parameters.Add("@description", System.Data.SqlDbType.VarChar, 100).Value = p.Descripcion;
                    SqlParameter output = new SqlParameter("@response", SqlDbType.Int);
                    output.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(output);

                    try
                    {
                        cmd.ExecuteNonQuery();
                        response.success = Convert.ToInt32(cmd.Parameters["@response"].Value);
                        response.mensaje = "The produc was updated succesfully";
                    }catch(SqlException ex)
                    {
                        response.success = 404;
                        response.mensaje = "There was error in the updating of this product";
                    }

                    conn.Close();
                }
            }    

            return response;
        }

        [HttpGet("getOneProducto/{id}")]
        public ProductoEntity GetOneProduct(int id)
        {
            var response = new ProductoEntity();
            response.Id = 0;

            using(var conn= new SqlConnection(UI.cadenaSql))
            {
                using(SqlCommand cmd = new SqlCommand("dbo.getOneProduct", conn))
                {
                    conn.Open();
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = id;
                    var adaptador = new SqlDataAdapter(cmd);
                    var reader = adaptador.SelectCommand.ExecuteReader();
                    while(reader.Read())
                    {
                        response.Id = Int32.Parse(reader.GetInt32(0).ToString());
                        response.Nombre = reader.GetString(1);
                        response.Precio=reader.GetDecimal(2);
                        response.Descripcion = reader.GetString(3);
                    }
                    conn.Close();
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
                    conn.Close();
                    }
            }

            return lista;
        }
    
    }
}
