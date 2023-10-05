using System;
using System.Windows.Input;
using DiPlayer_CSharp.ViewModels;

namespace DiPlayer_CSharp.Commands
{
    public class NextPreviousRepeatButtonsCommand : ICommand
    {
        private MainViewModel m_viewModel;
        public NextPreviousRepeatButtonsCommand(MainViewModel viewModel)
        {
            m_viewModel = viewModel;
        }

        //the event is never used - suppress :
#pragma warning disable CS0067
        public event EventHandler? CanExecuteChanged;
#pragma warning restore CS0067

        public bool CanExecute(object? parameter)
        {
            if (parameter?.GetType() == typeof(string))
            {
                string? strParam = parameter as string;

                if (strParam == "RepeatButton" ||
                    strParam == "PreviousButton" ||
                    strParam == "NextButton"
                    )
                {
                    return true;
                }
            }

            return false;
        }

        public void Execute(object? parameter)
        {
            if (CanExecute(parameter))
                m_viewModel.OnNextPreviousRepeatButtonCommand(parameter as string);
        }

    }
}
