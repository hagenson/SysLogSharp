using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syslog.Server.BoundHandlers
{
  public abstract class BoundHandlerBase
  {
    public BoundHandlerBase(IDictionary<string, string> settings)
    {
      string table = settings["table"];
      // Connect to the database
      IDbConnection con = CreateConnection(settings["connectionString"]);      

      // Get the table columns
      
    }

    protected abstract IDbConnection CreateConnection(string connectionString);

    protected abstract IDictionary<string, DbType> GetColumns(IDbConnection con, string table);
  }
}
