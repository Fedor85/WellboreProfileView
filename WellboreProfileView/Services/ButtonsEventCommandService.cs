using System;
using System.Collections.Generic;
using Prism.Commands;
using WellboreProfileView.Interfaces;

namespace WellboreProfileView.Services
{
    public class ButtonsEventCommandService : IButtonsEventCommandService
    {
        private List<Command> commands;

        public event Action<Command> AddComman;

        public ButtonsEventCommandService()
        {
            commands = new List<Command>();
        }

        public DelegateCommand FindOrRegisteredCommands(string commandName)
        {
            Command command = commands.Find(item => item.Name.Equals(commandName));
            if (command != null)
                return command.DelegateCommand;

            Command newCommand = new Command(commandName);
            commands.Add(newCommand);
            if (AddComman != null)
                AddComman.Invoke(newCommand);

            return newCommand.DelegateCommand;
        }

        public bool Subscribe(string commandName, Action action, bool isExecute)
        {
            Command command = GetCommand(commandName);
            if (command != null)
            {
                command.SetExecute(isExecute);
                command.CommandEvent += action;
                return true;
            }

            return false;
        }

        public void UnSubscribe(string commandName, Action action)
        {
            Command command = GetCommand(commandName);
            if (command != null)
                command.CommandEvent -= action;
        }

        public void Run(string commandName)
        {
            Command command = GetCommand(commandName);
            if (command != null)
                command.Execute();
        }

        public void Activate(string commandName)
        {
            SetExecute(commandName, true);
        }

        public void DeActivate(string commandName)
        {
            SetExecute(commandName, false);
        }

        public void SetExecute(string commandName, bool isExecute)
        {
            Command command = GetCommand(commandName);
            if (command != null)
                command.SetExecute(isExecute);
        }

        private Command GetCommand(string commandName)
        {
            return commands.Find(item => item.Name.Equals(commandName));
        }
    }
}