using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiPlayer_CSharp.Models.BusinessLogicClasses
{
    public class TrackProgressControl : INotifyPropertyChanged
    {
        private uint Hour{get; set;}
        private uint Minute { get; set; }
        private uint Second { get; set; }

        private uint TotalHour { get; set; }
        private uint TotalMinute { get; set; }
        private uint TotalSecond { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public string CurrentTime 
        {
            get {//2 digits decimal formatting ("D2")
                    return $"{Hour.ToString("D2")}:{Minute.ToString("D2")}:{Second.ToString("D2")}";
                }
        }

        public string TotalTime
        {
            get
            {//2 digits decimal formatting ("D2")
                return $"{TotalHour.ToString("D2")}:{TotalMinute.ToString("D2")}:{TotalSecond.ToString("D2")}";
            }
        }

        public double Percent { get; set; }//which % of the track is played (for the Slider)

        public TrackProgressControl() { Reset(); }

        public void Reset()
        {
            SetCurrentTime(0, 0, 0);
            SetTotalTime(0, 0, 0);
            Percent = 0;
        }

        public void SetCurrentTime(uint Hour, uint Minute, uint Second)
        {
            this.Hour = Hour;
            this.Minute = Minute;
            this.Second = Second;
            CalculatePercent();
            OnPropertyChanged(nameof(CurrentTime));
        }

        public void SetTotalTime(uint Hour, uint Minute, uint Second)
        {
            TotalHour = Hour;
            TotalMinute = Minute;
            TotalSecond = Second;
            CalculatePercent();
            OnPropertyChanged(nameof(TotalTime));
        }

        public ValueTuple<uint,uint,uint> FromTotalTimeExtractThePercent(double? percent)
        {
            uint totalTimeInSeconds = (TotalHour * 3600) + (TotalMinute * 60) + TotalSecond;
            if(totalTimeInSeconds == 0)
                return ValueTuple.Create((uint)0, (uint)0, (uint)0);


            uint percentTime = Convert.ToUInt32( (percent / 100) * totalTimeInSeconds);

            return ValueTuple.Create(percentTime / 3600, percentTime / 60, percentTime % 60);
        }

        private void CalculatePercent()
        {
            uint currentTimeInSeconds = (Hour * 3600) + (Minute * 60) + Second;
            uint totalTimeInSeconds = (TotalHour * 3600) + (TotalMinute * 60) + TotalSecond;

            if(totalTimeInSeconds == 0)
            {
                Percent = 0;
            }
            else 
                if( (totalTimeInSeconds > 0) && (currentTimeInSeconds == 0))
                {
                    Percent = 0;
                }
                else
                    {
                        Percent = ((double)currentTimeInSeconds / totalTimeInSeconds) * 100;
                    }

            //Notify that the property has changed
            OnPropertyChanged(nameof(Percent));
        }
        
        public bool PlaybackPositionOnFirst10Seconds()
        {//For the "previous track" button click. If this is true, we repeat the current song.
            uint currentTimeInSeconds = (Hour * 3600) + (Minute * 60) + Second;

            return currentTimeInSeconds <= 10;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
