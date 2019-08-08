namespace Actio.Common.Events
{
    public class UserCreated : IEvent
    {
        public string Email { get; }
        public string Name { get;  }

        protected UserCreated()
        {
            // so that our serializer will not have any issues 
        }

        public UserCreated(string email, string name) 
        {
            Email = email; 
            Name = name; 
        }
    }
}