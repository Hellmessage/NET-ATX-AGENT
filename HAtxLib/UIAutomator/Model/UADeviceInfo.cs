using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAtxLib.UIAutomator.Model {
    public class UADeviceInfo {
        [JsonProperty("currentPackageName")]
        public string CurrentPackageName { get; set; }

        [JsonProperty("displayRotation")]
        public int DisplayRotation { get; set; }

        [JsonProperty("displayHeight")]
        public int DisplayHeight { get; set; }
        [JsonProperty("displayWidth")]
        public int DisplayWidth { get; set; }

        [JsonProperty("displaySizeDpX")]
        public int DisplaySizeDpX { get; set; }
        [JsonProperty("displaySizeDpY")]
        public int DisplaySizeDpY { get; set; }

        [JsonProperty("productName")]
        public string ProductName { get; set; }

        [JsonProperty("screenOn")]
        public bool ScreenOn { get; set; }

        [JsonProperty("sdkInt")]
        public int SdkInt { get; set; }

        [JsonProperty("naturalOrientation")]
        public bool NaturalOrientation { get; set; }

    }
}
