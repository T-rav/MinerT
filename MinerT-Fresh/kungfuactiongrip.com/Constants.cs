using MinerT.kungfuactiongrip.com;
namespace MinerT
{
    public enum MiningType
    {
        Doge,
        Cosmos,
        Profitable
    }

    public class Constants
    {
        public static bool IsMining = false;

        public static bool HadFaultDoingBoth = false;

        public static bool IsGpuMode = false;

        public static bool IsCpuMode = false;

        public static bool ScheduleMissingUser = false;

        public static MiningType MiningMode = MiningType.Profitable;

        public static string UpdateUrl = "http://www.kungfuactiongrip.com/minerT/updates/update.xml";

        public static MiningUserConfig MiningUser;

        public static string BugFromEmail = "bugs@kungfuactiongrip.com";

        public static string BugToEmail = "tmfrisinger@gmail.com";

        public static string BugSubjectEmail = "MinerT Bug Report";

        public static string BtcWallet = "1CEbNtuxjk3SWpRmMZ16ZE3dS5G74MwvLD";

        // UpdateBootstraper
        public static string BootstrapExe = "UpdateBootstrapper.exe";

        public static string ScheduleFile = "schedule.json";
    }
}
