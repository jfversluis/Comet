namespace CometTaskApp;

/// <summary>
/// Main app using TabView for multi-page navigation.
/// Exercises: TabView, CometApp, UseCometApp builder pattern.
/// </summary>
public class TaskApp : CometApp
{
	public TaskApp()
	{
		Body = CreateRootView;
	}

	public static View CreateRootView() =>
		new TabView
		{
			new TaskListPage().Title("Tasks"),
			new StatsPage().Title("Stats"),
			new SettingsPage().Title("Settings"),
		};
}

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

#if DEBUG
		builder.UseCometSampleDebugHost(TaskApp.CreateRootView);
#else
		builder.UseCometApp<TaskApp>();
#endif

#if DEBUG
		builder.EnableSampleRuntimeDebugging();
#endif
		return builder.Build();
	}
}
