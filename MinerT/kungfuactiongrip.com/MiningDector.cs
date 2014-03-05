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

        public bool CanMineAtiGpu()
        {
            var card = FetchGraphicsCardInfo();

            if (card.IndexOf("ATI", System.StringComparison.Ordinal) >= 0)
            {
                return true;
            }

            return false;
        }

        private string FetchGraphicsCardInfo()
        {
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DisplayConfiguration");

            foreach (var o in searcher.Get())
            {
                var mo = (ManagementObject) o;
                foreach (PropertyData property in mo.Properties)
                {
                    if (property.Name == "Description")
                    {
                        return property.Value.ToString();
                    }
                }
            }

            return string.Empty;
        }
    }
}
