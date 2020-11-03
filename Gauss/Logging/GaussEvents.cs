using Microsoft.Extensions.Logging;

namespace Gauss {
	public static class LogEvent {
		public static EventId General { get; } = new EventId(10000, "Gauss");
		public static EventId Module { get; } = new EventId(10001, "Module");
		public static EventId Command { get; } = new EventId(10002, "Command");
		public static EventId UpdateMessage { get; } = new EventId(10003, "Message update");
		public static EventId Folding { get; } = new EventId(10004, "Folding");
	}
}