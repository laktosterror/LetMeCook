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

    public void CreateRecipe(Recipe recipe)
    {
        _recipesCollection.InsertOne(recipe);
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

    public long CountRecipes()
    {
        var recipesCount = _recipesCollection.CountDocuments(new BsonDocument());
        return recipesCount;
    }

    public void CreateMockRecipes()
    {
        var recipe1 = new Recipe
        {
            Title = "Cauliflower Buffalo Wings",
            Ingredients = new List<string>
            {
                "1 head cauliflower, cut into florets",
                "1 cup all-purpose flour",
                "1 cup water",
                "1 tsp garlic powder",
                "1 tsp paprika",
                "1/2 tsp salt",
                "1 cup buffalo sauce",
                "2 tbsp olive oil"
            },
            Instructions = new List<string>
            {
                "1. Preheat the oven to 450°F (230°C) and line a baking sheet with parchment paper.",
                "2. In a bowl, whisk together flour, water, garlic powder, paprika, and salt until smooth.",
                "3. Dip each cauliflower floret into the batter, allowing excess to drip off.",
                "4. Place the battered florets on the baking sheet and bake for 20 minutes.",
                "5. Remove from the oven and toss the florets in buffalo sauce mixed with olive oil.",
                "6. Return to the oven and bake for an additional 10-15 minutes until crispy."
            },
            Score = 4.7,
            CreatedBy = "Chef Jamie"
        };

        var recipe2 = new Recipe
        {
            Title = "Zucchini Noodles with Pesto",
            Ingredients = new List<string>
            {
                "2 medium zucchinis, spiralized",
                "1 cup fresh basil leaves",
                "1/4 cup pine nuts",
                "1/4 cup grated Parmesan cheese",
                "2 cloves garlic",
                "1/4 cup olive oil",
                "to taste salt and pepper"
            },
            Instructions = new List<string>
            {
                "1. In a food processor, combine basil, pine nuts, Parmesan cheese, garlic, and olive oil.",
                "2. Blend until smooth, adding salt and pepper to taste.",
                "3. In a skillet, lightly sauté the spiralized zucchini noodles for 2-3 minutes until just tender.",
                "4. Remove from heat and toss with the pesto until well coated.",
                "5. Serve immediately, garnished with extra Parmesan if desired."
            },
            Score = 4.6,
            CreatedBy = "Chef Mia"
        };

        var recipe3 = new Recipe
        {
            Title = "Sweet Potato and Black Bean Tacos",
            Ingredients = new List<string>
            {
                "2 medium sweet potatoes, peeled and diced",
                "1 can black beans, drained and rinsed",
                "1 tsp cumin",
                "1 tsp chili powder",
                "1 tbsp olive oil",
                "8 corn tortillas",
                "1 avocado, sliced",
                "to taste lime juice",
                "to taste cilantro"
            },
            Instructions = new List<string>
            {
                "1. Preheat the oven to 400°F (200°C) and line a baking sheet with parchment paper.",
                "2. Toss the diced sweet potatoes with olive oil, cumin, chili powder, salt, and pepper.",
                "3. Spread the sweet potatoes on the baking sheet and roast for 25-30 minutes until tender.",
                "4. In a bowl, combine the roasted sweet potatoes and black beans.",
                "5. Warm the corn tortillas in a skillet or microwave.",
                "6. Fill each tortilla with the sweet potato and black bean mixture, topped with avocado slices, lime juice, and cilantro."
            },
            Score = 4.8,
            CreatedBy = "Chef Alex"
        };

        _recipesCollection.InsertMany([recipe1, recipe2, recipe3]);
    }
}