using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySignupFB.Library.UiAutomation
{
	public class TouchLocation
	{
		public int x;

		public int y;

		public TouchLocation(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public static TouchLocation Parse(string input)
		{
			TouchLocation touchLocation;
			if ((string.IsNullOrEmpty(input) ? false : input.Contains(",")))
			{
				string[] strArrays = input.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
				if ((strArrays == null ? false : (int)strArrays.Length >= 2))
				{
					try
					{
						int num = StringHelpers.StringToInt(strArrays[0].Trim(), 0);
						int num1 = StringHelpers.StringToInt(strArrays[1].Trim(), 0);
						if ((num <= 0 ? false : num1 > 0))
						{
							touchLocation = new TouchLocation(num, num1);
							return touchLocation;
						}
					}
					catch (Exception exception)
					{
					}
				}
			}
			touchLocation = null;
			return touchLocation;
		}

		public static TouchLocation ParseFromBounds(Bounds bounds)
		{
			TouchLocation touchLocation;
			if (bounds != null)
			{
				try
				{
					int value3 = (bounds.value_3 - bounds.value_1) / 2 + bounds.value_1;
					int value4 = (bounds.value_4 - bounds.value_2) / 2 + bounds.value_2;
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

		public override string ToString()
		{
			string str = string.Format("{0},{1}", this.x, this.y);
			return str;
		}
	}
}
