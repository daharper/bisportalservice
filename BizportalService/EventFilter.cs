using System;
using System.Collections.Generic;

namespace BizportalService
{
    public enum EventType
    {
        Any,
        Changed,
        Created
    };

    public class EventFilter
    {
        private static readonly EventFilter Instance = new EventFilter();

        private readonly Dictionary<EventType, int> _tickCounts = new Dictionary<EventType, int>()
        {
            {EventType.Any, 0},
            {EventType.Changed, 0},
            {EventType.Created, 0}
        };

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
