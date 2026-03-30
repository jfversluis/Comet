using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using Google.Android.Material.BottomNavigation;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Platform;
using AView = Android.Views.View;

namespace Comet.Android.Controls
{
public class CometTabView : CustomFrameLayout
{
readonly BottomNavigationView _bottomNavigationView;
readonly FrameLayout _contentContainer;
List<View> _views;
AView _currentPlatformView;
IView _currentVirtualView;
int _selectedIndex = -1;

public IMauiContext MauiContext { get; set; }

public CometTabView(IMauiContext context) : base(context.Context)
{
MauiContext = context;

_contentContainer = new FrameLayout(context.Context)
{
LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent)
};
AddView(_contentContainer);

_bottomNavigationView = new BottomNavigationView(context.Context)
{
LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent)
{
Gravity = GravityFlags.Bottom
}
};

var val = new TypedValue();
context.Context.Theme.ResolveAttribute(global::Android.Resource.Attribute.ColorBackground, val, true);
_bottomNavigationView.SetBackgroundColor(new global::Android.Graphics.Color(val.Data));
_bottomNavigationView.ItemSelected += HandleNavigationItemSelected;
AddView(_bottomNavigationView);
}

public void CreateTabs(IList<View> views)
{
_views = views?.ToList();
_bottomNavigationView.Menu.Clear();

if (views == null) return;

for (int i = 0; i < views.Count; i++)
{
var view = views[i];
var title = view.GetEnvironment<string>(EnvironmentKeys.TabView.Title);
_bottomNavigationView.Menu.Add(0, i, i, title);
}
}

protected override void OnAttachedToWindow()
{
base.OnAttachedToWindow();
if (_views != null && _views.Count > 0 && _selectedIndex < 0)
ShowTab(0);
}

void ShowTab(int index)
{
if (_views == null || index < 0 || index >= _views.Count)
return;

if (_currentPlatformView != null)
{
_contentContainer.RemoveView(_currentPlatformView);
_currentPlatformView = null;
_currentVirtualView = null;
}

_selectedIndex = index;
var view = _views[index];
var renderView = view.GetView();
IView viewToRender = (renderView != null && renderView != view) ? renderView : view;

var platformView = viewToRender.ToPlatform(MauiContext);
if (platformView != null)
{
if (platformView.Parent != null)
(platformView.Parent as ViewGroup)?.RemoveView(platformView);
_currentPlatformView = platformView;
_currentVirtualView = viewToRender;
_contentContainer.AddView(_currentPlatformView, new FrameLayout.LayoutParams(
LayoutParams.MatchParent, LayoutParams.MatchParent));
RequestLayout();
Post(() =>
{
if (_currentPlatformView != null && _contentContainer.Width > 0 && _contentContainer.Height > 0)
{
MeasureAndArrangeContent();
_currentPlatformView.Layout(0, 0, _contentContainer.Width, _contentContainer.Height);
InvalidateViewTree(_currentPlatformView);
}
});
}
}

void MeasureAndArrangeContent()
{
if (_currentVirtualView == null) return;
var density = Context?.Resources?.DisplayMetrics?.Density ?? 1;
var widthDp = _contentContainer.Width / density;
var heightDp = _contentContainer.Height / density;
_currentVirtualView.Measure(widthDp, heightDp);
_currentVirtualView.Arrange(new Rect(0, 0, widthDp, heightDp));
}

static void InvalidateViewTree(AView view)
{
view.Invalidate();
if (view is ViewGroup vg)
{
for (int i = 0; i < vg.ChildCount; i++)
InvalidateViewTree(vg.GetChildAt(i));
}
}

protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
{
var widthSize = AView.MeasureSpec.GetSize(widthMeasureSpec);
var heightSize = AView.MeasureSpec.GetSize(heightMeasureSpec);

// Measure bottom nav first
_bottomNavigationView.Measure(
AView.MeasureSpec.MakeMeasureSpec(widthSize, MeasureSpecMode.Exactly),
AView.MeasureSpec.MakeMeasureSpec(heightSize, MeasureSpecMode.AtMost));
var navHeight = _bottomNavigationView.MeasuredHeight;

// Content gets remaining height
var contentHeight = heightSize - navHeight;
_contentContainer.Measure(
AView.MeasureSpec.MakeMeasureSpec(widthSize, MeasureSpecMode.Exactly),
AView.MeasureSpec.MakeMeasureSpec(contentHeight, MeasureSpecMode.Exactly));

SetMeasuredDimension(widthSize, heightSize);
}

protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
{
var width = right - left;
var height = bottom - top;
var navHeight = _bottomNavigationView.MeasuredHeight;
var contentHeight = height - navHeight;

_contentContainer.Layout(0, 0, width, contentHeight);
_bottomNavigationView.Layout(0, contentHeight, width, height);

MeasureAndArrangeContent();
}

void HandleNavigationItemSelected(object sender, Google.Android.Material.Navigation.NavigationBarView.ItemSelectedEventArgs e)
{
var index = e.Item.ItemId;
if (index != _selectedIndex)
ShowTab(index);
}
}
}
