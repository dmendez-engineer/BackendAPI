using System.Collections.ObjectModel;

namespace BackendAPI.Entity
{
    public class RequestDone
    {
        public string IdCliente { get; set; }
        public Collection<IndividualRequest> LineaPedido { get; set; }
        public int Total { get; set; }


        public RequestDone() { }

    
    }
}
