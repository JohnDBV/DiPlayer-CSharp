using libZPlay;
using DiPlayer_CSharp.Models.BusinessLogicClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DiPlayer_CSharp.Models
{
    public class Logic
    {
        private MainModel parent;

        #region Business Logic Required : 

        //From the external audio library : 
        private ZPlay m_audioLib;

        //Track progress control : 

        private TrackProgressControl m_trackProgress;
        private VolumeControl m_volumeControl;
        private PlaybackControl m_playbackControl;
        private PlaylistControl m_playlistControl;

        //A timer to update data every second : 
        private System.Threading.Timer m_audioLibraryChecksTimer;
        private volatile bool m_isAudioLibraryChecksTimerRunning = true;//volatile boolean to check the timer state
        private static object m_AudioLibraryChecksThreadLock = new object();

        #endregion

        public ZPlay AudioLibrary { get => m_audioLib; }

        public TrackProgressControl ProgressControl { get => m_trackProgress; }
        public VolumeControl VolumeControl { get => m_volumeControl; }
        public PlaybackControl PlaybackControl { get => m_playbackControl; }
        public PlaylistControl PlaylistControl { get => m_playlistControl; }

        public bool IsAudioLibraryChecksTimerRunning//Thread-safe property for the above boolean variable
        {
            get
            {
                lock (m_AudioLibraryChecksThreadLock)
                {
                    return m_isAudioLibraryChecksTimerRunning;
                }
            }
            set
            {
                lock (m_AudioLibraryChecksThreadLock)
                {
                    m_isAudioLibraryChecksTimerRunning = value;
                }
            }
        }

        public Logic(MainModel mainModel)
        {
            parent = mainModel;
            
            m_audioLib = new ZPlay();

            m_trackProgress = new TrackProgressControl();
            m_volumeControl = new VolumeControl(ref parent);
            m_playbackControl = new PlaybackControl(ref parent);
            m_playlistControl = new PlaylistControl(ref parent);

            m_audioLibraryChecksTimer = new System.Threading.Timer(AudioLibraryChecks,
                IsAudioLibraryChecksTimerRunning, 50, 1000);
            //fire AudioLibraryChecks(...) every 1000ms, with 50 ms delay for the timer start 
        }

        public void SeekTo(ValueTuple<uint, uint, uint> hmsPeriod)
        {
            /*  
                The idea to provide hms is apparently not supported by the audio library,
                but the Tuples are cool. So, we keep them for this demo purposes
            */

            uint totalSecondsToSeek = (hmsPeriod.Item1 * 3600) + (hmsPeriod.Item2 * 60) + hmsPeriod.Item3;

            if(0 == totalSecondsToSeek)
            {//seeking to the beginning of the file may instruct the audio library to restart
             //the playback on stopped tracks, which is DANGEROUS. Let's just safely exit.
                return;
            }

            TStreamTime timeToSeek = new TStreamTime();
            timeToSeek.sec = totalSecondsToSeek;

            AudioLibrary.Seek(TTimeFormat.tfSecond, ref timeToSeek, TSeekMethod.smFromBeginning);        
        }

        private void AudioLibraryChecks(object? stateInfo)
        {
            TStreamInfo info = new TStreamInfo();
            AudioLibrary.GetStreamInfo(ref info);
            ProgressControl.SetTotalTime(info.Length.hms.hour, info.Length.hms.minute, info.Length.hms.second);

            TStreamTime pos = new TStreamTime();
            AudioLibrary.GetPosition(ref pos);
            ProgressControl.SetCurrentTime(pos.hms.hour, pos.hms.minute, pos.hms.second);

            //we have no usable "track end event" on the library,so we do the "trick" on the next if statement :
            if (BusinessLogicUtilities.IsPlaybackOnTheLastSecond(pos, info.Length)
                && (PlaybackControl.PlaybackState == PlaybackState.Playing) )
            {
                PlaylistControl.OnNextSong();
            }
        }

        public ValueTuple<string,string,string, BitmapImage?>
            GetSelectedTrackStorageData(in DataClasses.AudioTrack? track)
        {
            string name = "", artist = "", album = "";
            BitmapImage? img = BusinessLogicUtilities.CreateEmptyBitmapImage() as BitmapImage;

            TID3InfoEx info = new TID3InfoEx();
            if (AudioLibrary.LoadID3Ex(ref info, true))
            {
                name = info.Title;
                artist = info.Artist;
                album = info.Album;
                if( (info.Picture.Bitmap.Width > 1) && (info.Picture.Bitmap.Height > 1) )
                {
                    img = BusinessLogicUtilities.BitmapImageFromMemoryStream(info.Picture.BitStream);
                }
            }

            return ValueTuple.Create(name, artist, album, img);
        }

    }
}
