using System;
using System.Threading.Tasks;
using Actio.Common.Events;

namespace Actio.Api.Handlers
{
    public class ActivityCreatedHandler : IEventHandler<ActivityCreated>
    {
        public async Task HandleAsync(ActivityCreated @event)
        {

            await Task.CompletedTask; 
            Console.WriteLine($"Activity Created: {@event.Name}"); 
        }
    }
}