namespace UsAcRe.Web.Shared.Exceptions {
	public class ObjectNotFoundException : UsAcReServerException {
		public ObjectNotFoundException()
			: base("Object not found.") {
		}
	}
}
