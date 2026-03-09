using System;
using System.Collections.Generic;
using static Comet.CometControls;

namespace Comet.Samples
{
	public class TextFieldSample1 : Component
	{
		readonly State<string> name1 = "";

				public override View Render() => VStack(
			TextField(name1, "Name", ()=>{
				Console.WriteLine("Completed");
			}),
			
			HStack(
				Text("onCommit:"),
				Text(name1),
				Spacer()
			)
		).FillHorizontal();
	}

}
