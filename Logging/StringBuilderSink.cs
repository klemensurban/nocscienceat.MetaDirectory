using System.IO;
using System.Text;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Display;

namespace nocscienceat.MetaDirectory.Logging
{
    /// <summary>
    /// Serilog sink that captures formatted log events in a shared <see cref="StringBuilder"/>.
    /// </summary>
    /// <remarks>
    /// Synchronization is handled via a simple lock to keep appends thread-safe.
    /// </remarks>
    public class StringBuilderSink : ILogEventSink
    {
        private readonly StringBuilder _buffer;
        private readonly MessageTemplateTextFormatter _formatter;
        private readonly object _lock = new();

        public StringBuilderSink(StringBuilder buffer, string? outputTemplate = null)
        {
            _buffer = buffer;
            _formatter = new MessageTemplateTextFormatter(
                outputTemplate ?? "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                null);
        }

        public void Emit(LogEvent logEvent)
        {
            using StringWriter sw = new();
            _formatter.Format(logEvent, sw);
            string text = sw.ToString();

            lock (_lock)
            {
                _buffer.Append(text);
            }
        }
    }
}
