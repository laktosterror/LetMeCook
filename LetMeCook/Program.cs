using Spectre.Console;

namespace LetMeCook;

internal class Program
{
    private static CookBookDB _cookBookDb;

    private static void Main(string[] args)
    {
        _cookBookDb = new CookBookDB();
        var cookBook = new CookBook(_cookBookDb);
        var createMockData = false;

        AnsiConsole.Status()
            .Start("Connecting To Database...", ctx =>
            {
                try
                {
                    _cookBookDb.TestDbServerConnection();
                }
                catch
                {
                    AnsiConsole.MarkupLine("[red bold]Error: Could not connect to the database![/]");
                    throw;
                }

                Thread.Sleep(1000);

                ctx.Status("Using CookBook database...");
                if (!_cookBookDb.IsDatabaseExist())
                {
                    AnsiConsole.MarkupLine("[red bold]Error: Could not find cookbook database![/]");
                    createMockData = true;
                }

                Thread.Sleep(1000);

                ctx.Status("Looking for recipes collection...");
                if (!_cookBookDb.IsCollectionExist())
                {
                    AnsiConsole.MarkupLine("[red bold]Error: Could not find recipes collection![/]");
                    createMockData = true;
                }

                Thread.Sleep(1000);

                if (createMockData)
                {
                    ctx.Status("Creating database, collection and mock recipes!");
                    Thread.Sleep(1000);
                    _cookBookDb.CreateMockRecipes();
                }
            });

        AnsiConsole.Clear();

        cookBook.ShowStartMenu();
    }
}