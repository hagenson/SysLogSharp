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
    public class DbStorer: IDataStore
    {
        public DbStorer()
        {
            Trace.WriteLine("Creating DbStorer");
            // Get the connection string
            conn = new OleDbConnection(ConfigurationManager.ConnectionStrings["trafficLog.Connection"].ConnectionString);
        }

        public bool StoreMessages(IEnumerable<string[]> messages)
        {
            Trace.WriteLine(String.Format("Writing {0} messages", messages.Count()));
            conn.Open();
            try
            {
                foreach (string[] msg in messages)
                {
                OleDbCommand cmd = conn.CreateCommand();
                cmd.CommandText = @"insert into traffic_log ([date], [src], [dst], [sent], [rcvd])
values (?, ?, ?, ?, ?)";
                    foreach(string val in msg)
                    {
                        cmd.Parameters.Add(new OleDbParameter { DbType = System.Data.DbType.String, Value = msg });
                    }
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                conn.Close();
            }

            return true;
        }

        private OleDbConnection conn;
    }
}
