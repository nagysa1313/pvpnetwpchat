
using System.Collections.Generic;
namespace PvPNetChatClient
{
    public static class Constants
    {
        public static class Server
        {
            public enum Region
            {
                UNK,
                EUNE,
                EUW,
                NA,
            }

            public static Dictionary<Region, string> RegionNames = new Dictionary<Region, string>()
            {
                {Region.EUNE,"Europe Nordic & East"},
                {Region.EUW, "Europe West"},
                {Region.NA, "North America"},
            };
        }
    }
}
