using System.Collections.Generic;

namespace azure_pub_sub.src
{
    public interface IMessagingRepository<T> where T: EntityBase 
    {
        T GetNextMessage(); 
        IEnumerable<T> List(); 

        void Add(T entity); 
         
    }
}