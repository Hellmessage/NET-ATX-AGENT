using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HAtxLib.UIAutomator.Model {
	public class JsonRpcResponse {
		[JsonProperty("jsonrpc")]
		public string Version;

		[JsonProperty("id")]
		public string Id;

		[JsonProperty("error")]
		public JObject Error = null;

		[JsonProperty("result")]
		public object Data;

	}
}