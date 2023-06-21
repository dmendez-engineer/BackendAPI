namespace BackendAPI.Entity
{
    public class ClienteEntity
    {
        public int ?Id { get; set; }
        public String Nombre { get; set; }
        public String ?Password { get; set; }
        public String Email { get; set; }
        public DateTime ?FechaAlta { get; set; }
        public DateTime ?FechaBaja { get; set; }

        public ClienteEntity()
        {

        }
        public ClienteEntity(string nombre,int id) {
        this.Nombre= nombre;
        this.Id = id;
        }


       
    }
}
