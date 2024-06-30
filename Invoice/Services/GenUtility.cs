namespace Invoice.Services
{
    public class GenUtility
    {
        public void LogError(Exception ex)
        {
            try
            {
                var logFilePath = Path.Combine("Logs", "error.txt");

                // Ensure the Logs directory exists
                var logDir = Path.GetDirectoryName(logFilePath);
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }

                // Write the error message and stack trace to the log file
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"{DateTime.Now}: {ex.Message}");
                    writer.WriteLine(ex.StackTrace);
                }
            }
            catch
            {
                // If logging fails, there's not much we can do, so we swallow the exception
            }
        }
    }
}
