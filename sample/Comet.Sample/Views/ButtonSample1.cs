using System;
using System.Collections.Generic;
using static Comet.CometControls;

namespace Comet.Samples
{
	public class ButtonSample1 : Component
	{
		readonly State<int> count = 0;

				public override View Render() => VStack(
			Button("Increment Value", () => count.Value ++ ),
			Text(() => $"Value: {count.Value}")
		);

	}
}
