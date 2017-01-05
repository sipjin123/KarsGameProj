//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
namespace Common.Query {
	/**
	 * An interface for a query request
	 */
	public interface IQueryRequest {

		/**
		 * Adds a parameter
		 */
		void AddParameter(string paramId, object value);

        /**
         * Returns true if the query request has a parameter of the given ID
         */
        bool HasParameter(string paramId);

		/**
		 * Retrieves a parameter value
		 * This used to be a templated method but it won't work on iOS (JIT-AOT thing)
		 */
		object GetParameter(string paramId);

	}
}

