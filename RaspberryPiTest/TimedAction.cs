using System;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using RaspberryPi;
using System.Xml.Serialization;

namespace Timed
{
    public class TimedAction_PWM : TimedAction
    {
        public int LED_Pin { get; set; }
        public int divider { get; set; }

        public TimedAction_PWM():base()
        {
            LED_Pin = 1;
        }

        protected override void initAction()
        {
            base.initAction();
            if (divider!=0)
                pwm_pin = new RPi_IO(LED_Pin, divider);
            else
                pwm_pin = new RPi_IO(LED_Pin, "pwm");

            stopAction();
        }

        protected override void startAction()
        {
            base.startAction();            
            act_pwm_value = pwm_werte[(Value - 1) / 2];
            Console.WriteLine(Name + " init PWM Value: " + act_pwm_value);
#if DEBUG
#else
            pwm_pin.setPWM(act_pwm_value);
#endif
        }

        protected override void Ramp_active()
        {
            base.Ramp_active();            
            act_pwm_value = pwm_werte[(Value - 1) / 2];
            Console.WriteLine(Name + " Ramp PWM Value: " + act_pwm_value);
#if DEBUG            
#else
            pwm_pin.setPWM(act_pwm_value);
#endif
        }

        protected override void stopAction()
        {
            base.stopAction();
            act_pwm_value = 0;
            Console.WriteLine(Name + " Stop PWM Value: " + act_pwm_value);
#if DEBUG            
#else
            pwm_pin.setPWM(act_pwm_value);
#endif
        }

        protected override void forceAction()
        {
            base.forceAction();
            act_pwm_value = pwm_werte[(Value - 1) / 2];
            Console.WriteLine(Name + " Force PWM Value: " + act_pwm_value);
#if DEBUG
#else
            pwm_pin.setPWM(act_pwm_value);
#endif
        }
        
        private RPi_IO pwm_pin;
        private int[] pwm_werte ={10,
                        11,12,13,15,16,18,19,21,23,26,
                        28,31,34,38,41,45,50,55,60,66,
                        73,80,88,96,106,117,128,141,155,
                        170,187,205,226,248,273,300,329,
                        362,398,437,481,528,580,638,701,
                        771,847,931,1023};
        private int act_pwm_value;
    }

    public class TimedAction_Music : TimedAction
    {
        public String Stream { get; set; }

        public TimedAction_Music() :base()
        {
            control = new mpc_Control();
        }

        protected override void initAction()
        {
            base.initAction();
            Console.WriteLine(Name + " init Vol: " + Value + " Stream: " + Stream);
#if DEBUG
#else
            control.setVolume(0);
            control.initStream(Stream);
#endif
        }

        protected override void startAction()
        {
            base.startAction();
            Console.WriteLine(Name + " start Vol: " + Value);
#if DEBUG
#else
            control.setVolume(Value);
            control.startStream(Stream);
#endif
        }

        protected override void Ramp_active()
        {
            base.Ramp_active();
            Console.WriteLine(Name + " Ramp Vol: " + Value);
#if DEBUG
#else
            control.setVolume(Value);
#endif
        }

        protected override void stopAction()
        {
            base.stopAction();
            Console.WriteLine(Name + " Stop Vol: " + Value);
#if DEBUG
#else
            control.stopStream();
#endif
        }

        protected override void forceAction()
        {
            base.forceAction();
#if DEBUG
#else
            control.startStream(Stream);
            control.setVolume(Value);
#endif
        }

        private mpc_Control control;
    }

    [XmlInclude(typeof(TimedAction_PWM))]
    [XmlInclude(typeof(TimedAction_Music))]
    public class TimedAction
    {
        public String Name { get; set; }        
        public myTime StartTime {get; set;}
        public bool active { get; set; }
        public bool force { get; set; }
        public int RampTime { get; set; }
        public int LagTime { get; set; }        
        public int Value_max { get; set; }
        public List<DayOfWeek> daystowork { get; set; }

        public void Start()
        {
            if (active)
            {
                newThread = new Thread(this.DoWork);
                newThread.Start();
            }
        }

        public void RequestStop()
        {
            shouldStop = true;
        }

        protected virtual void initAction()
        {
            Value = 0;
#if DEBUG
            Console.WriteLine(Name + " b_init Action..." + Value + " " + DateTime.Now.Minute + ":" + DateTime.Now.Second);
#endif
        }

        protected virtual void startAction()
        {
            Value = 1;
#if DEBUG
            Console.WriteLine(Name + " b_start Action..." + Value + " " + DateTime.Now.Minute + ":" + DateTime.Now.Second);
#endif
        }

        protected virtual void stopAction()
        {
            Value = 0;
#if DEBUG
            Console.WriteLine(Name + " b_stop Action..." + Value + " " + DateTime.Now.Minute + ":" + DateTime.Now.Second);
#endif
        }

        protected virtual void Ramp_active()
        {
            Value++;
#if DEBUG
            Console.WriteLine(Name + " b_Ramp active " + Value + " " + DateTime.Now.Minute + ":" + DateTime.Now.Second);            
#endif
        }

        protected virtual void forceAction()
        {
            Value = Value_max;
#if DEBUG
            Console.WriteLine(Name + " b_force " + Value + " " + DateTime.Now.Minute + ":" + DateTime.Now.Second);
#endif
        }

        protected int Value;

        private void DoWork()
        {
            shouldStop = false;
            initAction();
            if (force)
                forceAction();
            else
            {
                while (!shouldStop)
                {
                    if (StartTime.compare())
                    {
                        StartTime.addOneDay();
                        if (daystowork.Contains(DateTime.Now.DayOfWeek))
                        {
                            doAction();
                        }
                    }
#if DEBUG
                    Thread.Sleep(1000);
                    //Console.WriteLine(Name+" worker thread: working..."+day+" "+StartTime.Hour+":"+StartTime.Minute);
#else
                Thread.Sleep(5000);
#endif
                }
            }
            newThread = null;
            Console.WriteLine(Name+" worker thread: terminating gracefully.");
        }

        

        private volatile bool shouldStop=false;
        private Thread newThread;
        private System.Timers.Timer t;
        private bool LagTime_active = false;
        private int LagTime_ms;

        private void doAction()
        {
            startAction();
            //Console.WriteLine(Name+" do Action...");
            t = new System.Timers.Timer();
            t.Elapsed += this.doTimerEvent;
            t.Interval = (RampTime*60*1000)/Value_max;
            LagTime_ms = LagTime * 60 * 1000;            
            LagTime_active = false;
            t.Enabled = true;
        }        

        private void doTimerEvent(object source, ElapsedEventArgs e)
        {
            //Console.WriteLine(Name + " do Timer Event...");
            if (LagTime_active)
            {
                stopAction();
                t.Enabled = false;
            }
            else
            {
                Ramp_active();
                if (Value == Value_max)
                {
                    LagTime_active = true;
                    t.Interval = LagTime_ms;
                }
            }
        }        

        
            
    }

    public class myTime
    {
        public int hour { get; set; }
        public int minute { get; set; }

        public myTime()
        {
            day = DateTime.Now.Day;
        }

        public bool compare()
        {
            if (DateTime.Now.Day > day)
                return true;
            else if (DateTime.Now.Day < day)
                return false;
            else
                if (DateTime.Now.Hour > hour)
                    return true;
                else if (DateTime.Now.Hour < hour)
                    return false;
                else
                    if (DateTime.Now.Minute > minute)
                        return true;
                    else
                        return false;
        }

        public void addOneDay()
        {
            day = DateTime.Now.Day + 1;
            if (day > DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month))
                day = 1;
        }

        private int day;
    }
}
