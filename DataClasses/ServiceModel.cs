using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataClasses
{
    //Contains the entire description of a service
    public class ServiceModel
    {
        public string Name;
        public string Description;
        public string API_Endpoint;
        public int Number_Of_Operands;
        public string Operand_Type;

        public ServiceModel(string name, string desc, string api, int noop, string optype)
        {
            Name = name;
            Description = desc;
            API_Endpoint = api;
            Number_Of_Operands = noop;
            Operand_Type = optype;
        }
    }
}