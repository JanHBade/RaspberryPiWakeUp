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
        static volatile bool keepRunning = true;

        static void Main(string[] args)
        {
            Console.CancelKeyPress += delegate(object sender, ConsoleCancelEventArgs e)
            {
                keepRunning = false;
                e.Cancel = true;                
            };
            /*DateTime now = DateTime.Now;
            Console.WriteLine("Year: " + now.Year);
            Console.WriteLine("Month: " + now.Month);
            Console.WriteLine("Day: " + now.Day);
            Console.WriteLine("DayOfWeek: " + now.DayOfWeek);
            Console.WriteLine("DateTime.Now: " + DateTime.Now);
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd"));*/            

            readfromXML("actions.xml");

            foreach (TimedAction ta in actionstodo)
            {                
                ta.Start();
            }

            while(keepRunning)
            {
                Thread.Sleep(1000);
            }

            foreach (TimedAction ta in actionstodo)
            {
                ta.RequestStop();
            }

            /*TimedAction t = new TimedAction();
            t.Name = "Test1";
            t.StartTime = new DateTime(1900, 1, 1, 12, 55, 0);
            t.RampTime = 5;
            t.Value_max = 50;
            t.LagTime = 2;
            t.daystowork = new List<DayOfWeek>();
            t.daystowork.Add(DayOfWeek.Monday);
            t.daystowork.Add(DayOfWeek.Thursday);
            t.daystowork.Add(DayOfWeek.Wednesday);
            t.daystowork.Add(DayOfWeek.Saturday);

            actionstodo.Add(t);

            XmlSerializer serializer = new XmlSerializer(typeof(List<TimedAction>));
            TextWriter textWriter = new StreamWriter(@"actions.xml");
            serializer.Serialize(textWriter, actionstodo);
            textWriter.Close();*/

            /*Console.WriteLine("Raspberry PI Test");
            RaspberryPi.RPi_IO pwm0 = new RaspberryPi.RPi_IO(1, "pwm");

            for (; ; )
            {
                pwm0.setPWM(1,0);
                System.Threading.Thread.Sleep(500);
                pwm0.setPWM(1, 250);
                System.Threading.Thread.Sleep(500);
                pwm0.setPWM(1, 500);
                System.Threading.Thread.Sleep(500);
                pwm0.setPWM(1, 750);
                System.Threading.Thread.Sleep(500);
                pwm0.setPWM(1, 1000);
                System.Threading.Thread.Sleep(500);
            }*/
        }

        static void readfromXML(String file)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(List<TimedAction>));
            TextReader textReader = new StreamReader(file);            
            actionstodo = (List<TimedAction>)deserializer.Deserialize(textReader);
            textReader.Close();
        }
    }    
    
}
