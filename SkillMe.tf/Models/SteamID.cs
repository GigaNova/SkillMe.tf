using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillMe.tf.Models
{
    public class SteamId
    {
        public string NormalId { get; set; }
        public string Id64 { get; set; }
        public string Id3 { get; set; }

        public SteamId(string steamId64)
        {
            Id64 = steamId64;
            ConvertToOtherIds();
        }

        private void ConvertToOtherIds()
        {
            var steamId64 = long.Parse(Id64);
            long universe = (steamId64 >> 56) & 0xFF;

            if (universe == 1) universe = 0;

            long accountIdLowBit = steamId64 & 1;
            long accountIdHighBits = (steamId64 >> 1) & 0x7FFFFFF;

            NormalId = "STEAM_" + universe + ":" + accountIdLowBit + ":" + accountIdHighBits;
            Id3 = "[U:1:" + (accountIdHighBits * 2 + accountIdLowBit) + "]";
        }
    }
}
