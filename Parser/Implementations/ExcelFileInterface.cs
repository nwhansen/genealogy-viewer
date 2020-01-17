//==============================================
// Copyright (c) 2019 Nathan Hansen
//==============================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using ExcelDataReader;

namespace Genealogy.Implementations {

	internal class ExcelFileInterface : BaseColumnInterface {


		private class ReaderEnumerator : IEnumerator<string[]> {
			private IExcelDataReader reader;

			public ReaderEnumerator(IExcelDataReader reader) {
				this.reader = reader;
			}

			public string[] Current {
				get {
					string[] fields = new string[reader.FieldCount];
					for (int i = 0; i < fields.Length; i++) {
						object fieldData = reader.GetValue(i);
						fields[i] = fieldData != null ? fieldData.ToString() : string.Empty;
					}
					return fields;
				}
			}

			object IEnumerator.Current => Current;

			public void Dispose() {
				reader.Dispose();
			}

			public bool MoveNext() {
				return reader.Read();
			}

			public void Reset() {
				throw new NotImplementedException();
			}

		}

		protected override ParserHelper LoadFile(string filename) {
			var stream = File.Open(filename, FileMode.Open, FileAccess.Read);
			var reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
			//The enumerator will dispose of the reader
			return new ParserHelper(stream, new ReaderEnumerator(reader));
		}
	}
}
