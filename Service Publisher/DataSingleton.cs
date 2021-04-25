
namespace DataClasses
{
    class DataSingleton
    {
        private static DataSingleton instance = null;
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
    }
}
