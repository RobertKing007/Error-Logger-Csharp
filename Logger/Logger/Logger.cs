using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;

namespace Logger
{
    public class Logger
    {
        public static void WriteToFile()
        {
            using (StreamWriter sw = File.CreateText(@"C:\Users\Rober\Documents\Visual Studio 2015\Projects\ConsoleApplication1\ConsoleApplication1\bin\Debug\test.txt"))
            {
                sw.WriteLine("Please find the below generated table of 1 to 10");
            }
        }
    }
}
