using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pub.Class;
using System.Dynamic;
using System.Diagnostics;
using Newtonsoft.Json;
using fastJSON;
using System.IO;
using System.Security.Cryptography;
using System.Data;

namespace Pub.Class.Tests {
    public class dbs {
        public static string master1 = "master1";
        public static string master2 = "master2";
        public static string slave1 = "slave1";
        public static string slave2 = "slave2";
        public static string slave3 = "slave3";
        public static string[] masters = new string[] { "master1", "master2" };
        public static string[] slaves = new string[] { "slave1", "slave2", "slave3" };
        public static void init() {
            Data.AddPool(new Database("SqlServer", "server=.;uid=sa;pwd=123456789;database=THLottery;", "master1"));
            Data.AddPool(new Database("SqlServer", "server=.;uid=sa;pwd=123456789;database=THLottery;", "master2"));
            Data.AddPool(new Database("SqlServer", "server=.;uid=sa;pwd=123456789;database=THLottery;", "slave1"));
            Data.AddPool(new Database("SqlServer", "server=.;uid=sa;pwd=123456789;database=THLottery;", "slave2"));
            Data.AddPool(new Database("SqlServer", "server=.;uid=sa;pwd=123456789;database=THLottery;", "slave3"));
        }
    }
}
