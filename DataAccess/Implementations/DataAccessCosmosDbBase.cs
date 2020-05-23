using Microsoft.Azure.Cosmos;

namespace ProductOwnerSimGame.DataAccess.Implementations
{
    public class DataAccessCosmosDbBase
    {
        protected CosmosClient CosmosClient { get; private set; }
        
        public DataAccessCosmosDbBase(string connectionString)
        {
            CosmosClient = new CosmosClient(connectionString);
        }

    }
}
