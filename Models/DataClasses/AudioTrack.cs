using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DiPlayer_CSharp.Models.DataClasses
{
    public class AudioTrack:INotifyPropertyChanged
    {
        private string m_storageTrackName;
        private string m_storageArtistName;
        private string m_storageAlbumName;
        
        //Bitmaps : one is default, others are optional(if found in the file metadata)
        BitmapImage m_storageBitmap;
        readonly string defaultImageResourcePath;

        //Auto-properties(must initialize) :

        public string? FilePath { get; set; }
        public string? TrackName { get; set; }
        public string? TrackArtist { get; set; }

        //Not auto-properties :

        public string StorageTrackName
        {
            get => m_storageTrackName;
            set 
            {
                m_storageTrackName = value.Length > 0 ? value : "Unknown Track";
                OnPropertyChanged(nameof(StorageTrackName));
            }
        }
        public string StorageArtist
        {
            get => m_storageArtistName;
            set
            {
                m_storageArtistName = value.Length > 0 ? value : "Unknown Artist";
                OnPropertyChanged(nameof(StorageArtist));
            }
        }
        public string StorageAlbum
        {
            get => m_storageAlbumName;
            set
            {
                m_storageAlbumName = value.Length > 0 ? value : "Unknown Album";
                OnPropertyChanged(nameof(StorageAlbum));
            }
        }

        public BitmapImage? StorageBitmap
        {
            get => m_storageBitmap;
            set
            {
                SetBitmap(value);
                OnPropertyChanged(nameof(StorageBitmap));
            }
        }

        //INotifyPropertyChanged stuff :
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public AudioTrack()
        {
            FilePath = "";
            defaultImageResourcePath = @"\Resources\Images\music-41071_640.png";
            StorageBitmap = BusinessLogicClasses.BusinessLogicUtilities.CreateEmptyBitmapImage();
            ProcessFilePath();
        }

        public AudioTrack(string? filePath)
        {
            FilePath = filePath;
            defaultImageResourcePath = @"\Resources\Images\music-41071_640.png";
            StorageBitmap = BusinessLogicClasses.BusinessLogicUtilities.CreateEmptyBitmapImage();
            ProcessFilePath();
        }

        public AudioTrack(string name, string artist, string filePath)
        {
            FilePath = filePath;
            defaultImageResourcePath = @"\Resources\Images\music-41071_640.png";
            StorageBitmap = BusinessLogicClasses.BusinessLogicUtilities.CreateEmptyBitmapImage();
            ProcessFilePath();
            TrackName = name;
            TrackArtist = artist;
        }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        private void SetBitmap(BitmapImage value)
        {
            if( (value.PixelWidth == 1) && (value.PixelHeight == 1) )
            {
                //The Bitmap is empty, create from the file
                value = new BitmapImage(new Uri(defaultImageResourcePath, UriKind.Relative));
            }

            m_storageBitmap = value;

        }

        private void ProcessFilePath()
        {
            string[] artistTrackNameArr = System.IO.Path.GetFileNameWithoutExtension(FilePath).Split(
                new char[] { '-', '@' } );

            if (artistTrackNameArr.Length > 1)
            {//"artist - track name" or "artist - track name - some - other - artifacts"
                //if it has artifacts,let's get rid of them

                TrackArtist = artistTrackNameArr[0].Trim();
                TrackName = artistTrackNameArr[1].Trim();
            }
            else if (artistTrackNameArr.Length > 1)
            {//Let's get the track name only and hope the file has the artist in the metadata
                TrackArtist = "Unknown Artist";
                TrackName = artistTrackNameArr[0].Trim();
            }
            else
            {//some other case, not handled
                TrackArtist = "Unknown Artist";
                TrackName = "Unknown Track";
            }

            //With the audio library, we process the file metadata only when a file is opened.
            //Let's use defaults right now :

            m_storageTrackName = "Unknown Track";
            m_storageArtistName = "Unknown Artist";
            m_storageAlbumName = "Unknown Album";

        }
    }
}
