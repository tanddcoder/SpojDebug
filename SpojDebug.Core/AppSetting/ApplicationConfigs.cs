namespace SpojDebug.Core.AppSetting
{
    public static class ApplicationConfigs
    {
        public static SpojKey SpojKey { get; set; } = new SpojKey();
        //public static SpojInfo SpojInfo { get; set; } = new SpojInfo();
        public static SystemInfo SystemInfo { get; set; } = new SystemInfo();
        public static ConnectionStrings ConnectionStrings { get; set; } = new ConnectionStrings();
    }
}
