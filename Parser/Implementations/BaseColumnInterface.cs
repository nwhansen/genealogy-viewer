using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genealogy.Model;

namespace Genealogy.Implementations {
	/// <summary>
	/// Provides a base processing for column based file interfaces
	/// </summary>
	public abstract class BaseColumnInterface : IGenealogyFileInterface {

		protected class ParserHelper : IDisposable, IEnumerable<string[]> {
			private readonly IDisposable wrapped;
			private readonly IEnumerator<string[]> lineEnumerator;

			public ParserHelper(IDisposable disposable, IEnumerator<string[]> lineEnumerator) {
				wrapped = disposable;
				this.lineEnumerator = lineEnumerator;
			}

			public void Dispose() {
				wrapped?.Dispose();
			}

			public IEnumerator<string[]> GetEnumerator() {
				return lineEnumerator;
			}

			IEnumerator IEnumerable.GetEnumerator() {
				return GetEnumerator();
			}
		}
		private enum ParsingState { Unknown, Attributes, Individuals };

		private ParsingState parseState;

		protected struct IndividualParseState {
			public static readonly IndividualParseState Default = new IndividualParseState(3, 2, -1, -1, 1, 4);

			public int DeathDateIndex { get; }
			public int BirthDateIndex { get; }
			public int FatherIndex { get; }
			public int MotherIndex { get; }
			public int AttributeStart { get; }
			public int SexIndex { get; }

			public IndividualParseState(int father, int mother, int death, int birth, int sex, int attribute) {
				FatherIndex = father;
				MotherIndex = mother;
				DeathDateIndex = death;
				BirthDateIndex = birth;
				SexIndex = sex;
				AttributeStart = attribute;
			}
		}

		protected struct AttributeParseState {

			public static readonly AttributeParseState Default = new AttributeParseState(-1);
			public int DefaultIndex { get; }

			public AttributeParseState(int defaultIndex) {
				DefaultIndex = defaultIndex;
			}
		}

		protected IndividualParseState IndividualState { get; private set; }
		protected AttributeParseState AttributeState { get; private set; }

		protected int LineCount { get; private set; }

		public string Position {
			get {
				return String.Format("Line {0}", LineCount);
			}
		}

		public IndividualManager ParseFile(string filename) {
			IndividualManager manager = new IndividualManager();
			Reset();
			using (var wrapper = LoadFile(filename)) {
				foreach (var fields in wrapper) {
					LineCount++;
					if (fields == null || fields.Length == 0 || fields.All(i => string.IsNullOrWhiteSpace(i)))
						continue;
					//Determine our state
					switch (parseState) {
						case ParsingState.Unknown when IsAttributeHeader(fields):
							//Start Attribute parsing
							ParseAttributeHeaderLine(fields);
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
							ParseAttributeLine(fields, manager);
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
		/// Reset state information
		/// </summary>
		private void Reset() {
			LineCount = 0;
			AttributeState = AttributeParseState.Default;
			IndividualState = IndividualParseState.Default;
		}

		/// <summary>
		/// Provides a place for resources to be wrapped properly in the parsing logic
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		protected abstract ParserHelper LoadFile(string filename);
		/// <summary>
		/// Processes the attribute header line for key header lines
		/// </summary>
		/// <param name="fields"></param>
		private void ParseAttributeHeaderLine(string[] fields) {
			int defaultIndex = IndexOf("Highlight", fields);
			AttributeState = new AttributeParseState(defaultIndex);
		}

		/// <summary>
		/// Reads the attribute field lines
		/// </summary>
		/// <param name="fields">The columns to read</param>
		private void ParseAttributeLine(string[] fields, IndividualManager individualManager) {
			var attributesFactory = individualManager.AttributesFactory;
			var attribute = attributesFactory.CreateAttribute(fields[0], fields[1]);
			if (fields.Length > AttributeState.DefaultIndex && IsTrue(fields[AttributeState.DefaultIndex])) {
				individualManager.AddDefaultHighlighted(attribute);
			}
		}

		/// <summary>
		/// Reads an individual fields line
		/// </summary>
		/// <param name="manager">The manager to put the individuals into</param>
		/// <param name="fields">The fields to load</param>
		private void ParseIndividualLine(IndividualManager manager, string[] fields) {
			//We need at least as many columns as our attribute start (fail more predictably)
			if (fields.Length < IndividualState.AttributeStart)
				Array.Resize(ref fields, IndividualState.AttributeStart);
			string id = fields[0];
			if (string.IsNullOrWhiteSpace(id)) {
				throw new Exception(string.Format("Individual ID is missing at line: {0}", LineCount));
			}

			bool female = IsFemale(fields[IndividualState.SexIndex]);
			string motherCode = fields[IndividualState.MotherIndex];
			string fatherCode = fields[IndividualState.FatherIndex];

			Individual individual = manager.CreateIndividual(id, female, fatherCode, motherCode, false);
			if (IndividualState.DeathDateIndex > 0) {
				if (DateTime.TryParse(fields[IndividualState.DeathDateIndex], out DateTime death)) {
					individual.DeathDate = death;
				}
			}
			if (IndividualState.BirthDateIndex > 0) {
				if (DateTime.TryParse(fields[IndividualState.BirthDateIndex], out DateTime birth)) {
					individual.BirthDate = birth;
				}
			}
			for (int i = IndividualState.AttributeStart; i < fields.Length; i++) {
				if (string.IsNullOrEmpty(fields[i]))
					continue;
				individual.AddAttribute(manager.AttributesFactory.CreateAttribute(fields[i], fields[i]));
			}
		}

		private void ParseIndividualHeaderLine(string[] fields) {
			int fatherIndex = IndexOf("Father", fields);
			if (fatherIndex == -1) {
				throw new Exception(string.Format("Missing Father Header on line {0}", LineCount));
			}
			int motherIndex = IndexOf("Mother", fields);
			if (motherIndex == -1) {
				throw new Exception(string.Format("Missing Mother Header on line {0}", LineCount));
			}
			//These can be -1
			int deathDateIndex = IndexOf("Death Date", fields);
			int birthDateIndex = IndexOf("Birth Date", fields);
			//Figure out where we start attributes
			int attributeStart = Max(IndividualParseState.Default.AttributeStart,
								deathDateIndex + 1,
								birthDateIndex + 1,
								fatherIndex + 1,
								motherIndex + 1);
			IndividualState = new IndividualParseState(fatherIndex, motherIndex, deathDateIndex, birthDateIndex, 1, attributeStart);
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
			needle = needle.Trim();
			for (int i = 0; i < haystack.Length; i++) {
				if (needle.Equals(haystack[i]?.Trim(), StringComparison.InvariantCultureIgnoreCase))
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
		/// <summary>
		/// Returns if the value is considered "female"
		/// </summary>
		/// <param name="value">value under test</param>
		/// <returns>If the value is female</returns>
		public static bool IsFemale(string value) {
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

		public static bool IsTrue(string value) {
			if (string.IsNullOrEmpty(value))
				return false;
			if (value.Equals("y", StringComparison.CurrentCultureIgnoreCase)
				|| value.Equals("yes", StringComparison.CurrentCultureIgnoreCase)
				|| value.Equals("true", StringComparison.CurrentCultureIgnoreCase)
				|| value.Equals("t", StringComparison.CurrentCultureIgnoreCase)
				)
				return true;
			return false;
		}

	}
}
