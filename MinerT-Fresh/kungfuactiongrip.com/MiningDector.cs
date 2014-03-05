using System.IO;
using System.Management;

namespace MinerT.kungfuactiongrip.com
{
    public class MiningDector
    {
        public bool CanMineCpu()
        {
            return true;
        }

        public bool CanMineNividaGpu()
        {
            var card = FetchGraphicsCardInfo();

            if (card.IndexOf("NVIDIA", System.StringComparison.Ordinal) >= 0)
            {
                return true;
            }

            return false;
        }

        public bool CanMineAtiGpu(out string error)
        {
            var card = FetchGraphicsCardInfo();

            const string openClPath = "$WINDIR\\system32\\OpenCL.dll";

            if (card.IndexOf("ATI", System.StringComparison.Ordinal) >= 0 || File.Exists(openClPath))
            {
                error = string.Empty;
                return true;
            }

            error = "GRAPHICS CARD DETECTED AS : "+ card;

            return false;
        }

        public string FetchGraphicsCardInfo()
        {
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DisplayConfiguration");

            foreach (var o in searcher.Get())
            {
                var mo = (ManagementObject) o;
                foreach (PropertyData property in mo.Properties)
                {
                    if (property.Name == "Description")
                    {
                        return property.Value.ToString().ToUpper();
                    }
                }
            }

            return string.Empty;
        }

        public static bool CanMineCoinType(MiningType typeOf, string user)
        {
            if (typeOf == MiningType.Cosmos && user == "MrTtheMiner")
            {
                return true;
            }

            return false;
        }
    }
}
