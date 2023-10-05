using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DiPlayer_CSharp.Models.BusinessLogicClasses
{
    public enum PlaybackState
    {
        Stopped = 0,
        Playing = 1,
        Paused = 2
    }

    public class PlaybackControl : INotifyPropertyChanged
    { // Play/Pause/ Stop buttons only

        private MainModel? mainModelRef;

        private PlaybackState m_state;

        private readonly string m_playIconResourcePath;
        private readonly string m_pauseIconResourcePath;
        BitmapImage m_bitmapImage;

        public event PropertyChangedEventHandler? PropertyChanged;

        public PlaybackState PlaybackState { get => m_state; set { SetState(value); } }
        public string PlayIconPath { get; }
        public string PauseIconPath { get; }
        public string StopIconPath { get; }


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private PlaybackControl() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public PlaybackControl(ref MainModel mainModelRef)
        {
            this.mainModelRef = mainModelRef;
            m_state = PlaybackState.Stopped;
            PlayIconPath = "";
            PauseIconPath = "";
            StopIconPath = "";

            m_playIconResourcePath = @"\Resources\Images\PlayIcon.png";
            m_pauseIconResourcePath = @"\Resources\Images\PauseIcon.png";
            BitmapImage = new BitmapImage(new Uri(m_playIconResourcePath, UriKind.Relative));
        }

        private void SetState(PlaybackState value)
        {
            switch(value)
            {
                    case PlaybackState.Stopped:
                    {
                        mainModelRef?.Logic.AudioLibrary.StopPlayback();
                        m_state = PlaybackState.Stopped;
                    } break;

                    case PlaybackState.Playing:
                    {
                        if(m_state == PlaybackState.Paused)
                        {
                            mainModelRef?.Logic.AudioLibrary.ResumePlayback();
                        }
                        else
                        {
                            mainModelRef?.Logic.AudioLibrary.StartPlayback();
                        }
                        m_state = PlaybackState.Playing;
                    } break;

                    case PlaybackState.Paused:
                    {
                        mainModelRef?.Logic.AudioLibrary.PausePlayback();
                        m_state = PlaybackState.Paused;
                    } break;

                default: { } break;
            }

            RefreshPlayPauseIcon();
        }

        private void RefreshPlayPauseIcon()
        {
            if((PlaybackState.Paused == PlaybackState) || (PlaybackState.Stopped == PlaybackState))
            {
                BitmapImage = new BitmapImage(new Uri(m_playIconResourcePath, UriKind.Relative));
            } else
            if (PlaybackState.Playing == PlaybackState)
            {
                BitmapImage = new BitmapImage(new Uri(m_pauseIconResourcePath, UriKind.Relative));
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public BitmapImage? BitmapImage
        {
            get => m_bitmapImage;
            set
            {
                m_bitmapImage = value;
                OnPropertyChanged(nameof(BitmapImage));
            }
        }


    }
}
