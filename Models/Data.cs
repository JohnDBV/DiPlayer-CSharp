using DiPlayer_CSharp.Models.DataClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiPlayer_CSharp.Models
{
    public class Data : INotifyPropertyChanged
    {
        private MainModel parent;

        public IGenericPlaylist Playlist { get; set; }

        private int m_oldSelectedIndex;
        private int m_currentSelectedIndex;        

        public event PropertyChangedEventHandler? PropertyChanged;


        #region Properties
        public bool DoubleClickedTheSameTrack { get; set; }
        public int SelectedTrackIndex { get => m_currentSelectedIndex; set { OnSelectedIndexChanged(value); } }
        public AudioTrack SelectedTrack { get; set; }

        #endregion

        public Data(MainModel mainModel)
        {
            parent = mainModel;
            Playlist = new InternalPlaylist("Untitled Playlist");
            InitData();
            SelectedTrack = new AudioTrack();
        }

        private void InitData()
        {
            //selected indexes
            m_oldSelectedIndex = -1;
            m_currentSelectedIndex = -1;

            DoubleClickedTheSameTrack = false;
        }

        public void NotifyPlaylistOpened()
        {
            OnPropertyChanged(nameof(Playlist));
        }

        private void OnSelectedIndexChanged(int value)
        {
            if (m_oldSelectedIndex != value)
            {
                m_oldSelectedIndex = m_currentSelectedIndex = value;//surprise :)
                ChangeAndPlayTrackWithIndex(value);
                OnPropertyChanged(nameof(SelectedTrackIndex));
            }
            else
            {
                if(DoubleClickedTheSameTrack)
                {/*It may happen for the playback to stop,and double click the same selected track without losing focus.
                   Let's restart the playback for the current item, if so.
                   The SelectedIndex won't be changed, no event will be raised, just the "DoubleClickedTheSameTrack"
                   property value will be reset.
                 */
                    ChangeAndPlayTrackWithIndex(value);
                    DoubleClickedTheSameTrack = false;
                }
            }

            SelectedTrack = Playlist.Items[SelectedTrackIndex];
        }

        public void PlayTheTrackAgain()
        {//The index is not changed, so we expose this method for the "Repeat-one" option of playback
            //In any other case , we just change the "SelectedTrackIndex" property

            ChangeAndPlayTrackWithIndex(SelectedTrackIndex);
        }

        private void ChangeAndPlayTrackWithIndex(int index)
        {
            //update track visual
            SelectedTrack = (index >= 0) ? Playlist.Items[index] : new AudioTrack();

            parent.Logic.PlaybackControl.PlaybackState = BusinessLogicClasses.PlaybackState.Stopped;
            parent.Logic.AudioLibrary.OpenFile(SelectedTrack.FilePath, libZPlay.TStreamFormat.sfAutodetect);
            parent.Logic.PlaybackControl.PlaybackState = BusinessLogicClasses.PlaybackState.Playing;
            LoadSelectedTrackStorageData();
        }

        private void LoadSelectedTrackStorageData()
        {//This info it may or (NOT) be present

            var result = parent.Logic.GetSelectedTrackStorageData(SelectedTrack);
            SelectedTrack.StorageTrackName = result.Item1;
            SelectedTrack.StorageArtist = result.Item2;
            SelectedTrack.StorageAlbum = result.Item3;
            SelectedTrack.StorageBitmap = result.Item4;
            OnPropertyChanged(nameof(SelectedTrack));
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
