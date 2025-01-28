using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LetMeCook.model;

public class Recipe
{
    [BsonId] public ObjectId Id { get; set; }

    [BsonElement("title")] public string Title { get; set; }

    [BsonElement("ingredients")] public List<string> Ingredients { get; set; }

    [BsonElement("instructions")] public List<string> Instructions { get; set; }

    [BsonElement("score")] public double Score { get; set; }

    [BsonElement("createdBy")] public string CreatedBy { get; set; }
}