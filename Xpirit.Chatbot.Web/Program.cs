using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Xpirit.Chatbot.Domain.Contracts;
using Xpirit.Chatbot.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("MongoDb");
var mongoClient = new MongoClient(connectionString);
var database = mongoClient.GetDatabase("XpiritChatbot"); // Replace with the actual database name

builder.Services.AddSingleton<IMongoDatabase>(database);

builder.Services.AddSingleton<IMongoClient, MongoClient>(_ => new MongoClient(connectionString));
builder.Services.AddSingleton<IParkingRepository, ParkingMongoRepository>();

BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

var ignoreExtraElementsConventionPack = new ConventionPack()
			{
				new IgnoreExtraElementsConvention(ignoreExtraElements: true)
			};
ConventionRegistry.Register("IgnoreExtraElements", ignoreExtraElementsConventionPack, _ => true);


var ignoreIfNullConventionPack = new ConventionPack()
			{
				new IgnoreIfNullConvention(true)
			};

ConventionRegistry.Register("IgnoreIfNullConvention", ignoreIfNullConventionPack, _ => true);


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
