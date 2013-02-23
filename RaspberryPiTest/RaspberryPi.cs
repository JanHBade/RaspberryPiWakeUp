using System;
using System.Diagnostics;

namespace RaspberryPi
{
    class RPi_IO
    {


        public RPi_IO( int frequency)
        {

            Console.WriteLine("Setze PWM-Frequenz: " + frequency);
            frequency = (int)((1.0 / (float)frequency) * 18600.0);
            Console.WriteLine("Setze PWM-Divider: " + frequency);


            // divider = (1/f)*18600
            // divider = 93  = 200 Hz
            //           186 = 100 Hz
#if DEBUG
#else
            Process.Start("gpio", "mode 1 pwm");
            Process.Start("gpio", "pwm-ms");
            Process.Start("gpio", "pwmr 1024");
            Process.Start("gpio", "pwmc " + frequency);
#endif
            
        }

        public void setPWM(int value, bool InvertedPWM)
        {
#if DEBUG
            Console.WriteLine("Setze PWM auf: " + value);
#else
            if( InvertedPWM )
                Process.Start("gpio", "pwm 1 " + (1024-value));
            else
                Process.Start("gpio", "pwm 1 " +  value);
#endif
        }

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
