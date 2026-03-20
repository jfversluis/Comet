namespace CometTodoApp;

public class MainPageState
{
	public string NewTaskText { get; set; } = "";
}

public class MainPage : Component<MainPageState>
{
	static int _nextId = 1;
	readonly SignalList<TodoItem> _todos = new(new[]
	{
		new TodoItem { Id = _nextId++, Task = "Buy groceries", Done = false },
		new TodoItem { Id = _nextId++, Task = "Walk the dog", Done = false },
		new TodoItem { Id = _nextId++, Task = "Read Comet docs", Done = true },
	});

	public override View Render()
	{
		return VStack(
			// Header
			Text("Todo List")
				.FontSize(28)
				.FontWeight(FontWeight.Bold)
				.Color(Colors.DarkSlateBlue)
				.Margin(new Thickness(16, 16, 16, 8)),

			// New task input row
			HStack(8,
				TextField(State.NewTaskText, "What needs to be done?")
					.OnTextChanged(t => SetState(s => s.NewTaskText = t))
					.FillHorizontal(),
				Button("Add", () =>
				{
					var text = State.NewTaskText?.Trim();
					if (!string.IsNullOrEmpty(text))
					{
						_todos.Add(new TodoItem
						{
							Id = _nextId++,
							Task = text,
							Done = false
						});
						SetState(s => s.NewTaskText = "");
					}
				})
				.Color(Colors.White)
				.Background(Colors.DarkSlateBlue)
				.CornerRadius(8)
				.Frame(width: 70)
			)
			.Padding(new Thickness(16, 4)),

			// Summary bar
			HStack(16,
				Text(() => $"📋 {_todos.Count} total")
					.FontSize(13)
					.Color(Colors.DimGray),
				Text(() => $"✅ {_todos.Count(t => t.Done)} done")
					.FontSize(13)
					.Color(Colors.Green),
				Text(() => $"⏳ {_todos.Count(t => !t.Done)} pending")
					.FontSize(13)
					.Color(Colors.Orange),
				Spacer()
			)
			.Padding(new Thickness(16, 8)),

			// Todo list
			ScrollView(Orientation.Vertical,
				VStack(TodoRows())
			),

			// Clear completed button
			Button("Clear Completed", () =>
			{
				_todos.Batch(list =>
				{
					list.RemoveAll(t => t.Done);
				});
			})
			.Color(Colors.Crimson)
			.Margin(new Thickness(16, 8, 16, 16))
		);
	}

	View[] TodoRows()
	{
		var views = new View[_todos.Count];
		for (var i = 0; i < _todos.Count; i++)
		{
			views[i] = TodoRow(_todos[i]);
		}
		return views;
	}

	View TodoRow(TodoItem item)
	{
		return HStack(10,
			CheckBox(item.Done)
				.OnCheckedChanged(isChecked =>
				{
					item.Done = isChecked;
					// Trigger a reactive update by touching the list
					_todos.Batch(list => { });
				}),
			Text(item.Task)
				.FontSize(16)
				.TextDecorations(item.Done ? Microsoft.Maui.TextDecorations.Strikethrough : Microsoft.Maui.TextDecorations.None)
				.Color(item.Done ? Colors.Gray : Colors.Black)
				.VerticalTextAlignment(TextAlignment.Center)
				.FillHorizontal(),
			Button("🗑", () =>
			{
				_todos.Remove(item);
			})
			.Background(Colors.Transparent)
			.Color(Colors.Red)
			.Frame(width: 44, height: 44)
		)
		.Padding(new Thickness(16, 6));
	}
}
