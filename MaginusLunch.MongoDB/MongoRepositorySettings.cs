namespace MaginusLunch.MongoDB
{
    public class MongoRepositorySettings
    {
        public const string DefaultMongoRepositorySectionName = "MongoRepository";
        public string ConnectionString { get; set; }
    }
}
