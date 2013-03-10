using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Timed;
using System.IO;
using System.Xml.Serialization;
using System.Threading;

namespace RaspberryPiTest
{
    class Program
    {
        static List<TimedAction> actionstodo = new List<TimedAction>();
        static FileSystemWatcher FSW;
        static volatile bool keepRunning = true;
        static volatile bool restart;
        static String file = "actions.xml";

        static void Main(string[] args)
        {
            Console.CancelKeyPress += delegate(object sender, ConsoleCancelEventArgs e)
            {
                keepRunning = false;
                e.Cancel = true;
            };

            if (args.Length == 1)
                file = args[0];

#if DEBUG
            DateTime now = DateTime.Now;
            Console.WriteLine("Year: " + now.Year);
            Console.WriteLine("Month: " + now.Month);
            Console.WriteLine("Day: " + now.Day);
            Console.WriteLine("DayOfWeek: " + now.DayOfWeek);
            Console.WriteLine("DateTime.Now: " + DateTime.Now);
            Console.WriteLine(DateTime.Now.ToString("d.M.yyyy") + "\n");
#endif

            try
            {
                do
                {
                    restart = false;
                    FSW = new FileSystemWatcher();
                    FileInfo fi = new FileInfo(file);

                    // Pfad und Filter festlegen               
                    FSW.Path = fi.DirectoryName;
                    FSW.Filter = fi.Name;

                    // Events definieren
                    FSW.Changed += new FileSystemEventHandler(FSW_Changed);

                    // Filesystemwatcher aktivieren                
                    FSW.EnableRaisingEvents = true;

                    readfromXML();

                    foreach (TimedAction ta in actionstodo)
                    {
                        ta.Name = DateTime.Now.Second + " " + ta.Name;
                        ta.Start();
                    }

                    while (keepRunning)
                    {
                        Thread.Sleep(1000);
                        if (restart) break;
                    }

                    foreach (TimedAction ta in actionstodo)
                    {
                        ta.RequestStop();
                    }
                    Thread.Sleep(10000);
                } while (restart);
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
                Console.WriteLine(exp.StackTrace);
            }
        }

        static void readfromXML()
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(List<TimedAction>));
            TextReader textReader = new StreamReader(file);
            actionstodo.Clear();
            actionstodo = (List<TimedAction>)deserializer.Deserialize(textReader);
            textReader.Close();
        }

        static void FSW_Changed(object sender, FileSystemEventArgs e)
        {
            FSW.EnableRaisingEvents = false;    //wegen doppelter Events die Events ausschalten

            Console.WriteLine("FSW Changed: Fordere Neustart an");
            restart = true;
        }
    }
}
