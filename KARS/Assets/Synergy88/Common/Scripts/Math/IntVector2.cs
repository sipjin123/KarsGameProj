using System;
using UnityEngine;

namespace Common.Math {
	[Serializable]
	public class IntVector2 {
		
		public static readonly IntVector2 ZERO = new IntVector2(0, 0);
		
		[SerializeField]
		public int x;
		
		[SerializeField]
		public int y;
		
		/**
		 * Default constructor
		 */
		public IntVector2() : this(0, 0) {
		}
		
		/**
		 * Constructor with specified coordinates.
		 */
		public IntVector2(int x, int y) {
			this.x = x;
			this.y = y;
		}
		
		/**
		 * Copy constructor
		 */
		public IntVector2(IntVector2 other) : this(other.x, other.y) {
		}
		
		/**
		 * Sets the values of this vector using the specified one.
		 */
		public void Set(IntVector2 other) {
			this.x = other.x;
			this.y = other.y;
		}
		
		/**
		 * Returns whether or not this IntVector2 is equal to the specified one.
		 */
		public bool Equals(IntVector2 other) {
			if(other == null) {
				return false;
			}
			
			return this.x == other.x && this.y == other.y;
		}
		
		public override string ToString() {
			return "IntVector2 (" + x + ", " + y + ")";
		}
	}
}

