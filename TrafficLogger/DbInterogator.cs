using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syslog.TrafficLogger
{
  public class DbInterogator
  {
    private OleDbConnection connection;

    public DbInterogator(OleDbConnection connection)
    {
      this.connection = connection;
    }

    public IDictionary<string, DbType> GetColumns(string tableName)
    {
      DataTable data = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, tableName, null });
      Dictionary<string, DbType> result = new Dictionary<string, DbType>();
      data.Rows.Cast<DataRow>()
        .All(r =>
        {
          result[Convert.ToString(r["COLUMN_NAME"])] = (DbType)r["DATA_TYPE"];
          return true;
        });

      return result;
    }
  }
}
