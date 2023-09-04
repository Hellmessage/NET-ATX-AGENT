using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml;

namespace HAtxLib.UIAutomator {

	public enum ByMask {
		Xpath = 0,
		Text = 0x01,
		TextContains = 0x02,
		TextMatches = 0x04,
		TextStartsWith = 0x08,
		ClassName = 0x10,
		ClassNameMatches = 0x20,
		Description = 0x40,
		DescriptionContains = 0x80,
		DescriptionMatches = 0x0100,
		DescriptionStartsWith = 0x0200,
		Checkable = 0x0400,
		Checked = 0x0800,
		Clickable = 0x1000,
		LongClickable = 0x2000,
		Scrollable = 0x4000,
		Enabled = 0x8000,
		Focusable = 0x010000,
		Focused = 0x020000,
		Selected = 0x040000,
		PackageName = 0x080000,
		PackageNameMatches = 0x100000,
		ResourceId = 0x200000,
		ResourceIdMatches = 0x400000,
		Index = 0x800000,
		Instance = 0x01000000,
	}

	public class By : JObject {
		private readonly static Dictionary<ByMask, ByItem> MaskDict = new Dictionary<ByMask, ByItem>() {
			{ ByMask.Text,                  new ByItem(){ Name = "text", Flag = string.Empty, Mask = (int)ByMask.Text } },
			{ ByMask.TextContains,          new ByItem(){ Name = "textContains", Flag = string.Empty, Mask = (int)ByMask.TextContains } },
			{ ByMask.TextMatches,           new ByItem(){ Name = "textMatches", Flag = string.Empty, Mask = (int)ByMask.TextMatches } },
			{ ByMask.TextStartsWith,        new ByItem(){ Name = "textStartsWith", Flag = string.Empty, Mask = (int)ByMask.TextStartsWith } },
			{ ByMask.ClassName,             new ByItem(){ Name = "className", Flag = string.Empty, Mask = (int)ByMask.ClassName } },
			{ ByMask.ClassNameMatches,      new ByItem(){ Name = "classNameMatches", Flag = string.Empty, Mask = (int)ByMask.ClassNameMatches } },
			{ ByMask.Description,           new ByItem(){ Name = "description", Flag = string.Empty, Mask = (int)ByMask.Description } },
			{ ByMask.DescriptionContains,   new ByItem(){ Name = "descriptionContains", Flag = string.Empty, Mask = (int)ByMask.DescriptionContains } },
			{ ByMask.DescriptionMatches,    new ByItem(){ Name = "descriptionMatches", Flag = string.Empty, Mask = (int)ByMask.DescriptionMatches } },
			{ ByMask.DescriptionStartsWith, new ByItem(){ Name = "descriptionStartsWith", Flag = string.Empty, Mask = (int)ByMask.DescriptionStartsWith } },
			{ ByMask.Checkable,             new ByItem(){ Name = "checkable", Flag = false, Mask = (int)ByMask.Checkable } },
			{ ByMask.Checked,               new ByItem(){ Name = "checked", Flag = false, Mask = (int)ByMask.Checked } },
			{ ByMask.Clickable,             new ByItem(){ Name = "clickable", Flag = false, Mask = (int)ByMask.Clickable } },
			{ ByMask.LongClickable,         new ByItem(){ Name = "longClickable", Flag = false, Mask = (int)ByMask.LongClickable } },
			{ ByMask.Scrollable,			new ByItem(){ Name = "scrollable", Flag = false, Mask = (int)ByMask.Scrollable } },
			{ ByMask.Enabled,				new ByItem(){ Name = "enabled", Flag = false, Mask = (int)ByMask.Enabled } },
			{ ByMask.Focusable,				new ByItem(){ Name = "focusable", Flag = false, Mask = (int)ByMask.Focusable } },
			{ ByMask.Focused,				new ByItem(){ Name = "focused", Flag = false, Mask = (int)ByMask.Focused } },
			{ ByMask.Selected,				new ByItem(){ Name = "selected", Flag = false, Mask = (int)ByMask.Selected } },
			{ ByMask.PackageName,			new ByItem(){ Name = "packageName", Flag = string.Empty, Mask = (int)ByMask.PackageName } },
			{ ByMask.PackageNameMatches,	new ByItem(){ Name = "packageNameMatches", Flag = string.Empty, Mask = (int)ByMask.PackageNameMatches } },
			{ ByMask.ResourceId,			new ByItem(){ Name = "resourceId", Flag = string.Empty, Mask = (int)ByMask.ResourceId } },
			{ ByMask.ResourceIdMatches,		new ByItem(){ Name = "resourceIdMatches", Flag = string.Empty, Mask = (int)ByMask.ResourceIdMatches } },
			{ ByMask.Index,					new ByItem(){ Name = "index", Flag = 0, Mask = (int)ByMask.Index } },
			{ ByMask.Instance,				new ByItem(){ Name = "instance", Flag = 0, Mask = (int)ByMask.Instance } },
		};
		private const string DefaultEncoding = "ISO-8859-1";
		public static Encoding Encoding { get; } = Encoding.GetEncoding(DefaultEncoding);
		public ByMask Mask { get; private set; }
		private string Value { get; set; }
		private JArray ChildOrSibling { get; set; } = new JArray();
		private JArray ChildOrSiblingSelector { get; set; } = new JArray();
		private By[] SubBy { get; set; } = new By[0];
		public Point Pos { get; private set; } = Point.Empty;

