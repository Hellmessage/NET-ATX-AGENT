using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAtxLib.ADB.Entity {
    public class AndroidAppInfo {

        public string PackageName { get; set; }
        public string VersionName { get; set; } = "";
        public string VersionCode { get; set; } = "";
        public string Flags { get; set; } = "";
        public string FirstInstallTime { get; set; } = "";
        public string LastUpdateTime { get; set; } = "";
        public string Signature { get; set; } = "";
        public string Path { get; set; } = "";
        public string Dumpsys { get; set; } = "";
        public List<string> SubApkPaths { get; set; } = new List<string>();

        public AndroidAppInfo() { 
            
        }


    }
}
