using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace easy.MCommon
{
	internal class UIElement
	{
		private int int_0;

		private int int_1;

		public int X
		{
			get
			{
				return int_0;
			}
			set
			{
				int_0 = value;
			}
		}

		public int Y
		{
			get
			{
				return int_1;
			}
			set
			{
				int_1 = value;
			}
		}

		public bool isValid
		{
			get
			{
				if (X > 0)
				{
					return Y > 0;
				}
				return false;
			}
		}

		public UIElement(int int_2, int int_3)
		{
			int_0 = int_2;
			int_1 = int_3;
		}

		public UIElement(XDocument xdocument_0, string type, string content)
		{
			XElement xElement = (from xelement_0 in xdocument_0.Descendants()
								 where (string)xelement_0.Attribute(type) == content
								 select xelement_0).FirstOrDefault();
			if (xElement != null)
			{
				string value = xElement.Attribute("bounds").Value;
				Match match = new Regex("\\[(\\d+),(\\d+)\\]\\[(\\d+),(\\d+)\\]", RegexOptions.IgnoreCase).Match(value);
				if (match.Groups.Count != 5)
				{
					X = -1;
					Y = -1;
				}
				int num = int.Parse(match.Groups[1].Value);
				int num2 = int.Parse(match.Groups[2].Value);
				int num3 = int.Parse(match.Groups[3].Value);
				int num4 = int.Parse(match.Groups[4].Value);
				X = (num + num3) / 2;
				Y = (num2 + num4) / 2;
			}
			else
			{
				X = -1;
				Y = -1;
			}
		}

	}

}
