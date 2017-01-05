using System;
using System.Collections.Generic;

namespace Common.Utils {
	/**
	 * An abstraction of a list that is immutable. Commonly used for data only instances.
	 */
	public class ImmutableList<T> {
		
		private List<T> list;
		
		/**
		 * Constructor with specified elements.
		 */
		public ImmutableList(IEnumerable<T> elements) {
			list = new List<T>();
			
			// copy items
			foreach(T element in elements) {
				list.Add(element);
			}
		}
		
		/**
		 * Returns the number of elements.
		 */
		public int Count {
			get {
				return list.Count;
			}
		}
		
		/**
		 * Returns the element at the specified index.
		 */
		public T GetAt(int index) {
			return list[index];
		}
	}
}

