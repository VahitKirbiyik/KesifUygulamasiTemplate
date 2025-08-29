using System;
using System.Diagnostics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Extensions.Logging;

namespace KesifUygulamasiTemplate.Helpers
{
    /// <summary>
    /// Global exception handler for centralized error management
    /// </summary>
    public class GlobalExceptionHandler : IGlobalExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles unhandled exceptions globally
        /// </summary>
        public void HandleException(Exception exception)
        {
            if (exception == null)
                return;

            try
            {
                // Log the exception
                _logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);

                // Send to App Center for crash reporting
                Crashes.TrackError(exception);

                // Debug output for development
                Debug.WriteLine($"[GlobalExceptionHandler] Exception: {exception.Message}");
                Debug.WriteLine($"[GlobalExceptionHandler] StackTrace: {exception.StackTrace}");

                // Handle specific exception types
                HandleSpecificException(exception);
            }
            catch (Exception handlerException)
            {
                // Prevent infinite loop if handler itself throws
                Debug.WriteLine($"[GlobalExceptionHandler] Handler exception: {handlerException.Message}");
                Crashes.TrackError(handlerException);
            }
        }

        /// <summary>
        /// Handles specific exception types with custom logic
        /// </summary>
        private void HandleSpecificException(Exception exception)
        {
            switch (exception)
            {
                case System.Net.Http.HttpRequestException httpEx:
                    _logger.LogWarning("Network request failed: {Message}", httpEx.Message);
                    break;

                case System.IO.IOException ioEx:
                    _logger.LogWarning("IO operation failed: {Message}", ioEx.Message);
                    break;

                case System.Threading.Tasks.TaskCanceledException taskEx:
                    _logger.LogInformation("Task was cancelled: {Message}", taskEx.Message);
                    break;

                case Microsoft.Data.Sqlite.SqliteException sqliteEx:
                    _logger.LogError("Database error: {Message}", sqliteEx.Message);
                    break;

                default:
                    _logger.LogError("Unexpected error type: {Type}", exception.GetType().Name);
                    break;
            }
        }

        /// <summary>
        /// Logs user-friendly error message
        /// </summary>
        public void LogUserFriendlyError(string userMessage, Exception? technicalException = null)
        {
            _logger.LogWarning("User-facing error: {UserMessage}", userMessage);

            if (technicalException != null)
            {
                _logger.LogError(technicalException, "Technical details for user error: {UserMessage}", userMessage);
            }
        }

        /// <summary>
        /// Reports non-critical errors for monitoring
        /// </summary>
        public void ReportNonCriticalError(string context, Exception exception)
        {
            _logger.LogWarning(exception, "Non-critical error in {Context}: {Message}", context, exception.Message);

            // Send to analytics but don't crash the app
            try
            {
                Crashes.TrackError(exception, new Dictionary<string, string>
                {
                    { "Context", context },
                    { "IsCritical", "false" }
                });
            }
            catch
            {
                // Ignore analytics errors
            }
        }
    }

    /// <summary>
    /// Interface for global exception handling
    /// </summary>
    public interface IGlobalExceptionHandler
    {
        void HandleException(Exception exception);
        void LogUserFriendlyError(string userMessage, Exception? technicalException = null);
        void ReportNonCriticalError(string context, Exception exception);
    }
}
