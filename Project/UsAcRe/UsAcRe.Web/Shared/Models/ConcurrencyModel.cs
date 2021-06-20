namespace UsAcRe.Web.Shared.Models {
	public interface IConcurrencyModel {
		string ConcurrencyStamp { get; set; }
	}

	public class ConcurrencyModel : IConcurrencyModel {
		public string ConcurrencyStamp { get; set; }
	}
}
