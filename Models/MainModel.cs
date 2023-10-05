using DiPlayer_CSharp.Models.DataClasses;
using libZPlay;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//The model for everything, except playlists handling

namespace DiPlayer_CSharp.Models
{
    public class MainModel
    {
        private Data m_data;
        private Logic m_logic;


        public Data Data { get => m_data; }
        public Logic Logic { get => m_logic; }

        public MainModel()
        {
            m_data = new Data(this);
            m_logic = new Logic(this);
        }

        public void SetParentReference(ref MainModel mainModel)
        {
            m_data = new Data(mainModel);
            m_logic = new Logic(mainModel);
        }

        TimeOnly TrackCurrentHms { get; set; }
        TimeOnly TrackTotalHms { get; set; }

    }
}