		private By() {}

		public static By Create(ByMask mask, string value, params By[] sub) {
			return new By() {
				Mask = mask,
				Value = value,
				SubBy = sub
			};
		}

		public override string ToString() {
			return ToJson().ToString(Newtonsoft.Json.Formatting.None);
		}

		public JObject ToJson() {
			JObject json = new JObject {
				{ "childOrSibling", ChildOrSibling },
				{ "childOrSiblingSelector", ChildOrSiblingSelector },
				{ MaskDict[Mask].Name, Value }
			};
			int mask = (int)Mask;
			foreach (By by in SubBy) {
				mask |= (int)by.Mask;
				json.Add(MaskDict[by.Mask].Name, by.Value);
			}
			json.Add("mask", mask);
			return json;
		}

		public static By ResourceId(string value, params By[] sub) {
			return Create(ByMask.ResourceId, value, sub);
		}

		public static By Text(string value) {
			return Create(ByMask.Text, value);
		}

		public static By Description(string value) {
			return Create(ByMask.Description, value);
		}

		public static By Xpath(string value) {
			return Create(ByMask.Xpath, value);
		}

		public bool XpathToAndroid(string xml) {
			if (Mask == ByMask.Xpath) {
				try {
					XmlDocument doc = new XmlDocument();
					doc.LoadXml(xml);
					var data = doc.SelectNodes(Value);
					if (data != null) {
						var element = data[0];
						if (element != null) {
							var value = element.Attributes["bounds"].Value;
							var sp = value.Split(new string[] { "][" }, StringSplitOptions.None);
							var tl = sp[0].Substring(1).Split(',');
							var br = sp[1].Substring(0, sp[1].Length - 1).Split(',');
							int tx = int.Parse(tl[0]);
							int ty = int.Parse(tl[1]);
							int bx = int.Parse(br[0]);
							int by = int.Parse(br[1]);
							int cx = (bx - tx) / 2;
							int cy = (by - ty) / 2;
							Pos = new Point(cx + tx, cy + ty);
							return true;
						}
					}
					return false;
				} catch (Exception) {
					return false;
				}
			}
			return false;
		}

		internal class ByItem {
			public string Name { get; set; }
			public int Mask { get; set; }
			public object Flag { get; set; }
		} 
	}
}
