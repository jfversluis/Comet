namespace CometDigitsGame;

class MainPage : Component<GameState>
{
	public override View Render()
	{
		return ScrollView(
			VStack(16,
				RenderTarget(),
				RenderBoard(),
				RenderOperationBar(),
				RenderErrorMessage(),
				RenderWinMessage(),
				RenderViewToggle(),
				State.ShowOperationList ? RenderOperationList() : null!
			)
			.Padding(new Thickness(16))
		)
		.Background(Colors.White);
	}

	View RenderTarget()
	{
		return Text($"{State.CurrentGame.TargetValue}")
			.FontSize(48)
			.FontWeight(FontWeight.Bold)
			.HorizontalTextAlignment(TextAlignment.Center)
			.Color(Theme.GreenColor)
			.Margin(new Thickness(0, 8));
	}

	View RenderBoard()
	{
		var board = State.CurrentBoard;
		var row0 = board.Where(n => n.Position.Row == 0).OrderBy(n => n.Position.Column).ToArray();
		var row1 = board.Where(n => n.Position.Row == 1).OrderBy(n => n.Position.Column).ToArray();

		return VStack(12,
			HStack(12,
				row0.Select(RenderNumberTile).ToArray()
			).FillHorizontal(),
			HStack(12,
				row1.Select(RenderNumberTile).ToArray()
			).FillHorizontal()
		)
		.Padding(new Thickness(8));
	}

	View RenderNumberTile(GameNumber number)
	{
		if (number.Value == 0)
			return new Spacer().Frame(width: 86, height: 86);

		var isSelected = State.CurrentNumber == number;
		var isError = State.OperationInError?.Left == number || State.OperationInError?.Right == number;
		var bgColor = isError ? Theme.ErrorColor : isSelected ? Theme.GreenColor : Colors.White;
		var textColor = (isSelected || isError) ? Colors.White : Colors.Black;
		var borderColor = isError ? Theme.ErrorColor : isSelected ? Theme.GreenColor : Colors.DarkGrey;

		return Button($"{number.Value}", () => OnNumberClicked(number))
			.Background(bgColor)
			.Color(textColor)
			.FontSize(22)
			.FontWeight(FontWeight.Bold)
			.CornerRadius(43)
			.BorderWidth(isSelected ? 0 : 2)
			.BorderColor(borderColor)
			.Frame(width: 86, height: 86);
	}

	View RenderOperationBar()
	{
		return HStack(8,
			RenderUndoButton(),
			RenderOperationButton(Operation.Add, "add_icon.png", "add_icon_green.png", "+"),
			RenderOperationButton(Operation.Subtract, "subtract_icon.png", "subtract_icon_green.png", "−"),
			RenderOperationButton(Operation.Multiply, "multiply_icon.png", "multiply_icon_green.png", "×"),
			RenderOperationButton(Operation.Divide, "divide_icon.png", "divide_icon_green.png", "÷")
		)
		.Padding(new Thickness(8, 16));
	}

	View RenderUndoButton()
	{
		return Button("↩", () => OnUndoLastOperation())
			.FontSize(24)
			.Color(Theme.GreenColor)
			.Background(Colors.Transparent)
			.Frame(width: 50, height: 50);
	}

	View RenderOperationButton(Operation operation, string icon, string selectedIcon, string label)
	{
		var isSelected = State.CurrentOperation == operation;
		var isError = State.OperationInError?.Operation == operation;
		var bgColor = isError ? Theme.ErrorColor : isSelected ? Theme.GreenColor : Colors.LightGrey;
		var textColor = (isSelected || isError) ? Colors.White : Colors.Black;

		return Button(label, () => OnOperationSelected(operation))
			.Background(bgColor)
			.Color(textColor)
			.FontSize(24)
			.FontWeight(FontWeight.Bold)
			.CornerRadius(25)
			.Frame(width: 50, height: 50);
	}

	View RenderErrorMessage()
	{
		if (State.OperationInError == null) return new Spacer().Frame(height: 0);

		return Text("Invalid operation!")
			.Color(Theme.ErrorColor)
			.FontSize(16)
			.HorizontalTextAlignment(TextAlignment.Center)
			.Margin(new Thickness(0, 4));
	}

