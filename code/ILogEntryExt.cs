

namespace ConcurrentLogger
{
	/// <summary>
	/// An interface for a log entry extension.
	/// </summary>
	public interface ILogEntryExt
	{
		// ------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Serializes the extension members into a coma-separated values string.
		/// </summary>
		/// <returns>A string of coma-separated values.</returns>
		// ------------------------------------------------------------------------------------------------------------
		string ToCSV();
		// ------------------------------------------------------------------------------------------------------------
	}
}
