
namespace DataClasses
{
    //This object is used to return any object and the status from a service to the caller
    public class ReturnObject<T>
    {
        public T Returned;
        public string Status = "";
        public string Reason = "";
    }
}