using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiPlayer_CSharp.Models.DataClasses
{
    public interface IGenericPlaylist
    {
        public ObservableCollection<AudioTrack> Items { get;}
        public string PlaylistTitle { get; set; }
        public void ProcessPlaylist();

        public void SavePlaylistAs(string fileName);
    }
}
