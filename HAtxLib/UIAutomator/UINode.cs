using HAtxLib.Catch;
using HAtxLib.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Xml;

namespace HAtxLib.UIAutomator {
	public class UINode {
		private readonly static HLog Log = HLog.Get<HLog>("选择器");
		private readonly HAtx _atx;
		private readonly By _by;
		private XmlNodeList _nodes = null;
		private XmlNode _node = null;
		private NodeObj _info = null;
		private NodeObj Info {
			get {
				if (_info == null) {
					_info = ObjInfo();
					if (_info == null) {
						throw new ATXNodeException($"UINode({_by.Mask}<{_by}>) not found");
					}
				}
				return _info;
			}
		}

		private readonly List<UINode> _list = new List<UINode>();
		public UINode this[int index] {
			get {
				if (index >= _list.Count) {
					return null;
				}
				return _list[index];
			}
		}

		internal UINode(HAtx atx, By by) {
			_atx = atx;
			_by = by;
			_list.Add(this);
		}

		#region 获取元素文本内容
		/// <summary>
		/// 获取元素文本内容
		/// </summary>
		/// <returns></returns>
		public string Text() {
			return Info.Text;
		}
		#endregion

		#region 点击
		/// <summary>
		/// 元素点击
		/// </summary>
		/// <param name="wait">最长等待时间</param>
		/// <returns></returns>
		public bool Click(int wait = -1) {
			if (Exists(wait)) {
				Point pos = Info.Bound.CenterPos;
				return _atx.Click(pos.X, pos.Y);
			}
			return false;
		}
		#endregion

		#region 判断元素是否存在
		/// <summary>
		/// 判断元素是否存在
		/// </summary>
		/// <param name="wait">最大等待时长</param>
		/// <returns></returns>
		/// <exception cref="ATXException"></exception>
		/// <exception cref="ATXNodeException"></exception>
		public bool Exists(int wait = -1, bool xpathSub = false) {
			try {
                if (wait == -1) {
                    wait = _atx.UINodeMaxWaitTime;
                }
                if (_by.Mask == ByMask.Xpath) {
					if (xpathSub) {
                        Thread.Sleep(wait);
                        _nodes = FindNodes(_atx.DumpWindowHierarchy());
                        if (_nodes != null) {
                            _node = _nodes[0];
                        }
                        return _node != null;
                    }
					return XPathExist(wait);
                }
                var result = _atx.JsonRpc("waitForExists", _by, wait) ?? throw new ATXException("UINodeExists Fail");
                if (result.Error != null) {
                    throw new ATXNodeException("UINodeExists Fail", result.Error);
                }
                return result.Data is bool v && v;
            } catch (Exception ex) {
				Log.Error($"Serial<{_atx.UDID}>: {ex.Message}");
                return false;
			}
		}
		
		private bool XPathExist(int wait = -1) {
			if (wait == -1) {
				wait = _atx.UINodeMaxWaitTime;
			}
			while (wait > 0) {
				bool exist = HRuntime.Time(() => Exists(60, true), out int time);
				wait -= time;
				if (exist) {
					return true;
				}
			}
			return false;
		}
		#endregion

		#region 获取元素数据
		private NodeObj ObjInfo() {
			if (_by.Mask != ByMask.Xpath) {
				var result = _atx.JsonRpc("objInfo", _by) ?? throw new ATXException("Atx exception in <UINode.ObjInfo>");
				if (result.Error != null) {
					throw new ATXNodeException("ObjInfo Error", result.Error);
				}
				string data = (result.Data as JObject).ToString();
				return JsonConvert.DeserializeObject<NodeObj>(data);
			} else {
				if (_node == null) {
					if (!XPathExist()) {
						throw new ATXNodeException($"UINode({_by.Mask}<{_by}>) not found");
					}
				}
				return NodeObj.Create(_node);
			}
		}

		//private XmlNode XpathFindNode(string xml) {
		//	try {
		//		XmlDocument doc = new XmlDocument();
		//		doc.LoadXml(xml);
		//		return doc.SelectSingleNode(_by.Value);
		//	} catch (Exception) {
		//		return null;
		//	}
		//}

