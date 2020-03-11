using System;
using System.Collections.Generic;

namespace BizportalService
{
    /// <summary>
    /// The type of events being filtered.
    /// </summary>
    public enum EventType
    {
        Any,
        Changed,
        Created
    };

    /// <summary>
    /// The FileSystemWatcher can raise multiple events for a change,
    /// for example a Notepad save raises 2 events. We want to avoid
    /// triggering multiple restarts of the process, so this class
    /// offers a way to filter events by timing proximity.
    /// </summary>
    public class EventFilter
    {
        private static readonly EventFilter Instance = new EventFilter();

        private readonly Dictionary<EventType, int> _tickCounts = new Dictionary<EventType, int>()
        {
            {EventType.Any, 0},
            {EventType.Changed, 0},
            {EventType.Created, 0}
        };

        /// <summary>
        /// Determines whether this event can be accepted, or whether it
        /// should be ignored.
        /// </summary>
        /// <param name="type">The type of event.</param>
        public bool CanAcceptEvent(EventType type)
        {
            var tickCount = Environment.TickCount;
            var accepted = tickCount >= _tickCounts[type] + 1000;
            
            _tickCounts[EventType.Any] = tickCount;

            if (accepted) 
            {
                Log.WriteLine($"* {type} accepted");
                _tickCounts[type] = tickCount;
            }
            else
            {
                Log.WriteLine($"* {type} ignored");
            }

            return accepted;
        }

        public static bool CanAccept(EventType type) => Instance.CanAcceptEvent(type);
    }
}
