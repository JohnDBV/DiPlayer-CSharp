using System;
using System.Windows.Input;
using DiPlayer_CSharp.ViewModels;

namespace DiPlayer_CSharp.Commands
{
    public class OpenPlaylistCommand : ICommand
    {
        private MainViewModel m_viewModel;
        public OpenPlaylistCommand(MainViewModel viewModel)
        {
            m_viewModel = viewModel;
        }

        //the event is never used - suppress :
#pragma warning disable CS0067
        public event EventHandler? CanExecuteChanged;
#pragma warning restore CS0067

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (CanExecute(this))
                m_viewModel.OnOpenPlaylist();
        }

    }
}
