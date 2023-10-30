using System;
using System.Linq;
using System.Text.RegularExpressions;
using AE.Net.Mail;
using MCommon;

namespace maxcare.Helper
{
	// Token: 0x020002BE RID: 702
	internal class EmailHelper
	{
		// Token: 0x060017B5 RID: 6069 RVA: 0x0024F328 File Offset: 0x0024D528
		public static void DeleteMail(string username, string password)
		{
			int num = 0;
			for (;;)
			{
				try
				{
					string text = "outlook.office365.com";
					if (username.EndsWith("@hotmail.com") || username.EndsWith("@outlook.com"))
					{
						text = "outlook.office365.com";
					}
					else if (username.EndsWith("@yandex.com"))
					{
						text = "imap.yandex.com";
					}
					ImapClient imapClient = new ImapClient(text, username, password, AuthMethods.Login, 993, true, false);
					Lazy<MailMessage>[] array;
					if (text == "imap.yandex.com")
					{
						array = imapClient.SearchMessages(SearchCondition.Unseen(), false, false);
					}
					else
					{
						array = imapClient.SearchMessages(SearchCondition.From("security@facebookmail.com").And(new SearchCondition[]
						{
							SearchCondition.Unseen()
						}), false, false);
					}
					if (array.Length != 0)
					{
						for (int i = array.Length - 1; i >= 0; i--)
						{
							Lazy<MailMessage> lazy = array[i];
							imapClient.DeleteMessage(lazy.Value);
						}
					}
					if (imapClient.IsDisposed)
					{
						imapClient.Dispose();
					}
					if (imapClient.IsConnected)
					{
						imapClient.Disconnect();
					}
					break;
				}
				catch (Exception ex)
				{
					if (ex.ToString().Contains("The remote certificate is invalid according to the validation procedure"))
					{
						num++;
						if (num < 10)
						{
							continue;
						}
					}
					break;
				}
			}
		}

		// Token: 0x060017B6 RID: 6070 RVA: 0x0024F4C0 File Offset: 0x0024D6C0
		public static string GetOtpFromMail(int type, string username, string password, int timeout = 60)
		{
			int num = 0;
			int num2 = 10;
			string text = "outlook.office365.com";
			if (username.EndsWith("@hotmail.com") || username.EndsWith("@outlook.com"))
			{
				text = "outlook.office365.com";
			}
			else if (username.EndsWith("@yandex.com"))
			{
				text = "imap.yandex.com";
			}
			for (;;)
			{
				try
				{
					ImapClient imapClient = new ImapClient(text, username, password, AuthMethods.Login, 993, true, false);
					for (int i = 0; i < timeout; i++)
					{
						try
						{
							for (int j = 0; j < 2; j++)
							{
								if (text == "imap.yandex.com")
								{
									j = 1;
								}
								if (j == 0)
								{
									imapClient.SelectMailbox("Inbox");
								}
								else
								{
									imapClient.SelectMailbox("Spam");
								}
								int messageCount = imapClient.GetMessageCount();
								if (messageCount > 0)
								{
									Lazy<MailMessage>[] array;
									if (text == "imap.yandex.com")
									{
										array = imapClient.SearchMessages(SearchCondition.Unseen(), false, false);
									}
									else
									{
										array = imapClient.SearchMessages(SearchCondition.From("security@facebookmail.com").Or(new SearchCondition[]
										{
											SearchCondition.From("registration@facebookmail.com")
										}).And(new SearchCondition[]
										{
											SearchCondition.Unseen()
										}), false, false);
									}
									if (array.Length != 0)
									{
										for (int k = array.Count<Lazy<MailMessage>>() - 1; k >= 0; k--)
										{
											string input = array[k].Value.Body.ToString();
											string text2 = "";
											switch (type)
											{
											case 0:
												text2 = Regex.Match(input, "https://www.facebook.com/confirmcontact.php(.*?)\n").Value.Trim();
												break;
											case 1:
												text2 = Regex.Match(input, "\\d{8}").Value.Trim();
												break;
											case 2:
												text2 = Regex.Match(input, "https://www.facebook.com/n/\\?confirmemail.php(.*?)\n").Value.Trim();
												break;
											}
											if (text2 != "")
											{
												if (imapClient.IsDisposed)
												{
													imapClient.Dispose();
												}
												if (imapClient.IsConnected)
												{
													imapClient.Disconnect();
												}
												return text2;
											}
										}
									}
								}
							}
						}
						catch
						{
						}
						Common.DelayTime(1.0);
					}
					if (imapClient.IsDisposed)
					{
						imapClient.Dispose();
					}
					if (imapClient.IsConnected)
					{
						imapClient.Disconnect();
					}
					break;
				}
				catch (Exception ex)
				{
					if (ex.ToString().ToLower().Contains("blocked"))
					{
						return "block";
					}
					num++;
					if (num >= num2)
					{
						break;
					}
				}
			}
			return "";
		}
	}
}
