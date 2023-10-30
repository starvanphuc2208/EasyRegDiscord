using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MCommon
{
	public class AutoControl
	{
		public AutoControl()
		{
		}

		public static IntPtr BringToFront(string className, string windowName = null)
		{
			IntPtr intPtr = AutoControl.FindWindow(className, windowName);
			AutoControl.SetForegroundWindow(intPtr);
			return intPtr;
		}

		public static IntPtr BringToFront(IntPtr hWnd)
		{
			AutoControl.SetForegroundWindow(hWnd);
			return hWnd;
		}

		public static bool CallbackChild(IntPtr hWnd, IntPtr lParam)
		{
			bool flag;
			string text = AutoControl.GetText(hWnd);
			string className = AutoControl.GetClassName(hWnd);
			if ((text != "&Options >>" ? true : !className.StartsWith("ToolbarWindow32")))
			{
				flag = true;
			}
			else
			{
				AutoControl.SendMessage(hWnd, 0, IntPtr.Zero, IntPtr.Zero);
				flag = false;
			}
			return flag;
		}

		public static void Click(EMouseKey mouseKey = 0)
		{
			switch (mouseKey)
			{
				case EMouseKey.LEFT:
				{
					AutoControl.mouse_event(32774, 0, 0, 0, UIntPtr.Zero);
					break;
				}
				case EMouseKey.RIGHT:
				{
					AutoControl.mouse_event(32792, 0, 0, 0, UIntPtr.Zero);
					break;
				}
				case EMouseKey.DOUBLE_LEFT:
				{
					AutoControl.mouse_event(32774, 0, 0, 0, UIntPtr.Zero);
					AutoControl.mouse_event(32774, 0, 0, 0, UIntPtr.Zero);
					break;
				}
				case EMouseKey.DOUBLE_RIGHT:
				{
					AutoControl.mouse_event(32792, 0, 0, 0, UIntPtr.Zero);
					AutoControl.mouse_event(32792, 0, 0, 0, UIntPtr.Zero);
					break;
				}
			}
		}

		[DllImport("User32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern bool EnumChildWindows(IntPtr hWndParent, AutoControl.CallBack lpEnumFunc, IntPtr lParam);

		[DllImport("user32", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern bool EnumChildWindows(IntPtr window, AutoControl.EnumWindowProc callback, IntPtr lParam);

		public static IntPtr FindHandle(IntPtr parentHandle, string className, string text)
		{
			return AutoControl.FindHandleWithText(AutoControl.GetChildHandle(parentHandle), className, text);
		}

		public static List<IntPtr> FindHandles(IntPtr parentHandle, string className, string text)
		{
			return AutoControl.FindHandlesWithText(AutoControl.GetChildHandle(parentHandle), className, text);
		}

		public static List<IntPtr> FindHandlesWithText(List<IntPtr> handles, string className, string text)
		{
			List<IntPtr> intPtrs = new List<IntPtr>();
			IEnumerable<IntPtr> intPtrs1 = handles.Where<IntPtr>((IntPtr ptr) => {
				string str = AutoControl.GetClassName(ptr);
				string str1 = AutoControl.GetText(ptr);
				return (str == className ? true : str1 == text);
			});
			return intPtrs1.ToList<IntPtr>();
		}

		public static IntPtr FindHandleWithText(List<IntPtr> handles, string className, string text)
		{
			return handles.Find((IntPtr ptr) => {
				string str = AutoControl.GetClassName(ptr);
				string str1 = AutoControl.GetText(ptr);
				return (str == className ? true : str1 == text);
			});
		}

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll", CharSet=CharSet.None, EntryPoint="FindWindow", ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

		private static IntPtr FindWindowByIndex(IntPtr hWndParent, int index)
		{
			IntPtr intPtr;
			bool flag;
			if (index != 0)
			{
				int num = 0;
				IntPtr zero = IntPtr.Zero;
				do
				{
					zero = AutoControl.FindWindowEx(hWndParent, zero, "Button", null);
					if (zero != IntPtr.Zero)
					{
						num++;
					}
					flag = (num >= index ? false : zero != IntPtr.Zero);
				}
				while (flag);
				intPtr = zero;
			}
			else
			{
				intPtr = hWndParent;
			}
			return intPtr;
		}

		[DllImport("user32.dll", CharSet=CharSet.Unicode, ExactSpelling=false)]
		private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string lclassName, string windowTitle);

		public static IntPtr FindWindowExFromParent(IntPtr parentHandle, string text, string className)
		{
			return AutoControl.FindWindowEx(parentHandle, IntPtr.Zero, className, text);
		}

		public static IntPtr FindWindowHandle(string className, string windowName)
		{
			return AutoControl.FindWindow(className, windowName);
		}

		public static IntPtr FindWindowHandleFromProcesses(string className, string windowName)
		{
			Process[] processes = Process.GetProcesses();
			IntPtr zero = IntPtr.Zero;
			using (IEnumerator<Process> enumerator = (
				from p in (IEnumerable<Process>)processes
				where p.MainWindowHandle != IntPtr.Zero
				select p).GetEnumerator())
			{
				while (true)
				{
					if (enumerator.MoveNext())
					{
						IntPtr mainWindowHandle = enumerator.Current.MainWindowHandle;
						string str = AutoControl.GetClassName(mainWindowHandle);
						string text = AutoControl.GetText(mainWindowHandle);
						if ((str == className ? true : text == windowName))
						{
							zero = mainWindowHandle;
							break;
						}
					}
					else
					{
						break;
					}
				}
			}
			return zero;
		}

		public static List<IntPtr> FindWindowHandlesFromProcesses(string className, string windowName, int maxCount = 1)
		{
			Process[] processes = Process.GetProcesses();
			List<IntPtr> intPtrs = new List<IntPtr>();
			int num = 0;
			foreach (Process process in 
				from p in (IEnumerable<Process>)processes
				where p.MainWindowHandle != IntPtr.Zero
				select p)
			{
				IntPtr mainWindowHandle = process.MainWindowHandle;
				string str = AutoControl.GetClassName(mainWindowHandle);
				string text = AutoControl.GetText(mainWindowHandle);
				if ((str == className ? false : text != windowName))
				{
					continue;
				}
				intPtrs.Add(mainWindowHandle);
				if (num >= maxCount)
				{
					break;
				}
				num++;
			}
			return intPtrs;
		}

		public static List<IntPtr> GetChildHandle(IntPtr parentHandle)
		{
			List<IntPtr> list = new List<IntPtr>();
			GCHandle value = GCHandle.Alloc(list);
			IntPtr lParam2 = GCHandle.ToIntPtr(value);
			try
			{
				AutoControl.EnumWindowProc callback = delegate (IntPtr hWnd, IntPtr lParam)
				{
					GCHandle gchandle = GCHandle.FromIntPtr(lParam);
					bool flag = gchandle.Target == null;
					bool result;
					if (flag)
					{
						result = false;
					}
					else
					{
						List<IntPtr> list2 = gchandle.Target as List<IntPtr>;
						list2.Add(hWnd);
						result = true;
					}
					return result;
				};
				AutoControl.EnumChildWindows(parentHandle, callback, lParam2);
			}
			finally
			{
				value.Free();
			}
			return list;
		}

		[DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=false, SetLastError=true)]
		private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

		public static string GetClassName(IntPtr hWnd)
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			AutoControl.GetClassName(hWnd, stringBuilder, 256);
			return stringBuilder.ToString().Trim();
		}

		public static IntPtr GetControlHandleFromControlID(IntPtr parentHandle, int controlId)
		{
			return AutoControl.GetDlgItem(parentHandle, controlId);
		}

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern IntPtr GetDlgItem(IntPtr hWnd, int nIDDlgItem);

		public static Point GetGlobalPoint(IntPtr hWnd, Point? point = null)
		{
			Point value;
			Point x = new Point();
			RECT windowRect = AutoControl.GetWindowRect(hWnd);
			if (!point.HasValue)
			{
				value = new Point();
				point = new Point?(value);
			}
			value = point.Value;
			x.X = value.X + windowRect.Left;
			value = point.Value;
			x.Y = value.Y + windowRect.Top;
			return x;
		}

		public static Point GetGlobalPoint(IntPtr hWnd, int x = 0, int y = 0)
		{
			Point point = new Point();
			RECT windowRect = AutoControl.GetWindowRect(hWnd);
			point.X = x + windowRect.Left;
			point.Y = y + windowRect.Top;
			return point;
		}

		public static string GetText(IntPtr hWnd)
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			AutoControl.GetWindowText(hWnd, stringBuilder, 256);
			return stringBuilder.ToString().Trim();
		}

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

		public static RECT GetWindowRect(IntPtr hWnd)
		{
			RECT rECT = new RECT();
			AutoControl.GetWindowRect(hWnd, ref rECT);
			return rECT;
		}

		[DllImport("User32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder s, int nMaxCount);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern bool IsWindowVisible(IntPtr hWnd);

		public static bool IsWindowVisible_(IntPtr handle)
		{
			return AutoControl.IsWindowVisible(handle);
		}

		public static IntPtr MakeLParam(int LoWord, int HiWord)
		{
			return (IntPtr)(HiWord << 16 | LoWord & 65535);
		}

		public static IntPtr MakeLParamFromXY(int x, int y)
		{
			return (IntPtr)(y << 16 | x);
		}

		[DllImport("user32.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Auto, ExactSpelling=false)]
		public static extern void mouse_event(uint dwFlags, int dx, int dy, int dwData, UIntPtr dwExtraInfo);

		public static void MouseClick(int x, int y, EMouseKey mouseKey = 0)
		{
			Cursor.Position = new Point(x, y);
			AutoControl.Click(mouseKey);
		}

		public static void MouseClick(Point point, EMouseKey mouseKey = 0)
		{
			Cursor.Position = point;
			AutoControl.Click(mouseKey);
		}

		public static void MouseDragX(Point startPoint, int deltaX, bool isNegative = false)
		{
			Cursor.Position = startPoint;
			AutoControl.mouse_event(2, 0, 0, 0, UIntPtr.Zero);
			for (int i = 0; i < deltaX; i++)
			{
				if (isNegative)
				{
					AutoControl.mouse_event(1, -1, 0, 0, UIntPtr.Zero);
				}
				else
				{
					AutoControl.mouse_event(1, 1, 0, 0, UIntPtr.Zero);
				}
			}
			AutoControl.mouse_event(32772, 0, 0, 0, UIntPtr.Zero);
		}

		public static void MouseDragY(Point startPoint, int deltaY, bool isNegative = false)
		{
			Cursor.Position = startPoint;
			AutoControl.mouse_event(2, 0, 0, 0, UIntPtr.Zero);
			for (int i = 0; i < deltaY; i++)
			{
				if (isNegative)
				{
					AutoControl.mouse_event(1, 0, -1, 0, UIntPtr.Zero);
				}
				else
				{
					AutoControl.mouse_event(1, 0, 1, 0, UIntPtr.Zero);
				}
			}
			AutoControl.mouse_event(32772, 0, 0, 0, UIntPtr.Zero);
		}

		public static void MouseScroll(Point startPoint, int deltaY, bool isNegative = false)
		{
			Cursor.Position = startPoint;
			AutoControl.mouse_event(2048, 0, 0, deltaY, UIntPtr.Zero);
		}

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern bool PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern bool PostMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);

		public static void SendClickDownOnPosition(IntPtr controlHandle, int x, int y, EMouseKey mouseButton = 0, int clickTimes = 1)
		{
			int num = 0;
			if (mouseButton == EMouseKey.LEFT)
			{
				num = 513;
			}
			if (mouseButton == EMouseKey.RIGHT)
			{
				num = 516;
			}
			IntPtr intPtr = AutoControl.MakeLParamFromXY(x, y);
			for (int i = 0; i < clickTimes; i++)
			{
				AutoControl.PostMessage(controlHandle, 6, new IntPtr(1), intPtr);
				AutoControl.PostMessage(controlHandle, num, new IntPtr(1), intPtr);
			}
		}

		public static void SendClickOnControlByHandle(IntPtr hWndButton)
		{
			AutoControl.SendMessage(hWndButton, 513, IntPtr.Zero, IntPtr.Zero);
			AutoControl.SendMessage(hWndButton, 514, IntPtr.Zero, IntPtr.Zero);
		}

		public static void SendClickOnControlById(IntPtr parentHWND, int controlID)
		{
			IntPtr dlgItem = AutoControl.GetDlgItem(parentHWND, controlID);
			int num = 0 | controlID & 65535;
			AutoControl.SendMessage(parentHWND, 273, num, dlgItem);
		}

		public static void SendClickOnPosition(IntPtr controlHandle, int x, int y, EMouseKey mouseButton = 0, int clickTimes = 1)
		{
			int num = 0;
			int num1 = 0;
			if (mouseButton == EMouseKey.LEFT)
			{
				num = 513;
				num1 = 514;
			}
			if (mouseButton == EMouseKey.RIGHT)
			{
				num = 516;
				num1 = 517;
			}
			IntPtr intPtr = AutoControl.MakeLParamFromXY(x, y);
			if ((mouseButton == EMouseKey.LEFT ? false : mouseButton != EMouseKey.RIGHT))
			{
				if (mouseButton == EMouseKey.DOUBLE_LEFT)
				{
					num = 515;
					num1 = 514;
				}
				if (mouseButton == EMouseKey.DOUBLE_RIGHT)
				{
					num = 518;
					num1 = 517;
				}
				AutoControl.PostMessage(controlHandle, num, new IntPtr(1), intPtr);
				AutoControl.PostMessage(controlHandle, num1, new IntPtr(0), intPtr);
			}
			else
			{
				for (int i = 0; i < clickTimes; i++)
				{
					AutoControl.PostMessage(controlHandle, 6, new IntPtr(1), intPtr);
					AutoControl.PostMessage(controlHandle, num, new IntPtr(1), intPtr);
					AutoControl.PostMessage(controlHandle, num1, new IntPtr(0), intPtr);
				}
			}
		}

		public static void SendClickUpOnPosition(IntPtr controlHandle, int x, int y, EMouseKey mouseButton = 0, int clickTimes = 1)
		{
			int num = 0;
			if (mouseButton == EMouseKey.LEFT)
			{
				num = 514;
			}
			if (mouseButton == EMouseKey.RIGHT)
			{
				num = 517;
			}
			IntPtr intPtr = AutoControl.MakeLParamFromXY(x, y);
			for (int i = 0; i < clickTimes; i++)
			{
				AutoControl.PostMessage(controlHandle, 6, new IntPtr(1), intPtr);
				AutoControl.SendMessage(controlHandle, num, new IntPtr(0), intPtr);
			}
		}

		public static void SendDragAndDropOnPosition(IntPtr controlHandle, int x, int y, int x2, int y2, int stepx = 10, int stepy = 10, double delay = 0.05)
		{
			int num = 513;
			int num1 = 514;
			IntPtr intPtr = AutoControl.MakeLParamFromXY(x, y);
			IntPtr intPtr1 = AutoControl.MakeLParamFromXY(x2, y2);
			if (x2 < x)
			{
				stepx *= -1;
			}
			if (y2 < y)
			{
				stepy *= -1;
			}
			AutoControl.PostMessage(controlHandle, 6, new IntPtr(1), intPtr);
			AutoControl.PostMessage(controlHandle, num, new IntPtr(1), intPtr);
			bool flag = false;
			bool flag1 = false;
			while (true)
			{
				AutoControl.PostMessage(controlHandle, 512, new IntPtr(1), AutoControl.MakeLParamFromXY(x, y));
				if (stepx > 0)
				{
					if (x >= x2)
					{
						flag = true;
					}
					else
					{
						x += stepx;
					}
				}
				else if (x <= x2)
				{
					flag = true;
				}
				else
				{
					x += stepx;
				}
				if (stepy > 0)
				{
					if (y >= y2)
					{
						flag1 = true;
					}
					else
					{
						y += stepy;
					}
				}
				else if (y <= y2)
				{
					flag1 = true;
				}
				else
				{
					y += stepy;
				}
				if (flag & flag1)
				{
					break;
				}
				Thread.Sleep(TimeSpan.FromSeconds(delay));
			}
			AutoControl.PostMessage(controlHandle, num1, new IntPtr(0), intPtr1);
		}

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern uint SendInput(uint numberOfInputs, INPUT[] inputs, int sizeOfInputStructure);

		public static void SendKeyBoardDown(IntPtr handle, VKeys key)
		{
			AutoControl.PostMessage(handle, 6, new IntPtr(1), new IntPtr(0));
			AutoControl.PostMessage(handle, 256, new IntPtr((int)key), new IntPtr(0));
		}

		public static void SendKeyBoardPress(IntPtr handle, VKeys key)
		{
			AutoControl.PostMessage(handle, 6, new IntPtr(1), new IntPtr(0));
			AutoControl.PostMessage(handle, 256, new IntPtr((int)key), new IntPtr(1));
			AutoControl.PostMessage(handle, 257, new IntPtr((int)key), new IntPtr(0));
		}

		public static void SendKeyBoardPressStepByStep(IntPtr handle, string message, float delay = 0.1f)
		{
			string lower = message.ToLower();
			for (int i = 0; i < lower.Length; i++)
			{
				char chr = lower[i];
				VKeys vKey = VKeys.VK_0;
				char chr1 = chr;
				switch (chr1)
				{
					case '0':
					{
						vKey = VKeys.VK_0;
						break;
					}
					case '1':
					{
						vKey = VKeys.VK_1;
						break;
					}
					case '2':
					{
						vKey = VKeys.VK_2;
						break;
					}
					case '3':
					{
						vKey = VKeys.VK_3;
						break;
					}
					case '4':
					{
						vKey = VKeys.VK_4;
						break;
					}
					case '5':
					{
						vKey = VKeys.VK_5;
						break;
					}
					case '6':
					{
						vKey = VKeys.VK_6;
						break;
					}
					case '7':
					{
						vKey = VKeys.VK_7;
						break;
					}
					case '8':
					{
						vKey = VKeys.VK_8;
						break;
					}
					case '9':
					{
						vKey = VKeys.VK_9;
						break;
					}
					default:
					{
						switch (chr1)
						{
							case 'a':
							{
								vKey = VKeys.VK_A;
								break;
							}
							case 'b':
							{
								vKey = VKeys.VK_B;
								break;
							}
							case 'c':
							{
								vKey = VKeys.VK_V;
								break;
							}
							case 'd':
							{
								vKey = VKeys.VK_D;
								break;
							}
							case 'e':
							{
								vKey = VKeys.VK_E;
								break;
							}
							case 'f':
							{
								vKey = VKeys.VK_F;
								break;
							}
							case 'g':
							{
								vKey = VKeys.VK_G;
								break;
							}
							case 'h':
							{
								vKey = VKeys.VK_H;
								break;
							}
							case 'i':
							{
								vKey = VKeys.VK_I;
								break;
							}
							case 'j':
							{
								vKey = VKeys.VK_J;
								break;
							}
							case 'k':
							{
								vKey = VKeys.VK_K;
								break;
							}
							case 'l':
							{
								vKey = VKeys.VK_L;
								break;
							}
							case 'm':
							{
								vKey = VKeys.VK_M;
								break;
							}
							case 'n':
							{
								vKey = VKeys.VK_N;
								break;
							}
							case 'o':
							{
								vKey = VKeys.VK_O;
								break;
							}
							case 'p':
							{
								vKey = VKeys.VK_P;
								break;
							}
							case 'q':
							{
								vKey = VKeys.VK_Q;
								break;
							}
							case 'r':
							{
								vKey = VKeys.VK_R;
								break;
							}
							case 's':
							{
								vKey = VKeys.VK_S;
								break;
							}
							case 't':
							{
								vKey = VKeys.VK_T;
								break;
							}
							case 'u':
							{
								vKey = VKeys.VK_U;
								break;
							}
							case 'v':
							{
								vKey = VKeys.VK_V;
								break;
							}
							case 'w':
							{
								vKey = VKeys.VK_W;
								break;
							}
							case 'x':
							{
								vKey = VKeys.VK_X;
								break;
							}
							case 'y':
							{
								vKey = VKeys.VK_Y;
								break;
							}
							case 'z':
							{
								vKey = VKeys.VK_Z;
								break;
							}
						}
						break;
					}
				}
				AutoControl.SendKeyBoardPress(handle, vKey);
				Thread.Sleep(TimeSpan.FromSeconds((double)delay));
			}
		}

		public static void SendKeyBoardUp(IntPtr handle, VKeys key)
		{
			AutoControl.PostMessage(handle, 6, new IntPtr(1), new IntPtr(0));
			AutoControl.PostMessage(handle, 257, new IntPtr((int)key), new IntPtr(0));
		}

		public static void SendKeyChar(IntPtr handle, VKeys key)
		{
			AutoControl.PostMessage(handle, 6, new IntPtr(1), new IntPtr(0));
			AutoControl.PostMessage(handle, 258, new IntPtr((int)key), new IntPtr(0));
		}

		public static void SendKeyChar(IntPtr handle, int key)
		{
			AutoControl.PostMessage(handle, 6, new IntPtr(1), new IntPtr(0));
			AutoControl.PostMessage(handle, 258, new IntPtr(key), new IntPtr(0));
		}

		public static void SendKeyDown(KeyCode keyCode)
		{
			INPUT nPUT = new INPUT()
			{
				Type = 1
			};
			nPUT.Data.Keyboard = new KEYBDINPUT()
			{
				Vk = (ushort)keyCode,
				Scan = 0,
				Flags = 0,
				Time = 0,
				ExtraInfo = IntPtr.Zero
			};
			if (AutoControl.SendInput(1, new INPUT[] { nPUT }, Marshal.SizeOf(typeof(INPUT))) == 0)
			{
				throw new Exception();
			}
		}

		public static void SendKeyFocus(KeyCode key)
		{
			AutoControl.SendKeyPress(key);
		}

		public static void SendKeyPress(KeyCode keyCode)
		{
			INPUT nPUT = new INPUT()
			{
				Type = 1
			};
			INPUT nPUT1 = nPUT;
			KEYBDINPUT kEYBDINPUT = new KEYBDINPUT()
			{
				Vk = (ushort)keyCode,
				Scan = 0,
				Flags = 0,
				Time = 0,
				ExtraInfo = IntPtr.Zero
			};
			nPUT1.Data.Keyboard = kEYBDINPUT;
			nPUT = new INPUT()
			{
				Type = 1
			};
			INPUT nPUT2 = nPUT;
			kEYBDINPUT = new KEYBDINPUT()
			{
				Vk = (ushort)keyCode,
				Scan = 0,
				Flags = 2,
				Time = 0,
				ExtraInfo = IntPtr.Zero
			};
			nPUT2.Data.Keyboard = kEYBDINPUT;
			if (AutoControl.SendInput(2, new INPUT[] { nPUT1, nPUT2 }, Marshal.SizeOf(typeof(INPUT))) == 0)
			{
				throw new Exception();
			}
		}

		public static void SendKeyPressStepByStep(string message, double delay = 0.5)
		{
			for (int i = 0; i < message.Length; i++)
			{
				switch (message[i])
				{
					case '0':
					{
						AutoControl.SendKeyPress(KeyCode.KEY_0);
						break;
					}
					case '1':
					{
						AutoControl.SendKeyPress(KeyCode.KEY_1);
						break;
					}
					case '2':
					{
						AutoControl.SendKeyPress(KeyCode.KEY_2);
						break;
					}
					case '3':
					{
						AutoControl.SendKeyPress(KeyCode.KEY_3);
						break;
					}
					case '4':
					{
						AutoControl.SendKeyPress(KeyCode.KEY_4);
						break;
					}
					case '5':
					{
						AutoControl.SendKeyPress(KeyCode.KEY_5);
						break;
					}
					case '6':
					{
						AutoControl.SendKeyPress(KeyCode.KEY_6);
						break;
					}
					case '7':
					{
						AutoControl.SendKeyPress(KeyCode.KEY_7);
						break;
					}
					case '8':
					{
						AutoControl.SendKeyPress(KeyCode.KEY_8);
						break;
					}
					case '9':
					{
						AutoControl.SendKeyPress(KeyCode.KEY_9);
						break;
					}
				}
				Thread.Sleep(TimeSpan.FromSeconds(delay));
			}
		}

		public static void SendKeyUp(KeyCode keyCode)
		{
			INPUT nPUT = new INPUT()
			{
				Type = 1
			};
			nPUT.Data.Keyboard = new KEYBDINPUT()
			{
				Vk = (ushort)keyCode,
				Scan = 0,
				Flags = 2,
				Time = 0,
				ExtraInfo = IntPtr.Zero
			};
			if (AutoControl.SendInput(1, new INPUT[] { nPUT }, Marshal.SizeOf(typeof(INPUT))) == 0)
			{
				throw new Exception();
			}
		}

		[DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=false)]
		private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);

		[DllImport("user32.dll", CharSet=CharSet.Unicode, ExactSpelling=false)]
		public static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);

		public static void SendMultiKeysFocus(KeyCode[] keys)
		{
			KeyCode[] keyCodeArray = keys;
			for (int i = 0; i < (int)keyCodeArray.Length; i++)
			{
				AutoControl.SendKeyDown(keyCodeArray[i]);
			}
			KeyCode[] keyCodeArray1 = keys;
			for (int j = 0; j < (int)keyCodeArray1.Length; j++)
			{
				AutoControl.SendKeyUp(keyCodeArray1[j]);
			}
		}

		public static void SendStringFocus(string message)
		{
			Clipboard.SetText(message);
			AutoControl.SendMultiKeysFocus(new KeyCode[] { KeyCode.CONTROL, KeyCode.KEY_V });
		}

		public static void SendText(IntPtr handle, string text)
		{
			AutoControl.SendMessage(handle, 12, 0, text);
		}

		public static void SendTextKeyBoard(IntPtr handle, string text, float delay = 0.1f)
		{
			string lower = text.ToLower();
			for (int i = 0; i < lower.Length; i++)
			{
				AutoControl.SendKeyChar(handle, lower[i]);
			}
		}

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern bool SetDlgItemTextA(IntPtr hWnd, int nIDDlgItem, string gchar);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern bool SetForegroundWindow(IntPtr hWnd);

		public delegate bool CallBack(IntPtr hwnd, IntPtr lParam);

		private delegate bool EnumWindowProc(IntPtr hwnd, IntPtr lParam);

		[Flags]
		public enum MouseEventFlags : uint
		{
			MOVE = 1,
			XBUTTON1 = 1,
			LEFTDOWN = 2,
			XBUTTON2 = 2,
			LEFTUP = 4,
			RIGHTDOWN = 8,
			RIGHTUP = 16,
			MIDDLEDOWN = 32,
			MIDDLEUP = 64,
			XDOWN = 128,
			XUP = 256,
			WHEEL = 2048,
			ABSOLUTE = 32768
		}
	}
}