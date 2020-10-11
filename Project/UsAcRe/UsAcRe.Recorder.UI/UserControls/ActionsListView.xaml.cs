using System;
using System.Collections.Specialized;
using System.Windows.Controls;

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
				ColumnAutoWidth();
			}
			ShowFooter();
		}

		void ShowFooter() {
			lbFooter.Text = $"{ListActions.SelectedIndex + 1} of {ListActions.Items.Count}";
		}

		private void ListActions_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			ShowFooter();
		}

		void ColumnAutoWidth() {
			ActionDataColumn.Width = ActionDataColumn.ActualWidth;
			ActionDataColumn.Width = Double.NaN;
		}
	}
}
