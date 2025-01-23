using Spectre.Console;

namespace LetMeCook;

internal class Program
{
    private static CookBookDB _cookBookDb;

    private static void Main(string[] args)
    {
        _cookBookDb = new CookBookDB();
        var cookBook = new CookBook(_cookBookDb);

        var layout = new Layout("Title")
            .Size(3);

        layout["Title"].Update(
            new Panel(
                Align.Center(
                    new Markup("Starting Up!", new Style(Color.Yellow, null, Decoration.Bold)),
                    VerticalAlignment.Top)));

        AnsiConsole.Clear();
        AnsiConsole.Render(layout);
        Console.SetCursorPosition(0, 3);

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
                    throw new Exception("Could not find cookbook database!");
                }

                Thread.Sleep(1000);

                ctx.Status("Looking for recipes collection...");
                if (!_cookBookDb.IsCollectionExist())
                {
                    AnsiConsole.MarkupLine("[red bold]Error: Could not find recipes collection![/]");
                    throw new Exception("Could not find recipes collection!");
                }

                Thread.Sleep(1000);

                ctx.Status("Connected!");
                ctx.Spinner(Spinner.Known.Earth);
                ctx.SpinnerStyle(Style.Parse("green"));
                Thread.Sleep(1000);
            });

        AnsiConsole.Clear();

        cookBook.ShowStartMenu();
    }
}