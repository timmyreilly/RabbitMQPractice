using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Actio.Services.Activities.Domain.Repositories
{
    public interface IActivityRepository
    {
         Task<Activity> GetAsync(Guid id); 
         Task AddAsync(Activity activity); 
    }
}