using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genealogy.Model {
	public class IndividualAttributesFactory {

		private readonly object syncLock;
		private ISet<IndividualAttribute> allAttributes;
		private Dictionary<string, IndividualAttribute> codeAttributes;

		/// <summary>
		/// All of the attributes in the system currently
		/// </summary>
		public IEnumerable<IndividualAttribute> AllAttributes { get { return allAttributes; } }


		/// <summary>
		/// Compiles the attributes that are for fixed values
		/// </summary>
		public IndividualAttributesFactory() {
			syncLock = new object();
			allAttributes = new HashSet<IndividualAttribute>();
			codeAttributes = new Dictionary<string, IndividualAttribute>();
		}

		/// <summary>
		/// Return the attributes that could "match" this attribute there is no particular order to the return of this query
		/// </summary>
		/// <param name="query">The attribute query to attempt to find</param>
		/// <returns>The list of attributes this could be</returns>
		public IEnumerable<IndividualAttribute> GetPossibleAttributes(string query) {
			var results = allAttributes.AsParallel()
				.Where(i => i.AttributeCode.Contains(query) || i.AttributeName.Contains(query));
			return results;
		}

		/// <summary>
		/// Attempts to get an attribute by the code, this is a faster search
		/// </summary>
		/// <param name="code">The short code to search</param>
		/// <returns>The </returns>
		public IndividualAttribute GetAttributes(string code) {
			if (codeAttributes.TryGetValue(code, out IndividualAttribute attr)) {
				return attr;
			}
			return null;
		}

		/// <summary>
		/// Attempts to create a new attribute with the given code and name. If the attribute code exists returns the existing
		/// </summary>
		/// <param name="code">The Code To add</param>
		/// <param name="name">The Attribute Description</param>
		/// <returns>The existing attribute or the new one</returns>
		public IndividualAttribute CreateAttribute(string code, string name) {
			lock (syncLock) {
				var duplicate = GetAttributes(code);
				if (duplicate != null) {
					return duplicate;
				}
				//If we found one with the exact same code return them
				IndividualAttribute newAttribute = new IndividualAttribute(code, name, Guid.NewGuid(), this);
				allAttributes.Add(newAttribute);
				codeAttributes.Add(code, newAttribute);
				return newAttribute;
			}
		}


	}
}
