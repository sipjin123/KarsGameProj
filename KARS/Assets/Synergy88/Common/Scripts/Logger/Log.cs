using System;

namespace Common.Logger {
	/**
	 * Represents one log. This class is immutable.
	 */
	public class Log {
		
		private LogLevel level;
		private string message;
		private DateTime timestamp;
		
		/**
		 * Constructor with specified level and message.
		 */
		public Log(LogLevel level, string message) {
			this.level = level;
			this.message = message;
			this.timestamp = DateTime.Now;
		}

		public LogLevel Level {
			get {
				return this.level;
			}
		}

		public string Message {
			get {
				return this.message;
			}
		}

		public DateTime Timestamp {
			get {
				return this.timestamp;
			}
		}
	}
}

