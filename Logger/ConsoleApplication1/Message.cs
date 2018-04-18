using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ConsoleApplication1;
using BlockingQueueTest;
using System.Threading;

namespace ConsoleApplication1
{
    //enum for different error levels
    public enum ErrorLevel { Debug, Information, Warning, Error };
    //Message Constructor
    public class Message
    {
        private BlockingQueue<Message> Queue = new BlockingQueue<Message>();
        public  string MessageText { get; set; }
        public  ErrorLevel ErrorLevel { get; set; }
        public  DateTime TimeStamp { get; set; }

    }
}   
