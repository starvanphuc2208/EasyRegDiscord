namespace easy.Helper
{
    using System.Windows.Forms;

    public class MessageBoxHelper
    {
        public static void ShowMessageBox(object s, int type = 1)
        {
            switch (type)
            {
                case 1:
                    MessageBox.Show(s.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    break;
                case 2:
                    MessageBox.Show(s.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    break;
                case 3:
                    MessageBox.Show(s.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    break;
            }
        }

        public static DialogResult ShowMessageBoxWithQuestion(string content)
        {
            return MessageBox.Show(content, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }
    }
}
