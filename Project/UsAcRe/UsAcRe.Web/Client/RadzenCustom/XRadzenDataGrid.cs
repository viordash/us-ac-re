using System.Linq;
using System.Threading.Tasks;
using Radzen;
using Radzen.Blazor;

namespace UsAcRe.Web.Client.RadzenCustom {
	public class XRadzenDataGrid<TItem> : RadzenDataGrid<TItem> {

		public new Task OnPageChanged(PagerEventArgs args) {
			return base.OnPageChanged(args);
		}

		public new Task OnPageSizeChanged(int value) {
			return base.OnPageSizeChanged(value);
		}

		int pageSize = 8;
		protected int CurrentPageSize {
			get {
				return pageSize;
			}
		}

		protected override void OnInitialized() {
			if(PageSize == default) {
				PageSize = pageSize;
			}
			pageSize = PageSize;
		}
	}
}
