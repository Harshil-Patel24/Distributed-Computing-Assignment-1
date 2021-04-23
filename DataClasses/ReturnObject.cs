using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataClasses
{
    public class ReturnObject<T>
    {
        public T Returned;
        public string Status = "";
        public string Reason = "";

        //public ReturnObject()
        //{
            
        //}
    }
}