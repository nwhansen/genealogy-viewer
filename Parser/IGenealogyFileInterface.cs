using Genealogy.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genealogy {
	/// <summary>
	/// Represents an interface to a file for parsing
	/// </summary>
	public interface IGenealogyFileInterface {

		/// <summary>
		/// Parses the file and returns the population
		/// </summary>
		/// <param name="filename">The filename to parse</param>
		/// <returns>The population at that given file</returns>
		IndividualManager ParseFile(string filename);

		/// <summary>
		/// Where in the file we have errored
		/// </summary>
		string Position { get; }

	}
}
