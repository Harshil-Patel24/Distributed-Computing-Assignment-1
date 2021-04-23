using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataClasses
{
    [ServiceContract]
    public interface ServicePublisherInterface
    {
        [OperationContract]
        string Register(string name, string password);

        [OperationContract]
        //[FaultContract(typeof(FileFormatInvalidFault))]
        //[FaultContract(typeof(AccountNotFoundFault))]
        int Login(string name, string password);

        [OperationContract]
        void Publish(string name, string description, string api_endpoint, int no_operands, string operand_types);

        [OperationContract]
        void Unpublish(string api_endpoint);
    }
}
