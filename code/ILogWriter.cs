using System.Collections.Generic;


namespace ConcurrentLogger
{
	/// <summary>
	/// An interface for writing log entries.
	/// </summary>
	public interface ILogWriter
	{
		// ------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Writes a collection of log entries for a specified log.
		/// </summary>
		/// <param name="logName">The name of the log.</param>
		/// <param name="logEntries">A collection of log entries.</param>
		// ------------------------------------------------------------------------------------------------------------
		void Write(string logName, IEnumerable<LogEntry> logEntries);
		// ------------------------------------------------------------------------------------------------------------
	}
}
