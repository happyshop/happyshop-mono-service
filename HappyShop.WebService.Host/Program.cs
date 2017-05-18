using System;
using System.Threading;
using log4net;

namespace HappyShop.WebService.Host
{
  class Program
  {
    private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    static void Main(string[] args)
    {
      Configuration.Logger.Setup();

      string serviceBind = Configuration.Static.Merged.ServiceBind;
      Console.WriteLine("Starting HappyShop WebService Host on port " + serviceBind);

      using (var host = new AuthenticatedWebServiceHost(typeof(Service), new Uri(serviceBind)))
      {
        Console.CancelKeyPress += delegate(object sender, ConsoleCancelEventArgs e)
        {
          e.Cancel = true;
          StopEvent.Set();
        };

        try
        {
          Console.Write("Starting HappyShop web service host ... ");
          host.Open();
          Console.WriteLine("done.");
        }
        catch (Exception)
        {
          Console.WriteLine("failed.");
          throw;
        }
        
        Console.WriteLine("HappyShop web service host running.");
        Console.WriteLine("Press CTRL+C to gracefully shut down the host.");
        StopEvent.WaitOne();

        try
        {
          Console.Write("Closing HappyShop web service host ... ");
          host.Close();
          Console.WriteLine("done.");
        }
        catch (Exception)
        {
          Console.WriteLine("failed.");
          throw;
        }
      }
    }

    private static readonly ManualResetEvent StopEvent = new ManualResetEvent(false);
  }
}
