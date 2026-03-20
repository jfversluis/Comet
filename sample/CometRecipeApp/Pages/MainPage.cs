using System;
using System.Collections.Generic;
using System.Linq;
using Comet;
using CometRecipeApp.Model;
using CometRecipeApp.Styles;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;

namespace CometRecipeApp.Pages;

public class MainPage : View
{
	[Body]
	View body()
	{
		var cards = RecipesData.DessertMenu.Select(RenderRecipeCard).ToArray();

		return NavigationView(
			new ScrollView
			{
				new VStack(spacing: 16)
				{
					Text("Dessert Menu")
						.FontSize(34)
						.FontWeight(FontWeight.Bold)
						.Color(AppColors.Black)
						.Padding(new Thickness(20, 16, 20, 0)),

					Text("Choose your favorite treat")
						.FontSize(16)
						.Color(Colors.Grey)
						.Padding(new Thickness(20, 0, 20, 8)),

					new VStack(spacing: 20)
					{
						cards
					}.Padding(new Thickness(16, 0, 16, 20))
				}
			}
		)
		.Title("Recipes")
		.Background(Colors.White);
	}

	View RenderRecipeCard(Recipe recipe)
	{
		return new Grid(columns: new object[] { "*", 140 })
		{
			new VStack(spacing: 6)
			{
				Text(recipe.Title)
					.FontSize(24)
					.FontWeight(FontWeight.Bold)
					.Color(Colors.White),

				Text(recipe.Description)
					.FontSize(12)
					.Color(Colors.White.WithAlpha(0.85f))
			}
			.Padding(new Thickness(16, 16, 8, 16))
			.Cell(row: 0, column: 0),

			Image(recipe.ImageSource)
				.Aspect(Aspect.AspectFit)
				.Frame(width: 130, height: 130)
				.Cell(row: 0, column: 1)
		}
		.Background(new SolidPaint(recipe.BgColor))
		.ClipShape(new RoundedRectangle(20))
		.Shadow(AppColors.BlackLight.WithAlpha(0.4f), radius: 15f, y: 6f)
		.Frame(height: 200)
		.FillHorizontal()
		.OnTap(_ => this.Navigate(new RecipeDetailPage(recipe)));
	}
}
