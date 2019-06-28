using System;
using System.IO;
using System.Text;
using System.Collections.Generic;


namespace ConcurrentLogger
{
	/// <summary>
	/// A log writer that writes to timestamped csv files.
	/// </summary>
	public class LogCSVFileWriter : ILogWriter
	{

		// Members
		// ------------------------------------------------------------------------------------------------------------
		private string path;
		private bool verbose;
		// ------------------------------------------------------------------------------------------------------------


		// Constructor
		// ------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Creates a log file writer that saves csv files at specified folder with specified extension.
		/// </summary>
		/// <param name="path">The target folder where the log files will be saved (default is ./).</param>
		/// <param name="verbose">Determines the verbosity of the log file (default is true).</param>
		// ------------------------------------------------------------------------------------------------------------
		public LogCSVFileWriter(string path = "./", bool verbose = true)
		{
			this.path = path;
			this.verbose = verbose;

			if (!Directory.Exists(path))
			{ Directory.CreateDirectory(path); }
		}
		// ------------------------------------------------------------------------------------------------------------



		// ------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Write a set of log entries to a specific log file.
		/// </summary>
		/// <param name="logName">The name of the log file to be written to.</param>
		/// <param name="logEntries">The collection of log entries.</param>
		// ------------------------------------------------------------------------------------------------------------
		public void Write(string logName, IEnumerable<LogEntry> logEntries)
		{
			// Build a csv string
			StringBuilder sb = new StringBuilder();
			foreach (LogEntry entry in logEntries)
			{
				// Append the timestamp and message fields to the string builder
				sb.Append("\"").Append(entry.Timestamp.ToLocalTime().ToString("MMM dd, yy - HH:mm:ss:fff")).Append("\"")
					.Append(",\"").Append(entry.Message).Append("\"");

				// Append extended data if available
				if (entry.Extension != null)
				{
					sb.Append(",").Append(entry.Extension.ToCSV());
				}

				// Append debug information if verboses
				if (verbose)
				{
					sb.Append(",").Append(entry.LineNumber)
						.Append(",\"").Append(entry.Method).Append("\"")
						.Append(",\"").Append(entry.Filename).Append("\"");
				}

				// Append a line break
				sb.Append("\n");
			}

			// Write to file
			try
			{
				File.AppendAllText(GenerateFileName(logName), sb.ToString(), Encoding.UTF8);
			}
			catch
			{
				// Exception
				throw;
			}
		}
		// ------------------------------------------------------------------------------------------------------------


		// ------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Generates the full filepath that also includes a timestamp in the name.
		/// </summary>
		/// <param name="filename">The base name to use for the log file.</param>
		/// <returns>The full filepath as a string.</returns>
		// ------------------------------------------------------------------------------------------------------------
		private string GenerateFileName(string filename)
		{
			string timestamp = DateTime.Now.ToString("yyyy'-'MM'-'dd");
			return (path + "/" + filename + "-" + timestamp + ".csv");
		}
		// ------------------------------------------------------------------------------------------------------------
	}
}
