using System;
using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;
using Microsoft.Maui;

namespace Comet.Android.Controls
{
	public class CometNavigationView : CustomFrameLayout
	{
		IMauiContext MauiContext { get; set; }
		public CometNavigationView(IMauiContext context) : base(context.Context)
		{
			MauiContext = context;
		}

		FragmentManager GetSupportFragmentManager() =>
			(MauiContext.Context as AppCompatActivity)?.SupportFragmentManager;

		public void SetRoot(View view)
		{
			if (!isAttached)
			{
				contentView = view;
			}
			else
			{
				var mgr = GetSupportFragmentManager();
				mgr?.BeginTransaction()
					.Replace(Id, new CometFragment(view, MauiContext))
					.CommitAllowingStateLoss();
			}
		}
		 
		public void NavigateTo(View view)
		{
			var mgr = GetSupportFragmentManager();
			mgr?.BeginTransaction()
				.SetTransition((int)global::Android.App.FragmentTransit.FragmentFade)
				.AddToBackStack(view.Id)
				.Replace(Id, new CometFragment(view, MauiContext))
				.CommitAllowingStateLoss();
		}
		bool isAttached = false;
		View contentView;
		protected override void OnAttachedToWindow()
		{
			base.OnAttachedToWindow();
			isAttached = true;
			if(contentView != null)
				SetRoot(contentView);
			contentView = null;
		}

		public void Pop() =>
			GetSupportFragmentManager()?.PopBackStack();
	}
}
