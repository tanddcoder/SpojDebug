
namespace SpojDebug.Core
{
    public static class JobLocker
    {
        public static bool IsDownloadSpojInfoInProcess { get; set; } = false;
        public static bool IsDownloadTestCasesInProcess { get; set; } = false;
        public static bool IsDownloadSubmissionInfoInProcess { get; set; } = false;
    }
}
