using kx;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedAdvancedRealDemo
{
  internal class Feeder
  {
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private const string QFunc = ".u.upd";
    private const string TableName = "myrealtable";

    string host = "localhost";
    int port = 5001;
    string usernamePassword = $"{Environment.UserName}:mypassword";


    public void Feed(MarketEntry MarketEntry)
    {

      c connection = null;
      try
      {
        connection = new c(host, port, usernamePassword);

        //Example of 1 single row inserts to a table
        InsertRows(connection, MarketEntry);


      }
      catch (Exception ex)
      {
        Logger.Error($"Error occurred running Feed-Demo. \r\n{ex}");
      }
      finally
      {
        if (connection != null)
        {
          connection.Close();
        }
      }
    }

    private void InsertRows(c connection, MarketEntry marketEntry)
    {
      DateTime dt = DateTime.Now;
      // Single row insert - not as efficient as bulk insert
      Logger.Info("Populating '{0}' table on kdb server with 1 row...", TableName);

      
        // Assumes a remote schema of mytable:([]time:`timespan$();sym:`symbol$();price:`float$();size:`long$())
        object[] row =
        [
                    new c.KTimespan((long)marketEntry.Timestamp),
                   marketEntry.Code,
                    marketEntry.Open
        ];

        connection.ks(QFunc, TableName, row);
      

      Logger.Info("Successfully inserted 1 row to {0}, {1} ms", TableName, (DateTime.Now - dt).TotalMilliseconds);
    }
  }
}