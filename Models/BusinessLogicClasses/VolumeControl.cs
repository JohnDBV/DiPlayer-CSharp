using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiPlayer_CSharp.Models.BusinessLogicClasses
{
    public class VolumeControl
    {
        private MainModel? mainModelRef;

        private int m_Volume;
        public int Volume { get => m_Volume; set { SetVolume(value); } }

        private VolumeControl() { }

        public VolumeControl(ref MainModel mainModelRef)
        {
            this.mainModelRef = mainModelRef;
            m_Volume = 90;
        }

        private void SetVolume(int value)
        {
            m_Volume = value;
            mainModelRef?.Logic.AudioLibrary.SetMasterVolume(value, value);
        }

    }
}
