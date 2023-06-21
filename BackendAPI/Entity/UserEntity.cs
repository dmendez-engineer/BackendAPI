namespace BackendAPI.Entity
{
    public class UserEntity
    {

        public String? Password { get; set; }
        public String Email { get; set; }
        public String TipoUsuario { get; set; }

        public UserEntity() { }
    }
}
