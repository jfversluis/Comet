namespace CometTaskApp;

public class TaskDetailProps
{
	public string TaskId { get; set; } = "";
}

public class TaskDetailState
{
	public string LoadedTaskId { get; set; } = "";
	public string Title { get; set; } = "";
	public string Description { get; set; } = "";
	public int Priority { get; set; }
	public int Category { get; set; }
}

/// <summary>
/// Detail view for a single task with typed props and edit capability.
/// Exercises: Component props/state, TextField, Picker, typed navigation.
/// </summary>
public class TaskDetailPage : Component<TaskDetailState, TaskDetailProps>
{
	[State] readonly AppState _state = AppState.Instance;

	public override View Render()
	{
		var task = _state.GetTask(Props.TaskId);
		if (task == null)
		{
			return new NavigationView
			{
				new VStack(spacing: 16)
				{
					new Text("Task not found")
						.FontSize(24)
						.FontWeight(FontWeight.Bold)
						.SemanticHeadingLevel(SemanticHeadingLevel.Level1),
					new Text("The selected task may have been removed from the list.")
						.FontSize(14)
						.Color(Colors.Gray),
					new Button("Back to tasks", () => Navigation?.Pop())
						.AutomationId("BackToTasksButton"),
				}
				.Padding(16)
			}
			.Title("Task Details");
		}

		EnsureStateMatches(task);

		return new NavigationView
		{
			new ScrollView
			{
				new VStack(spacing: 16)
				{
					new HStack(spacing: 8)
					{
						new Text(task.IsCompleted ? "✅ Completed" : "⏳ Pending")
							.FontSize(14)
							.Color(task.IsCompleted ? Colors.Green : Colors.Orange),
						new Spacer(),
						new Text($"Created: {task.CreatedAt:MMM dd, yyyy}")
							.FontSize(12)
							.Color(Colors.Gray),
					},

					new Text("Title").FontSize(12).Color(Colors.Gray),
					new TextField(State.Title, "Task title...")
						.FontSize(18)
						.SemanticDescription("Task title")
						.AutomationId("TaskDetailTitleField")
						.OnTextChanged(value => SetState(state => state.Title = value ?? "")),

					new Text("Description").FontSize(12).Color(Colors.Gray),
					new TextField(State.Description, "Task description...")
						.FontSize(14)
						.SemanticDescription("Task description")
						.AutomationId("TaskDetailDescriptionField")
						.OnTextChanged(value => SetState(state => state.Description = value ?? "")),

					new Text("Priority").FontSize(12).Color(Colors.Gray),
					new Picker(State.Priority, "Low", "Medium", "High", "Critical")
						.SemanticDescription("Task priority")
						.OnSelectedIndexChanged(index => SetState(state => state.Priority = index)),

					new Text("Category").FontSize(12).Color(Colors.Gray),
					new Picker(State.Category, "Personal", "Work", "Shopping", "Health", "Learning", "Other")
						.SemanticDescription("Task category")
						.OnSelectedIndexChanged(index => SetState(state => state.Category = index)),

					new Spacer().Frame(height: 20),

					new Button("Save Changes", SaveChanges)
						.SemanticDescription("Save task changes")
						.AutomationId("SaveTaskButton"),

					new HStack(spacing: 12)
					{
						new Button(task.IsCompleted ? "Mark Pending" : "Mark Complete", ToggleComplete)
							.Color(task.IsCompleted ? Colors.Orange : Colors.Green)
							.SemanticDescription("Toggle task completion")
							.AutomationId("ToggleTaskCompletionButton"),

						new Button("Delete", DeleteTask)
							.Color(Colors.Red)
							.SemanticDescription("Delete this task")
							.AutomationId("DeleteTaskButton"),
					},
				}
				.Padding(16)
			}
		}
		.Title("Task Details");
	}

	protected override bool ShouldUpdate(TaskDetailProps oldProps, TaskDetailProps newProps) =>
		oldProps?.TaskId != newProps?.TaskId;

	void EnsureStateMatches(TaskItem task)
	{
		if (State.LoadedTaskId == task.Id)
			return;

		State.LoadedTaskId = task.Id;
		State.Title = task.Title;
		State.Description = task.Description;
		State.Priority = (int)task.Priority;
		State.Category = (int)task.Category;
	}

	void SaveChanges()
	{
		_state.UpdateTask(Props.TaskId, task =>
		{
			task.Title = State.Title ?? "";
			task.Description = State.Description ?? "";
			task.Priority = (TaskPriority)State.Priority;
			task.Category = (TaskCategory)State.Category;
		});

		Navigation?.Pop();
	}

	void ToggleComplete()
	{
		_state.ToggleComplete(Props.TaskId);
		Navigation?.Pop();
	}

	void DeleteTask()
	{
		_state.RemoveTask(Props.TaskId);
		Navigation?.Pop();
	}
}
