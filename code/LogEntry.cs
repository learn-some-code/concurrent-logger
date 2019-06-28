using System;
using System.Text;


namespace ConcurrentLogger
{
	/// <summary>
	/// A log entry structure.
	/// </summary>
	public struct LogEntry
	{
		// Members
		// ------------------------------------------------------------------------------------------------------------
		/// <summary>The timestamp information for the log entry.</summary>
		public DateTime Timestamp;
		/// <summary>The source file of the log entry.</summary>
		public string Filename;
		/// <summary>The source method of the log entry.</summary>
		public string Method;
		/// <summary>The source line number of the log entry.</summary>
		public int LineNumber;
		/// <summary>Basic log entry messsage.</summary>
		public string Message;
		/// <summary>Extra custom data for the log entry.</summary>
		public ILogEntryExt Extension;
		// ------------------------------------------------------------------------------------------------------------


		// Constructor
		// ------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Creates a new log entry object.
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="method"></param>
		/// <param name="lineNumber"></param>
		/// <param name="message"></param>
		/// <param name="extension"></param>
		// ------------------------------------------------------------------------------------------------------------
		public LogEntry(string filename, string method, int lineNumber, string message, ILogEntryExt extension = null)
		{
			Timestamp = DateTime.UtcNow;
			Filename = filename;
			Method = method;
			LineNumber = lineNumber;
			Message = message;
			Extension = extension;
		}
		// ------------------------------------------------------------------------------------------------------------



		// ------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Serializes the log entry into a coma-separated values string.
		/// </summary>
		/// <returns>A string of coma-separated values.</returns>
		// ------------------------------------------------------------------------------------------------------------
		public string ToCSV()
		{
			// Append the member fields to the string builder
			var sb = new StringBuilder()
					.Append("\"").Append(Timestamp.ToLocalTime().ToString("MMM dd, yy - HH:mm:ss:fff")).Append("\"")
					.Append(",").Append(LineNumber)
					.Append(",\"").Append(Method).Append("\"")
					.Append(",\"").Append(Filename).Append("\"")
					.Append(",\"").Append(Message).Append("\"");

			return sb.ToString();
		}
		// ------------------------------------------------------------------------------------------------------------
	}
}
