using DataDB.Model;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDB
{
    public class Connection:DataConnection
    {
    public Connection():base("default") { }
    
    public ITable<ClienteModel> _Cliente { get { return this.GetTable<ClienteModel>(); } }

    }
}
