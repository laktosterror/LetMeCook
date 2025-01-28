using LetMeCook.model;
using Spectre.Console;

namespace LetMeCook;

public class CookBook(CookBookDB cookBookDb)
{
    private bool _continueMenu = true;

    public void ShowStartMenu()
    {
        do
        {
            var selectedMenuAction = SelectMenuItem();
            switch (selectedMenuAction)
            {
                case "View Recipes":
                    if (cookBookDb.CountRecipes() > 0)
                    {
                        var selectedRecipeToView = SelectRecipe("View");
                        ShowRecipe(selectedRecipeToView, "View");
                    }
                    else
                    {
                        AnsiConsole.WriteLine("No recipes found. Create one!");
                    }
                    
                    AnsiConsole.Write("Press any key to go back!");
                    Console.ReadKey();

                    break;
                case "New Recipe":
                    var recipeTemplate = new Recipe
                    {
                        Title = "New Recipe",
                        Ingredients = new List<string>(),
                        Instructions = new List<string>(),
                        Score = 0,
                        CreatedBy = "Chef X",
                        IsEdited = false
                    };
                    var newRecipe = EditRecipe(recipeTemplate);
                    if (newRecipe.IsEdited) cookBookDb.CreateRecipe(newRecipe);


                    break;
                case "Edit Recipe":
                    var selectedRecipeToEdit = SelectRecipe("Edit");
                    var editedRecipe = EditRecipe(selectedRecipeToEdit);
                    if (editedRecipe.IsEdited) cookBookDb.ReplaceRecipe(editedRecipe);

                    break;
                case "Delete Recipe":
                    var selectedRecipeToDelete = SelectRecipe("Delete");
                    ShowRecipe(selectedRecipeToDelete, "Delete");
                    var selectedActionConfirmed = ConfirmAction("Delete this recipe?");
                    if (selectedActionConfirmed) cookBookDb.DeleteRecipe(selectedRecipeToDelete);

                    break;
                default:
                    _continueMenu = false;
                    break;
            }
        } while (_continueMenu);

        AnsiConsole.Clear();
    }

    private string SelectMenuItem()
    {
        var layout = new Layout("Title");

        layout["Title"].Update(
            new Panel(
                Align.Center(
                    new Markup("[bold yellow]Main Menu[/]"),
                    VerticalAlignment.Top)));

        AnsiConsole.Write(layout);

        Console.SetCursorPosition(0, 4);
        var selectedMenuAction = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more actions)[/]")
                .AddChoices("View Recipes", "New Recipe", "Edit Recipe", "Delete Recipe", "Exit"));

        return selectedMenuAction;
    }

    private Recipe SelectRecipe(string actionText)
    {
        var recipes = cookBookDb.GetAllRecipes();

        var layout = new Layout("Title");

        layout["Title"].Update(
            new Panel(
                Align.Center(
                    new Markup($"[yellow bold]Select Recipe to [/]{actionText}"),
                    VerticalAlignment.Top)));

        AnsiConsole.Write(layout);

        Console.SetCursorPosition(0, 4);
        var selectedRecipeTitle = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .EnableSearch()
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more recipes)[/]")
                .AddChoices(recipes.Select(r => r.Title).ToList()));

        var selectedRecipe = cookBookDb.FindRecipeByTitle(selectedRecipeTitle);

        return selectedRecipe;
    }

    private void ShowRecipe(Recipe selectedRecipe, string actionText)
    {
        var layout = new Layout("Root")
            .SplitRows(
                new Layout("Title")
                    .Size(3),
                new Layout("Body")
                    .Size(20)
                    .SplitColumns(
                        new Layout("Left"),
                        new Layout("Right")
                            .SplitRows(
                                new Layout("Top"),
                                new Layout("Misc")
                                    .Size(3),
                                new Layout("Bottom")
                                    .Size(3))),
                new Layout("Footer")
                    .Size(3));

        layout["Title"].Update(
            new Panel(
                    Align.Center(
                        new Markup("Selected Recipe: " + selectedRecipe.Title,
                            new Style(Color.Green, null, Decoration.Bold)),
                        VerticalAlignment.Top))
                .Expand());

        layout["Left"].Update(
            new Panel(
                    Align.Left(
                        new Rows(selectedRecipe.Instructions.Select(i => new Text("\n" + i)).ToList()),
                        VerticalAlignment.Top))
                .Header("Instructions")
                .Expand());

        layout["Top"].Update(
            new Panel(
                    Align.Left(
                        new Rows(selectedRecipe.Ingredients
                            .Select(i => new Text(i, new Style(Color.Blue, null, Decoration.Italic))).ToList()),
                        VerticalAlignment.Top))
                .Header("Ingredients")
                .Expand());

        layout["Misc"].Update(
            new Panel(
                    Align.Left(
                        new BarChart()
                            .Width(50)
                            .AddItem(string.Empty, selectedRecipe.Score, Color.Yellow)))
                .Header("Score")
                .Expand());

        layout["Bottom"].Update(
            new Panel(
                    Align.Left(
                        new Markup(selectedRecipe.CreatedBy, new Style(Color.Red, null, Decoration.Italic))))
                .Header("Created By")
                .Expand());

        layout["Footer"].Update(
            new Panel(
                    Align.Left(
                        new Grid()
                            .AddColumns(4)
                            .AddRow("E - Edit Recipe", "D - Delete Recipe", "R - Rate Recipe", "B - Go Back")))
                .Header("Actions")
                .Expand());

        AnsiConsole.Clear();
        AnsiConsole.Write(layout);
    }

    private Recipe EditRecipe(Recipe selectedRecipe)
    {
        var editedRecipe = selectedRecipe;
        
        
        var layout = new Layout("Title");

        layout["Title"].Update(
            new Panel(
                Align.Center(
                    new Markup($"Recipe Editor: select action for {selectedRecipe.Title}"),
                    VerticalAlignment.Top)));

        AnsiConsole.Render(layout);

        Console.SetCursorPosition(0, 4);
        var selectedEditorAction = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more actions)[/]")
                .AddChoices("Edit Name", "Edit Instructions", "Edit Ingredients", "Edit Score", "Edit Creator", "Exit"));

        switch (selectedEditorAction)
        {
            case "Edit Name":
                AnsiConsole.WriteLine($"Old name: {selectedRecipe.Title}");
                var newName = AnsiConsole.Prompt(
                    new TextPrompt<string>("Enter new name:"));
                editedRecipe.Title = newName;
                
                break;
            case "Edit Instructions":
                layout["Title"].Update(
                    new Panel(
                        Align.Center(
                            new Markup($"Recipe Editor: Update Instructions for {selectedRecipe.Title}"),
                            VerticalAlignment.Top)));

                    do
                    {
                        AnsiConsole.Write(layout);

                Console.SetCursorPosition(0, 4);
                var selectedInstructionStep = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .PageSize(10)
                        .MoreChoicesText("[grey](Move up and down to reveal more steps)[/]")
                        .AddChoices(selectedRecipe.Instructions));
                
                var name = AnsiConsole.Prompt(
                    new TextPrompt<string>("What's your name?"));


                break;
            case "Edit Ingredients":
                break;
            case "Edit Score":
                break;
            case "Edit Creator":
                break;
            default:
                break;
        }

        return editedRecipe;
    }

    private bool ConfirmAction(string confirmationText)
    {
        var confirmation = AnsiConsole.Prompt(
            new TextPrompt<bool>(confirmationText)
                .AddChoice(true)
                .AddChoice(false)
                .DefaultValue(false)
                .WithConverter(choice => choice ? "y" : "n"));
        
        return confirmation;
    }
}