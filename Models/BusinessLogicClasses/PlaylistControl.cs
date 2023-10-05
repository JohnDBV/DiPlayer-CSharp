using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DiPlayer_CSharp.Models.BusinessLogicClasses
{
    public enum RepeatMode
    {
        RepeatOff,
        RepeatAll,
        RepeatOne
    }

    public class PlaylistControl : INotifyPropertyChanged
    {
        private MainModel mainModelRef;
        private RepeatMode m_repeatMode;

        private readonly string m_repeatOffIconResourcePath;
        private readonly string m_repeatAllIconResourcePath;
        private readonly string m_repeatOneIconResourcePath;
        BitmapImage m_bitmapImage;

        public RepeatMode RepeatMode
        { 
            get => m_repeatMode;
            set { SetRepeatMode(value);}
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private PlaylistControl() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public PlaylistControl(ref MainModel mainModelRef)
        {
            this.mainModelRef = mainModelRef;
            RepeatMode = RepeatMode.RepeatOff;

            m_repeatOffIconResourcePath = @"\Resources\Images\RepeatOffIcon96.png";
            m_repeatAllIconResourcePath = @"\Resources\Images\RepeatAllIcon96.png";
            m_repeatOneIconResourcePath = @"\Resources\Images\RepeatOneIcon96.png";

            BitmapImage = new BitmapImage(new Uri(m_repeatOffIconResourcePath, UriKind.Relative));
        }

        public void OnNextSong()
        {
            //Stop the playback - mandatory !
            mainModelRef.Logic.PlaybackControl.PlaybackState = PlaybackState.Stopped;

            if(RepeatMode == RepeatMode.RepeatOne)
            {
                mainModelRef.Data.PlayTheTrackAgain();
            }
            else
            {//RepeatAll or RepeatOff cases :

                if (mainModelRef.Data.SelectedTrackIndex == (mainModelRef.Data.Playlist.Items.Count - 1))
                {//we are on the last track
                    if (RepeatMode == RepeatMode.RepeatAll)
                    {//go back to the track #1.
                        mainModelRef.Data.SelectedTrackIndex = 0;
                    }
                    //else - Do Nothing. The repeat is off.
                }
                else
                {//Not on the last track. Increment the selected track index
                    mainModelRef.Data.SelectedTrackIndex++;
                }
            }
        }

        public void OnPreviousSong()
        {
            bool isPlaybackNotStoppedYet = mainModelRef.Logic.PlaybackControl.PlaybackState != PlaybackState.Stopped;

            //Stop the playback - mandatory !
            mainModelRef.Logic.PlaybackControl.PlaybackState = PlaybackState.Stopped;

            if (isPlaybackNotStoppedYet && !mainModelRef.Logic.ProgressControl.PlaybackPositionOnFirst10Seconds())
            {//if we are advancing through the playback, we should play it again.
             //If it was recently "reset" (sooner than 10 seconds) - change the item on the list with the previous one
                mainModelRef.Data.PlayTheTrackAgain();
            }
            else
            {
                if (mainModelRef.Data.SelectedTrackIndex == 0)
                {//we are positioned on the first track
                    mainModelRef.Data.SelectedTrackIndex = mainModelRef.Data.Playlist.Items.Count - 1;
                }
                else
                {
                    mainModelRef.Data.SelectedTrackIndex--;
                }
            }
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

        private void SetRepeatMode(RepeatMode value)
        {
            m_repeatMode = value;
            OnPropertyChanged(nameof(RepeatMode));
        }

        public void RefreshRepeatIcon()
        {
            switch (RepeatMode)
            {
                case RepeatMode.RepeatOff:
                { 
                    BitmapImage = new BitmapImage(new Uri(m_repeatOffIconResourcePath, UriKind.Relative));
                }break;
                case RepeatMode.RepeatAll:
                {
                    BitmapImage = new BitmapImage(new Uri(m_repeatAllIconResourcePath, UriKind.Relative));
                }break;
                case RepeatMode.RepeatOne:
                {
                    BitmapImage = new BitmapImage(new Uri(m_repeatOneIconResourcePath, UriKind.Relative));
                }break;

                default: { } break;
            }
        }
    }
}
