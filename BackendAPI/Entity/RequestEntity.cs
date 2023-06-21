namespace BackendAPI.Entity
{
    public class RequestEntity
    {
        public string Name_Customer { get; set; }
        public string Name_Product { get; set; }
        public int Cantidad { get;set; }
        public Decimal Total { get; set; }
        public DateTime FechaPedido { get; set; }

        public RequestEntity()
        {

        }
    
    }
}
