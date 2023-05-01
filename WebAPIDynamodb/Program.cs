using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Amazon;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebAPIDynamodb.Models;

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
var credentials = new BasicAWSCredentials(accessKey, secretKey);

//Method-1
var config = new AmazonDynamoDBConfig()
{
    AuthenticationRegion = region
};
if (runLocalDynamoDb)
{
    //Method-2
    config.ServiceURL = localServiceUrl;
}

var dynamoDbClient = new AmazonDynamoDBClient(credentials, config);
builder.Services.AddSingleton<IAmazonDynamoDB>(dynamoDbClient);
var context = new DynamoDBContext(dynamoDbClient);
builder.Services.AddSingleton<IDynamoDBContext>(context);

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