# ConcurrentLogger

A small efficient library for concurrent logging in C#.\
Includes a CSV file writer. Can be extended to write to a database, TCP stream, text file, etc.

## Example

```csharp
	Logger log = new Logger("mainlog", new LogCSVFileWriter());
	log.Start();
	// ...
	log.Record("a new log entry");			// To default
	log.Record("another log entry");		// To default
	log.RecordTo("errorlog", "found an error");	// To errorlog
	// ...
	log.Stop();
```

### CSV output

| mainlog-2019-06-28.csv    |                   |     |      |                       |
| ------------------------- | ----------------- | --- | ---- | --------------------- |
| Jun 28, 19 - 11:14:07:157 | a new log entry   | 13  | Main | C:\YourApp\Program.cs |
| Jun 28, 19 - 11:14:07:158 | another log entry | 14  | Main | C:\YourApp\Program.cs |

| errorlog-2019-06-28.csv   |                |     |      |                       |
| ------------------------- | -------------- | --- | ---- | --------------------- |
| Jun 28, 19 - 11:14:07:158 | found an error | 15  | Main | C:\YourApp\Program.cs |

## Customization

By default a log entry contains the following information:
- Timestamp
- Text message
- Source line number
- Calling method
- Source file path

You can add extra information by writing extension structs or classes that inherit from ILogEntryExt.\
For example, if you want to include a message type, you can do this:

```csharp
	public enum LogType
	{ General, Error, Exception };

	public struct LogEntryType : ILogEntryExt
	{
		public LogType Type;
		
		public LogEntryType(LogType type) { Type = type; }
		public string ToCSV() {	return (Type.ToString()); }
	}
```

```csharp
	log.Record("connection established", new LogEntryType(LogType.General));
	log.Record("access denied", new LogEntryType(LogType.Error));
```

### CSV output

| mainlog-2019-06-28.csv    |                        |         |     |      |                       |
| ------------------------- | -----------------------|---------| --- | ---- | --------------------- |
| Jun 28, 19 - 11:40:02:530 | connection established | General | 17  | Main | C:\YourApp\Program.cs |
| Jun 28, 19 - 11:40:02:530 | access denied          | Error   | 18  | Main | C:\YourApp\Program.cs |
