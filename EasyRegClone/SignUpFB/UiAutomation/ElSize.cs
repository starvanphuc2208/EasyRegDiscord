using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySignupFB.Library.UiAutomation
{
	public class ElSize
	{
		public int height
		{
			get;
			set;
		}

		public int width
		{
			get;
			set;
		}

		public ElSize(int width, int height)
		{
			this.width = width;
			this.height = height;
		}

		public static ElSize ParseFromBounds(Bounds bounds)
		{
			ElSize elSize;
			if (bounds == null)
			{
				elSize = null;
			}
			else
			{
				int value3 = bounds.value_3 - bounds.value_1;
				int value4 = bounds.value_4 - bounds.value_2;
				elSize = new ElSize(value3, value4);
			}
			return elSize;
		}

		public override string ToString()
		{
			string str = string.Format("{0},{1}", this.width, this.height);
			return str;
		}
	}
}
