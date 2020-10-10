using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace UsAcRe.Recorder.UI {
	/// <summary>
	/// Interaction logic for ActionsListView.xaml
	/// </summary>
	public partial class ActionsListView : UserControl {

		public ActionsListView() {
			InitializeComponent();
			((INotifyCollectionChanged)ListActions.Items).CollectionChanged += ListView_CollectionChanged;
			ShowFooter();
		}

		private void ListView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if(e.Action == NotifyCollectionChangedAction.Add) {
				ListActions.SelectedIndex = ListActions.Items.Count - 1;
				ListActions.ScrollIntoView(ListActions.SelectedItem);
			}
			ShowFooter();
		}

		void ShowFooter() {
			lbFooter.Text = $"{ListActions.SelectedIndex + 1} of {ListActions.Items.Count}";
		}

		private void ListActions_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			ShowFooter();
		}
	}
}
