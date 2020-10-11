using System.IO;
using UsAcRe.Core.Services;

namespace UsAcRe.Core.UI.Services {
	public class FileService : IFileService {
		public string ReadAllText(string path) {
			return File.ReadAllText(path);
		}

		public void WriteAllText(string path, string contents) {
			File.WriteAllText(path, contents);
		}
	}
}
