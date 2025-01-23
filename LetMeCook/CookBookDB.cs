using LetMeCook.model;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LetMeCook;

public class CookBookDB
{
    private readonly MongoClient _client;
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<Recipe> _recipesCollection;

    public CookBookDB()
    {
        _client = new MongoClient("mongodb://localhost:27017");
        _database = _client.GetDatabase("cookbook");
        _recipesCollection = _database.GetCollection<Recipe>("recipes");
    }

    public List<Recipe> GetAllRecipes()
    {
        var recipes = _recipesCollection.Find(Builders<Recipe>.Filter.Empty).ToList();
        return recipes;
    }

    public Recipe FindRecipeByTitle(string title)
    {
        var recipe = _recipesCollection.Find<Recipe>(r => r.Title == title).FirstOrDefault();
        return recipe;
    }

    public void ReplaceRecipe(Recipe selectedRecipe)
    {
        _recipesCollection.ReplaceOne(r => r.Id == selectedRecipe.Id, selectedRecipe);
    }

    public void DeleteRecipe(Recipe selectedRecipe)
    {
        _recipesCollection.DeleteOne(r => r.Id == selectedRecipe.Id);
    }

    public void TestDbServerConnection()
    {
        _database.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait();
    }

    public bool IsDatabaseExist()
    {
        var databaseNames = _client.ListDatabaseNames().ToList();
        return databaseNames.Contains("cookbook");
    }

    public bool IsCollectionExist()
    {
        var collectionNames = _database.ListCollectionNames().ToList();
        return collectionNames.Contains("recipes");
    }
}