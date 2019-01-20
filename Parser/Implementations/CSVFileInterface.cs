using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genealogy.Model;
using Microsoft.VisualBasic.FileIO;

namespace Genealogy.Implementations {

	internal class CSVFileInterface : BaseColumnInterface {

		private class TextParserEnumerator : IEnumerator<string[]> {
			private TextFieldParser parser;

			public TextParserEnumerator(TextFieldParser parser) {
				this.parser = parser;
			}

			public string[] Current { get; private set; }

			object IEnumerator.Current => Current;

			public void Dispose() {
				parser.Dispose();
			}

			public bool MoveNext() {
				if (parser.EndOfData)
					return false;
				Current = parser.ReadFields();
				return true;
			}

			public void Reset() {
				throw new NotImplementedException();
			}
		}

		protected override ParserHelper LoadFile(string filename) {
			TextFieldParser parser = new TextFieldParser(filename) {
				TextFieldType = FieldType.Delimited
			};
			parser.SetDelimiters(",");
			//No additional disposing of resources required
			return new ParserHelper(null, new TextParserEnumerator(parser));
		}
	}
	/*
	public class CSVFileInterface : IGenealogyFileInterface {

		private enum ParsingState { Unknown, Attributes, Individuals };

		private ParsingState parseState = ParsingState.Unknown;
		private int lineCount = 0;
		private int deathDateIndex = -1;
		private int birthDateIndex = -1;
		private int fatherIndex = 3;
		private int motherIndex = 2;
		private int attributeStart = 4;
		private int sexIndex = 1;

		public CSVFileInterface() {
		}

		public IndividualManager ParseFile(string filename) {
			IndividualManager manager = new IndividualManager();
			Reset();
			using (TextFieldParser parser = new TextFieldParser(filename)) {
				parser.TextFieldType = FieldType.Delimited;
				parser.SetDelimiters(",");
				while (!parser.EndOfData) {
					string[] fields = parser.ReadFields();
					lineCount++;
					if (fields == null || fields.Length == 0 || fields.All(i => string.IsNullOrWhiteSpace(i)))
						continue;
					//Determine our state
					switch (parseState) {
						case ParsingState.Unknown when IsAttributeHeader(fields):
							//Start Attribute parsing
							parseState = ParsingState.Attributes;
							continue;
						case ParsingState.Unknown when IsIndividualHeader(fields):
							//Start individual processing
							ParseIndividualHeaderLine(fields);
							parseState = ParsingState.Individuals;
							continue;
						case ParsingState.Unknown:
							//Fall into our parse state of individual
							parseState = ParsingState.Individuals;
							ParseIndividualLine(manager, fields);
							break;
						case ParsingState.Attributes when IsIndividualHeader(fields):
							//We switch into individual processing
							ParseIndividualHeaderLine(fields);
							parseState = ParsingState.Individuals;
							continue;
						case ParsingState.Attributes:
							ParseAttributeLine(fields, manager.AttributesFactory);
							break;
						case ParsingState.Individuals:
							ParseIndividualLine(manager, fields);
							break;
					}
				}
			}
			return manager;
		}

		/// <summary>
		/// Reads the attribute field lines
		/// </summary>
		/// <param name="fields">The columns to read</param>
		private void ParseAttributeLine(string[] fields, IndividualAttributesFactory attributesFactory) {
			attributesFactory.CreateAttribute(fields[0], fields[1]);
		}

		/// <summary>
		/// Reads an individual fields line
		/// </summary>
		/// <param name="manager">The manager to put the individuals into</param>
		/// <param name="fields">The fields to load</param>
		private void ParseIndividualLine(IndividualManager manager, string[] fields) {
			//We need at least as many columns as our attribute start (fail more predictably)
			if (fields.Length < attributeStart)
				Array.Resize(ref fields, attributeStart);
			string id = fields[0];
			if (string.IsNullOrWhiteSpace(id)) {
				throw new Exception(string.Format("Individual ID is missing at line: {0}", lineCount));
			}

			bool female = IsFemale(fields[sexIndex]);
			string motherCode = fields[motherIndex];
			string fatherCode = fields[fatherIndex];

			Individual individual = manager.CreateIndividual(id, female, fatherCode, motherCode, false);
			if (deathDateIndex > 0) {
				try {
					individual.DeathDate = DateTime.Parse(fields[deathDateIndex]);
				} catch (FormatException) { } //Swallow, don't really care enough
			}
			if (birthDateIndex > 0) {
				try {
					individual.BirthDate = DateTime.Parse(fields[birthDateIndex]);
				} catch (FormatException) { } //Swallow, dont really care enough
			}
			for (int i = attributeStart; i < fields.Length; i++) {
				if (string.IsNullOrEmpty(fields[i]))
					continue;
				individual.AddAttribute(manager.AttributesFactory.CreateAttribute(fields[i], fields[i]));
			}
		}

		private void ParseIndividualHeaderLine(string[] fields) {
			fatherIndex = IndexOf("Father", fields);
			if (fatherIndex == -1) {
				throw new Exception(string.Format("Missing Father Header on line {0}", lineCount));
			}
			motherIndex = IndexOf("Mother", fields);
			if (fatherIndex == -1) {
				throw new Exception(string.Format("Missing Mother Header on line {0}", lineCount));
			}
			//These can be -1
			deathDateIndex = IndexOf("Death Date", fields);
			birthDateIndex = IndexOf("Birth Date", fields);
			//Figure out where we start attributes
			attributeStart = Max(deathDateIndex + 1, birthDateIndex + 1, attributeStart);
		}

		private void Reset() {
			lineCount = 0;
			deathDateIndex = -1;
			birthDateIndex = -1;
			fatherIndex = 3;
			motherIndex = 2;
			attributeStart = 4;
			sexIndex = 1;
		}

		/// <summary>
		/// Returns the max value a set of numbers
		/// </summary>
		/// <param name="values">The array to search</param>
		/// <returns>The max value</returns>
		private int Max(params int[] values) {
			int max = int.MinValue;
			for (int i = 0; i < values.Length; i++) {
				if (values[i] > max) {
					max = values[i];
				}
			}
			return max;
		}

		/// <summary>
		/// Returns where a needle is in a haystack
		/// </summary>
		/// <param name="needle">The needle to find</param>
		/// <param name="haystack">The haystack to search</param>
		/// <returns>the index or -1</returns>
		private static int IndexOf(string needle, string[] haystack) {
			for (int i = 0; i < haystack.Length; i++) {
				if (needle.Equals(haystack[i], StringComparison.InvariantCultureIgnoreCase))
					return i;
			}
			return -1;
		}

		/// <summary>
		/// Tests if the header is for attributes (Must be before the individual)
		/// </summary>
		/// <param name="line">The line to parse</param>
		/// <returns>If this is an attribute header</returns>
		private static bool IsAttributeHeader(string[] line) {
			return line[0].Equals("Attribute Code", StringComparison.CurrentCultureIgnoreCase);
		}

		/// <summary>
		/// Tests if the header is for individuals
		/// </summary>
		/// <param name="line">The line to parse</param>
		/// <returns>If this is an individual header</returns>
		private static bool IsIndividualHeader(string[] line) {
			return line[0].Equals("Individual Code", StringComparison.CurrentCultureIgnoreCase) && line.Length > 3;
		}

		private static bool IsFemale(string value) {
			if (string.IsNullOrEmpty(value))
				return true;
			if (value.Equals("y", StringComparison.CurrentCultureIgnoreCase)
				|| value.Equals("t", StringComparison.CurrentCultureIgnoreCase)
				|| value.Equals("true", StringComparison.CurrentCultureIgnoreCase)
				|| value.Equals("female", StringComparison.CurrentCultureIgnoreCase)
				|| value.Equals("fem", StringComparison.CurrentCultureIgnoreCase)
				|| value.Equals("f", StringComparison.CurrentCultureIgnoreCase)
				)

				return true;
			return false;
		}
	}
	*/
}
