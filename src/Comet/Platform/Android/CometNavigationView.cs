using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Platform;
using AView = Android.Views.View;

namespace Comet.Android.Controls
{
public class CometNavigationView : CustomFrameLayout
{
IMauiContext MauiContext { get; set; }
readonly Stack<View> viewStack = new();
AView currentPlatformView;
IView currentVirtualView;
View currentView;

public CometNavigationView(IMauiContext context) : base(context.Context)
{
MauiContext = context;
}

public void SetRoot(View view)
{
if (!isAttached)
{
contentView = view;
}
else
{
viewStack.Clear();
ShowView(view);
}
}

void ShowView(View view)
{
if (currentPlatformView != null)
{
RemoveView(currentPlatformView);
currentPlatformView = null;
currentVirtualView = null;
}

currentView = view;

var renderView = view.GetView();
IView viewToRender = (renderView != null && renderView != view) ? renderView : view;

var platformView = viewToRender.ToPlatform(MauiContext);
if (platformView != null)
{
if (platformView.Parent != null)
(platformView.Parent as ViewGroup)?.RemoveView(platformView);
currentPlatformView = platformView;
currentVirtualView = viewToRender;
AddView(currentPlatformView, new FrameLayout.LayoutParams(
LayoutParams.MatchParent, LayoutParams.MatchParent));
RequestLayout();
}
}

public void NavigateTo(View view)
{
if (currentView != null)
viewStack.Push(currentView);
ShowView(view);
}

bool isAttached = false;
View contentView;
protected override void OnAttachedToWindow()
{
base.OnAttachedToWindow();
isAttached = true;
if (contentView != null)
{
SetRoot(contentView);
contentView = null;
}
}

protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
{
base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
var widthSize = AView.MeasureSpec.GetSize(widthMeasureSpec);
var heightSize = AView.MeasureSpec.GetSize(heightMeasureSpec);
for (int i = 0; i < ChildCount; i++)
{
var child = GetChildAt(i);
child?.Measure(
AView.MeasureSpec.MakeMeasureSpec(widthSize, global::Android.Views.MeasureSpecMode.Exactly),
AView.MeasureSpec.MakeMeasureSpec(heightSize, global::Android.Views.MeasureSpecMode.Exactly));
}
}

protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
{
var width = right - left;
var height = bottom - top;

if (currentVirtualView != null && width > 0 && height > 0)
{
var density = Context?.Resources?.DisplayMetrics?.Density ?? 1;
var widthDp = width / density;
var heightDp = height / density;
currentVirtualView.Measure(widthDp, heightDp);
currentVirtualView.Arrange(new Microsoft.Maui.Graphics.Rect(0, 0, widthDp, heightDp));
}

for (int i = 0; i < ChildCount; i++)
{
var child = GetChildAt(i);
child?.Layout(0, 0, width, height);
}
}

public void Pop()
{
if (viewStack.Count > 0)
{
var previous = viewStack.Pop();
ShowView(previous);
}
}
}
}