	View RenderWinMessage()
	{
		if (!State.IsWon) return new Spacer().Frame(height: 0);

		return Text("🎉 You got it!")
			.Color(Theme.GreenColor)
			.FontSize(28)
			.FontWeight(FontWeight.Bold)
			.HorizontalTextAlignment(TextAlignment.Center)
			.Margin(new Thickness(0, 8));
	}

	View RenderViewToggle()
	{
		return Button(
			State.ShowOperationList ? "Hide Operations" : "Show Operations",
			() => SetState(s => s.ShowOperationList = !s.ShowOperationList))
			.Background(Theme.GreenColor)
			.Color(Colors.White)
			.CornerRadius(8)
			.Margin(new Thickness(0, 8));
	}

	View RenderOperationList()
	{
		var ops = State.Operations.Reverse().ToArray();
		if (ops.Length == 0)
			return Text("No operations yet.")
				.Color(Colors.Grey)
				.FontSize(16)
				.HorizontalTextAlignment(TextAlignment.Center);

		return VStack(4,
			Text("Your operations:")
				.FontSize(20)
				.FontWeight(FontWeight.Bold)
				.HorizontalTextAlignment(TextAlignment.Center)
				.Margin(new Thickness(0, 8)),
			VStack(2,
				ops.Select(RenderOperationItem).ToArray()
			)
		);
	}

	View RenderOperationItem(OperationItem op)
	{
		var sign = op.Operation switch
		{
			Operation.Add => "+",
			Operation.Subtract => "−",
			Operation.Multiply => "×",
			Operation.Divide => "÷",
			_ => "?"
		};
		return Text($"{op.Left.Value} {sign} {op.Right.Value} = {op.CalcValue()}")
			.FontSize(18)
			.Color(Colors.Black)
			.HorizontalTextAlignment(TextAlignment.Center)
			.Padding(new Thickness(0, 4));
	}

	void OnNumberClicked(GameNumber number)
	{
		if (State.CurrentOperation == null)
		{
			SetState(s => s.CurrentNumber = s.CurrentNumber == number ? null : number);
		}
		else if (State.CurrentNumber != null)
		{
			var newOperation = new OperationItem(State.CurrentNumber, number, State.CurrentOperation.Value);

			if (!newOperation.IsValid())
			{
				SetState(s =>
				{
					s.CurrentNumber = null;
					s.CurrentOperation = null;
					s.OperationInError = newOperation;
				});

				// Clear error after a delay
				ClearErrorAfterDelay();
			}
			else
			{
				SetState(s =>
				{
					s.CurrentNumber = null;
					s.CurrentOperation = null;
					s.OperationInError = null;
				});

				ApplyOperation(newOperation);
			}
		}
	}

	void OnOperationSelected(Operation operation)
	{
		if (State.CurrentNumber != null)
		{
			SetState(s => s.CurrentOperation = operation);
		}
	}

	void OnUndoLastOperation()
	{
		if (State.BoardStates.Count == 0) return;

		SetState(s =>
		{
			s.BoardStates.Pop();
			s.Operations.Pop();
			s.CurrentNumber = null;
			s.CurrentOperation = null;
			s.OperationInError = null;
		});
	}

	void ApplyOperation(OperationItem newOperation)
	{
		SetState(s =>
		{
			var currentBoard = s.CurrentBoard;
			var newBoard = currentBoard
				.Select(n =>
				{
					if (n.Id == newOperation.Left.Id || n.Value == 0)
						return new GameNumber(newOperation.Left.Id, newOperation.Right.Position, 0);
					else if (n.Id == newOperation.Right.Id)
						return new GameNumber(newOperation.Right.Id, newOperation.Right.Position, newOperation.CalcValue());
					return n;
				})
				.ToArray();

			s.BoardStates.Push(newBoard);
			s.Operations.Push(newOperation);
		});
	}

	async void ClearErrorAfterDelay()
	{
		await Task.Delay(1000);
		SetState(s => s.OperationInError = null);
	}
}
