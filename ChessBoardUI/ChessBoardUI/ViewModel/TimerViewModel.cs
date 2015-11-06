using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ChessBoardUI.ViewModel
{

    public enum Participant
    {
        Player,
        PC
    }

    class TimerViewModel: ViewModelBase
    {
        private DispatcherTimer _timer;
        private TimeSpan _time;
        private RelayCommand _start;
        private RelayCommand _stop;
        private Participant _participant;
        private String _display;

        public String Display
        {
            get { return this._display; }
            set
            {
                this._display = value; RaisePropertyChanged(() => this.Display);
            }
        }


        public Participant Participant
        {
            get { return this._participant; }
            set { this._participant = value;  RaisePropertyChanged(() => this.Participant);
            }
        }

        public DispatcherTimer TimerDispatcher
        {
            get { return this._timer; }
            set {
                this._timer = value;
                this._timer.Tick += new EventHandler(DispatcherTimer_Tick);
                this._timer.Interval = new TimeSpan(0, 0, 1); 
                this._display = this._time.ToString("c");
                RaisePropertyChanged(() => this.TimerDispatcher);
            }
        }
        public TimeSpan TimeSpan
        {
            get { return this._time; }
            set { this._time = value; }
        }


        public RelayCommand StartCountDown
        {
            get { return this._start; }
            set { this._start = new RelayCommand(this.startClock); }
        }

        public RelayCommand StopCountDown
        {
            get { return this._stop; }
            set { this._stop = new RelayCommand(this.stopClock); }
        }


        public void startClock()
        {
            this._timer.Start();
        }

        public void stopClock()
        {
            this._timer.Stop();
        }


        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
           
            if (this._time == TimeSpan.Zero) this._timer.Stop();
            this._time = this._time.Add(TimeSpan.FromSeconds(-1));
            this.Display = this._time.ToString("c");   //change the text of the propeerty that are binding with a dependencyobject
           
        }
    }
}
