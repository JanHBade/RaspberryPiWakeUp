﻿using System;
using System.Diagnostics;

namespace RaspberryPi
{
    class RPi_IO
    {
        public RPi_IO(int pin, String mode)
        {
            this.pin = pin;
#if DEBUG
            Console.WriteLine("Setze Pin: " + pin + " auf " + mode);
#else
            Process.Start("gpio", "mode " + pin + " " + mode);
#endif
        }

        public void setPWM(int value)
        {
#if DEBUG
            Console.WriteLine("Setze Pin: " + pin + " auf " + value);
#else
            Process.Start("gpio", "pwm " + pin + " " + value);
#endif
        }

        public int pin;
    }

    public class mpc_Control
    {
        private Process p=new Process();        

        public void startStream(String stream)
        {
            p.StartInfo.FileName = "mpc";
            p.StartInfo.Arguments = "--wait clear";
            
            p.Start();
            p.WaitForExit();           
                        
            p.StartInfo.Arguments = "--wait add "+stream;

            p.Start();
            p.WaitForExit();

            p.StartInfo.Arguments = "--wait play";

            p.Start();
            p.WaitForExit();

            p = null;
        }

        public void stopStram()
        {
            p.StartInfo.Arguments = "--wait stop";

            p.Start();
            p.WaitForExit();

            p = null;
        }

        public void setVolume(int value)
        {
            p.StartInfo.Arguments = "--wait volume "+value;

            p.Start();
            p.WaitForExit();

            p = null;
        }
    }
}
