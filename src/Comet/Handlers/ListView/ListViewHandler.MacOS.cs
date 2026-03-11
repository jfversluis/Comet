using System;
using AppKit;
using Microsoft.Maui;
using Microsoft.Maui.Handlers;

namespace Comet.Handlers
{
	public partial class ListViewHandler : ViewHandler<IListView, NSScrollView>
	{
		NSTableView _tableView;

		public static void MapListViewProperty(IElementHandler viewHandler, IListView virtualView)
		{
			// TODO: Implement NSTableView data source binding
		}

#nullable enable
		public static void MapReloadData(ListViewHandler viewHandler, IListView virtualView, object? value)
#nullable restore
		{
			viewHandler._tableView?.ReloadData();
		}

		protected override NSScrollView CreatePlatformView()
		{
			_tableView = new NSTableView();
			var column = new NSTableColumn("Content") { Title = "" };
			_tableView.AddColumn(column);
			_tableView.HeaderView = null;

			var scrollView = new NSScrollView
			{
				DocumentView = _tableView,
				HasVerticalScroller = true,
				AutohidesScrollers = true,
			};
			return scrollView;
		}
	}
}
