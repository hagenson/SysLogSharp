using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syslog.Server.BoundHandlers
{
  public class SqlActivator: IHandlerActivator<IParser>, IHandlerActivator<IDataStore>
  {
    private SqlConnection con;
    private string connectionString;
    private string table;
    private List<KeyValuePair<string, SqlDbType>> columns;       

    IParser IHandlerActivator<IParser>.Activate(IDictionary<string, string> settings)
    {
      InitConnection(settings);

      throw new NotImplementedException();
    }

    IDataStore IHandlerActivator<IDataStore>.Activate(IDictionary<string, string> settings)
    {
      throw new NotImplementedException();
    }

    private void InitConnection(IDictionary<string, string> settings)
    {
      if (connectionString != settings["connectionString"] || table != settings["table"])
      {
        if (con != null)
        {
          con.Dispose();
          con = null;
        }

        connectionString = settings["connectionString"];
        table = settings["table"];
        con = new SqlConnection(connectionString);
        DataTable data = con.GetSchema("Tables", new string[] { null, null, table, null });
        columns = new List<KeyValuePair<string, SqlDbType>>();
        data.Rows.Cast<DataRow>()
          .All(row =>
          {
            columns.Add(
              new KeyValuePair<string, SqlDbType>(
                Convert.ToString(row["COLUMN_NAME"]), (SqlDbType)row["DATA_TYPE"]));
            return true;
          });
        
      }
    }
  }
}
