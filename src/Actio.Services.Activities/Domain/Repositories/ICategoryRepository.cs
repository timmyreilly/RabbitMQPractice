using System.Collections.Generic;
using System.Threading.Tasks;
using Actio.Services.Activities.Domain.Models;

namespace Actio.Services.Activities.Domain.Repositories
{
    public interface ICategoryRepository
    {
         Task<Category> GetAsync(string name);
         Task<IEnumerable<Category>> BrowseAsync(); 

         Task AddAsync(Category category);  
    }
}

/*

# Challenge 1 

How does one test RabbitMq locally? 

Discuss with your team the merits of Events, Command and Messaging in a Microservices Architecture. 

# Challenge 2 

Use docker to host RabbitMQ in a container in Azure. 

Then connect the two using a client of your choice. 

How does your team translate and interpret messages between disparate systems 

# Challenge 3 

Your customer only wants messages that are related to specific data fields. How do you configure this with RabbitMQ 

# Other challenges

Can you deal with a down container? What hosting mechanism do you choose and why? 
How can you ensure RabbitMq health, performance, and service availability? 
Can you connect two container's together? (maybe use a logic app to tie them together?) 

 */