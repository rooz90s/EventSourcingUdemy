using CQRS.Core.Commands;

namespace POST.CMD.Api.Commands
{
    public class EditPostCommand : BaseCommand
    {
        public string Message { get; set; }
    }
}
