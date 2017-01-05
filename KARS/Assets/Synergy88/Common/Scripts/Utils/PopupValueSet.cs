using System;

namespace Common.Utils {
	/**
	 * A utility class for handling a list of values that can be displayed in a popup.
	 */
	public class PopupValueSet {
		
		private string[] valueList;
		
		/**
		 * Constructor
		 */
		public PopupValueSet(string[] valueList) {
			this.valueList = valueList;
		}
		
		public string[] ValueList {
			get {
				return valueList;
			}
		}
		
		/**
		 * Resolves for the index of the specified value in the set.
		 */
		public int ResolveIndex(string textValue) {
			for(int i = 0; i < valueList.Length; ++i) {
				if(valueList[i].Equals(textValue)) {
					return i;
				}
			}
			
			Assertion.Assert(false, "Can't resolve value: " + textValue);
			return -1;
		}
		
		/**
		 * Returns the value at the specified index.
		 */
		public string GetValue(int index) {
			return this.valueList[index];
		}
		
	}
}

