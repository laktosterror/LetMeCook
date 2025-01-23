using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LetMeCook.model;

public class Recipe
{
    [BsonId] // This attribute indicates that this property is the primary key
    public ObjectId Id { get; set; }

    [BsonElement("title")] // Maps to the "title" field in BSON
    public string Title { get; set; }

    [BsonElement("ingredients")] // Maps to the "ingredients" field in BSON
    public List<string> Ingredients { get; set; }

    [BsonElement("instructions")] // Maps to the "instructions" field in BSON
    public List<string> Instructions { get; set; }

    [BsonElement("score")] // Maps to the "score" field in BSON
    public double Score { get; set; }

    [BsonElement("createdBy")] // Maps to the "createdBy" field in BSON
    public string CreatedBy { get; set; }
}