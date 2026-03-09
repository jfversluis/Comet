namespace CometTaskApp;

public class AddTaskPageState
{
	public string Title { get; set; } = "";
	public string Description { get; set; } = "";
	public int Priority { get; set; } = 1;
	public int Category { get; set; }
	public string ErrorMessage { get; set; } = "";
}

/// <summary>
/// Form for adding a new task.
/// Exercises: Component state, picker callbacks, form validation, navigation.
/// </summary>
public class AddTaskPage : Component<AddTaskPageState>
{
	public override View Render() =>
		new NavigationView
		{
			new ScrollView
			{
				new VStack(spacing: 16)
				{
					new Text("Create New Task")
						.FontSize(24)
						.FontWeight(FontWeight.Bold)
						.SemanticHeadingLevel(SemanticHeadingLevel.Level1),

					string.IsNullOrEmpty(State.ErrorMessage)
						? (View)new Spacer().Frame(height: 0)
						: new Text(State.ErrorMessage)
							.FontSize(14)
							.Color(Colors.Red),

					new Text("Title *").FontSize(12).Color(Colors.Gray),
					new TextField(State.Title, "What needs to be done?")
						.FontSize(16)
						.SemanticDescription("Task title, required")
						.AutomationId("NewTaskTitleField")
						.OnTextChanged(value => SetState(state =>
						{
							state.Title = value ?? "";
							if (!string.IsNullOrWhiteSpace(state.Title))
								state.ErrorMessage = "";
						})),

					new Text("Description").FontSize(12).Color(Colors.Gray),
					new TextField(State.Description, "Add more details...")
						.FontSize(14)
						.SemanticDescription("Task description")
						.AutomationId("NewTaskDescriptionField")
						.OnTextChanged(value => SetState(state => state.Description = value ?? "")),

					new Text("Priority").FontSize(12).Color(Colors.Gray),
					new Picker(State.Priority, "🟢 Low", "🟡 Medium", "🟠 High", "🔴 Critical")
						.SemanticDescription("Task priority level")
						.OnSelectedIndexChanged(index => SetState(state => state.Priority = index)),

					new Text("Category").FontSize(12).Color(Colors.Gray),
					new Picker(State.Category, "🏠 Personal", "💼 Work", "🛒 Shopping", "💪 Health", "📚 Learning", "📌 Other")
						.SemanticDescription("Task category")
						.OnSelectedIndexChanged(index => SetState(state => state.Category = index)),

					new Spacer().Frame(height: 20),

					new Button("Create Task", CreateTask)
					.SemanticDescription("Create the task")
					.AutomationId("CreateTaskButton"),

					new Button("Cancel", () => Navigation?.Pop())
						.Color(Colors.Gray)
						.SemanticDescription("Cancel and go back")
						.AutomationId("CancelCreateTaskButton"),
				}
				.Padding(16)
			}
		}
		.Title("New Task");

	void CreateTask()
	{
		if (string.IsNullOrWhiteSpace(State.Title))
		{
			SetState(state => state.ErrorMessage = "Please enter a task title.");
			return;
		}

		AppState.Instance.AddTask(new TaskItem
		{
			Title = State.Title.Trim(),
			Description = State.Description ?? "",
			Priority = (TaskPriority)State.Priority,
			Category = (TaskCategory)State.Category,
		});

		Navigation?.Pop();
	}
}
