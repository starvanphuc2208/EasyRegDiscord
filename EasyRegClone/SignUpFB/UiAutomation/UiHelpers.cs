using MCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EasySignupFB.Library.UiAutomation
{
	public static class UiHelpers
	{
		public static UiElement FindElementByClassPackage(this Device adbClient, string className, string package)
		{
			UiElement uiElement;
			XmlNodeList xmlNodeLists = adbClient.GetXmlNodeList();
			if ((xmlNodeLists == null ? false : xmlNodeLists.Count > 0))
			{
				foreach (XmlNode xmlNodes in xmlNodeLists)
				{
					UiElement uiElement1 = UiElement.Parse(xmlNodes);
					if ((!uiElement1.IsVisible() || string.IsNullOrEmpty(uiElement1.classname) || !uiElement1.classname.Equals(className) || string.IsNullOrEmpty(uiElement1.package) ? true : !uiElement1.package.Equals(package)))
					{
						continue;
					}
					uiElement = uiElement1;
					return uiElement;
				}
			}
			uiElement = null;
			return uiElement;
		}

		public static UiElement FindElementByClassPackage(this UiElement uiElement, string className, string package)
		{
			UiElement uiElement1;
			if ((!uiElement.IsVisible() ? false : uiElement.node != null))
			{
				try
				{
					IEnumerator enumerator = uiElement.node.GetEnumerator();
					while (enumerator.MoveNext())
					{
						UiElement uiElement2 = UiElement.Parse((XmlNode)enumerator.Current);
						if ((!uiElement2.IsVisible() || string.IsNullOrEmpty(uiElement2.classname) || !uiElement2.classname.Equals(className) || string.IsNullOrEmpty(uiElement2.package) ? true : !uiElement2.package.Equals(package)))
						{
							continue;
						}
						uiElement1 = uiElement2;
						return uiElement1;
					}
				}
				catch (Exception exception)
				{
				}
			}
			uiElement1 = null;
			return uiElement1;
		}



		public static UiElement FindElementByClassPackageContentDesc(this Device adbClient, string className, string package, string contentDesc, bool toLower = false)
		{
			UiElement uiElement;
			XmlNodeList xmlNodeLists = adbClient.GetXmlNodeList();
			if ((xmlNodeLists == null ? false : xmlNodeLists.Count > 0))
			{
				foreach (XmlNode xmlNodes in xmlNodeLists)
				{
					if (!toLower)
					{
						UiElement uiElement1 = UiElement.Parse(xmlNodes);
						if ((!uiElement1.IsVisible() || string.IsNullOrEmpty(uiElement1.classname) || !uiElement1.classname.Equals(className) || string.IsNullOrEmpty(uiElement1.package) || !uiElement1.package.Equals(package) || string.IsNullOrEmpty(uiElement1.contentdesc) ? true : !uiElement1.contentdesc.Equals(contentDesc)))
						{
							continue;
						}
						uiElement = uiElement1;
						return uiElement;
					}
					else
					{
						UiElement uiElement2 = UiElement.Parse(xmlNodes);
						if ((!uiElement2.IsVisible() || string.IsNullOrEmpty(uiElement2.classname) || !uiElement2.classname.Equals(className) || string.IsNullOrEmpty(uiElement2.package) || !uiElement2.package.Equals(package) || string.IsNullOrEmpty(uiElement2.contentdesc) ? true : !uiElement2.contentdesc.ToLower().Equals(contentDesc.ToLower())))
						{
							continue;
						}
						uiElement = uiElement2;
						return uiElement;
					}
				}
			}
			uiElement = null;
			return uiElement;
		}

		public static UiElement FindElementByText(this Device adbClient, string text, bool isLower = false)
		{
			UiElement uiElement;
			XmlNodeList xmlNodeLists = null;

			for (int j = 0; j < 10; j++)
            {
				xmlNodeLists = adbClient.GetXmlNodeList();
				if ((xmlNodeLists == null ? false : xmlNodeLists.Count > 0)) break;
			}
			if ((xmlNodeLists == null ? false : xmlNodeLists.Count > 0))
			{
				foreach (XmlNode xmlNodes in xmlNodeLists)
				{
					UiElement uiElement1 = UiElement.Parse(xmlNodes);
					if (isLower)
                    {
						if ((!uiElement1.IsVisible() ? true : !uiElement1.text.ToLower().Contains(text)))
						{
							continue;
						}
						uiElement = uiElement1;
						return uiElement;
					} else
                    {
						if ((!uiElement1.IsVisible() ? true : !uiElement1.text.Contains(text)))
						{
							continue;
						}
						uiElement = uiElement1;
						return uiElement;
					}
					
				}
			}
			uiElement = null;
			return uiElement;
		}

		public static UiElement FindElementByResourceId(this Device adbClient, string id)
		{
			UiElement uiElement;
			XmlNodeList xmlNodeLists = adbClient.GetXmlNodeList();
			if ((xmlNodeLists == null ? false : xmlNodeLists.Count > 0))
			{
				foreach (XmlNode xmlNodes in xmlNodeLists)
				{
					UiElement uiElement1 = UiElement.Parse(xmlNodes);
					if ((!uiElement1.IsVisible() ? true : !uiElement1.resource_id.Equals(id)))
					{
						continue;
					}
					uiElement = uiElement1;
					return uiElement;
				}
			}
			uiElement = null;
			return uiElement;
		}

		public static UiElement FindElementById(this UiElement uiElement, string resource_id)
		{
			UiElement uiElement1;
			if ((!uiElement.IsVisible() ? false : uiElement.node != null))
			{
				try
				{
					IEnumerator enumerator = uiElement.node.GetEnumerator();
					while (enumerator.MoveNext())
					{
						UiElement uiElement2 = UiElement.Parse((XmlNode)enumerator.Current);
						if ((!uiElement2.IsVisible() || string.IsNullOrEmpty(uiElement2.resource_id) ? true : !uiElement2.resource_id.Equals(resource_id)))
						{
							continue;
						}
						uiElement1 = uiElement2;
						return uiElement1;
					}
				}
				catch (Exception exception)
				{
				}
			}
			uiElement1 = null;
			return uiElement1;
		}

		public static UiElement FindElementByNaf(this Device adbClient, bool naf)
		{
			UiElement uiElement;
			if (naf != false)
			{
				XmlNodeList xmlNodeLists = adbClient.GetXmlNodeList();
				if ((xmlNodeLists == null ? false : xmlNodeLists.Count > 0))
				{
					foreach (XmlNode xmlNodes in xmlNodeLists)
					{
						UiElement uiElement1 = UiElement.Parse(xmlNodes);
						if ((!uiElement1.IsVisible() || !uiElement1.naf.Equals(true)))
						{
							continue;
						}
						uiElement = uiElement1;
						return uiElement;
					}
				}
			}
			uiElement = null;
			return uiElement;
		}

		public static UiElement FindElementByIdClass(this Device adbClient, string id, string clzz)
		{
			UiElement uiElement;
			if ((string.IsNullOrEmpty(id) ? false : !string.IsNullOrEmpty(clzz)))
			{
				XmlNodeList xmlNodeLists = adbClient.GetXmlNodeList();
				if ((xmlNodeLists == null ? false : xmlNodeLists.Count > 0))
				{
					foreach (XmlNode xmlNodes in xmlNodeLists)
					{
						UiElement uiElement1 = UiElement.Parse(xmlNodes);
						if ((!uiElement1.IsVisible() || !uiElement1.resource_id.Equals(id) ? true : !uiElement1.classname.Equals(clzz)))
						{
							continue;
						}
						uiElement = uiElement1;
						return uiElement;
					}
				}
			}
			uiElement = null;
			return uiElement;
		}

		public static UiElement FindElementByIndexClassPackage(this Device adbClient, int index, string className, string package)
		{
			UiElement uiElement;
			XmlNodeList xmlNodeLists = adbClient.GetXmlNodeList();
			if ((xmlNodeLists == null ? false : xmlNodeLists.Count > 0))
			{
				foreach (XmlNode xmlNodes in xmlNodeLists)
				{
					UiElement uiElement1 = UiElement.Parse(xmlNodes);
					if ((!uiElement1.IsVisible() || uiElement1.index != index || string.IsNullOrEmpty(uiElement1.classname) || !uiElement1.classname.Equals(className) || string.IsNullOrEmpty(uiElement1.package) ? true : !uiElement1.package.Equals(package)))
					{
						continue;
					}
					uiElement = uiElement1;
					return uiElement;
				}
			}
			uiElement = null;
			return uiElement;
		}

		public static UiElement FindElementByIndexClassPackage(this UiElement uiElement, int index, string className, string package)
		{
			UiElement uiElement1;
			if ((!uiElement.IsVisible() ? false : uiElement.node != null))
			{
				try
				{
					IEnumerator enumerator = uiElement.node.GetEnumerator();
					while (enumerator.MoveNext())
					{
						UiElement uiElement2 = UiElement.Parse((XmlNode)enumerator.Current);
						if ((!uiElement2.IsVisible() || uiElement2.index != index || string.IsNullOrEmpty(uiElement2.classname) || !uiElement2.classname.Equals(className) || string.IsNullOrEmpty(uiElement2.package) ? true : !uiElement2.package.Equals(package)))
						{
							continue;
						}
						uiElement1 = uiElement2;
						return uiElement1;
					}
				}
				catch (Exception exception)
				{
				}
			}
			uiElement1 = null;
			return uiElement1;
		}

		public static UiElement FindElementByIndexId(this Device adbClient, int index, string id)
		{
			UiElement uiElement;
			XmlNodeList xmlNodeLists = adbClient.GetXmlNodeList();
			if ((xmlNodeLists == null ? false : xmlNodeLists.Count > 0))
			{
				foreach (XmlNode xmlNodes in xmlNodeLists)
				{
					UiElement uiElement1 = UiElement.Parse(xmlNodes);
					if ((!uiElement1.IsVisible() || uiElement1.index != index ? true : !uiElement1.resource_id.Equals(id)))
					{
						continue;
					}
					uiElement = uiElement1;
					return uiElement;
				}
			}
			uiElement = null;
			return uiElement;
		}

		public static UiElement FindElementByIndexTextClassPackage(this Device adbClient, int index, string text, string className, string package)
		{
			UiElement uiElement;
			XmlNodeList xmlNodeLists = adbClient.GetXmlNodeList();
			if ((xmlNodeLists == null ? false : xmlNodeLists.Count > 0))
			{
				foreach (XmlNode xmlNodes in xmlNodeLists)
				{
					UiElement uiElement1 = UiElement.Parse(xmlNodes);
					if ((!uiElement1.IsVisible() || uiElement1.index != index || string.IsNullOrEmpty(uiElement1.classname) || !uiElement1.classname.Equals(className) || string.IsNullOrEmpty(uiElement1.text) || !uiElement1.text.Equals(text) || string.IsNullOrEmpty(uiElement1.package) ? true : !uiElement1.package.Equals(package)))
					{
						continue;
					}
					uiElement = uiElement1;
					return uiElement;
				}
			}
			uiElement = null;
			return uiElement;
		}

		public static UiElement FindElementByTextClassPackage(this Device adbClient, string text, string className, string package, bool lowerText = false)
		{
			UiElement uiElement;
			XmlNodeList xmlNodeLists = adbClient.GetXmlNodeList();
			if ((xmlNodeLists == null ? false : xmlNodeLists.Count > 0))
			{
				foreach (XmlNode xmlNodes in xmlNodeLists)
				{
					UiElement uiElement1 = UiElement.Parse(xmlNodes);
					if (!uiElement1.IsVisible())
					{
						continue;
					}
					if (!lowerText)
					{
						if ((string.IsNullOrEmpty(uiElement1.classname) || !uiElement1.classname.Equals(className) || string.IsNullOrEmpty(uiElement1.text) || !uiElement1.text.Equals(text) || string.IsNullOrEmpty(uiElement1.package) ? true : !uiElement1.package.Equals(package)))
						{
							continue;
						}
						uiElement = uiElement1;
						return uiElement;
					}
					else
					{
						if ((string.IsNullOrEmpty(uiElement1.classname) || !uiElement1.classname.Equals(className) || string.IsNullOrEmpty(uiElement1.text) || !uiElement1.text.ToLower().Equals(text.ToLower()) || string.IsNullOrEmpty(uiElement1.package) ? true : !uiElement1.package.Equals(package)))
						{
							continue;
						}
						uiElement = uiElement1;
						return uiElement;
					}
				}
			}
			uiElement = null;
			return uiElement;
		}

		public static UiElement FindElementByTextClassPackage(this UiElement uiElement, int index, string className, string package)
		{
			UiElement uiElement1;
			if ((!uiElement.IsVisible() ? false : uiElement.node != null))
			{
				try
				{
					IEnumerator enumerator = uiElement.node.GetEnumerator();
					while (enumerator.MoveNext())
					{
						UiElement uiElement2 = UiElement.Parse((XmlNode)enumerator.Current);
						if ((!uiElement2.IsVisible() || uiElement2.index != index || string.IsNullOrEmpty(uiElement2.classname) || !uiElement2.classname.Equals(className) || string.IsNullOrEmpty(uiElement2.package) ? true : !uiElement2.package.Equals(package)))
						{
							continue;
						}
						uiElement1 = uiElement2;
						return uiElement1;
					}
				}
				catch (Exception exception)
				{
				}
			}
			uiElement1 = null;
			return uiElement1;
		}

		public static UiElement FindElementByTextId(this Device adbClient, string text, string id)
		{
			UiElement uiElement;
			XmlNodeList xmlNodeLists = adbClient.GetXmlNodeList();
			if ((xmlNodeLists == null ? false : xmlNodeLists.Count > 0))
			{
				foreach (XmlNode xmlNodes in xmlNodeLists)
				{
					UiElement uiElement1 = UiElement.Parse(xmlNodes);
					if ((!uiElement1.IsVisible() || !uiElement1.text.Equals(text) ? true : !uiElement1.resource_id.Equals(id)))
					{
						continue;
					}
					uiElement = uiElement1;
					return uiElement;
				}
			}
			uiElement = null;
			return uiElement;
		}

		public static UiElement FindElementContainsTextClassPackage(this Device adbClient, string text, string className, string package, bool lowerText = false)
		{
			UiElement uiElement;
			XmlNodeList xmlNodeLists = adbClient.GetXmlNodeList();
			if ((xmlNodeLists == null ? false : xmlNodeLists.Count > 0))
			{
				foreach (XmlNode xmlNodes in xmlNodeLists)
				{
					UiElement uiElement1 = UiElement.Parse(xmlNodes);
					if (!uiElement1.IsVisible())
					{
						continue;
					}
					if (!lowerText)
					{
						if ((string.IsNullOrEmpty(uiElement1.classname) || !uiElement1.classname.Equals(className) || string.IsNullOrEmpty(uiElement1.text) || !uiElement1.text.Contains(text) || string.IsNullOrEmpty(uiElement1.package) ? true : !uiElement1.package.Equals(package)))
						{
							continue;
						}
						uiElement = uiElement1;
						return uiElement;
					}
					else
					{
						if ((string.IsNullOrEmpty(uiElement1.classname) || !uiElement1.classname.Equals(className) || string.IsNullOrEmpty(uiElement1.text) || !uiElement1.text.ToLower().Contains(text.ToLower()) || string.IsNullOrEmpty(uiElement1.package) ? true : !uiElement1.package.Equals(package)))
						{
							continue;
						}
						uiElement = uiElement1;
						return uiElement;
					}
				}
			}
			uiElement = null;
			return uiElement;
		}

		public static List<UiElement> FindElementsByClass(this Device adbClient, string className)
		{
			List<UiElement> uiElements;
			XmlNodeList xmlNodeLists = adbClient.GetXmlNodeList();
			if ((xmlNodeLists == null ? true : xmlNodeLists.Count <= 0))
			{
				uiElements = null;
			}
			else
			{
				List<UiElement> uiElements1 = new List<UiElement>();
				foreach (XmlNode xmlNodes in xmlNodeLists)
				{
					UiElement uiElement = UiElement.Parse(xmlNodes);
					if (!uiElement.IsVisible())
					{
						continue;
					}
					if ((string.IsNullOrEmpty(uiElement.classname) ? true : !uiElement.classname.Equals(className)))
					{
						continue;
					}
					uiElements1.Add(uiElement);
				}
				uiElements = uiElements1;
			}
			return uiElements;
		}

		public static List<UiElement> FindElementsByClassContainsContentDesc(this UiElement uiElement, string className, string contentDesc, bool toLower)
		{
			List<UiElement> uiElements = new List<UiElement>();
			if ((!uiElement.IsVisible() ? false : uiElement.node != null))
			{
				try
				{
					IEnumerator enumerator = uiElement.node.GetEnumerator();
					while (enumerator.MoveNext())
					{
						UiElement uiElement1 = UiElement.Parse((XmlNode)enumerator.Current);
						if (!uiElement1.IsVisible())
						{
							continue;
						}
						if (!toLower)
						{
							if ((string.IsNullOrEmpty(uiElement1.classname) || !uiElement1.classname.Equals(className) || string.IsNullOrEmpty(uiElement1.contentdesc) ? true : !uiElement1.contentdesc.Contains(contentDesc)))
							{
								continue;
							}
							uiElements.Add(uiElement1);
						}
						else
						{
							if ((string.IsNullOrEmpty(uiElement1.classname) || !uiElement1.classname.ToLower().Equals(className.ToLower()) || string.IsNullOrEmpty(uiElement1.contentdesc) ? true : !uiElement1.contentdesc.ToLower().Contains(contentDesc.ToLower())))
							{
								continue;
							}
							uiElements.Add(uiElement1);
						}
					}
				}
				catch (Exception exception)
				{
				}
			}
			return uiElements;
		}

		public static List<UiElement> FindElementsByClassContentDesc(this Device adbClient, string className, string contentDesc, bool toLower)
		{
			List<UiElement> uiElements;
			XmlNodeList xmlNodeLists = adbClient.GetXmlNodeList();
			if ((xmlNodeLists == null ? true : xmlNodeLists.Count <= 0))
			{
				uiElements = null;
			}
			else
			{
				List<UiElement> uiElements1 = new List<UiElement>();
				foreach (XmlNode xmlNodes in xmlNodeLists)
				{
					UiElement uiElement = UiElement.Parse(xmlNodes);
					if (!uiElement.IsVisible())
					{
						continue;
					}
					if (!toLower)
					{
						if ((string.IsNullOrEmpty(uiElement.classname) || !uiElement.classname.Equals(className) || string.IsNullOrEmpty(uiElement.contentdesc) ? true : !uiElement.contentdesc.Equals(contentDesc)))
						{
							continue;
						}
						uiElements1.Add(uiElement);
					}
					else
					{
						if ((string.IsNullOrEmpty(uiElement.classname) || !uiElement.classname.Equals(className) || string.IsNullOrEmpty(uiElement.contentdesc) ? true : !uiElement.contentdesc.ToLower().Equals(contentDesc.ToLower())))
						{
							continue;
						}
						uiElements1.Add(uiElement);
					}
				}
				uiElements = uiElements1;
			}
			return uiElements;
		}

		public static List<UiElement> FindElementsByClassContentDesc(this UiElement uiElement, string className, string contentDesc, bool toLower)
		{
			List<UiElement> uiElements = new List<UiElement>();
			if ((!uiElement.IsVisible() ? false : uiElement.node != null))
			{
				try
				{
					IEnumerator enumerator = uiElement.node.GetEnumerator();
					while (enumerator.MoveNext())
					{
						UiElement uiElement1 = UiElement.Parse((XmlNode)enumerator.Current);
						if (!uiElement1.IsVisible())
						{
							continue;
						}
						if (!toLower)
						{
							if ((string.IsNullOrEmpty(uiElement1.classname) || !uiElement1.classname.Equals(className) || string.IsNullOrEmpty(uiElement1.contentdesc) ? true : !uiElement1.contentdesc.Equals(contentDesc)))
							{
								continue;
							}
							uiElements.Add(uiElement1);
						}
						else
						{
							if ((string.IsNullOrEmpty(uiElement1.classname) || !uiElement1.classname.ToLower().Equals(className.ToLower()) || string.IsNullOrEmpty(uiElement1.contentdesc) ? true : !uiElement1.contentdesc.ToLower().Equals(contentDesc.ToLower())))
							{
								continue;
							}
							uiElements.Add(uiElement1);
						}
					}
				}
				catch (Exception exception)
				{
				}
			}
			return uiElements;
		}

		public static UiElement FindElementStartWithEndWithTextClassPackage(this Device adbClient, string startWithText, string enWithText, string className, string package, bool lowerText = false)
		{
			UiElement uiElement;
			XmlNodeList xmlNodeLists = adbClient.GetXmlNodeList();
			if ((xmlNodeLists == null ? false : xmlNodeLists.Count > 0))
			{
				foreach (XmlNode xmlNodes in xmlNodeLists)
				{
					UiElement uiElement1 = UiElement.Parse(xmlNodes);
					if (!uiElement1.IsVisible())
					{
						continue;
					}
					if (!lowerText)
					{
						if ((string.IsNullOrEmpty(uiElement1.classname) || !uiElement1.classname.Equals(className) || string.IsNullOrEmpty(uiElement1.text) || !uiElement1.text.StartsWith(startWithText) || !uiElement1.text.EndsWith(enWithText) || string.IsNullOrEmpty(uiElement1.package) ? true : !uiElement1.package.Equals(package)))
						{
							continue;
						}
						uiElement = uiElement1;
						return uiElement;
					}
					else
					{
						if ((string.IsNullOrEmpty(uiElement1.classname) || !uiElement1.classname.Equals(className) || string.IsNullOrEmpty(uiElement1.text) || !uiElement1.text.ToLower().StartsWith(startWithText.ToLower()) || !uiElement1.text.ToLower().EndsWith(enWithText.ToLower()) || string.IsNullOrEmpty(uiElement1.package) ? true : !uiElement1.package.Equals(package)))
						{
							continue;
						}
						uiElement = uiElement1;
						return uiElement;
					}
				}
			}
			uiElement = null;
			return uiElement;
		}

		public static List<UiElement> GetUiElements(this UiElement uiElement, bool shuff = false)
		{
			List<UiElement> uiElements = new List<UiElement>();
			if ((!uiElement.IsVisible() ? false : uiElement.node != null))
			{
				IEnumerator enumerator = uiElement.node.GetEnumerator();
				while (enumerator.MoveNext())
				{
					UiElement uiElement1 = UiElement.Parse((XmlNode)enumerator.Current);
					if (!uiElement1.IsVisible())
					{
						continue;
					}
					uiElements.Add(uiElement1);
				}
				if ((!shuff || uiElements == null ? false : uiElements.Count > 0))
				{
					Random random = new Random(DateTime.Now.Millisecond);
					uiElements = (
						from item in uiElements
						orderby random.Next()
						select item).ToList<UiElement>();
				}
			}
			return uiElements;
		}

		public static bool IsVisible(this UiElement uiElement)
		{
			return (uiElement == null ? false : uiElement.Visible);
		}
	}
}
