//==============================================
// Copyright (c) 2019 Nathan Hansen
//==============================================
using System;
using System.IO;

using Genealogy.Implementations;

namespace Genealogy {
	public class FileInterfaceFactory {

		public string FileFilter { get; } = "Excel|*.xsl;*.xlsx|CSV|*.csv";

		public IGenealogyFileInterface CreateFileInterface(string path) {
			string extension = Path.GetExtension(path);
			if (string.IsNullOrEmpty(extension))
				throw new Exception("Extension missing I can't do that");
			if (AnyMatch(extension, ".csv")) {
				return new CSVFileInterface();
			} else if (AnyMatch(extension, ".xlsx", ".xsl")) {
				return new ExcelFileInterface();
			}
			throw new Exception("Unknown extension: " + extension);
		}


		private bool AnyMatch(string ext, params string[] tests) {
			if (string.IsNullOrEmpty(ext))
				return false;
			foreach (var test in tests) {
				if (test.Equals(ext, StringComparison.InvariantCultureIgnoreCase))
					return true;
			}
			return false;
		}
	}
}
