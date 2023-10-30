namespace easy.Helper
{
    public class ADBCommands
    {
        public static string VIEW = "shell am start -W -a android.intent.action.VIEW -d \"{0}\" {1}";

        public static string OPEN_LINK = "shell am start -n {0}";

        public static string PUT_HTTP_PROXY = "shell settings put global http_proxy {0}";

        public static string DELETE_HTTP_PROXY = "shell settings delete global http_proxy";

        public static string DELETE_HTTP_PROXY_HOST = "shell settings delete global global_http_proxy_host";

        public static string DELETE_HTTP_PROXY_PORT = "shell settings delete global global_http_proxy_port";

        public static string CURL = "shell curl {0}";

        public static string SCREENCAP = "shell screencap -p \"{0}\"";

        public static string CLEAR_DATA_APP = "shell pm clear {0}";

        public static string PUSH_FILE = "push \"{0}\" \"{1}\"";

        public static string PULL_FILE = "pull \"{0}\" \"{1}\"";

        public static string DELETE_FILE = "shell rm \"{0}\"";

        public static string DELETE_FOLDER = "shell rm -r \"{0}\"";

        public static string DUMP_SCREEN = "shell uiautomator dump {0}";

        public static string READ_FILE = "shell cat {0}";

        public static string IMPORT_CONTACT = "shell am start -t \"text/vcard\" -d \"file://{0}\" -a android.intent.action.VIEW com.android.contacts";

        public static string ZIP_FILE = "shell tar -cvzf {0} {1}";

        public static string UNZIP_FILE = "shell tar -xvzf {0} -C /";

        public static string INPUT_SWIPE = "shell input touchscreen swipe {0} {1} {2} {3} {4}";

        public static string INPUT_KEYEVENT = "shell input keyevent {0}";

        public static string INPUT_TEXT = "shell input text \"{0}\"";

        public static string TAP = "shell input tap {0} {1}";

        public static string SWITCH_ADBKEYBOARD = "shell ime set com.android.adbkeyboard/.AdbIME";

        public static string SWITCH_ANDROID_KEYBOARD = "shell ime set com.android.inputmethod.pinyin/.InputService";

        public static string DUMP_ACTIVITY = "shell \"dumpsys window windows | grep -E 'mCurrentFocus'\"";

        public static string OPEN_APP = "shell monkey -p {0} -c android.intent.category.LAUNCHER 1";

        public static string OPEN_APP_CHANGE = "shell monkey -p {0} -c android.intent.category.LEANBACK_LAUNCHER 1";

        public static string CLOSE_APP = "shell am force-stop {0}";

        public static string INSTALL_APP = "install {0}";

        public static string UNINSTALL_APP = "uninstall {0}";

        public static string LIST_PACKAGES = "shell pm list packages";

        public static string LIST_PACKAGES_USER_INSTALL = "shell pm list packages -3\" | cut - f 2 -d \":";
    }
}
