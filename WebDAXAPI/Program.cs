using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Amazon.DAX;
using Amazon.DynamoDBv2.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// Configure AWS service clients to use these credentials
var dynamoDbConfig = builder.Configuration.GetSection("AWSDynamoDbConfig");
var accessKey = dynamoDbConfig.GetValue<string>("Access_Key");
var secretKey = dynamoDbConfig.GetValue<string>("Secret_Key");
var region = dynamoDbConfig.GetValue<string>("Region");
var runLocalDynamoDb = dynamoDbConfig.GetValue<bool>("LocalMode");
var localServiceUrl = dynamoDbConfig.GetValue<string>("LocalServiceUrl");
var endpointUri= dynamoDbConfig.GetValue<string>("EndpointUri");
var credentials = new BasicAWSCredentials(accessKey, secretKey);

var clientConfig = new DaxClientConfig("daxcluster.xgknlg.dax-clusters.eu-west-1.amazonaws.com",9111)
{
    AwsCredentials = FallbackCredentialsFactory.GetCredentials()
};

var dynamoDbClient = new ClusterDaxClient(clientConfig);

//var dynamoDbClient = new AmazonDynamoDBClient(credentials, config);
builder.Services.AddSingleton<IAmazonDynamoDB>(dynamoDbClient);
var context = new DynamoDBContext(dynamoDbClient);
builder.Services.AddSingleton<IDynamoDBContext>(context);


var tableName = "Products";

var pk = 1;
var sk = 10;
var iterations = 5;

var startTime = System.DateTime.Now;

for (var i = 0; i < iterations; i++)
{
    for (var ipk = 1; ipk <= pk; ipk++)
    {
        for (var isk = 1; isk <= sk; isk++)
        {
            var request = new GetItemRequest()
            {
                TableName = tableName,
                Key = new Dictionary<string, AttributeValue>() {
                            {"category", new AttributeValue {N = ipk.ToString()} },
                            {"name", new AttributeValue {N = isk.ToString() } }
                        }
            };
            var response = await dynamoDbClient.GetItemAsync(request);
            Console.WriteLine($"GetItem succeeded for pk: {ipk},sk: {isk}");
        }
    }
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AWS: WebAPI");
    });
}


//Cognito Configuration: 
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();