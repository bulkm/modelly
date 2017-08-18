using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Models.DataBase
{
    public class Config 
    {
        public const int MAX_BDMS_PARAMS_NUM = 100;
        public static string ServerIP { get; set; }
        private static bool DebugMode = true;
        
        public static string GetConnectionString()
        {
            return @"Server=pesnic;Trusted_Connection=True;Connection Timeout=30;Database=bulk;";
        }

        public static bool GetDebugMode()
        {
            return DebugMode;
        }

    }
}
