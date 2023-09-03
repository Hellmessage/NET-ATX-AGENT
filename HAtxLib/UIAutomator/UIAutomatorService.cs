using HAtxLib.Utils;
using Newtonsoft.Json.Linq;
using System.Net;

namespace HAtxLib.UIAutomator {
    internal class UIAutomatorService {
        private readonly static string SERVICE_NAME = "uiautomator";
        private readonly string _url;

        public UIAutomatorService(HAtx atx) {
            _url = $"{atx.AtxAgentUrl}/services/{SERVICE_NAME}";
        }

        public bool Start() {
            HttpWebResponse response = HApi.Post(_url, "");
            if (response == null) {
                return false;
            }
            return response.StatusCode == HttpStatusCode.OK;
        }

        public bool Stop() {
            HttpWebResponse response = HApi.Delete(_url);
            if (response == null) {
                return false;
            }
            return response.StatusCode == HttpStatusCode.OK;
        }

        public bool Running() {
            HttpWebResponse response = HApi.Get(_url);
            if (response == null) {
                return false;
            }
            if (response.StatusCode == HttpStatusCode.OK) {
                JObject json = JObject.Parse(response.DecodeDefault());
                return json.Value<bool>("running");
            }
            return false;
        }

    }
}
