namespace BackendAPI.Entity
{
    public class TransaccionEntidad
    {

        
        public int success { get; set; } = 0;
        public string mensaje { get; set; } = "";
        public string? token { get; set; }
        public ProductoEntity? producto { get; set; }
        public ClienteEntity? cliente { get; set; }
        
        public TransaccionEntidad()
        {

        }
    }
}
