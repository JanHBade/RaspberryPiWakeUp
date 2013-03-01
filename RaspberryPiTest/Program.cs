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
        static String file = "actions.xml";

        static void Main(string[] args)
        {
            Console.CancelKeyPress += delegate(object sender, ConsoleCancelEventArgs e)
            {
                keepRunning = false;
                e.Cancel = true;                
            };

            if(args.Length==1)
                file=args[0];

#if DEBUG
            DateTime now = DateTime.Now;
            Console.WriteLine("Year: " + now.Year);
            Console.WriteLine("Month: " + now.Month);
            Console.WriteLine("Day: " + now.Day);
            Console.WriteLine("DayOfWeek: " + now.DayOfWeek);
            Console.WriteLine("DateTime.Now: " + DateTime.Now);
            Console.WriteLine(DateTime.Now.ToString("dd.mm.yyyy")+"\n");
#endif

            try
            {                
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
                    ta.Start();
                }

                while (keepRunning)
                {
                    Thread.Sleep(1000);
                }

                foreach (TimedAction ta in actionstodo)
                {
                    ta.RequestStop();
                }
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
            actionstodo = (List<TimedAction>)deserializer.Deserialize(textReader);
            textReader.Close();
        }        

        static void FSW_Changed(object sender, FileSystemEventArgs e)
        {
            FSW.EnableRaisingEvents = false;    //wegen doppelter Events die Events ausschalten
                       
#if DEBUG
            Console.WriteLine("Main: Lese neu ein!");
#endif
            foreach (TimedAction ta in actionstodo)
            {
                ta.RequestStop();
            }

            Thread.Sleep(10000);

            readfromXML();

            foreach (TimedAction ta in actionstodo)
            {
                if (ta.StartTime.compare())
                    ta.StartTime.addOneDay();

                ta.Start();
            }

            FSW.EnableRaisingEvents = true; //File Watcher wieder aktivieren
        }   
    }        
}
