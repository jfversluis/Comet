using System;
using System.Linq;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Comet
{
	public class RadioButton : View, IRadioButton
	{
		bool _hasInitializedIsChecked;

		public RadioButton(
			Binding<string> label = null,
			Binding<bool> selected = null,
			Action onClick = null)
		{
			Label = label;
			Selected = selected;
			OnClick = onClick;
		}

		public RadioButton(
			Func<string> label,
			Func<bool> selected = null,
			Action onClick = null)
			: this(
				  (Binding<string>)label,
				  (Binding<bool>)selected,
				  onClick)
		{

		}

		Binding<string> _label;
		public Binding<string> Label
		{
			get => _label;
			private set => this.SetBindingValue(ref _label, value);
		}

		Binding<bool> _selected;
		public Binding<bool> Selected
		{
			get => _selected;
			private set => this.SetBindingValue(ref _selected, value);
		}

		public Action OnClick { get; private set; }

		public string GroupName { get; set; }

		public object Value { get; set; }

		public event EventHandler<CheckedChangedEventArgs> CheckedChanged;

		object IContentView.Content => Value ?? Label?.CurrentValue;

		IView IContentView.PresentedContent => null;

		Size IContentView.CrossPlatformMeasure(double widthConstraint, double heightConstraint)
		{
			// Return content size without recursing into IView.Measure
			// (which would call GetDesiredSize → CrossPlatformMeasure → infinite loop)
			var content = ((IContentView)this).PresentedContent;
			return content?.Measure(widthConstraint, heightConstraint) ?? Size.Zero;
		}

		Size IContentView.CrossPlatformArrange(Rect bounds)
		{
			var content = ((IContentView)this).PresentedContent;
			content?.Arrange(bounds);
			return bounds.Size;
		}

		bool IRadioButton.IsChecked
		{
			get => Selected?.CurrentValue ?? false;
			set => SetIsChecked(value);
		}

		Color ITextStyle.TextColor => this.GetEnvironment<Color>(nameof(ITextStyle.TextColor))
			?? this.GetColor();

		Font ITextStyle.Font => this.GetFont(null);

		double ITextStyle.CharacterSpacing => this.GetEnvironment<double?>(nameof(ITextStyle.CharacterSpacing)) ?? 0;

		Color IButtonStroke.StrokeColor => this.GetEnvironment<Color>(nameof(IButtonStroke.StrokeColor))
			?? this.GetEnvironment<Color>(EnvironmentKeys.Button.BorderColor);

		double IButtonStroke.StrokeThickness => this.GetEnvironment<double?>(nameof(IButtonStroke.StrokeThickness))
			?? this.GetEnvironment<double?>(EnvironmentKeys.Button.BorderWidth)
			?? 0;

		int IButtonStroke.CornerRadius => this.GetEnvironment<int?>(nameof(IButtonStroke.CornerRadius))
			?? this.GetEnvironment<int?>(EnvironmentKeys.Button.CornerRadius)
			?? 0;

		protected override View GetRenderView()
		{
			View view = base.GetRenderView();

			if (view.Parent is RadioGroup)
			{
				return view;
			}

			// TODO: Create Comet-specific UI exceptions
			throw new Exception("A RadioButton must be in a RadioGroup");
		}

		internal string ResolveGroupName()
		{
			if (!string.IsNullOrWhiteSpace(GroupName))
			{
				return GroupName;
			}

			if (Parent is RadioGroup radioGroup)
			{
				return radioGroup.ResolveGroupName();
			}

			return null;
		}

		void SetIsChecked(bool value, bool notify = true)
		{
			var previous = Selected?.CurrentValue ?? false;
			var shouldNotify = notify && _hasInitializedIsChecked && previous != value;

			if (Selected != null)
			{
				Selected.Set(value);
			}
			else
			{
				Selected = value;
			}

			_hasInitializedIsChecked = true;

			if (value)
			{
				UncheckSiblings();
			}

			if (!shouldNotify)
			{
				return;
			}

			CheckedChanged?.Invoke(this, new CheckedChangedEventArgs(value));

			if (value)
			{
				OnClick?.Invoke();
			}
		}

		void UncheckSiblings()
		{
			if (Parent is not RadioGroup radioGroup)
			{
				return;
			}

			var groupName = ResolveGroupName();

			foreach (var sibling in radioGroup.OfType<RadioButton>().Where(x => x != this))
			{
				if (groupName != null && sibling.ResolveGroupName() != groupName)
				{
					continue;
				}

				sibling.SetIsChecked(false);
			}
		}
	}
}
 
