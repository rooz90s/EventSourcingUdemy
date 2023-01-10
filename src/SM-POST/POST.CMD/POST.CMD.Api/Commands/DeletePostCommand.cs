using CQRS.Core.Commands;

namespace POST.CMD.Api.Commands
{
    public class DeletePostCommand :  BaseCommand
    {
        public string Username { get; set; }
    }
}
