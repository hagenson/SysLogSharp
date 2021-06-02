using Syslog.Server;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syslog.Server.BoundHandlers
{
  public class MSSqlStorer : IDataStore
  {
    private string query;

    public MSSqlStorer(IDictionary<string, string> settings)
    {
      // Get the connection string
      conn = new SqlConnection(settings["connectionString"]);
      query = settings["query"];
    }

    public bool StoreMessages(IEnumerable<string[]> messages)
    {
      Trace.WriteLine("Store messages");
      lock (conn)
      {
        conn.Open();
        try
        {
          using (SqlCommand cmd = conn.CreateCommand())
          {
            foreach (string[] msg in messages)
            {
              cmd.CommandText =  String.Format(query, msg.Select(val => val != null ? val.Replace("'", "''"): null).ToArray());
              cmd.ExecuteNonQuery();
            }
          }
        }
        finally
        {
          conn.Close();
        }
      }
      return true;
    }

    private SqlConnection conn;
  }
}
