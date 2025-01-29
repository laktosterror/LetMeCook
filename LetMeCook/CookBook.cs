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
                .Title("What would you like to do?\n[italic gray](Use arrow keys and enter)[/]")
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
                    new Markup($"[yellow bold]Main Menu > {actionText} Recipe[/]"),
                    VerticalAlignment.Top)));

        AnsiConsole.Write(layout);

        Console.SetCursorPosition(0, 4);
        var selectedRecipeTitle = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .EnableSearch()
                .PageSize(10)
                .Title($"Select Recipe to {actionText}")
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
                                    .Size(3))));

        layout["Title"].Update(
            new Panel(
                    Align.Center(
                        new Markup($"[yellow bold]Main Menu > {actionText} Recipe > Recipe:[/] [green bold]{selectedRecipe.Title}[/]"),
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

        AnsiConsole.Clear();
        AnsiConsole.Write(layout);
    }

    private Recipe EditRecipe(Recipe selectedRecipe)
    {
        var editedRecipe = selectedRecipe;
        var continueEditRecipe = true;

        do
        {
            var layout = new Layout("Title");

            layout["Title"].Update(
                new Panel(
                    Align.Center(
                        new Markup($"[yellow bold]Main Menu > Edit Recipe > Recipe:[/] [green bold]{selectedRecipe.Title}[/]"),
                        VerticalAlignment.Top)));

            AnsiConsole.Write(layout);

            Console.SetCursorPosition(0, 4);
            var selectedEditorAction = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more actions)[/]")
                    .AddChoices("Edit Name", "Edit Instructions", "Edit Ingredients", "Edit Score", "Edit Creator",
                        "Save and go back", "Cancel and go back"));

            switch (selectedEditorAction)
            {
                case "Edit Name":
                    AnsiConsole.WriteLine($"Old name: {selectedRecipe.Title}");
                    var newName = AnsiConsole.Prompt(
                        new TextPrompt<string>("New name:"));
                    editedRecipe.Title = newName;

                    break;
                case "Edit Instructions":
                    var continueEditInstructions = true;

                    do
                    {
                        AnsiConsole.Write(layout);

                        Console.SetCursorPosition(0, 4);
                        var selectedInstructionEditType = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .PageSize(10)
                                .MoreChoicesText("[grey](Move up and down to reveal more steps)[/]")
                                .AddChoices("Add step", "Edit steps", "Delete step", "Go back"));

                        switch (selectedInstructionEditType)
                        {
                            case "Add step":
                                AnsiConsole.WriteLine($"Previous step: {editedRecipe.Instructions.LastOrDefault()}");
                                var newInstructionStep = AnsiConsole.Prompt(
                                    new TextPrompt<string>("Enter new instruction:"));
                                editedRecipe.Instructions.Add(newInstructionStep);

                                break;
                            case "Edit steps":
                                if (selectedRecipe.Instructions.Count > 0)
                                {
                                    var selectedInstructionStep = AnsiConsole.Prompt(
                                        new SelectionPrompt<string>()
                                            .PageSize(10)
                                            .EnableSearch()
                                            .Title("Select instruction you want to edit.")
                                            .MoreChoicesText("[grey](Move up and down to reveal more steps)[/]")
                                            .AddChoices(selectedRecipe.Instructions));

                                    AnsiConsole.WriteLine($"Old instruction: {selectedInstructionStep}");
                                    var editedInstructionStep = AnsiConsole.Prompt(
                                        new TextPrompt<string>("Enter new instruction:"));
                                    var indexOfOldInstructionStep =
                                        editedRecipe.Instructions.FindIndex(i => i == selectedInstructionStep);
                                    editedRecipe.Instructions[indexOfOldInstructionStep] = editedInstructionStep;
                                }
                                else
                                {
                                    AnsiConsole.WriteLine("No instructions found. Create one!");
                                    AnsiConsole.WriteLine("Press any key to go back!");
                                    Console.ReadKey();
                                }

                                break;
                            case "Delete step":
                                if (selectedRecipe.Instructions.Count > 0)
                                {
                                    var selectedInstructionStepToDelete = AnsiConsole.Prompt(
                                        new SelectionPrompt<string>()
                                            .PageSize(10)
                                            .EnableSearch()
                                            .Title("Select instruction you want to delete.")
                                            .MoreChoicesText("[grey](Move up and down to reveal more steps)[/]")
                                            .AddChoices(selectedRecipe.Instructions));

                                    editedRecipe.Instructions.Remove(selectedInstructionStepToDelete);
                                }
                                else
                                {
                                    AnsiConsole.WriteLine("No instructions found. Create one!");
                                    AnsiConsole.WriteLine("Press any key to go back!");
                                    Console.ReadKey();
                                }

                                break;
                            default:
                                continueEditInstructions = false;
                                break;
                        }
                    } while (continueEditInstructions);

                    break;
                case "Edit Ingredients":
                    var continueEditIngredients = true;

                    do
                    {
                        var selectedIngredientEditType = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .PageSize(10)
                                .MoreChoicesText("[grey](Move up and down to reveal more steps)[/]")
                                .AddChoices("Add ingredient", "Edit ingredients", "Delete ingredient", "Go back"));

                        switch (selectedIngredientEditType)
                        {
                            case "Add ingredient":
                                var mewIngredient = AnsiConsole.Prompt(
                                    new TextPrompt<string>("Enter new ingredient:"));
                                editedRecipe.Ingredients.Add(mewIngredient);

                                break;
                            case "Edit ingredients":
                                if (selectedRecipe.Ingredients.Count > 0)
                                {
                                    var selectedIngredient = AnsiConsole.Prompt(
                                        new SelectionPrompt<string>()
                                            .PageSize(10)
                                            .EnableSearch()
                                            .Title("Select ingredient you want to edit.")
                                            .MoreChoicesText("[grey](Move up and down to reveal more steps)[/]")
                                            .AddChoices(selectedRecipe.Ingredients));

                                    AnsiConsole.WriteLine($"Old Ingredient: {selectedIngredient}");
                                    var editedIngredient = AnsiConsole.Prompt(
                                        new TextPrompt<string>("Enter new instruction:"));
                                    var indexOfOldIngredient =
                                        editedRecipe.Ingredients.FindIndex(i => i == selectedIngredient);
                                    editedRecipe.Ingredients[indexOfOldIngredient] = editedIngredient;
                                }
                                else
                                {
                                    AnsiConsole.WriteLine("No ingredients found. Create one!");
                                    AnsiConsole.WriteLine("Press any key to go back!");
                                    Console.ReadKey();
                                }

                                break;
                            case "Delete ingredient":
                                if (selectedRecipe.Ingredients.Count > 0)
                                {
                                    var selectedIngredientToDelete = AnsiConsole.Prompt(
                                        new SelectionPrompt<string>()
                                            .PageSize(10)
                                            .EnableSearch()
                                            .Title("Select ingredients you want to delete.")
                                            .MoreChoicesText("[grey](Move up and down to reveal more steps)[/]")
                                            .AddChoices(selectedRecipe.Ingredients));

                                    editedRecipe.Ingredients.Remove(selectedIngredientToDelete);
                                }
                                else
                                {
                                    AnsiConsole.WriteLine("No ingredients found. Create one!");
                                    AnsiConsole.WriteLine("Press any key to go back!");
                                    Console.ReadKey();
                                }

                                break;
                            default:
                                continueEditIngredients = false;
                                break;
                        }
                    } while (continueEditIngredients);

                    break;
                case "Edit Score":
                    AnsiConsole.WriteLine("Enter new score for recipe, 0.0 - 5.0");
                    AnsiConsole.WriteLine($"Old score: {editedRecipe.Score}");
                    var newScore = AnsiConsole.Prompt(
                        new TextPrompt<double>("New score:"));
                    if (newScore < 0) newScore = 0;
                    if (newScore > 5) newScore = 5;
                    editedRecipe.Score = newScore;

                    break;
                case "Edit Creator":
                    AnsiConsole.WriteLine("Enter new creator name.");
                    AnsiConsole.WriteLine($"Old creator: {editedRecipe.CreatedBy}");
                    var newCreator = AnsiConsole.Prompt(
                        new TextPrompt<string>("New creator:"));
                    editedRecipe.CreatedBy = newCreator;

                    break;
                case "Save and go back":
                    editedRecipe.IsEdited = true;
                    continueEditRecipe = false;
                    break;
                default:
                    continueEditRecipe = false;
                    break;
            }
        } while (continueEditRecipe);

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