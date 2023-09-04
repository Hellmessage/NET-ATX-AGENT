using HAtxLib.Utils;
using Newtonsoft.Json.Linq;

namespace HAtxLib.UIAutomator {
    internal class UIAutomatorService {
		private readonly static string SERVICE_NAME = "uiautomator";
		private readonly static string _path = $"/services/{SERVICE_NAME}";
		private readonly string _url;

		public UIAutomatorService(HAtx atx) {
			_url = atx.AtxAgentUrl;
		}

		public bool Start() {
			using (HSocket socket = HSocket.Create(_url)) {
				var result = socket.HttpDelete(_path);
				if (result == null || result.Code != 200) {
					return false;
				}
				//string data = socket.PostData(_path, null);
				//if (data == null) {
				//	return false;
				//}
				return true;
			}
		}

		public bool Stop() {
            using (HSocket socket = HSocket.Create(_url)) {
				var result = socket.HttpDelete(_path);
				if (result == null || result.Code != 200) {
					return false;
				}
                return true;
            }
		}

		public bool Running() {
			using (HSocket socket = HSocket.Create(_url)) {
				var result = socket.HttpGet(_path);
				if (result == null || result.Code != 200) {
					return false;
				}
				JObject json = JObject.Parse(result.Content);
				return json.Value<bool>("running");
			}
		}

	}
}
