using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EasySignupFB.Library.UiAutomation
{
	public class UiElement
	{
		public int index;

		public string text;

		public string resource_id;

		public string classname;

		public string package;

		public string contentdesc;

		public bool checkable;

		public bool ischecked;

		public bool clickable;

		public bool enabled;

		public bool focusable;

		public bool focused;

		public bool scrollable;

		public bool long_clickable;

		public bool password;

		public bool selected;

		public string bounds;

		public bool naf;

		public Bounds boundsObject;

		public TouchLocation touchLocation;

		public ElSize size;

		public XmlNode node;

		public bool Visible
		{
			get
			{
				return this.touchLocation != null;
			}
		}

		public UiElement(XmlNode node, int index, string text, string resource_id, string classname, string package, string contentdesc, bool checkable, bool ischecked, bool clickable, bool enabled, bool focusable, bool focused, bool scrollable, bool long_clickable, bool password, bool selected, string bounds, bool naf)
		{
			this.node = node;
			this.index = index;
			this.text = text;
			this.resource_id = resource_id;
			this.classname = classname;
			this.package = package;
			this.contentdesc = contentdesc;
			this.checkable = checkable;
			this.ischecked = ischecked;
			this.clickable = clickable;
			this.enabled = enabled;
			this.focusable = focusable;
			this.focused = focused;
			this.scrollable = scrollable;
			this.long_clickable = long_clickable;
			this.password = password;
			this.selected = selected;
			this.bounds = bounds;
			this.naf = naf;
			this.boundsObject = Bounds.Parse(this.bounds);
			this.size = ElSize.ParseFromBounds(this.boundsObject);
			this.touchLocation = TouchLocation.ParseFromBounds(this.boundsObject);
		}

		public TouchLocation GetRandomPoint()
		{
			TouchLocation touchLocation;
			if ((this.boundsObject == null ? false : this.size != null))
			{
				Random random = new Random();
				try
				{
					int value3 = this.boundsObject.value_3 - this.boundsObject.value_1 + this.boundsObject.value_1 - random.Next(1, this.size.width);
					int value4 = this.boundsObject.value_4 - this.boundsObject.value_2 + this.boundsObject.value_2 - random.Next(1, this.size.height);
					touchLocation = new TouchLocation(value3, value4);
					return touchLocation;
				}
				catch (Exception exception)
				{
				}
			}
			touchLocation = null;
			return touchLocation;
		}

		public string GetText()
		{
			string empty;
			try
			{
				empty = this.text.Trim();
				return empty;
			}
			catch (Exception exception)
			{
			}
			empty = string.Empty;
			return empty;
		}

		public static UiElement Parse(XmlNode xmlNode)
		{
			UiElement uiElement;
			if (xmlNode != null)
			{
				try
				{
					int num = StringHelpers.StringToInt(xmlNode.Attributes["index"].Value, -1);
					string value = xmlNode.Attributes["text"].Value;
					string str = xmlNode.Attributes["resource-id"].Value;
					string value1 = xmlNode.Attributes["class"].Value;
					string str1 = xmlNode.Attributes["package"].Value;
					string value2 = xmlNode.Attributes["content-desc"].Value;
					bool flag = StringHelpers.StringToBool(xmlNode.Attributes["checkable"].Value, false);
					bool flag1 = StringHelpers.StringToBool(xmlNode.Attributes["checked"].Value, false);
					bool flag2 = StringHelpers.StringToBool(xmlNode.Attributes["clickable"].Value, false);
					bool flag3 = StringHelpers.StringToBool(xmlNode.Attributes["enabled"].Value, false);
					bool flag4 = StringHelpers.StringToBool(xmlNode.Attributes["focusable"].Value, false);
					bool flag5 = StringHelpers.StringToBool(xmlNode.Attributes["focused"].Value, false);
					bool flag6 = StringHelpers.StringToBool(xmlNode.Attributes["scrollable"].Value, false);
					bool flag7 = StringHelpers.StringToBool(xmlNode.Attributes["long-clickable"].Value, false);
					bool flag8 = StringHelpers.StringToBool(xmlNode.Attributes["password"].Value, false);
					bool flag9 = StringHelpers.StringToBool(xmlNode.Attributes["selected"].Value, false);
					string str2 = xmlNode.Attributes["bounds"].Value;
					bool naf = StringHelpers.StringToBool(xmlNode.Attributes["selected"].Value, false);
					uiElement = new UiElement(xmlNode, num, value, str, value1, str1, value2, flag, flag1, flag2, flag3, flag4, flag5, flag6, flag7, flag8, flag9, str2, naf);
					return uiElement;
				}
				catch (Exception exception)
				{
				}
			}
			uiElement = null;
			return uiElement;
		}
	}
}
