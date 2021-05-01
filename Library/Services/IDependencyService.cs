using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Services
{
    public interface IDependencyService 
    {
        void Register<T>(object implementation);
        T Get<T>() where T : class;
    }
}
