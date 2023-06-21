using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDB.Model
{
    public class ClienteModel
    {
        [PrimaryKey,Identity]
        public int? Id { get; set; }
        public String Nombre { get; set; }
        public String Password { get; set; }
        public String Email { get; set; }
        public DateTime? FechaAlta { get; set; }
        public DateTime? FechaBaja { get; set; }

    }
}
