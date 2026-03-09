using System;
using System.Collections.Generic;

using System.Text;
using Microsoft.Maui.Graphics;
using static Comet.CometControls;


/*
 
import SwiftUI

struct ContentView: View {
    var body: some View {
        VStack(alignment: .leading) {
            Text("Turtle Rock")
                .font(.title)
            HStack {
                Text("Joshua Tree National Park")
                    .font(.subheadline)
                Spacer()
                Text("California")
                    .font(.subheadline)
            }
        }
        .padding()
    }
}

 */

namespace Comet.Samples.Comparisons
{
	public class Section3 : Component
	{
				public override View Render() =>
				 VStack(LayoutAlignment.Start,
					Text("Turtle Rock"),
					HStack(
						Text("Joshua Tree National Park")
							.Background(Colors.Salmon),
						new Spacer(),
						Text("California")
							.Background(Colors.Green)
					)
				 ).Margin();

	}
}
