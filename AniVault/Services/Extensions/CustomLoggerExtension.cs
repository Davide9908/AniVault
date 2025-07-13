using JetBrains.Annotations;

namespace AniVault.Services.Extensions{


    /// <summary>
    /// ILogger custom extension methods.
    /// </summary>
    public static class CustomLoggerExtension
    {

        //------------------------------------------DEBUG------------------------------------------//

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <example>
        /// <code language="csharp">
        /// logger.LogDebug(0, exception, "Error while processing request from {Address}", address)
        /// </code>
        /// </example>
        public static void Debug(this ILogger logger, EventId eventId, Exception? exception, [StructuredMessageTemplate]string? message, params object?[] args)
        {
            logger.Log(LogLevel.Debug, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <example>
        /// <code language="csharp">
        /// logger.LogDebug(0, "Processing request from {Address}", address)
        /// </code>
        /// </example>
        public static void Debug(this ILogger logger, EventId eventId, [StructuredMessageTemplate]string? message, params object?[] args)
        {
            logger.Log(LogLevel.Debug, eventId, message, args);
        }

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <example>
        /// <code language="csharp">
        /// logger.LogDebug(exception, "Error while processing request from {Address}", address)
        /// </code>
        /// </example>
        public static void Debug(this ILogger logger, Exception? exception, [StructuredMessageTemplate]string? message, params object?[] args)
        {
            logger.Log(LogLevel.Debug, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
        /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <example>
        /// <code language="csharp">
        /// logger.LogDebug("Processing request from {Address}", address)
        /// </code>
        /// </example>
        public static void Debug(this ILogger logger, [StructuredMessageTemplate]string? message, params object?[] args)
        {
            logger.Log(LogLevel.Debug, message, args);
        }

        //------------------------------------------TRACE------------------------------------------//

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <example>
        /// <code language="csharp">
        /// logger.LogTrace(0, exception, "Error while processing request from {Address}", address)
        /// </code>
        /// </example>
        public static void Trace(this ILogger logger, EventId eventId, Exception? exception, [StructuredMessageTemplate]string? message, params object?[] args)
        {
            logger.Log(LogLevel.Trace, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <example>
        /// <code language="csharp">
        /// logger.LogTrace(0, "Processing request from {Address}", address)
        /// </code>
        /// </example>
        public static void Trace(this ILogger logger, EventId eventId, [StructuredMessageTemplate]string? message, params object?[] args)
        {
            logger.Log(LogLevel.Trace, eventId, message, args);
        }

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <example>
        /// <code language="csharp">
        /// logger.LogTrace(exception, "Error while processing request from {Address}", address)
        /// </code>
        /// </example>
        public static void Trace(this ILogger logger, Exception? exception, [StructuredMessageTemplate]string? message, params object?[] args)
        {
            logger.Log(LogLevel.Trace, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
        /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <example>
        /// <code language="csharp">
        /// logger.LogTrace("Processing request from {Address}", address)
        /// </code>
        /// </example>
        public static void Trace(this ILogger logger, [StructuredMessageTemplate]string? message, params object?[] args)
        {
            logger.Log(LogLevel.Trace, message, args);
        }

        //------------------------------------------INFORMATION------------------------------------------//

        /// <summary>
        /// Formats and writes an informational log message.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <example>
        /// <code language="csharp">
        /// logger.LogInformation(0, exception, "Error while processing request from {Address}", address)
        /// </code>
        /// </example>
        public static void Info(this ILogger logger, EventId eventId, Exception? exception, [StructuredMessageTemplate]string? message, params object?[] args)
        {
            logger.Log(LogLevel.Information, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes an informational log message.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <example>
        /// <code language="csharp">
        /// logger.LogInformation(0, "Processing request from {Address}", address)
        /// </code>
        /// </example>
        public static void Info(this ILogger logger, EventId eventId, [StructuredMessageTemplate]string? message, params object?[] args)
        {
            logger.Log(LogLevel.Information, eventId, message, args);
        }

        /// <summary>
        /// Formats and writes an informational log message.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <example>
        /// <code language="csharp">
        /// logger.LogInformation(exception, "Error while processing request from {Address}", address)
        /// </code>
        /// </example>
        public static void Info(this ILogger logger, Exception? exception, [StructuredMessageTemplate]string? message, params object?[] args)
        {
            logger.Log(LogLevel.Information, exception, message, args);
        }

        /// <summary>
        /// Formats and writes an informational log message.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
        /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <example>
        /// <code language="csharp">
        /// logger.LogInformation("Processing request from {Address}", address)
        /// </code>
        /// </example>
        public static void Info(this ILogger logger, [StructuredMessageTemplate]string? message, params object?[] args)
        {
            logger.Log(LogLevel.Information, message, args);
        }

        //------------------------------------------WARNING------------------------------------------//

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <example>
        /// <code language="csharp">
        /// logger.LogWarning(0, exception, "Error while processing request from {Address}", address)
        /// </code>
        /// </example>
        public static void Warning(this ILogger logger, EventId eventId, Exception? exception, [StructuredMessageTemplate]string? message, params object?[] args)
        {
            logger.Log(LogLevel.Warning, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <example>
        /// <code language="csharp">
        /// logger.LogWarning(0, "Processing request from {Address}", address)
        /// </code>
        /// </example>
        public static void Warning(this ILogger logger, EventId eventId, [StructuredMessageTemplate]string? message, params object?[] args)
        {
            logger.Log(LogLevel.Warning, eventId, message, args);
        }

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <example>
        /// <code language="csharp">
        /// logger.LogWarning(exception, "Error while processing request from {Address}", address)
        /// </code>
        /// </example>
        public static void Warning(this ILogger logger, Exception? exception, [StructuredMessageTemplate]string? message, params object?[] args)
        {
            logger.Log(LogLevel.Warning, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
        /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <example>
        /// <code language="csharp">
        /// logger.LogWarning("Processing request from {Address}", address)
        /// </code>
        /// </example>
        public static void Warning(this ILogger logger, [StructuredMessageTemplate]string? message, params object?[] args)
        {
            logger.Log(LogLevel.Warning, message, args);
        }

        //------------------------------------------ERROR------------------------------------------//

        /// <summary>
        /// Formats and writes an error log message.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <example>
        /// <code language="csharp">
        /// logger.LogError(0, exception, "Error while processing request from {Address}", address)
        /// </code>
        /// </example>
        public static void Error(this ILogger logger, EventId eventId, Exception? exception, [StructuredMessageTemplate]string? message, params object?[] args)
        {
            logger.Log(LogLevel.Error, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes an error log message.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <example>
        /// <code language="csharp">
        /// logger.LogError(0, "Processing request from {Address}", address)
        /// </code>
        /// </example>
        public static void Error(this ILogger logger, EventId eventId, [StructuredMessageTemplate]string? message, params object?[] args)
        {
            logger.Log(LogLevel.Error, eventId, message, args);
        }

        /// <summary>
        /// Formats and writes an error log message.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <example>
        /// <code language="csharp">
        /// logger.LogError(exception, "Error while processing request from {Address}", address)
        /// </code>
        /// </example>
        public static void Error(this ILogger logger, Exception? exception, [StructuredMessageTemplate]string? message, params object?[] args)
        {
            logger.Log(LogLevel.Error, exception, message, args);
        }

        /// <summary>
        /// Formats and writes an error log message.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
        /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <example>
        /// <code language="csharp">
        /// logger.LogError("Processing request from {Address}", address)
        /// </code>
        /// </example>
        public static void Error(this ILogger logger, [StructuredMessageTemplate]string? message, params object?[] args)
        {
            logger.Log(LogLevel.Error, message, args);
        }

        //------------------------------------------CRITICAL------------------------------------------//

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <example>
        /// <code language="csharp">
        /// logger.LogCritical(0, exception, "Error while processing request from {Address}", address)
        /// </code>
        /// </example>
        public static void Critical(this ILogger logger, EventId eventId, Exception? exception, [StructuredMessageTemplate]string? message, params object?[] args)
        {
            logger.Log(LogLevel.Critical, eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <example>
        /// <code language="csharp">
        /// logger.LogCritical(0, "Processing request from {Address}", address)
        /// </code>
        /// </example>
        public static void Critical(this ILogger logger, EventId eventId, [StructuredMessageTemplate]string? message, params object?[] args)
        {
            logger.Log(LogLevel.Critical, eventId, message, args);
        }

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <example>
        /// <code language="csharp">
        /// logger.LogCritical(exception, "Error while processing request from {Address}", address)
        /// </code>
        /// </example>
        public static void Critical(this ILogger logger, Exception? exception, [StructuredMessageTemplate]string? message, params object?[] args)
        {
            logger.Log(LogLevel.Critical, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to write to.</param>
        /// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <example>
        /// <code language="csharp">
        /// logger.LogCritical("Processing request from {Address}", address)
        /// </code>
        /// </example>
        public static void Critical(this ILogger logger, [StructuredMessageTemplate]string? message, params object?[] args)
        {
            logger.Log(LogLevel.Critical, message, args);
        }

        
    }
}
