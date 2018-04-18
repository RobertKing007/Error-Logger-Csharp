using BlockingQueueTest;
using System.Threading;
using System.Threading.Tasks;
using System.Data.SqlClient;
namespace ConsoleApplication1
{
    class Program
    {   
        static void Main(string[] args)
        {      
            //create logger object
            Logger Logger = new Logger();
            Logger.WriteToDatabase = true;
            Logger.WriteToFlatFile = true;

            // create Message
            Logger.Error("Message goes here");
            Logger.Information("Message goes here");
            Logger.Debug("Message goes here");
            Logger.Warning("Message goes here");       
        }
    }   
}
      


