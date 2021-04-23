using System;
//using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace DataClasses
{ 
    [DataContract]
    public class FileFormatInvalidFault
    {
        [DataMember]
        public string Issue { get; set; }
    }
}
