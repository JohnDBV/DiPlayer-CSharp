using DiPlayer_CSharp.Commands;
using DiPlayer_CSharp.Models;
using DiPlayer_CSharp.Models.BusinessLogicClasses;
using DiPlayer_CSharp.Models.DataClasses;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DiPlayer_CSharp.ViewModels
{
    public class MainViewModel
    {
        #region Commands area

        private MainModel m_model;

        private ICommand m_openFilesCommand;
        private ICommand m_openPlaylistCommand;
        private ICommand m_savePlaylistCommand;
        private ICommand m_closeCommand;
        private ICommand m_doubleClickCommand;
        private ICommand m_stopButtonCommand;
        private ICommand m_playPauseButtonCommand;
        private ICommand m_nextPreviousRepeatButtonCommand;

        public MainModel Model { get => m_model; }

        public ICommand OpenFiles { get => m_openFilesCommand;}
        public ICommand OpenPlaylist { get => m_openPlaylistCommand;}
        public ICommand SavePlaylistCommand { get => m_savePlaylistCommand; }
        public ICommand Close { get => m_closeCommand;}
        public ICommand DoubleClick { get => m_doubleClickCommand; }
        public ICommand StopButton { get => m_stopButtonCommand; }
        public ICommand PlayPauseButton { get => m_playPauseButtonCommand; }
        public ICommand RepeatButtonCommand { get => m_nextPreviousRepeatButtonCommand; }


        #endregion



        public MainViewModel()
        {
            m_model = new MainModel();
            //m_model.SetParentReference(ref m_model);//strange but working


            m_openFilesCommand = new OpenFilesCommand(this);
            m_openPlaylistCommand = new OpenPlaylistCommand(this);
            m_savePlaylistCommand = new SavePlaylistCommand(this);
            m_closeCommand = new CloseCommand(this);
            m_doubleClickCommand = new DoubleClickCommand(this);
            m_stopButtonCommand = new StopButtonCommand(this);
            m_playPauseButtonCommand = new PlayPauseButtonCommand(this);
            m_nextPreviousRepeatButtonCommand = new NextPreviousRepeatButtonsCommand(this);
        }

        internal void OnOpenFiles()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "MP3 Files|*.mp3|All Files|*.*";
            ofd.Multiselect = true;

            bool? result = ofd.ShowDialog();

            if (result == true)
            {
                foreach (var filePath in ofd.FileNames)
                    Model.Data.Playlist.Items.Add(new AudioTrack(filePath));
            }
            
        }

        internal void OnOpenPlaylist()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Internal Playlist|*.diplaylist|Generic Playlist|*.m3u";
            ofd.Multiselect = false;

            bool? result = ofd.ShowDialog();
            
            if (result == true)
            {
                OpenPlaylistUsingPath(ofd.FileName);
            }
        }

        void OpenPlaylistUsingPath(string path)
        {
            string extension = System.IO.Path.GetExtension(path).Substring(1);

            if ((extension.ToLower() != "diplaylist") &&
                (extension.ToLower() != "m3u"))
            {
                return;
            }

            Model.Data.Playlist = (extension.ToLower() != "diplaylist") ?
                new InternalPlaylist(path) : new UniversalPlaylist(path);

            Model.Data.NotifyPlaylistOpened();//call this to also notify the UI that the playlist has been opened
        }

        internal void OnSavePlaylist()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Internal Playlist|*.diplaylist|Generic Playlist|*.m3u";
            sfd.FileName = Model.Data.Playlist.PlaylistTitle;

            bool? result = sfd.ShowDialog();

            if (result == true)
            {
                Model.Data.Playlist.SavePlaylistAs(sfd.FileName);
                OpenPlaylistUsingPath(sfd.FileName);
            }
        }

        internal void OnClose()
        {
            Application.Current.Shutdown();
        }

        internal void OnDoubleClick(object? parameter)
        {
            int index = Convert.ToInt32(parameter);

            if ((Model.Data.SelectedTrackIndex == index) &&
                (Model.Logic.PlaybackControl.PlaybackState == PlaybackState.Stopped))
                Model.Data.DoubleClickedTheSameTrack = true;

            Model.Data.SelectedTrackIndex = index;
        }

        internal void OnSeekTrack(double? percent)
        {
            var result = Model.Logic.ProgressControl.FromTotalTimeExtractThePercent(percent);
            Model.Logic.SeekTo(result);
        }

        internal void OnStopButtonClick()
        {
            Model.Logic.PlaybackControl.PlaybackState = PlaybackState.Stopped;
        }

        internal void OnPlayPauseButtonClick()
        {
            if (Model.Data.Playlist.Items.Count > 0)
            {

                if (PlaybackState.Playing == Model.Logic.PlaybackControl.PlaybackState)
                {
                    Model.Logic.PlaybackControl.PlaybackState = PlaybackState.Paused;
                }
                else
                {
                    Model.Logic.PlaybackControl.PlaybackState = PlaybackState.Playing;
                }
            }
        }

        internal void OnNextPreviousRepeatButtonCommand(string? parameter)
        {
            switch(parameter)
            {
                case "RepeatButton" : 
                {
                    switch (Model.Logic.PlaylistControl.RepeatMode)
                    {
                        case RepeatMode.RepeatOff: { Model.Logic.PlaylistControl.RepeatMode = RepeatMode.RepeatAll; } break;
                        case RepeatMode.RepeatAll: { Model.Logic.PlaylistControl.RepeatMode = RepeatMode.RepeatOne; } break;
                        case RepeatMode.RepeatOne: { Model.Logic.PlaylistControl.RepeatMode = RepeatMode.RepeatOff; } break;
                        default: { } break;
                    }
                        Model.Logic.PlaylistControl.RefreshRepeatIcon();
                } break;
                case "PreviousButton" :
                {
                        if (Model.Data.Playlist.Items.Count > 0)
                            Model.Logic.PlaylistControl.OnPreviousSong();
                } break;
                case "NextButton" :
                {
                        if (Model.Data.Playlist.Items.Count > 0)
                            Model.Logic.PlaylistControl.OnNextSong();
                } break;
                default: { } break;
            }

        }

    }
}
