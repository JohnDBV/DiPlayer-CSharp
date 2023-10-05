using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiPlayer_CSharp.Models.DataClasses
{
    internal class UniversalPlaylist : ConcretePlaylist
    {
        private UniversalPlaylist() { }

        public UniversalPlaylist(string fileDialogPath) : base(fileDialogPath)
        {
            m_extension = "m3u";
        }

    }
}
