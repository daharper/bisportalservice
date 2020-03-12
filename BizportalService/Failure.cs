using System;

namespace BizportalService
{
    /// <summary>
    /// Central place for raising and managing exceptions.
    /// </summary>
    public static class Failure
    {
        /// <summary>
        /// Register to receive exception notifications.
        /// </summary>
        public static Action<Exception, string> OnException { get; set; }

        /// <summary>
        /// Raise the specified condition.
        /// </summary>
        public static void Raise<T>(params object[] args) where T:Exception 
        {
            var e = (T)Activator.CreateInstance(typeof(T), args);
            Raise(e);
        }

        /// <summary>
        /// If the specified condition is false, raise the condition.
        /// </summary>
        public static void RaiseIf<T>(bool condition, params object[] args) where T : Exception
        {
            if (condition)
            {
                Raise<T>(args);
            }
        }

        /// <summary>
        /// If the specified condition is false, raise the condition.
        /// </summary>
        public static void RaiseIf(bool condition, Exception e, string message = "")
        {
            if (condition)
            {
                Raise(e, message);
            }
        }

        /// <summary>
        /// Raise the specified error.
        /// </summary>
        public static void Raise(Exception e, string message = "")
        {
            OnException?.Invoke(e, message);
            throw e;
        }
    }
}
