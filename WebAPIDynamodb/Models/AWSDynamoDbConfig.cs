namespace WebAPIDynamodb.Models
{
    public class AWSDynamoDbConfig
    {
        public string Access_Key { get; set; }
        public string Secret_Key { get; set; }
        public string Region { get; set; }
        public string LocalServiceUrl { get; set; }
        public string TableNamePrefix { get; set; }
        public  bool LocalMode { get; set; }
    }
}
