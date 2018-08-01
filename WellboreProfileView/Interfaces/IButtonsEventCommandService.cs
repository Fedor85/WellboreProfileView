using System;
using Prism.Commands;

namespace WellboreProfileView.Interfaces
{
    public interface IButtonsEventCommandService
    {
        event Action<Command> AddComman;

        DelegateCommand FindOrRegisteredCommands(string commandName);

        bool Subscribe(string commandName, Action action, bool isExecute);

        void UnSubscribe(string commandName, Action action);

        void Run(string commandName);

        void Activate(string commandName);

        void DeActivate(string commandName);

        void SetExecute(string commandName, bool isExecute);
    }
}