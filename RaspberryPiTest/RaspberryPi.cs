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

        public RPi_IO(int pin, int divider)
        {
            this.pin = pin;
#if DEBUG
            Console.WriteLine("Setze Pin: " + pin + " auf pwm");
            Console.WriteLine("Setze Divider: " + divider);
#else
            // divider = (1/f)*18600
            // divider = 93  = 200 Hz
            //           186 = 100 Hz
            
            Process.Start("gpio", "mode " + pin + " pwm");  // Set Pin to PWM mode
            Process.Start("gpio", "pwm-ms");                // disable balanced pwm mode
            Process.Start("gpio", "pwmr 1024");             // Set the PWM Range to 1024
            Process.Start("gpio", "pwmc " + divider);       // Set the PWM divider 
            
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
        private Process p;

        public mpc_Control()
        {
            p = new Process();
            p.StartInfo.FileName = "mpc";
        }

        public void initStream(String stream)
        {
            stopStream();
#if DEBUG
#else            
            p.StartInfo.Arguments = "--wait clear";
            p.Start();
            p.WaitForExit();

            p.StartInfo.Arguments = "--wait add " + stream;
            p.Start();
            p.WaitForExit();
#endif
        }

        public void startStream(String stream)
        {
#if DEBUG
#else
            p.StartInfo.Arguments = "--wait play";
            p.Start();
            p.WaitForExit();
#endif
        }

        public void stopStream()
        {
#if DEBUG
#else
            p.StartInfo.Arguments = "--wait stop";

            p.Start();
            p.WaitForExit();
#endif
        }

        public void setVolume(int value)
        {
#if DEBUG
#else
            p.StartInfo.Arguments = "--wait volume "+value;

            p.Start();
            p.WaitForExit();
#endif
        }
    }
}
