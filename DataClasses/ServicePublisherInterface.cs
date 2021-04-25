using System.ServiceModel;

namespace DataClasses
{
    [ServiceContract]
    public interface ServicePublisherInterface
    {
        [OperationContract]
        string Register(string name, string password);

        [OperationContract]
        int Login(string name, string password);

        [OperationContract]
        void Publish(ServiceModel sm);

        [OperationContract]
        void Unpublish(string api_endpoint);
    }
}
