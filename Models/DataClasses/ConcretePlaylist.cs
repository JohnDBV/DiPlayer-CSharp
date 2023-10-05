using DiPlayer_CSharp.Models.DataClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiPlayer_CSharp.Models.DataClasses
{
    internal abstract class ConcretePlaylist : IGenericPlaylist, INotifyPropertyChanged
    {
        protected string m_extension;

        private string m_playlistFilePath;
        private string m_playlistTitle;
        private readonly ObservableCollection<AudioTrack> m_tracks;

        public event PropertyChangedEventHandler? PropertyChanged;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected ConcretePlaylist() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public ConcretePlaylist(string fileDialogPath)
        {
            m_extension = "NONE";
            m_playlistFilePath = fileDialogPath;
            m_playlistTitle = System.IO.Path.GetFileNameWithoutExtension(fileDialogPath);
            m_tracks = new ObservableCollection<AudioTrack>();
            ProcessPlaylist();
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        //This one is private,we can't use for binding :
        ObservableCollection<AudioTrack> IGenericPlaylist.Items { get => m_tracks; }
        //This one has exactly the same target,but it's public. It's also intelli-sense hidden and runtime bound
        public ObservableCollection<AudioTrack> Tracks => m_tracks;

        string IGenericPlaylist.PlaylistTitle { get => PlaylistTitle; set => PlaylistTitle = value; }
        public string PlaylistTitle { get => m_playlistTitle; set => OnPlaylistTitleChanged(value); }

        void IGenericPlaylist.ProcessPlaylist()
        {
            ProcessPlaylist();
        }

        public void ProcessPlaylist()
        {
            if (!System.IO.File.Exists(m_playlistFilePath))
                return;//treat the "Untitled Playlist" case, for example

            using (System.IO.StreamReader sr = new System.IO.StreamReader(m_playlistFilePath))
            {
                while (!sr.EndOfStream)
                {
                    Tracks.Add(new AudioTrack(sr.ReadLine()));
                }

                sr.Close();
            }//auto-disposed here
        }

        void IGenericPlaylist.SavePlaylistAs(string fileName)
        {
            SavePlaylistAs(fileName);
        }

        public void SavePlaylistAs(string fileName)
        {
            bool privilegesOk = true;

            if (!System.IO.File.Exists(m_playlistFilePath))
            {
                System.IO.FileStream? fs = null;

                //Let's make sure it can be created(i.e. : We have enough privileges)
                try
                {
                    fs = System.IO.File.Create(fileName);
                }
                catch (Exception e)
                {
                    privilegesOk = false;
                }
                finally
                {
                    fs.Dispose();
                }


            }

            if(privilegesOk)
            {//we have the blank file or an old version of it, already

                using(System.IO.StreamWriter sw = new System.IO.StreamWriter(fileName))
                {
                    foreach(AudioTrack track in Tracks)
                    {
                        if(System.IO.File.Exists(track.FilePath))
                            sw.WriteLine(track.FilePath);
                    }

                    sw.Close();
                }//auto-disposing here


            }
        }

        public void OnPlaylistTitleChanged(string value)
        {
            m_playlistTitle = value;
            OnPropertyChanged(nameof(PlaylistTitle));
            OnPropertyChanged(nameof(IGenericPlaylist.PlaylistTitle));
        }

    }
}
