using System;
using Prism.Commands;

namespace WellboreProfileView
{
    public class Command
    {
        private bool isExecute;

        public string Name { get; set; }

        public DelegateCommand DelegateCommand;

        public event Action CommandEvent;

        public Command(string name)
        {
            Name = name;
            isExecute = false;
            DelegateCommand = new DelegateCommand(Execute, CanExecute);
        }

        public void SetExecute(bool isExecute)
        {
            this.isExecute = isExecute;
            DelegateCommand.RaiseCanExecuteChanged();
        }

        public void Execute()
        {
            if (CommandEvent != null)
                CommandEvent.Invoke();
        }

        private bool CanExecute()
        {
            return isExecute;
        }
    }
}