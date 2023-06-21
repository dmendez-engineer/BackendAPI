namespace BackendAPI.Entity
{
    public class ProductoEntity
    {

        public int Id { get; set; }
        public String Nombre { get; set; }
        public Decimal Precio { get; set; }
        public String Descripcion { get; set; }

        public ProductoEntity() { }
    
    }
}
