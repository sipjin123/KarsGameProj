using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Common.Xml;

namespace Common {
	/**
	 * Accepts an XML file that contains the variables
	 */
	public class XmlVariables : MonoBehaviour {

		[SerializeField]
		private TextAsset xmlFile;

		private Dictionary<string, string> varMap;

		void Awake() {
			Assertion.AssertNotNull(this.xmlFile);

			this.varMap = new Dictionary<string, string>();
			Parse();
		}

		private const string ENTRY = "Entry";

		// attributes
		private const string KEY = "key";
		private const string VALUE = "value";

		private void Parse() {
			SimpleXmlReader reader = new SimpleXmlReader();
			SimpleXmlNode root = reader.Read(xmlFile.text).FindFirstNodeInChildren("Variables");

			for(int i = 0; i < root.Children.Count; ++i) {
				SimpleXmlNode child = root.Children[i];
				if(ENTRY.Equals(child.TagName)) {
					string key = child.GetAttribute(KEY);
					string value = child.GetAttribute(VALUE);
					this.varMap[key] = value;
				}
			}
		}

		/**
		 * Retrieves the variable with the specified key
		 */
		public string Get(string key) {
			string value = null;
			Assertion.Assert(this.varMap.TryGetValue(key, out value));
			return value;
		}

		private const string TRUE = "true";
		private const string FALSE = "false";

		/**
		 * Retrieves the variable with the specified key as boolean
		 */
		public bool GetAsBool(string key) {
			return ToBool(Get(key));
		}

		/**
		 * Converts the specified string to bool (custom logic)
		 */
		public static bool ToBool(string value) {
			switch(value) {
			case TRUE:
				return true;
				
			case FALSE:
				return false;
			}
			
			Assertion.Assert(false, "Can't convert the value to a bool: " + value);
			return false;
		}

		/**
		 * Retrieves the variable with the specified key as integer
		 */
		public int GetAsInt(string key) {
			return int.Parse(key);
		}

		/**
		 * Retrieves the variable with the specified key as float
		 */
		public float GetAsFloat(string key) {
			return float.Parse(key);
		}

	}
}
