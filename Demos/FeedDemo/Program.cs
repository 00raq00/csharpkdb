﻿using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using kx;
using NLog;

namespace FeedDemo
{
  static class Program
  {
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private const string QFunc = ".u.upd";
    private const string TableName = "mytable";

    static void Main()
    {
      string host = "localhost";
      int port = 5001;
      string usernamePassword = $"{Environment.UserName}:mypassword";

      c connection = null;
      try
      {
        connection = new c(host, port, usernamePassword);

        //Example of 10 single row inserts to a table
        InsertRows(connection);

        //Parallel example of 100 single row inserts to a table
        ParallelInsertRows(host, port, usernamePassword, 100, 10, 4, false);

        //Parallel example of 1000 single row inserts to a table
        ParallelInsertRows(host, port, usernamePassword, 1000, 100, 300, false);

        //Parallel example of 1000 single row inserts to a table
        ParallelInsertRows(host, port, usernamePassword, 1000, 1000, 1000, false);

        //Example of bulk inserts to a table to improve throughput
        BulkInsertRows(connection);

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

    private static void InsertRows(c connection)
    {
      DateTime dt = DateTime.Now;
      // Single row insert - not as efficient as bulk insert
      Logger.Info("Populating '{0}' table on kdb server with 10 rows...", TableName);

      for (int i = 0; i < 10; i++)
      {
        // Assumes a remote schema of mytable:([]time:`timespan$();sym:`symbol$();price:`float$();size:`long$())
        object[] row = new object[]
        {
                    new c.KTimespan(100),
                    "SYMBOL",
                    93.5,
                    300L
        };

        connection.ks(QFunc, TableName, row);
      }

      Logger.Info("Successfully inserted 10 rows to {0}, {1} ms", TableName, (DateTime.Now - dt).TotalMilliseconds);
    }
    private static void ParallelInsertRows(string host, int port, string usernamePassword, int rowCount, int minThreads, int maxDegreeOfParallelism, bool debugLog)
    {
      DateTime dt = DateTime.Now;
      // Single row insert - not as efficient as bulk insert
      Logger.Info("Populating '{0}' table on kdb server with {1} rows...", TableName, rowCount);
      ThreadPool.SetMinThreads(minThreads, minThreads);

      var parallelOptions = new ParallelOptions
      {
        MaxDegreeOfParallelism = maxDegreeOfParallelism
      };

      Parallel.For(0, rowCount, parallelOptions, i =>
      {
        {
          // Assumes a remote schema of mytable:([]time:`timespan$();sym:`symbol$();price:`float$();size:`long$())
          object[] row = new object[]
          {
                    new c.KTimespan(i),
                    "SYMBOL",
                    (Double)i,
                    300L
          };

          var c = new c(host, port, usernamePassword);
          c.ks(QFunc, TableName, row);

          if (debugLog) Logger.Info("Successfully inserted {1} row to {0}", TableName, i);
        }
      });

      Logger.Info("Successfully inserted {1} rows to {0}, {2} ms", TableName, rowCount, (DateTime.Now - dt).TotalMilliseconds);
    }

    private static void BulkInsertRows(c connection)
    {
      // Bulk row insert - more efficient
      string[] syms = new[] { "ABC", "DEF", "GHI", "JKL" };

      c.KTimespan[] times = CreateTestArray(i => new c.KTimespan(i), 1000);
      string[] symbols = CreateTestArray(i => syms[RandomNumberGenerator.GetInt32(syms.Length)], 1000);
      double[] prices = CreateTestArray(i => i * 1.1, 1000);
      long[] sizes = CreateTestArray(i => (long)(i * 100), 1000);

      DateTime dt = DateTime.Now;
      Logger.Info("Bulk populating '{0}' table on kdb server without using column names", TableName);

      connection.ks(QFunc, TableName, new object[] { times, symbols, prices, sizes });
      
      Logger.Info("Successfully bull inserted {1} rows to {0} without using column names, {2} ms", TableName, 1000, (DateTime.Now - dt).TotalMilliseconds);
      
      dt = DateTime.Now;

      Logger.Info("Bulk populating '{0}' table on kdb server using column names", TableName);

      connection.ks(QFunc, TableName, new c.Flip(new c.Dict(new string[] { "time", "sym", "price", "size" }, new object[] { times, symbols, prices, sizes })));

      Logger.Info("Successfully bull inserted {1} rows to {0} with using column names, {2} ms", TableName, 1000, (DateTime.Now - dt).TotalMilliseconds);

      //block until all messages are processed
      connection.k(string.Empty);
    }

    private static T[] CreateTestArray<T>(Func<int, T> elementBuilder, int arraySize)
    {
      T[] array = new T[arraySize];

      for (int i = 0; i < arraySize; i++)
      {
        array[i] = elementBuilder(i);
      }
      return array;
    }
  }
}