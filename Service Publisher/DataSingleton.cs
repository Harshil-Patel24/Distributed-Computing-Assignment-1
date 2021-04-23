using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataClasses;

namespace DataClasses
{
    class DataSingleton
    {
        private static DataSingleton instance = null;
        //public List<ServiceModel> serviceModel;
        public int token;
        private DataSingleton(){}
        public static DataSingleton Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new DataSingleton();
                }
                return instance;
            }
        }

        //public static ServiceModel FindService(string endpoint)
        //{
        //    foreach (ServiceModel service in DataSingleton.Instance.serviceModel)
        //    {
        //        if (service.API_Endpoint.Equals(endpoint))
        //        {
        //            return service;
        //        }
        //    }

        //    return null;
        //}

        //public static string FindEndpoint(string name)
        //{
        //    foreach(ServiceModel service in DataSingleton.Instance.serviceModel)
        //    {
        //        if(service.Name.Equals(name))
        //        {
        //            return service.API_Endpoint;
        //        }
        //    }

        //    return null;
        //}
    }
}
