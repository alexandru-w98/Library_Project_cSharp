using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Services
{
    public class DependencyService : IDependencyService
    {
        private readonly Dictionary<Type, object> registeredServices = new Dictionary<Type, object>();

        public void Register<T>(object implementation)
        {
            this.registeredServices[typeof(T)] = implementation;
        }

        public T Get<T>() where T : class
        {
            return (T)registeredServices[typeof(T)];
        }
    }
}
