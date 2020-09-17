using System;
using System.Timers;

namespace StarBlue.Utilities
{
    public class Threading
    {
        private Timer _timer { get; set; }
        private Action Action { get; set; }

        public Threading()
        {
            _timer = new Timer();
        }

        public void SetSeconds(int time)
        {
            _timer.Interval = time * 1000;
        }

        public void SetMinutes(int time)
        {
            _timer.Interval = time * 1000 * 60;
        }

        public void SetAction(Action Action)
        {
            this.Action = Action;
        }

        public void _timerTick(object sender, ElapsedEventArgs e)
        {
            this.Action();
            _timer.Stop();
            _timer.Dispose();
        }

        public void Start()
        {
            _timer.Elapsed += _timerTick;
            _timer.AutoReset = false;
            _timer.Start();
        }
    }
}