		private XmlNodeList FindNodes(string xml) {
			try {
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(xml);
				return doc.SelectNodes(_by.Value);
			} catch (Exception) {
				return null;
			}
		}
		#endregion

	}
	
	internal class NodeObj {
		[JsonProperty("bounds")]
		public Bounds Bound { get; set; }
		[JsonProperty("childCount")]
		public int ChildCount { get; set; }
		[JsonProperty("className")]
		public string ClassName { get; set; }
		[JsonProperty("contentDescription")]
		public string ContentDescription { get; set; }
		[JsonProperty("packageName")]
		public string PackageName { get; set; }
		[JsonProperty("resourceName")]
		public string ResourceName { get; set; }
		[JsonProperty("text")]
		public string Text { get; set; }
		[JsonProperty("visibleBounds")]
		public Bounds VisibleBounds { get; set; }
		[JsonProperty("checkable")]
		public bool Checkable { get; set; }
		[JsonProperty("checked")]
		public bool Checked { get; set; }
		[JsonProperty("clickable")]
		public bool Clickable { get; set; }
		[JsonProperty("enabled")]
		public bool Enabled { get; set; }
		[JsonProperty("focusable")]
		public bool Focusable { get; set; }
		[JsonProperty("focused")]
		public bool Focused { get; set; }
		[JsonProperty("longClickable")]
		public bool LongClickable { get; set; }
		[JsonProperty("scrollable")]
		public bool Scrollable { get; set; }
		[JsonProperty("selected")]
		public bool Selected { get; set; }
		[JsonProperty("index")]
		public int Index { get; set; } = 0;

		public static NodeObj Create(XmlNode node) {
			if (node == null) {
				return null;
			}
			var obj = new NodeObj() {
				Index = int.Parse(node.Attributes["index"].Value),
				Bound = Bounds.Create(node.Attributes["bounds"].Value),
				ChildCount = 0,
				ClassName = node.Attributes["class"].Value,
				ContentDescription = node.Attributes["content-desc"].Value,
				PackageName = node.Attributes["package"].Value,
				ResourceName = node.Attributes["resource-id"].Value,
				Text = node.Attributes["text"].Value,
				Checkable = bool.Parse(node.Attributes["checkable"].Value),
				Checked = bool.Parse(node.Attributes["checked"].Value),
				Clickable = bool.Parse(node.Attributes["clickable"].Value),
				Enabled = bool.Parse(node.Attributes["enabled"].Value),
				Focusable = bool.Parse(node.Attributes["focusable"].Value),
				Focused = bool.Parse(node.Attributes["focused"].Value),
				LongClickable = bool.Parse(node.Attributes["long-clickable"].Value),
				Scrollable = bool.Parse(node.Attributes["scrollable"].Value),
				Selected = bool.Parse(node.Attributes["selected"].Value),
			};
			obj.VisibleBounds = obj.Bound;
			return obj;
		}

		internal class Bounds {
			public readonly static Bounds Empty = new Bounds();

			[JsonProperty("top")]
			public int Top { get; set; } = 0;
			[JsonProperty("left")]
			public int Left { get; set; } = 0;
			[JsonProperty("bottom")]
			public int Bottom { get; set; } = 0;
			[JsonProperty("right")]
			public int Right { get; set; } = 0;

			[JsonIgnore]
			public int X { get => Left; }
			[JsonIgnore]
			public int Y { get => Top; }
			[JsonIgnore]
			public int Width { get => Right - Left; }
			[JsonIgnore]
			public int Height { get => Bottom - Top; }

			[JsonIgnore]
			public Point CenterPos {
				get {
					return new Point((Width / 2) + X, (Height / 2) + Y);
				}
			}

			public static Bounds Create(string value) {
				var sp = value.Split(new string[] { "][" }, StringSplitOptions.None);
				var tl = sp[0].Substring(1).Split(',');
				var br = sp[1].Substring(0, sp[1].Length - 1).Split(',');
				int tx = int.Parse(tl[0]);
				int ty = int.Parse(tl[1]);
				int bx = int.Parse(br[0]);
				int by = int.Parse(br[1]);
				return new Bounds() {
					Top = ty,
					Left = tx,
					Bottom = by,
					Right = bx
				};
			}
		}
	}
}