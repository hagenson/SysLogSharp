using Syslog.Server;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syslog.TrafficLogger
{
  public class DbStorer : AbstractDatabaseDataStore
  {
    public DbStorer(string connectionString)
      : base(connectionString)
    {
      // Get the connection string
      conn = new OleDbConnection(connectionString);
    }

    public override bool StoreMessages(IEnumerable<string[]> messages)
    {
      Trace.WriteLine("Store messages");
      lock (conn)
      {
        conn.Open();
        try
        {
          foreach (string[] msg in messages)
          {
            OleDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"insert into traffic_log ([date], [src], [dst], [sent], [rcvd])
values (?, ?, ?, ?, ?)";
            foreach (string val in msg)
            {
                Trace.WriteLine(val);
                cmd.Parameters.Add(new OleDbParameter { DbType = System.Data.DbType.String, Value = val });
            }
            cmd.ExecuteNonQuery();
          }
        }
        finally
        {
          conn.Close();
        }
      }
      return true;
    }

    private OleDbConnection conn;
  }
}
