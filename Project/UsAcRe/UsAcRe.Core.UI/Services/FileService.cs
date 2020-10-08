using System.IO;

namespace UsAcRe.Core.UI.Services {
	public interface IFileService {
		string ReadAllText(string path);
		void WriteAllText(string path, string contents);
	}

	public class FileService : IFileService {
		public string ReadAllText(string path) {
			return File.ReadAllText(path);
		}

		public void WriteAllText(string path, string contents) {
			File.WriteAllText(path, contents);
		}
	}
}
