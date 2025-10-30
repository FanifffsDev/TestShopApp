using System.Windows.Input;

namespace TestShopApp.Telegram.Commands
{
    public class CommandProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, ICommand> _commands = new();

        public CommandProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            RegisterCommands();
        }

        private void RegisterCommands()
        {
            foreach (var command in _serviceProvider.GetServices<ICommand>())
            {
                _commands[command.Command] = command;
            }
        }

        public ICommand GetCommand(string command)
        {
            _commands.TryGetValue(command, out var cmd);
            return cmd;
        }
    }
}
