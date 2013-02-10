using System;
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

        private int pin;
    }

    public class mpc_Control
    {
        public void startStream(String stream)
        {
        }

        public void stopStram()
        {
        }

        public void setVolume(int value)
        {
        }
    }
}
