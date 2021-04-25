using System.ServiceModel;

namespace DataClasses
{
    [ServiceContract]
    public interface AuthenticatorInterface
    {
        [OperationContract]
        string Register(string name, string password);

        [OperationContract]
        string Login(string name, string password);

        [OperationContract]
        string Validate(int token);

        [OperationContract]
        bool AccountExists(string name);
    }
}
