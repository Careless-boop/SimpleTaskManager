namespace SimpleTaskManager.WebApi.Extensions
{
    public static class LoggingBuilderExtensions
    {
        public static void AddLogging(this ILoggingBuilder builder)
        {
            builder.ClearProviders();
            builder.AddConsole(options =>
            {
                options.LogToStandardErrorThreshold = LogLevel.Information;
            });
        }
    }
}
