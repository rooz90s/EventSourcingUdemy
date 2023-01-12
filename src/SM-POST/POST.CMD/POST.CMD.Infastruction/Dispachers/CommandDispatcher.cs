using CQRS.Core.Commands;
using CQRS.Core.Infrastructure;



namespace POST.CMD.Infrastructure.Dispachers
{
    public class CommandDispatcher : ICommandDispatcher
    {

        private readonly Dictionary<Type, Func<BaseCommand, Task>> _handlers = new ();

        public void RegisterHandler<T>(Func<T, Task> handler) where T : BaseCommand
        {
            if (_handlers.ContainsKey(typeof(T)))
            {
                throw new IndexOutOfRangeException("you cannot register same handler");
            }

            _handlers.Add(typeof(T), x => handler((T)x)); 
        }

        public async Task SendAsync(BaseCommand Command)
        {
            if (_handlers.TryGetValue(Command.GetType(),out Func<BaseCommand, Task> handler))
            {
                await handler(Command);
            }
            else
            {
                throw new ArgumentNullException(nameof(handler), "No command Handler found");
            }
        }
    }
}
