using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataClasses
{
    [ServiceContract]
    public interface AuthenticatorInterface
    {
        [OperationContract]
        string Register(string name, string password);

        [OperationContract]
        [FaultContract(typeof(FileFormatInvalidFault))]
        [FaultContract(typeof(AccountNotFoundFault))]
        string Login(string name, string password);

        [OperationContract]
        string Validate(int token);
    }
}
