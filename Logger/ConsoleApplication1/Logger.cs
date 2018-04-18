using BlockingQueueTest;
using System.IO;
using System;
using System.Threading;
using System.Configuration;
using System.Collections.Specialized;
using System.Data.SqlClient;
namespace ConsoleApplication1
{
    
    
public class Logger {
        public static bool WriteToFlatFile { get; set; }
        public static bool WriteToDatabase { get; set; }
        private static Thread t = null;
        private static object ThreadLock = new object();
        public Logger()
        {
            lock (ThreadLock)
            {
                if (t == null)
                {
                    t = new Thread(new ThreadStart(Logger.Log));
                    t.Start();
                }
            }
        }
        //instance of BlockingQueues's queue
        private static BlockingQueue<Message> Queue = new BlockingQueue<Message>();
        // Methods to create messages for each error level
        private void EnqueueMessage(string Message, ErrorLevel Level)
        {
            Message LogMessage = new Message();
            LogMessage.MessageText = Message;
            LogMessage.ErrorLevel = Level;
            LogMessage.TimeStamp = DateTime.Now;
            Queue.Enqueue(LogMessage);
        }
        public  void Debug(string Message)
        {
            EnqueueMessage(Message, ErrorLevel.Debug);
        }
        public void Information(string Message)
        {
            EnqueueMessage(Message, ErrorLevel.Information);
        }
        public  void Warning(string Message)
        {
            EnqueueMessage(Message, ErrorLevel.Warning);
        }
        public  void Error(string Message)
        {
           EnqueueMessage(Message, ErrorLevel.Error);
        }

        private static void FileWrite(Message LogMessage, string path)
        {
            using (StreamWriter w = new StreamWriter(path, true))
            {
                w.WriteLine(String.Format(@"{0} {1} {2}", LogMessage.TimeStamp, LogMessage.ErrorLevel, LogMessage.MessageText));
                w.Close();
            }
        }

        private static void DatabaseWrite(Message LogMessage, SqlConnection conn)
        {
            try
            {
                string insert = "INSERT INTO Logger.dbo.Log (EventDate, LogType, LogMessage) VALUES (@TimeStamp, @ErrorLevel, @MessageText)";
                using (SqlCommand cmd = new SqlCommand(insert, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("TimeStamp", LogMessage.TimeStamp));
                    cmd.Parameters.Add(new SqlParameter("ErrorLevel", LogMessage.ErrorLevel));
                    cmd.Parameters.Add(new SqlParameter("MessageText", LogMessage.MessageText));
                    int rowsAffected = cmd.ExecuteNonQuery();
                    Console.WriteLine("RowsAffected: {0}", rowsAffected);
                }
            }
            catch (Exception ex)
            {
                string Message = "Could not insert row. ";
                Console.WriteLine(Message + ex.Message);
                throw new Exception(Message, ex);
            }

        }


        //Method Writes to file and Dequeues     
        private static void Log()
        {
            Message LogMessage = null;
            //path for text file location
            string path = ConfigurationManager.AppSettings["LogFile"];
            string ConnectionString = ConfigurationManager.ConnectionStrings["Logger"].ConnectionString;
            SqlConnection conn = new SqlConnection(ConnectionString);
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                throw new Exception("Could not open database connection.", ex);
            }

            while (true)
            {
                //Dequeue item
                LogMessage = Queue.Dequeue();
                if(WriteToDatabase) DatabaseWrite(LogMessage, conn);
                if(WriteToFlatFile) FileWrite(LogMessage, path);
            }
        }
    }   
}


