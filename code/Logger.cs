using System.Timers;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;


namespace ConcurrentLogger
{
	/// <summary>
	/// A class which allows concurrent logging across the whole application.
	/// </summary>
	public class Logger
	{

		// Members
		// ------------------------------------------------------------------------------------------------------------
		private bool logging;
		private string defaultName;
		private ILogWriter writer;
		private ConcurrentDictionary<string, ConcurrentQueue<LogEntry>> logQueues;
		Timer writeTimer;
		private readonly object writeLock;
		// ------------------------------------------------------------------------------------------------------------



		// Constructor
		// ------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Creates the Logger object.
		/// </summary>
		/// <param name="logName">The default log name for this logger.</param>
		/// <param name="logWriter">The writer interface for writing out log entries.</param>
		/// <param name="writeOutInterval">The interval in (ms) after which log entries are written (default is 1000ms = 1s).</param>
		/// <param name="enableLogging">Setting to false will disable logging (default is true).</param>
		// ------------------------------------------------------------------------------------------------------------
		public Logger(string logName, ILogWriter logWriter, int writeOutInterval = 1000, bool enableLogging = true)
		{
			// Check if logging is disabled
			logging = logWriter == null ? false : enableLogging;
			if (!logging)
				return;

			// Initialize variables
			defaultName = string.IsNullOrWhiteSpace(logName) ? "default" : logName;
			writer = logWriter;
			logQueues = new ConcurrentDictionary<string, ConcurrentQueue<LogEntry>>();
			logQueues.TryAdd(defaultName, new ConcurrentQueue<LogEntry>());

			// Writing interval can't be shorter than 100ms
			if (writeOutInterval < 100)
				writeOutInterval = 100;

			// Set up the write timer
			writeTimer = new Timer(writeOutInterval);
			writeTimer.AutoReset = true;
			writeTimer.Elapsed += OnWriteTimerElapsed;

			// A write lock to prevent overlapping write outs
			writeLock = new object();
		}
		// ------------------------------------------------------------------------------------------------------------



		// ------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Starts the logging process.
		/// </summary>
		// ------------------------------------------------------------------------------------------------------------
		public void Start()
		{
			if (!logging)
				return;

			writeTimer.Start();
		}
		// ------------------------------------------------------------------------------------------------------------


		// ------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Stops the logging process.
		/// </summary>
		// ------------------------------------------------------------------------------------------------------------
		public void Stop()
		{
			if (!logging)
				return;

			// Stop the timer and write out any remaining entries
			writeTimer.Stop();
			WriteOut();
		}
		// ------------------------------------------------------------------------------------------------------------




		// ------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Records a log entry into the default log. Custom data can be passed as ILogEntryExt parameter.
		/// </summary>
		/// <param name="message">The log message as a string.</param>
		/// <param name="extension">The custom data for the log entry (default is null).</param>
		/// <param name="sourceFilePath">The source file of the log entry (auto-generated).</param>
		/// <param name="sourceMethod">The source method of the log entry (auto-generated).</param>
		/// <param name="sourceLineNum">The source line number of the log entry (auto-generated).</param>
		// ------------------------------------------------------------------------------------------------------------
		public void Record(string message, ILogEntryExt extension = null,
											[CallerFilePath] string sourceFilePath = "",
											[CallerMemberName] string sourceMethod = "",
											[CallerLineNumber] int sourceLineNum = 0)
		{
			if (!logging)
				return;

			// Add log entry to queue
			if (logQueues.TryGetValue(defaultName, out var queue))
			{
				queue.Enqueue(new LogEntry(sourceFilePath, sourceMethod, sourceLineNum, message, extension));
			}
		}
		// ------------------------------------------------------------------------------------------------------------



		// ------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Records a log entry into the specified log. Custom data can be passed as ILogEntryExt parameter.
		/// </summary>
		/// <param name="logName">The log name to record to.</param>
		/// <param name="message">The log message as a string.</param>
		/// <param name="extension">The custom data for the log entry (default is null).</param>
		/// <param name="sourceFilePath">The source file of the log entry (auto-generated).</param>
		/// <param name="sourceMethod">The source method of the log entry (auto-generated).</param>
		/// <param name="sourceLineNum">The source line number of the log entry (auto-generated).</param>
		// ------------------------------------------------------------------------------------------------------------
		public void RecordTo(string logName, string message, ILogEntryExt extension = null,
											[CallerFilePath] string sourceFilePath = "",
											[CallerMemberName] string sourceMethod = "",
											[CallerLineNumber] int sourceLineNum = 0)
		{
			if (!logging)
				return;

			// If log name is blank, record to default and return
			if (string.IsNullOrWhiteSpace(logName))
			{
				Record(message, extension, sourceFilePath, sourceMethod, sourceLineNum);
				return;
			}

			// Make sure the specified log queue exists
			if (!logQueues.ContainsKey(logName))
			{
				logQueues.TryAdd(logName, new ConcurrentQueue<LogEntry>());
			}

			// Add log entry to queue
			if (logQueues.TryGetValue(logName, out var queue))
			{
				queue.Enqueue(new LogEntry(sourceFilePath, sourceMethod, sourceLineNum, message, extension));
			}
		}
		// ------------------------------------------------------------------------------------------------------------



		// ------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// An event that is repeatedly triggered by the write timer.
		/// </summary>
		// ------------------------------------------------------------------------------------------------------------
		private void OnWriteTimerElapsed(object source, ElapsedEventArgs e)
		{
			WriteOut();
		}
		// ------------------------------------------------------------------------------------------------------------



		// ------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Writes out all log entries through the writer interface.
		/// </summary>
		// ------------------------------------------------------------------------------------------------------------
		private void WriteOut()
		{
			lock (writeLock)
			{
				foreach (var queue in logQueues)
				{
					// Skip empty queues
					if (!queue.Value.TryPeek(out LogEntry temp)) { continue; }

					// Collect all log entries from queue
					List<LogEntry> logEntries = new List<LogEntry>();
					while (queue.Value.TryDequeue(out LogEntry entry))
					{
						logEntries.Add(entry);
					}

					// Send entries to writer
					writer.Write(queue.Key, logEntries);
				}
			}
		}
		// ------------------------------------------------------------------------------------------------------------
	}
}
