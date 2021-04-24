using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataClasses
{
    //This objeect is used to pass the token and any data to a service
    public class PassObject<T>
    {
        public T Pass;
        public int Token;
    }
}
