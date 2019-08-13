using System;
using System.Threading.Tasks;
using Actio.Common.Commands;
using Actio.Common.Events;
using RawRabbit;

namespace Actio.Services.Activities.Handlers
{
    public class CreateActivityHandler : ICommandHandler<CreateActivity>
    {
        private readonly IBusClient _busClient; 
        public CreateActivityHandler(IBusClient busClient) 
        {
            _busClient = busClient; 
        }
        public async Task HandleAsync(CreateActivity command)
        {
            Console.WriteLine($"Creating activity: {command.Name}"); 
            await _busClient.PublishAsync( new ActivityCreated(command.Id, command.UserId, command.Category, command.Name)); 
        }
    }
}