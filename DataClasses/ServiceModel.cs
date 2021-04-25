using System.Runtime.Serialization;

namespace DataClasses
{
    //Contains the entire description of a service
    [DataContract]
    public class ServiceModel
    {
        [DataMember]
        public string Name;
        [DataMember]
        public string Description;
        [DataMember]
        public string API_Endpoint;
        [DataMember]
        public int Number_Of_Operands;
        [DataMember]
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