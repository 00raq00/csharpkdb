using System;
using System.Collections;
using System.Threading.Tasks;
using kx;
using NLog;

namespace SubscriberDemo
{
  static class Program
  {
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    static void Main()
    {
      string host = "localhost";
      int port = 5001;
      string usernamePassword = $"{Environment.UserName}:mypassword";

      c connection = null;
      try
      {
        connection = new c(host, port, usernamePassword);
        connection.k("upd:{[t;x].[t;();,;show x]};");
        //open subscription
        connection.ks(".u.sub", "mytable", "SYMBOL");

        bool subscribing = true;

        //start processing subscriptions until user exit or error
        Task.Factory.StartNew(() =>
                {
                  Logger.Info("Processing subscription results. Press any key to exit");
                  while (subscribing)
                  {
                    try
                    {
                      dynamic result = connection.k();

                      Logger.Info($"Received subscription result:{result}");
                      if (result == null || result.Length < 2)
                        continue;

                      kx.c.Flip flip = result[2];
                      string columnValue = "";
                      for (int i = 0; i < flip.x.Length; i++)
                      {
                        IEnumerator enumerator = ((IEnumerable)flip.y[i]).GetEnumerator();
                        enumerator.MoveNext();
                        columnValue += $"{flip.x[i]}:{enumerator.Current} ";
                      }

                      Logger.Info($"Received subscription result:{result[0]} {result[1]} {columnValue}");
                    }
                    catch (Exception ex)
                    {
                      Logger.Error($"Error occurred processing Subscription. Exiting Subscription-Demo {ex}");
                      subscribing = false;
                    }
                  }
                });

        Console.ReadLine();
        subscribing = false;

      }
      catch (Exception ex)
      {
        Logger.Error($"Error occurred running Subscription-Demo. \r\n{ex}");
      }
      finally
      {
        if (connection != null)
        {
          connection.Close();
        }
      }
    }
  }
}