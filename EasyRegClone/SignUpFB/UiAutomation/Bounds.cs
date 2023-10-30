using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EasySignupFB.Library.UiAutomation
{
	public class Bounds
	{
		public int value_1
		{
			get;
			set;
		}

		public int value_2
		{
			get;
			set;
		}

		public int value_3
		{
			get;
			set;
		}

		public int value_4
		{
			get;
			set;
		}

		public Bounds(int value_1, int value_2, int value_3, int value_4)
		{
			this.value_1 = value_1;
			this.value_2 = value_2;
			this.value_3 = value_3;
			this.value_4 = value_4;
		}

		public static Bounds Parse(string bounds)
		{
			Bounds bound;
			if (!string.IsNullOrEmpty(bounds))
			{
				try
				{
					MatchCollection matchCollections = Regex.Matches(bounds, "[0-9]+");
					int num = StringHelpers.StringToInt(matchCollections[0].ToString(), 0);
					int num1 = StringHelpers.StringToInt(matchCollections[1].ToString(), 0);
					int num2 = StringHelpers.StringToInt(matchCollections[2].ToString(), 0);
					int num3 = StringHelpers.StringToInt(matchCollections[3].ToString(), 0);
					bound = new Bounds(num, num1, num2, num3);
					return bound;
				}
				catch (Exception exception)
				{
				}
			}
			bound = new Bounds(0, 0, 0, 0);
			return bound;
		}

		public override string ToString()
		{
			return string.Format("{0},{1},{2},{3}", new object[] { this.value_1, this.value_2, this.value_3, this.value_4 });
		}
	}
}
