namespace MCommon
{
    using easy.Helper;
    using easy.UI;
    using EasySignupFB.Library.UiAutomation;
    using Emgu.CV;
    using Emgu.CV.CvEnum;
    using Emgu.CV.Structure;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Windows.Forms;
    using System.Xml;
    using System.IO.Compression;

    public class Device
    {
        public string DeviceId { get; set; }

        public string PathLDPlayer { get; set; }

        public Process process { get; set; }

        private Random rd { get; set; }

        public Device(string pathLDPlayer, string indexDevice)
        {
            this.IndexDevice = Convert.ToInt32(indexDevice);
            this.PathLDPlayer = pathLDPlayer;
            this.rd = new Random();
        }

        public void GrantPermissionLite()
        {
            this.ExecuteCMD("shell pm grant com.facebook.lite android.permission.READ_CALENDAR", 10);
            this.ExecuteCMD("shell pm grant com.facebook.lite android.permission.READ_CONTACTS", 10);
            this.ExecuteCMD("shell pm grant com.facebook.lite android.permission.READ_LOCATION", 10);
            this.ExecuteCMD("shell pm grant com.facebook.lite android.permission.ACCESS_BACKGROUND_LOCATION", 10);
            this.ExecuteCMD("shell pm grant com.facebook.lite android.permission.ACCESS_COARSE_LOCATION", 10);
            this.ExecuteCMD("shell pm grant com.facebook.lite android.permission.ACCESS_FINE_LOCATION", 10);
            this.ExecuteCMD("shell pm grant com.facebook.lite android.permission.RECORD_AUDIO", 10);
            this.ExecuteCMD("shell pm grant com.facebook.lite android.permission.CALL_PHONE", 10);
            this.ExecuteCMD("shell pm grant com.facebook.lite android.permission.READ_EXTERNAL_STORAGE", 10);
            this.ExecuteCMD("shell pm grant com.facebook.lite android.permission.WRITE_EXTERNAL_STORAGE", 10);
        }

        public void GrantPermission()
        {
            this.ExecuteCMD("shell pm grant org.telegram.messenger android.permission.CAMERA", 10);
            this.ExecuteCMD("shell pm grant org.telegram.messenger android.permission.READ_CALL_LOG", 10);
            this.ExecuteCMD("shell pm grant org.telegram.messenger android.permission.WRITE_EXTERNAL_STORAGE", 10);
            this.ExecuteCMD("shell pm grant org.telegram.messenger android.permission.CALL_PHONE", 10);
            this.ExecuteCMD("shell pm grant org.telegram.messenger android.permission.READ_CONTACTS", 10);
            this.ExecuteCMD("shell pm grant org.telegram.messenger android.permission.READ_PHONE_STATE", 10);
            this.ExecuteCMD("shell pm grant org.telegram.messenger android.permission.RECORD_AUDIO", 10);
            this.ExecuteCMD("shell pm grant org.telegram.messenger android.permission.READ_CALL_LOG", 10);
        }

        public void ChangeHardwareLDPlayer()
        {
            List<string> list = new List<string>
            {
                "xiaomi 6|xiaomi",
                "google Pixel 2|google",
                "xiaomi 8|xiaomi",
                "huawei Honor V9|huawei",
                "vivo X9 Plus|vivo",
                "oppo r11|oppo",
                "oppo r11 plus|oppo",
                "oppo R11 Plus|oppo",
                "oppo R17 Pro|oppo",
                "meizu PRO 7 Plus|meizu",
                "meizu PRO 6 Plus|meizu",
                "xiaomi mix|xiaomi",
                "mi 3|xiaomi"
            };
            Random random = new Random();
            string text = list[random.Next(0, list.Count - 1)];
            string text2 = text.Split(new char[]
            {
                '|'
            })[0];
            string text3 = text.Split(new char[]
            {
                '|'
            })[1];
            string str = "+849";
            string text4 = str + random.Next(10000000, 99999999).ToString();
            Process process = new Process();
            string arguments = string.Concat(new object[]
            {
                "modify --index ",
                this.IndexDevice,
                " --imei auto --model \"",
                text2,
                "\" --manufacturer ",
                text3,
                " --pnumber ",
                text4
            });
            process.StartInfo = new ProcessStartInfo
            {
                FileName = this.PathLDPlayer + "\\dnconsole.exe",
                Arguments = arguments,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };
            process.Start();
            process.WaitForExit(5000);
        }

        private List<string> GetListInfoDevice()
        {
            new List<string>();
            string text = Common.Base64Decode("QXN1c3xBbGNhdGVsIFBpeGkgNCAoNSkqQXN1c3xBc3VzIFJPRyBQaG9uZSpBc3VzfEFzdXMgWmVuZm9uZSAyIExhc2VyKkFzdXN8QXN1cyBaZW5Gb25lIDMqQXN1c3xBc3VzIFplbmZvbmUgMypBc3VzfEFzdXMgWmVuZm9uZSAzIERlbHV4ZSA1LjUqQXN1c3xBc3VzIFplbmZvbmUgMyBMYXNlcipBc3VzfEFzdXMgWmVuZm9uZSAzIE1heCpBc3VzfEFzdXMgWmVuZm9uZSAzIFVsdHJhKkFzdXN8QXN1cyBaZW5mb25lIDMgWm9vbSpBc3VzfEFzdXMgWmVuZm9uZSAzcyBNYXgqQXN1c3xBc3VzIFplbkZvbmUgNCAoWkU1NTRLTCkqQXN1c3xBc3VzIFplbkZvbmUgNCBNYXgqQXN1c3xBc3VzIFplbkZvbmUgNCBNYXggUHJvIChaQzU1NEtMKSpBc3VzfEFzdXMgWmVuRm9uZSA0IFBybypBc3VzfEFzdXMgWmVuRm9uZSA0IFNlbGZpZSAoWkQ1NTNLTCkqQXN1c3xBc3VzIFplbkZvbmUgNCBTZWxmaWUgUHJvIChaRDU1MktMKSpBc3VzfEFzdXMgWmVuZm9uZSA1KkFzdXN8QXN1cyBaZW5mb25lIDUgTGl0ZSpBc3VzfEFzdXMgWmVuZm9uZSA1USpBc3VzfEFzdXMgWmVuZm9uZSA1WipBc3VzfEFzdXMgWmVuZm9uZSBBUipBc3VzfEFzdXMgWmVuZm9uZSBHbypBc3VzfEFzdXMgWmVuZm9uZSBHbyAoWkI1NTJLTCkqQXN1c3xBc3VzIFplbkZvbmUgTGl2ZSAoTDEpIFpBNTUwS0wqQXN1c3xBc3VzIFplbmZvbmUgTGl2ZSAoWkI1MDFLTCkqQXN1c3xBc3VzIFplbmZvbmUgTWF4KkFzdXN8QXN1cyBaZW5mb25lIE1heCAoTTEpIFpCNTU1S0wqQXN1c3xBc3VzIFplbmZvbmUgTWF4IFBsdXMgKE0xKSBaQjU3MFRMKkFzdXN8QXN1cyBaZW5mb25lIE1heCBQcm8gKE0xKSBaQjYwMUtMKkFzdXN8QXN1cyBaZW5mb25lIE1heCBQcm8gTTEqQXN1c3xBc3VzIFplbkZvbmUgTWF4IFBybyBNMSpBc3VzfEFzdXMgWmVuZm9uZSBWIFY1MjBLTCpBc3VzfEFzdXMgWmVuUGFkIDNzIDEwKkFzdXN8QXN1cyBaZW5QYWQgM3MgOC4wKkFzdXN8QXN1cyBaZW5QYWQgWjEwKkFzdXN8QXN1cyBaZW5wYWQgWjhzIChaVDU4MktMKSpBc3VzfEJsdWJvbyBENipBc3VzfEJRIEFxdWFyaXMgTTUqQXN1c3xEb29nZWUgWDUgTWF4KkFzdXN8RWxlcGhvbmUgVSBQcm8qQXN1c3xFc3NlbnRpYWwgUGhvbmUgUEgtMSpBc3VzfEdlbmVyYWwgTW9iaWxlIEdNIDUqR29vZ2xlfEdvb2dsZSBOZXh1cyAxMCpHb29nbGV8R29vZ2xlIE5leHVzIDQqR29vZ2xlfEdvb2dsZSBOZXh1cyA1Kkdvb2dsZXxHb29nbGUgTmV4dXMgNipHb29nbGV8R29vZ2xlIE5leHVzIDZQKkdvb2dsZXxHb29nbGUgUGl4ZWwqR29vZ2xlfEdvb2dsZSBQaXhlbCAyKkdvb2dsZXxHb29nbGUgUGl4ZWwgMiBYTCpHb29nbGV8R29vZ2xlIFBpeGVsIEMqR29vZ2xlfEdvb2dsZSBQaXhlbCBYTCpIVEN8SFRDIDEwKkhUQ3xIVEMgMTAgRXZvKkhUQ3xIVEMgMTAgTGlmZXN0eWxlKkhUQ3xIVEMgRGVzaXJlIDEwIExpZmVzdHlsZSpIVEN8SFRDIERlc2lyZSAxMCBQcm8qSFRDfEhUQyBEZXNpcmUgMTIqSFRDfEhUQyBEZXNpcmUgMTIrKkhUQ3xIVEMgT25lIE04KkhUQ3xIVEMgT25lIE05KkhUQ3xIVEMgVSBQbGF5KkhUQ3xIVEMgVSBVbHRyYSpIVEN8SFRDIFUxMSpIVEN8SFRDIFUxMSBFeWVzKkhUQ3xIVEMgVTExIExpZmUqSFRDfEhUQyBVMTErKkhUQ3xIVEMgVTEyIGxpZmUqSFRDfEhUQyBVMTIrKkh1YXdlaXxIdWF3ZWkgR1IzICgyMDE3KSpIdWF3ZWl8SHVhd2VpIEhvbm9yIDEwKkh1YXdlaXxIdWF3ZWkgSG9ub3IgNlgqSHVhd2VpfEh1YXdlaSBIb25vciA3QSpIdWF3ZWl8SHVhd2VpIEhvbm9yIDdzKkh1YXdlaXxIdWF3ZWkgSG9ub3IgN1gqSHVhd2VpfEh1YXdlaSBIb25vciA4IExpdGUqSHVhd2VpfEh1YXdlaSBIb25vciA4IFBybypIdWF3ZWl8SHVhd2VpIEhvbm9yIDkqSHVhd2VpfEh1YXdlaSBIb25vciA5IExpdGUqSHVhd2VpfEh1YXdlaSBIb25vciA5TiAoOWkpKkh1YXdlaXxIdWF3ZWkgSG9ub3IgTm90ZSAxMCpIdWF3ZWl8SHVhd2VpIEhvbm9yIFBsYXkqSHVhd2VpfEh1YXdlaSBIb25vciBWaWV3IDEwKkh1YXdlaXxIdWF3ZWkgTWF0ZSAxMCpIdWF3ZWl8SHVhd2VpIE1hdGUgMTAgTGl0ZSpIdWF3ZWl8SHVhd2VpIE1hdGUgMTAgUHJvKkh1YXdlaXxIdWF3ZWkgTWF0ZSAyMCBMaXRlKkh1YXdlaXxIdWF3ZWkgTWF0ZSA4Kkh1YXdlaXxIdWF3ZWkgTWF0ZSA5Kkh1YXdlaXxIdWF3ZWkgTWF0ZSA5IFBvcnNjaGUgRGVzaWduKkh1YXdlaXxIdWF3ZWkgTWF0ZSA5IFBybypIdWF3ZWl8SHVhd2VpIE5vdmEgMipIdWF3ZWl8SHVhd2VpIE5vdmEgMiBQbHVzKkh1YXdlaXxIdWF3ZWkgTm92YSAyaSpIdWF3ZWl8SHVhd2VpIG5vdmEgMypIdWF3ZWl8SHVhd2VpIE5vdmEgM2UqSHVhd2VpfEh1YXdlaSBub3ZhIDNpKkh1YXdlaXxIdWF3ZWkgTm92YSBMaXRlKkh1YXdlaXxIdWF3ZWkgUCBzbWFydCpIdWF3ZWl8SHVhd2VpIFAgU21hcnQrKkh1YXdlaXxIdWF3ZWkgUDEwKkh1YXdlaXxIdWF3ZWkgUDEwIExpdGUqSHVhd2VpfEh1YXdlaSBQMTAgUGx1cypIdWF3ZWl8SHVhd2VpIFAyMCpIdWF3ZWl8SHVhd2VpIFAyMCBMaXRlKkh1YXdlaXxIdWF3ZWkgUDIwIFBybypIdWF3ZWl8SHVhd2VpIFA4IExpdGUgKDIwMTcpKkh1YXdlaXxIdWF3ZWkgUDggTGl0ZSAyMDE3Kkh1YXdlaXxIdWF3ZWkgUDkgTGl0ZSpIdWF3ZWl8SHVhd2VpIFA5IExpdGUgKDIwMTcpKkh1YXdlaXxIdWF3ZWkgUDkgTGl0ZSAyMDE3Kkh1YXdlaXxIdWF3ZWkgWTMgKDIwMTgpKkh1YXdlaXxIdWF3ZWkgWTUgUHJpbWUgKDIwMTgpKkh1YXdlaXxIdWF3ZWkgWTYgKDIwMTgpKkh1YXdlaXxIdWF3ZWkgWTcgKDIwMTgpKkh1YXdlaXxIdWF3ZWkgWTcgUHJpbWUqSHVhd2VpfEh1YXdlaSBZNyBQcmltZSAyMDE4Kkh1YXdlaXxIdWF3ZWkgWTcgUHJvICgyMDE4KSpIdWF3ZWl8SVVOSSBVMipMZUVjb3xMZUVjbyBMZSAxcypMZUVjb3xMZUVjbyBMZSAyKkxlRWNvfExlRWNvIExlIE1heCAyKkxlRWNvfExlRWNvIExlIFBybyAzKkxlbm92b3xMZW5vdm8gQTUqTGVub3ZvfExlbm92byBBNjAwMCpMZW5vdm98TGVub3ZvIEE2MDAwIFBsdXMqTGVub3ZvfExlbm92byBBNjYwMCBQbHVzKkxlbm92b3xMZW5vdm8gSzMyMHQqTGVub3ZvfExlbm92byBLNSpMZW5vdm98TGVub3ZvIEs1IE5vdGUgKDIwMTgpKkxlbm92b3xMZW5vdm8gSzUgcGxheSpMZW5vdm98TGVub3ZvIEs2Kkxlbm92b3xMZW5vdm8gSzYgTm90ZSpMZW5vdm98TGVub3ZvIEs2IFBvd2VyKkxlbm92b3xMZW5vdm8gSzgqTGVub3ZvfExlbm92byBLOCBOb3RlKkxlbm92b3xMZW5vdm8gSzggUGx1cypMZW5vdm98TGVub3ZvIFAyKkxlbm92b3xMZW5vdm8gUzUqTGVub3ZvfExlbm92byBaNSpMZW5vdm98TGVub3ZvIFp1ayBFZGdlKkxlbm92b3xMZW5vdm8gWnVrIFoxKkxlbm92b3xMZW5vdm8gWnVrIFoyKkxlbm92b3xMZW5vdm8gWlVLIFoyIChQbHVzKSpMZW5vdm98TGVub3ZvIFp1ayBaMiBQcm8qTEd8TEcgQXJpc3RvIDIqTEd8TEcgRzIqTEd8TEcgRzMqTEd8TEcgRzUqTEd8TEcgRzYqTEd8TEcgRzcgRml0KkxHfExHIEc3IE9uZSpMR3xMRyBHNyBUaGluUSpMR3xMRyBHNyBUaGluUSBQbHVzKkxHfExHIEsxMCAyMDE4KkxHfExHIEsxMSBQbHVzKkxHfExHIEszMCpMR3xMRyBLOCAyMDE4KkxHfExHIE5leHVzIDVYKkxHfExHIFBhZCBJViA4LjAqTEd8TEcgUSBTdHlsdXMqTEd8TEcgUTYqTEd8TEcgUTcqTEd8TEcgUTgqTEd8TEcgVjEwKkxHfExHIFYyMCpMR3xMRyBWMzAqTEd8TEcgVjMwUyBUaGluUSpMR3xMRyBWMzUgVGhpblEqTEd8TEcgWCBWZW50dXJlKkxHfExHIFpvbmUgNCpNb3Rvcm9sYXxNb3RvIEMqTW90b3JvbGF8TW90byBFIDIwMTUqTW90b3JvbGF8TW90byBFNCpNb3Rvcm9sYXxNb3RvIEU0IFBsdXMqTW90b3JvbGF8TW90byBFNSpNb3Rvcm9sYXxNb3RvIEU1IFBsYXkqTW90b3JvbGF8TW90byBFNSBQbGF5IEdvKk1vdG9yb2xhfE1vdG8gRTUgUGx1cypNb3Rvcm9sYXxNb3RvIEcgMjAxMypNb3Rvcm9sYXxNb3RvIEcgMjAxNCpNb3Rvcm9sYXxNb3RvIEcgMjAxNSpNb3Rvcm9sYXxNb3RvIEc0Kk1vdG9yb2xhfE1vdG8gRzQgUGx1cypNb3Rvcm9sYXxNb3RvIEc1Kk1vdG9yb2xhfE1vdG8gRzUgUGx1cypNb3Rvcm9sYXxNb3RvIEc1UypNb3Rvcm9sYXxNb3RvIEc1UyBQbHVzKk1vdG9yb2xhfE1vdG8gRzYqTW90b3JvbGF8TW90byBHNiBQbGF5Kk1vdG9yb2xhfE1vdG8gRzYgUGx1cypNb3Rvcm9sYXxNb3RvIE0qTW90b3JvbGF8TW90byBYIFB1cmUqTW90b3JvbGF8TW90byBYNCpNb3Rvcm9sYXxNb3RvIFoqTW90b3JvbGF8TW90byBaIEZvcmNlKk1vdG9yb2xhfE1vdG8gWiBQbGF5Kk1vdG9yb2xhfE1vdG8gWjIgRm9yY2UqTW90b3JvbGF8TW90byBaMiBQbGF5Kk1vdG9yb2xhfE1vdG8gWjMqTW90b3JvbGF8TW90byBaMyBQbGF5Kk1vdG9yb2xhfE1vdG9yb2xhIE1vdG8gRTQqTW90b3JvbGF8TW90b3JvbGEgTW90byBHNiBQbHVzKk1vdG9yb2xhfE1vdG9yb2xhIE1vdG8gWCBQbGF5Kk1vdG9yb2xhfE1vdG9yb2xhIE9uZSBQb3dlcipNb3Rvcm9sYXxNb3Rvcm9sYSBQMzAqTW90b3JvbGF8TmV4dXMgNVgqTW90b3JvbGF8TmV4dXMgNlAqTW90b3JvbGF8TmV4dXMgUGxheWVyKk5va2lhfE5va2lhIDEqTm9raWF8Tm9raWEgMipOb2tpYXxOb2tpYSAyLjEqTm9raWF8Tm9raWEgMypOb2tpYXxOb2tpYSAzLjEqTm9raWF8Tm9raWEgNSpOb2tpYXxOb2tpYSA1LjEqTm9raWF8Tm9raWEgNS4xIFBsdXMqTm9raWF8Tm9raWEgNipOb2tpYXxOb2tpYSA2LjEqTm9raWF8Tm9raWEgNi4xIFBsdXMqTm9raWF8Tm9raWEgNypOb2tpYXxOb2tpYSA3IFBsdXMqTm9raWF8Tm9raWEgNy4xKk5va2lhfE5va2lhIDgqTm9raWF8Tm9raWEgOCBTaXJvY2NvKk5va2lhfE5va2lhIFg1Kk5va2lhfE5va2lhIFg2Kk5va2lhfE51YmlhIFoxNypPbmVQbHVzfE9uZVBsdXMgMipPbmVQbHVzfE9uZVBsdXMgMypPbmVQbHVzfE9uZVBsdXMgM1QqT25lUGx1c3xPbmVQbHVzIDUqT25lUGx1c3xPbmVQbHVzIDUvNVQqT25lUGx1c3xPbmVQbHVzIDVUKk9uZVBsdXN8T25lUGx1cyBPbmUqT25lUGx1c3xPbmVQbHVzIFgqUmVkbWl8UmVkbWkgM3MvM3gvUHJpbWUqUmVkbWl8UmVkbWkgNCBQcmltZSpSZWRtaXxSZWRtaSA0WCpSZWRtaXxSZWRtaSA1IFBsdXMqUmVkbWl8UmVkbWkgTm90ZSA1KlNhbXN1bmd8U2Ftc3VuZyBHYWxheHkgQTMgKDIwMTcpKlNhbXN1bmd8U2Ftc3VuZyBHYWxheHkgQTUgKDIwMTYpKlNhbXN1bmd8U2Ftc3VuZyBHYWxheHkgQTUgKDIwMTcpKlNhbXN1bmd8U2Ftc3VuZyBHYWxheHkgQTYgMjAxOCpTYW1zdW5nfFNhbXN1bmcgR2FsYXh5IEE2KyAyMDE4KlNhbXN1bmd8U2Ftc3VuZyBHYWxheHkgQTcgKDIwMTYpKlNhbXN1bmd8U2Ftc3VuZyBHYWxheHkgQTcgKDIwMTcpKlNhbXN1bmd8U2Ftc3VuZyBHYWxheHkgQTggKDIwMTYpKlNhbXN1bmd8U2Ftc3VuZyBHYWxheHkgQTggKDIwMTcpKlNhbXN1bmd8U2Ftc3VuZyBHYWxheHkgQTggMjAxOCpTYW1zdW5nfFNhbXN1bmcgR2FsYXh5IEE4IFN0YXIqU2Ftc3VuZ3xTYW1zdW5nIEdhbGF4eSBBOCsgMjAxOCpTYW1zdW5nfFNhbXN1bmcgR2FsYXh5IEE5IFBybypTYW1zdW5nfFNhbXN1bmcgR2FsYXh5IEE5IFN0YXIqU2Ftc3VuZ3xTYW1zdW5nIEdhbGF4eSBBbHBoYSpTYW1zdW5nfFNhbXN1bmcgR2FsYXh5IEM3IFBybypTYW1zdW5nfFNhbXN1bmcgR2FsYXh5IEM5IFBybypTYW1zdW5nfFNhbXN1bmcgR2FsYXh5IEdyYW5kIFByaW1lKlNhbXN1bmd8U2Ftc3VuZyBHYWxheHkgSipTYW1zdW5nfFNhbXN1bmcgR2FsYXh5IEoyIENvcmUqU2Ftc3VuZ3xTYW1zdW5nIEdhbGF4eSBKMiBQcm8gKDIwMTgpKlNhbXN1bmd8U2Ftc3VuZyBHYWxheHkgSjMgKDIwMTgpKlNhbXN1bmd8U2Ftc3VuZyBHYWxheHkgSjQgKDIwMTgpKlNhbXN1bmd8U2Ftc3VuZyBHYWxheHkgSjUqU2Ftc3VuZ3xTYW1zdW5nIEdhbGF4eSBKNSAoMjAxNykqU2Ftc3VuZ3xTYW1zdW5nIEdhbGF4eSBKNiAoMjAxOCkqU2Ftc3VuZ3xTYW1zdW5nIEdhbGF4eSBKNyAoMjAxOCkqU2Ftc3VuZ3xTYW1zdW5nIEdhbGF4eSBKNyBEdW8qU2Ftc3VuZ3xTYW1zdW5nIEdhbGF4eSBKNyBNYXggKDIwMTcpKlNhbXN1bmd8U2Ftc3VuZyBHYWxheHkgSjcgUHJpbWUqU2Ftc3VuZ3xTYW1zdW5nIEdhbGF4eSBKNyBQcmltZSAyKlNhbXN1bmd8U2Ftc3VuZyBHYWxheHkgSjcgUHJvICgyMDE3KSpTYW1zdW5nfFNhbXN1bmcgR2FsYXh5IEo3IFYqU2Ftc3VuZ3xTYW1zdW5nIEdhbGF4eSBKOCAoMjAxOCkqU2Ftc3VuZ3xTYW1zdW5nIEdhbGF4eSBNZWdhIDYuMypTYW1zdW5nfFNhbXN1bmcgR2FsYXh5IE5vdGUgMypTYW1zdW5nfFNhbXN1bmcgR2FsYXh5IE5vdGUgOCpTYW1zdW5nfFNhbXN1bmcgR2FsYXh5IE5vdGUgOSpTYW1zdW5nfFNhbXN1bmcgR2FsYXh5IE5vdGUgRkUqU2Ftc3VuZ3xTYW1zdW5nIEdhbGF4eSBPbjYqU2Ftc3VuZ3xTYW1zdW5nIEdhbGF4eSBTMipTYW1zdW5nfFNhbXN1bmcgR2FsYXh5IFM0KlNhbXN1bmd8U2Ftc3VuZyBHYWxheHkgUzQgTWluaSpTYW1zdW5nfFNhbXN1bmcgR2FsYXh5IFM1IFtrbHRlXSpTYW1zdW5nfFNhbXN1bmcgR2FsYXh5IFM2KlNhbXN1bmd8U2Ftc3VuZyBHYWxheHkgUzcqU2Ftc3VuZ3xTYW1zdW5nIEdhbGF4eSBTNyBBY3RpdmUqU2Ftc3VuZ3xTYW1zdW5nIEdhbGF4eSBTNyBFZGdlKlNhbXN1bmd8U2Ftc3VuZyBHYWxheHkgUzgqU2Ftc3VuZ3xTYW1zdW5nIEdhbGF4eSBTOCBBY3RpdmUqU2Ftc3VuZ3xTYW1zdW5nIEdhbGF4eSBTOCBQbHVzKlNhbXN1bmd8U2Ftc3VuZyBHYWxheHkgUzkqU2Ftc3VuZ3xTYW1zdW5nIEdhbGF4eSBTOSBQbHVzKlNhbXN1bmd8U2Ftc3VuZyBHYWxheHkgVGFiIEEgMTAuNSpTYW1zdW5nfFNhbXN1bmcgR2FsYXh5IFRhYiBBIDkuNypTYW1zdW5nfFNhbXN1bmcgR2FsYXh5IFRhYiBFIDkuNipTYW1zdW5nfFNhbXN1bmcgR2FsYXh5IFRhYiBTMypTYW1zdW5nfFNhbXN1bmcgR2FsYXh5IFRhYiBTNCAxMC41KlNvbnl8U29ueSBYcGVyaWEgRTUqU29ueXxTb255IFhwZXJpYSBMMSpTb255fFNvbnkgWHBlcmlhIFIxIChQbHVzKSpTb255fFNvbnkgWHBlcmlhIFRvdWNoKlNvbnl8U29ueSBYcGVyaWEgWCpTb255fFNvbnkgWHBlcmlhIFggQ29tcGFjdCpTb255fFNvbnkgWHBlcmlhIFggUGVyZm9ybWFuY2UqU29ueXxTb255IFhwZXJpYSBYQSpTb255fFNvbnkgWHBlcmlhIFhBIFVsdHJhKlNvbnl8U29ueSBYcGVyaWEgWEExKlNvbnl8U29ueSBYcGVyaWEgWEExIFBsdXMqU29ueXxTb255IFhwZXJpYSBYQTEgVWx0cmEqU29ueXxTb255IFhwZXJpYSBYQTIqU29ueXxTb255IFhwZXJpYSBYQTIgUGx1cypTb255fFNvbnkgWHBlcmlhIFhBMiBVbHRyYSpTb255fFNvbnkgWHBlcmlhIFhaKlNvbnl8U29ueSBYcGVyaWEgWFogUHJlbWl1bSpTb255fFNvbnkgWHBlcmlhIFhaMSpTb255fFNvbnkgWHBlcmlhIFhaMSBDb21wYWN0KlNvbnl8U29ueSBYcGVyaWEgWFoyKlNvbnl8U29ueSBYcGVyaWEgWFoyIENvbXBhY3QqU29ueXxTb255IFhwZXJpYSBYWjIgUHJlbWl1bSpTb255fFNvbnkgWHBlcmlhIFhacypTb255fFNvbnkgWHBlcmlhIFoxKlNvbnl8U29ueSBYcGVyaWEgWjUqU29ueXxTb255IFhwZXJpYSBaNSBQcmVtaXVtKlNvbnl8U3ByaW50IEdhbGF4eSBUYWIgRSA4LjAqVml2b3x2aXZvIE5FWCpWaXZvfHZpdm8gTkVYIEEqVml2b3x2aXZvIE5FWCBTKlZpdm98dml2byBWMTEqVml2b3x2aXZvIFYxMSBQcm8qVml2b3x2aXZvIFYxMWkqVml2b3x2aXZvIFY3KlZpdm98dml2byBWNyBQbHVzKlZpdm98dml2byBWOSpWaXZvfHZpdm8gVjkgWW91dGgqVml2b3x2aXZvIFgyMCpWaXZvfHZpdm8gWDIwIFBsdXMqVml2b3x2aXZvIFgyMCBVRCpWaXZvfHZpdm8gWDIxKlZpdm98dml2byBYMjEgVUQqVml2b3x2aXZvIFgyMWkqVml2b3x2aXZvIFgyMypWaXZvfFZpdm8gWDkqVml2b3xWaXZvIFg5IFBsdXMqVml2b3xWaXZvIFg5cypWaXZvfFZpdm8gWDlzIFBsdXMqVml2b3x2aXZvIFk1M2kqVml2b3x2aXZvIFk3MSpWaXZvfHZpdm8gWTcxaSpWaXZvfHZpdm8gWTgxKlZpdm98dml2byBZODMqVml2b3x2aXZvIFk4MyBQcm8qVml2b3x2aXZvIFk5NypWaXZvfHZpdm8gWjEqVml2b3x2aXZvIFoxaSpWaXZvfFdpbGV5Zm94IFN3aWZ0KlhpYW9taXxYaWFvbWkgTWkgMypYaWFvbWl8WGlhb21pIE1pIDQqWGlhb21pfFhpYW9taSBNaSA1KlhpYW9taXxYaWFvbWkgTWkgNipYaWFvbWl8WGlhb21pIE1pIEExKlhpYW9taXxYaWFvbWkgTWkgQTIgTGl0ZSpYaWFvbWl8WGlhb21pIE1pIE1heCpYaWFvbWl8WGlhb21pIFBvY28gRjEqWGlhb21pfFhpYW9taSBSZWRtaSAyKlhpYW9taXxYaWFvbWkgUmVkbWkgNCBQcmltZSpYaWFvbWl8WGlhb21pIFJlZG1pIDRYKlhpYW9taXxYaWFvbWkgUmVkbWkgTm90ZSAzKlhpYW9taXxYaWFvbWkgUmVkbWkgTm90ZSA0KlhpYW9taXxYaWFvbWkgUmVkbWkgTm90ZSA1KlhpYW9taXxYaWFvbWkgUmVkbWkgTm90ZSA1IFBsdXMqWGlhb21pfFhpYW9taSBSZWRtaSBOb3RlIDUgUHJvKllVfFl1IEFjZSpZVXxZdSBZdW5pY29ybipZVXxZVSBZdW5pcXVlKllVfFl1IFl1bmlxdWUgMipZVXxZVSBZdXBob3JpYSpZVXxZVSBZdXJla2EqWVV8WVUgWXVyZWthIEJsYWNrKllVfFl1IFl1cmVrYSBOb3RlKllVfFl1IFl1cmVrYSBTKlpURXxaVEUgQXhvbiA3KlpURXxaVEUgQXhvbiA3IE1pbmkqWlRFfFpURSBBeG9uIDdzKlpURXxaVEUgQXhvbiA5IFBybypaVEV8WlRFIEF4b24gRWxpdGUqWlRFfFpURSBBeG9uIE1pbmkqWlRFfFpURSBBeG9uIFBybypaVEV8WlRFIEJsYWRlIEEzKlpURXxaVEUgQmxhZGUgQTYqWlRFfFpURSBCbGFkZSBWNypaVEV8WlRFIEJsYWRlIFY4KlpURXxaVEUgQmxhZGUgVjkqWlRFfFpURSBCbGFkZSBWOSBNaW5pKlpURXxaVEUgTWF2ZW4gMipaVEV8WlRFIE1heCBYTCpaVEV8WlRFIE51YmlhIFoxNypaVEV8WlRFIFRlbXBvIEdvKnhpYW9taXx4aWFvbWkgNipnb29nbGV8Z29vZ2xlIFBpeGVsIDIqeGlhb21pfHhpYW9taSA4Kmh1YXdlaXxodWF3ZWkgSG9ub3IgVjkqdml2b3x2aXZvIFg5IFBsdXMqb3Bwb3xvcHBvIHIxMSpvcHBvfG9wcG8gcjExIHBsdXMqb3Bwb3xvcHBvIFIxMSBQbHVzKm9wcG98b3BwbyBSMTcgUHJvKm1laXp1fG1laXp1IFBSTyA3IFBsdXMqbWVpenV8bWVpenUgUFJPIDYgUGx1cyp4aWFvbWl8eGlhb21pIG1peCp4aWFvbWl8bWkgMw==");
            return text.Split(new char[]
            {
                '*'
            }).ToList<string>();
        }

        public void ChangeHardwareLDPlayer2()
        {
            Random random = new Random();
            List<string> listInfoDevice = this.GetListInfoDevice();
            string text = listInfoDevice[random.Next(0, listInfoDevice.Count)];
            string model = text.Split(new char[]
            {
                '|'
            })[1];
            string text3 = text.Split(new char[]
            {
                '|'
            })[0];
            string imei = "86516602" + Common.CreateRandomNumber(7, random);
            string text5 = "46000" + Common.CreateRandomNumber(10, random);
            string text6 = "898600" + Common.CreateRandomNumber(14, random);
            string text7 = Common.Md5Encode(Common.CreateRandomStringNumber(32, random), "x2").Substring(random.Next(0, 16), 16);
            string[] array = "+8486|+8496|+8497|+8498|+8432|+8433|+8434|+8435|+8436|+8437|+8438|+8439|+8488|+8491|+8494|+8483|+8484|+8485|+8481|+8482|+8489|+8490|+8493|+8470|+8479|+8477|+8476|+8478|+8492|+8456|+8458|+8499|+8459".Split(new char[]
            {
                '|'
            });
            string pnumber = array[random.Next(array.Length)] + Common.CreateRandomNumber(7, random);
            string arguments = string.Concat(new object[]
            {
                "modify --index ",
                this.IndexDevice,
                " --imei ",
                imei,
                " --model \"",
                model,
                "\" --manufacturer ",
                text3,
                " --pnumber ",
                pnumber,
                " --imsi ",
                text5,
                " --simserial ",
                text6,
                " --androidid ",
                text7,
                " --mac"
            });
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = ConfigHelper.GetPathLDPlayer(0) + "\\dnconsole.exe",
                Arguments = arguments,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };
            process.Start();
            process.WaitForExit(5000);
        }

        public bool ChangeFileConfig()
        {
            try
            {
                JSON_Settings json_Settings = new JSON_Settings("configLDPlayer", false);
                string path = this.PathLDPlayer + "/vms/config/leidian" + this.IndexDevice.ToString() + ".config";
                string json = File.ReadAllText(path);
                JObject jobject;
                try
                {
                    jobject = JObject.Parse(json);
                }
                catch
                {
                    jobject = new JObject();
                }
                jobject["statusSettings.playerName"] = "LDPlayer-" + this.IndexDevice.ToString();
                //int width = json_Settings.GetValueInt("nudSizeW", 320);
                //int height = json_Settings.GetValueInt("nudSizeH", 480);
                //int dpi = json_Settings.GetValueInt("nudDPI", 120);

                int width = json_Settings.GetValueInt("nudSizeW", 540);
                int height = json_Settings.GetValueInt("nudSizeH", 960);
                int dpi = json_Settings.GetValueInt("nudDPI", 240);

                //int core = Convert.ToInt32(json_Settings.GetValue("cbbCPU", "1 cores (Khuyến nghị)").Split(new char[]
                //{
                //    ' '
                //})[0]);
                //int ram = Convert.ToInt32(json_Settings.GetValue("cbbRAM", "1024M (Khuyến nghị)").Split(new char[]
                //{
                //    'M'
                //})[0]);

                int core = 2;
                int ram = 2048;

                if (!jobject.ContainsKey("advancedSettings.resolution"))
                {
                    jobject.Add("advancedSettings.resolution", new JObject
                    {
                        {
                            "width",
                            width
                        },
                        {
                            "height",
                            height
                        }
                    });
                }
                else
                {
                    jobject["advancedSettings.resolution"]["width"] = width;
                    jobject["advancedSettings.resolution"]["height"] = height;
                }
                jobject["advancedSettings.resolutionDpi"] = dpi;
                jobject["advancedSettings.cpuCount"] = core;
                jobject["advancedSettings.memorySize"] = ram;
                File.WriteAllText(path, jobject.ToString());
                return true;
            }
            catch
            {
            }
            return false;
        }

        public void Restore()
        {
        }

        public string GetClipboard()
        {
            string text = ADBHelper.GetClipboard(this.DeviceId);
            return text;
        }

        public bool CheckOpenedDevice(int timeout = 60)
        {
            bool flag;
            try
            {
                List<string> strs = new List<string>();
                int num = 0;
                while (true)
                {
                    if (num < timeout)
                    {
                        if (!this.CheckIsLive())
                        {
                            break;
                        }
                        List<string> devices = ADBHelper.GetDevices();
                        List<string> strs1 = new List<string>();
                        int indexDevice = this.IndexDevice * 2 + 5555;
                        strs1.Add(string.Concat("127.0.0.1:", indexDevice.ToString()));
                        indexDevice = this.IndexDevice * 2 + 5554;
                        strs1.Add(string.Concat("emulator-", indexDevice.ToString()));
                        List<string> strs2 = strs1;
                        strs = (
                            from x in devices
                            where strs2.Contains(x)
                            select x).ToList<string>();
                        if (strs.Count > 0)
                        {
                            this.DeviceId = strs[0];
                            int num1 = 0;
                            while (true)
                            {
                                if (num1 < 60)
                                {
                                    if (!this.CheckIsLive())
                                    {
                                        break;
                                    }
                                    string activity = this.GetActivity();
                                    if ((activity == "com.android.launcher3/com.android.launcher3.Launcher" ? true : activity != ""))
                                    {
                                        flag = true;
                                        return flag;
                                    }
                                    else
                                    {
                                        Thread.Sleep(1000);
                                        num1++;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            flag = false;
                            return flag;
                        }
                        else
                        {
                            if (num == 30)
                            {
                                for (int i = 0; i < strs2.Count; i++)
                                {
                                    ADBHelper.DisconnectDevice(strs2[i]);
                                }
                                ADBHelper.ConnectDevice(strs2[0]);
                            }
                            this.DelayTime(1);
                            num++;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch
            {
            }
            flag = false;
            return flag;
        }

        public bool DoubleTap(int x, int y)
        {
            bool result;
            if (this.CheckBoundsContainLocation("[0,0][540,960]", x, y))
            {
                this.ExecuteCMD(string.Format("shell \"input tap {0} {1} & sleep 0.1; input tap {2} {3}\"", new object[]
                {
                    x,
                    y,
                    x,
                    y
                }), 10);
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        public bool DoubleTap(Point p)
        {
            bool result;
            if (this.CheckBoundsContainLocation("[0,0][540,960]", p.X, p.Y))
            {
                this.ExecuteCMD(string.Format("shell \"input tap {0} {1} & sleep 0.1; input tap {2} {3}\"", new object[]
                {
                    p.X,
                    p.Y,
                    p.X,
                    p.Y
                }), 10);
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        public string CheckIP()
        {
            return ADBHelper.Curl(this.DeviceId, "http://api64.ipify.org/").Trim();
        }

        public void Remove()
        {
            try
            {
                ADBHelper.RemoveDevice(this.PathLDPlayer, this.IndexDevice);
            }
            catch
            {
            }
        }

        public void Close()
        {
            try
            {
                ADBHelper.QuitDevice(this.PathLDPlayer, this.IndexDevice);
            }
            catch
            {
            }
        }

        public void Open(int timeout = 120)
        {
            ADBHelper.LaunchDevice(this.PathLDPlayer, this.IndexDevice);
            int tickCount = Environment.TickCount;
            try
            {
                int num = 0;
                do
                {
                    this.process = (from x in Process.GetProcessesByName("dnplayer")
                                    where x.MainWindowTitle.Equals("LDPlayer-" + this.IndexDevice.ToString())
                                    select x).FirstOrDefault<Process>();
                    if (this.process != null)
                    {
                        break;
                    }
                    num++;
                    if (num % 5 == 0)
                    {
                        ADBHelper.LaunchDevice(this.PathLDPlayer, this.IndexDevice);
                    }
                    this.DelayTime(1.0);
                }
                while (Environment.TickCount - tickCount <= timeout * 1000);
            }
            catch
            {
            }
        }

        public string ScreenShoot(string pathFolder = "", string fileName = "*.png")
        {
            try
            {
                if (!string.IsNullOrEmpty(pathFolder))
                {
                    Directory.CreateDirectory(pathFolder);
                }
                fileName = Path.GetFileNameWithoutExtension(fileName) + Path.GetExtension(fileName);
                ADBHelper.ScreenCap(this.DeviceId, "/sdcard/" + fileName);
                if (string.IsNullOrEmpty(pathFolder))
                {
                    pathFolder = FileHelper.GetPathToCurrentFolder();
                }
                this.PullFile("/sdcard/" + fileName, pathFolder);
                this.ExecuteCMD("shell rm /sdcard/*.png", 10);
                return pathFolder + "\\" + fileName;
            }
            catch
            {
            }
            return "";
        }

        public Bitmap ScreenShoot()
        {
            Bitmap result = null;
            try
            {
                string fileName = this.ScreenShoot("", this.CreateRandomString(10, "a", null) + ".png");
                result = this.GetBitmapFromFile(fileName, true);
            }
            catch
            {
            }
            return result;
        }

        private Bitmap GetBitmapFromFile(string fileName, bool isDeleteFile = true)
        {
            Bitmap result = null;
            try
            {
                using (FileStream fileStream = File.OpenRead(fileName))
                {
                    result = (Bitmap)Image.FromStream(fileStream);
                }
            }
            catch
            {
            }
            if (isDeleteFile)
            {
                Common.DeleteFile(fileName);
            }
            return result;
        }

        public void PushImageToDevice(string filePath)
        {
            this.DeleteFolder("/sdcard/launcher/ad");
            this.DeleteFolder("/sdcard/Pictures");
            this.PushFile(filePath, "/sdcard/Pictures");
            this.ExecuteCMD("shell rm /sdcard/*.png", 10);
            this.ExecuteCMD("shell am broadcast -a android.intent.action.MEDIA_MOUNTED -d file:///sdcard/Pictures", 10);
        }

        public bool ClosePopup(ref string html)
        {
            bool result;
            if (this.ClosePopup(html))
            {
                html = this.GetHtml();
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        public bool ClosePopup(string html = "")
        {
            bool flag = false;
            if (this.CheckExistImage("DataClick\\image\\x", null, 0))
            {
                this.TapByImageWait("DataClick\\image\\x", 0, 3);
                flag = true;
            }
            else
            {
                if (html == "")
                {
                    html = this.GetHtml();
                }
                string text = this.CheckExistTextsV2(html, 0.0, Device.GetListTextClosePopup().ToArray());
                if (text != "" && (!(text == "\"cancel\"") || !this.CheckExistText("request sent", html, 0.0)))
                {
                    this.TapByText(text, html, 0);
                }
            }
            if (flag)
            {
                this.DelayTime(1.0);
                if (this.CheckExistImage("DataClick\\image\\stop", null, 0))
                {
                    this.TapByImageWait("DataClick\\image\\stop", 0, 10);
                }
                else
                {
                    html = this.GetHtml();
                    if (this.CheckExistText("\"stop\"", html, 0.0))
                    {
                        this.TapByText("\"stop\"", html, 0);
                    }
                }
            }
            return flag;
        }

        public int CheckStatusDevice(ref string html, bool isAllowClickImageX = true)
        {
            bool flag;
            int num;
            int num1;
            bool flag1;
            try
            {
                if (this.CheckIsLive())
                {
                    Bitmap bitmap = this.ScreenShoot();
                    if (this.CheckExistImage("DataClick\\image\\logout", bitmap, 0))
                    {
                        num = -11;
                    }
                    else if (this.CheckExistImage("DataClick\\image\\checkpoint", bitmap, 0))
                    {
                        num = 2;
                    }
                    else if (!this.CheckExistImage("DataClick\\image\\openappagain", bitmap, 0))
                    {
                        string activity = this.GetActivity();
                        if (activity.Contains("Launcher"))
                        {
                            this.OpenAppFacebook();
                            num = 1;
                        }
                        else if (activity != "Application")
                        {
                            int num2 = this.CheckStringContainText(activity, new Dictionary<int, List<string>>()
                            {
                                { 1, this.GetListActivityCheckpointFacebook() },
                                { 2, this.GetListActivityNotLoginFacebook() }
                            });
                            if (num2 == 1)
                            {
                                num = 2;
                            }
                            else if (num2 == 2)
                            {
                                num = -12;
                            }
                            else
                            {
                                html = this.GetHtml();
                                this.ClosePopup(ref html);
                                List<string> listText = this.GetListText(html, 0);
                                if (listText.Count != 2 || !listText.Contains("back"))
                                {
                                    flag1 = false;
                                }
                                else
                                {
                                    flag1 = (listText.Contains("search") ? true : listText.Contains("web view"));
                                }
                                if (flag1)
                                {
                                    num = -13;
                                }
                                else if ((listText.Count != 3 || !listText.Contains("back") || !listText.Contains("facebook") ? true : !listText.Contains("web view")))
                                {
                                    switch (this.CheckStringContainText(html, new Dictionary<int, List<string>>()
                                    {
                                        { 1, Device.GetListTextCheckpointFacebook() },
                                        { 4, new List<string>()
                                        {
                                            "session expired",
                                            "please log in again."
                                        } }
                                    }))
                                    {
                                        case 1:
                                            {
                                                num = 2;
                                                return num;
                                            }
                                        case 2:
                                            {
                                                num = -4;
                                                return num;
                                            }
                                        case 3:
                                            {
                                                num = 7;
                                                return num;
                                            }
                                        case 4:
                                            {
                                                num = -15;
                                                return num;
                                            }
                                        default:
                                            {
                                                flag = true;
                                                num1 = 0;
                                                break;
                                            }
                                    }
                                    while (true)
                                    {
                                        if (num1 < 2)
                                        {
                                            html = this.GetHtml();
                                            if (!this.CheckExistText("\"tap to retry\"", html, 0))
                                            {
                                                flag = false;
                                                break;
                                            }
                                            else
                                            {
                                                this.TapByText("\"tap to retry\"", html, 0);
                                                this.DelayTime(1);
                                                num1++;
                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    if (!flag)
                                    {
                                        this.demDisconnect = 0;
                                    }
                                    else if (this.demDisconnect < this.maxDisconnect)
                                    {
                                        this.demDisconnect++;
                                        flag = false;
                                    }
                                    if (!flag)
                                    {
                                        num = 0;
                                        return num;
                                    }
                                    else
                                    {
                                        num = 7;
                                    }
                                }
                                else
                                {
                                    num = -14;
                                }
                            }
                        }
                        else
                        {
                            num = -4;
                        }
                    }
                    else
                    {
                        this.countOpenAppAgain++;
                        if (this.countOpenAppAgain < 3)
                        {
                            this.TapByImageWait("DataClick\\image\\openappagain", 0, 10);
                            num = 1;
                        }
                        else
                        {
                            num = -4;
                        }
                    }
                }
                else
                {
                    num = -2;
                }
            }
            catch
            {
                num = 0;
                return num;
            }
            return num;
        }

        private int CheckStringContainText(string sChuoi, Dictionary<int, List<string>> dic)
        {
            foreach (KeyValuePair<int, List<string>> keyValuePair in dic)
            {
                foreach (string text in keyValuePair.Value)
                {
                    if (Regex.IsMatch(sChuoi, text) || sChuoi.Contains(text))
                    {
                        return keyValuePair.Key;
                    }
                }
            }
            return 0;
        }

        private bool CheckItemIsExistInList(string item, List<string> lst)
        {
            for (int i = 0; i < lst.Count; i++)
            {
                if (item.Contains(lst[i]))
                {
                    return true;
                }
            }
            return false;
        }

        private int CheckStatusLoginFacebookByActivity(string activity)
        {
            int result = -1;
            try
            {
                if (this.CheckItemIsExistInList(activity, this.GetListActivityLoginedFacebook()))
                {
                    result = 1;
                }
                else if (this.CheckItemIsExistInList(activity, this.GetListActivityCheckpointFacebook()))
                {
                    result = 2;
                }
                else if (this.CheckItemIsExistInList(activity, this.GetListActivityLoginedNoveryFacebook()))
                {
                    result = 8;
                }
                else if (this.CheckItemIsExistInList(activity, this.GetListActivityNotLoginFacebook()))
                {
                    result = 0;
                }
            }
            catch
            {
            }
            return result;
        }

        private int CheckStatusLoginFacebookByText(string xml)
        {
            int result = 0;
            try
            {
                if (xml == "")
                {
                    xml = this.GetHtml();
                }
                this.ClosePopup(ref xml);
                if (this.CheckExistTexts(xml, 0.0, Device.GetListTextLoginedFacebook().ToArray()) != 0)
                {
                    result = 1;
                }
                else if (this.CheckExistTexts(xml, 0.0, Device.GetListTextCheckpointFacebook().ToArray()) != 0)
                {
                    result = 2;
                }
                else if (this.CheckExistTexts(xml, 0.0, Device.GetListTextLoginedNoveryFacebook().ToArray()) != 0)
                {
                    result = 8;
                }
                else if (this.CheckExistTexts(xml, 0.0, new string[]
                {
                    "session expired",
                    "please log in again."
                }) != 0)
                {
                    result = 11;
                }
            }
            catch
            {
            }
            return result;
        }

        private List<string> GetListActivityNotLoginFacebook()
        {
            return new List<string>
            {
                "com.facebook.katana/com.facebook.katana.dbl.activity.DeviceBasedLoginActivity"
            };
        }

        private List<string> GetListActivityLoginedFacebook()
        {
            return new List<string>
            {
                "com.facebook.katana/com.facebook.katana.activity.FbMainTabActivity",
                "com.facebook.katana/com.facebook.location.optin.DeviceLocationSettingsOptInActivity",
                "com.facebook.katana/com.facebook.location.optin.AccountLocationSettingsOptInActivity",
                "com.facebook.katana/com.facebook.account.switcher.nux.ActivateDeviceBasedLoginNuxActivity"
            };
        }

        private List<string> GetListActivityCheckpointFacebook()
        {
            return new List<string>
            {
                "com.facebook.katana/com.facebook.checkpoint.CheckpointActivity",
                "checkpoint"
            };
        }

        private List<string> GetListActivityLoginedNoveryFacebook()
        {
            return new List<string>
            {
                "com.facebook.confirmation.activity.SimpleConfirmAccountActivity"
            };
        }

        private static List<string> GetListTextLoginedFacebook()
        {
            return new List<string>
            {
                "\"search facebook\"",
                "\"go to profile\"",
                "\"make a post on facebook\"",
                "\"live\"",
                "\"messaging\"",
                "\"photo\"",
                "\"check in\"",
                "\"stories\"",
                "\"marketplace\"",
                "\"notifications,",
                "\"save your login info\""
            };
        }

        private static List<string> GetListTextCheckpointFacebook()
        {
            return new List<string>
            {
                "your account is temporarily unavailable",
                "your account is temporarily locked",
                "your account has been disabled",
                "your account has been locked",
                "\"learn more\"",
                "download your information",
                "go to community standards",
                "choose a security check",
                "provide your birthday",
                "identify photos of friends",
                "get a code sent to your email",
                "enter number",
                "check the login details shown. was it you?"
            };
        }

        private static List<string> GetListTextLoginedNoveryFacebook()
        {
            return new List<string>
            {
                "\"confirmation code\"",
                "\"change phone number\"",
                "\"confirm by email\"",
                "\"change email address\"",
                "\"confirm by phone number\""
            };
        }

        public static List<string> GetListTextClosePopup()
        {
            return new List<string>
            {
                "\"close\"",
                "\"skip\"",
                "\"cancel\"",
                "\"got it\""
            };
        }

        public void OpenAppFacebookLite()
        {
            this.GrantPermissionLite();
            this.OpenApp("com.facebook.lite");
            for (int i = 0; i < 10; i++)
            {
                string activity = this.GetActivity();
                if (activity.Contains("com.facebook.lite"))
                {
                    break;
                }
                if (activity.Contains("Launcher"))
                {
                    this.TapByText("facebook", "", 0);
                }
                else
                {
                    this.OpenApp("com.facebook.lite");
                }
                this.DelayTime(3.0);
            }
        }

        public void OpenAppFacebook()
        {
            this.GrantPermission();
            this.OpenApp("com.facebook.katana");
            for (int i = 0; i < 10; i++)
            {
                string activity = this.GetActivity();
                if (activity.Contains("com.facebook.katana"))
                {
                    break;
                }
                if (activity.Contains("Launcher"))
                {
                    this.TapByText("facebook", "", 0);
                }
                else
                {
                    this.OpenApp("com.facebook.katana");
                }
                this.DelayTime(3.0);
            }
        }

        public void CloseAppFacebook()
        {
            this.CloseApp("com.facebook.katana");
        }

        public bool SyncContactByFb()
        {
            bool result = true;
            string html = "";
            try
            {
                this.OpenActivity("com.facebook.katana/com.facebook.katana.settings.activity.SettingsActivity");
                for (int i = 0; i < 2; i++)
                {
                    int num = this.CheckExistTexts("", 5.0, new string[]
                    {
                        "continuous contacts upload, off, switch",
                        "continuous contacts upload, on, switch"
                    });
                    int num2 = num;
                    int num3 = num2;
                    if (num3 != 1)
                    {
                        if (num3 == 2)
                        {
                            int num4 = 1;
                            bool flag;
                            do
                            {
                                int num5 = num4;
                                int num6 = num5;
                                if (num6 != 1)
                                {
                                    if (num6 != 2)
                                    {
                                        break;
                                    }
                                    flag = this.TapByText("turn off", "", 5);
                                }
                                else
                                {
                                    flag = this.TapByText("continuous contacts upload, on, switch", "", 0);
                                }
                                num4++;
                            }
                            while (flag);
                        }
                    }
                    else
                    {
                        i++;
                        int num7 = 1;
                        for (; ; )
                        {
                        IL_237:
                            bool flag2 = this.TapByText("continuous contacts upload, off, switch", "", 0);
                            for (; ; )
                            {
                                num7++;
                                if (!flag2)
                                {
                                    goto Block_12;
                                }
                                switch (num7)
                                {
                                    case 1:
                                        goto IL_237;
                                    case 2:
                                        {
                                            int num8 = this.CheckExistTexts(ref html, 5.0, new string[]
                                            {
                                        "\"contacts upload\"",
                                        "\"get started\""
                                            });
                                            int num9 = num8;
                                            int num10 = num9;
                                            if (num10 != 1)
                                            {
                                                flag2 = (num10 == 2 && this.TapByText("\"get started\"", html, 0));
                                                continue;
                                            }
                                            flag2 = this.TapByText("\"contacts upload\"", html, 0);
                                            continue;
                                        }
                                    case 3:
                                        for (int j = 0; j < 30; j++)
                                        {
                                            html = this.GetHtml();
                                            if (this.CheckExistText("people you may know", html, 0.0))
                                            {
                                                break;
                                            }
                                            this.DelayTime(1.0);
                                        }
                                        continue;
                                }
                                goto Block_8;
                            }
                        }
                    Block_8:
                        if (num7 < 4)
                        {
                            this.ExportError(null, "SyncContactByFb: step " + num7.ToString());
                        }
                    Block_12:;
                    }
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public bool ClickReactions(int typeReaction = 1)
        {
            try
            {
                List<string> list = new List<string>
                {
                    "like",
                    "love",
                    "haha",
                    "wow",
                    "sad",
                    "angry"
                };
                if (typeReaction == 6)
                {
                    typeReaction = this.GetRandomInt(0, 4);
                }
                this.TapByImage("DataClick\\image\\reaction\\" + list[typeReaction], null, 0);
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }

        public string OpenFacebookAndCheckStatusLogin(int timeout = 120)
        {
            string str = "0|";
            try
            {
                string activity = "";
                string html = "";
                int num = -1;
                int tickCount = Environment.TickCount;
                do
                {
                    if (this.CheckIsLive())
                    {
                        activity = this.GetActivity();
                        if (activity.Contains("Launcher"))
                        {
                            this.OpenAppFacebook();
                        }
                        else if (activity == "Application")
                        {
                            str = "2|";
                            return str;
                        }
                        else if (activity != "")
                        {
                            if ((new List<string>()
                            {
                                "com.facebook.katana/com.facebook.katana.app.FacebookSplashScreenActivity",
                                "com.facebook.katana/com.facebook.deeplinking.activity.StoryDeepLinkLoadingActivity",
                                "com.facebook.katana/com.facebook.resources.impl.WaitingForStringsActivity"
                            }).Contains(activity))
                            {
                                continue;
                            }
                            Bitmap bitmap = this.ScreenShoot();
                            string boundsByImage = this.GetBoundsByImage("DataClick\\image\\phoneoremail", bitmap, 0);
                            string boundsByImage1 = this.GetBoundsByImage("DataClick\\image\\password", bitmap, 0);
                            string str1 = this.GetBoundsByImage("DataClick\\image\\login", bitmap, 0);
                            if ((!(boundsByImage != "") || !(boundsByImage1 != "") ? false : str1 != ""))
                            {
                                str = "1|0";
                                return str;
                            }
                            else
                            {
                                if (!this.CheckExistImage("DataClick\\image\\checkpoint", null, 0))
                                {
                                    html = "";
                                    if ((!this.CheckExistImage("DataClick\\image\\ok", null, 0) ? false : !this.CheckExistImage("DataClick\\image\\facebook", null, 0)))
                                    {
                                        this.TapByImageWait("DataClick\\image\\ok", 0, 10);
                                    }
                                    else if (this.CheckExistText("\"ok\"", ref html, 0))
                                    {
                                        this.TapByText("\"ok\"", html, 0);
                                    }
                                    else if (!this.ClosePopup(html) && this.CheckExistText("\"close", html, 0))
                                    {
                                        this.TapByText("\"close", html, 0);
                                        this.TapByText("\"back\"", "", 0);
                                    }
                                    num = this.CheckStatusLoginFacebookByActivity(activity);
                                    if (num == -1)
                                    {
                                        html = this.GetHtml();
                                        num = this.CheckStatusLoginFacebookByText(html);
                                    }
                                }
                                else
                                {
                                    num = 2;
                                }
                                if (Environment.TickCount - tickCount >= timeout * 1000)
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (this.ClosePopup(html))
                            {
                                continue;
                            }
                            if (!this.CheckExistText("\"close", html, 0))
                            {
                                this.OpenAppFacebook();
                            }
                            else
                            {
                                this.TapByText("\"close", html, 0);
                                this.TapByText("\"back\"", "", 0);
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                while (num == -1);
                if (num != -1)
                {
                    str = string.Concat("1|", num.ToString());
                }
            }
            catch
            {
            }
            return str;
        }

        public void CheckDevice(string activity = "", string html = "", string folderPath = "")
        {
            if (fViewLD.remote != null)
            {
                fViewLD.remote.ExportLog(this.IndexDevice, activity, html, folderPath);
            }
        }

        public void LoadStatusLD(string content)
        {
            if (fViewLD.remote != null)
            {
                fViewLD.remote.LoadStatus(this.IndexDevice, content);
            }
        }

        public void LoadHanhDongLD(string content)
        {
            if (fViewLD.remote != null)
            {
                fViewLD.remote.LoadHanhDong(this.IndexDevice, content);
            }
        }

        public int LoginFacebook(string username, string password, string fa2, int timeOut = 180)
        {
            int num = 0;
            int tickCount = Environment.TickCount;
            string text = "";
            int num2 = 0;
            int num3 = 0;
            while (num == 0)
            {
                if (Environment.TickCount - tickCount < timeOut * 1000 && this.CheckIsLive())
                {
                    string activity = this.GetActivity();
                    if (activity.Contains("Launcher"))
                    {
                        this.OpenAppFacebook();
                        continue;
                    }
                    List<string> list = new List<string>
                    {
                        "",
                        "com.facebook.katana/com.facebook.deeplinking.activity.StoryDeepLinkLoadingActivity",
                        "com.facebook.katana/com.facebook.katana.app.FacebookSplashScreenActivity",
                        "com.facebook.katana/com.facebook.resources.impl.WaitingForStringsActivity"
                    };
                    if (list.Contains(activity))
                    {
                        if (!(activity == "com.facebook.katana/com.facebook.resources.impl.WaitingForStringsActivity") && activity == "")
                        {
                            this.ClosePopup("");
                            continue;
                        }
                        continue;
                    }
                    else if (activity == "com.facebook.katana/com.facebook.katana.dbl.activity.DeviceBasedLoginActivity")
                    {
                        Dictionary<int, List<string>> dic = new Dictionary<int, List<string>>
                        {
                            {
                                1,
                                new List<string>
                                {
                                    "\"lần tới đăng nhập"
                                }
                            },
                            {
                                2,
                                new List<string>
                                {
                                    "sai thông tin đăng nhập",
                                    "session expired",
                                    "please log in again."
                                }
                            },
                            {
                                3,
                                new List<string>
                                {
                                    "xác nhận danh tính của bạn"
                                }
                            },
                            {
                                4,
                                new List<string>
                                {
                                    "log into another account"
                                }
                            },
                            {
                                5,
                                new List<string>
                                {
                                    "\"log in\""
                                }
                            },
                            {
                                6,
                                new List<string>
                                {
                                    "\"tap to retry\""
                                }
                            },
                            {
                                7,
                                new List<string>
                                {
                                    "\"login code required",
                                    "\"login code",
                                    "\"cần mã",
                                    "\"lỗi xác thực\"",
                                    "yêu cầu xác thực hai yếu tố"
                                }
                            },
                            {
                                8,
                                new List<string>
                                {
                                    "your account is temporarily unavailable"
                                }
                            }
                        };
                        text = this.GetHtml();
                        switch (this.CheckExistTexts(text, 0.0, dic))
                        {
                            case 1:
                                this.TapByText("\"close\"", text, 0);
                                continue;
                            case 2:
                                this.TapByText("\"ok\"", text, 0);
                                continue;
                            case 3:
                                this.TapByText("\"get started\"", text, 0);
                                continue;
                            case 4:
                                {
                                    Point locationFromBounds = this.GetLocationFromBounds(this.GetBoundsByText("log into another account", text, 0));
                                    if (!locationFromBounds.IsEmpty)
                                    {
                                        this.Tap(locationFromBounds.X, locationFromBounds.Y - 49, 1);
                                        this.DelayTime(1.0);
                                        continue;
                                    }
                                    continue;
                                }
                            case 5:
                                this.InputText(password);
                                this.TapByText("\"log in\"", text, 0);
                                continue;
                            case 6:
                                if (num3 < 3)
                                {
                                    this.TapByText("\"tap to retry\"", text, 0);
                                    num3++;
                                    continue;
                                }
                                break;
                            case 7:
                                if (string.IsNullOrEmpty(fa2))
                                {
                                    num = 3;
                                    continue;
                                }
                                num2++;
                                if (num2 <= 2)
                                {
                                    this.TapByText("\"ok\"", text, 0);
                                    text = this.GetHtml();
                                    this.LoadStatusLD("Get 2FA...");
                                    string totp = Common.GetTotp(fa2);
                                    if (!string.IsNullOrEmpty(totp))
                                    {
                                        this.TapByText("\"login code\"", text, 0);
                                        this.InputText(totp);
                                        this.TapByText("\"continue\"", text, 0);
                                        continue;
                                    }
                                    num = 6;
                                }
                                else
                                {
                                    num = 6;
                                }
                                break;
                            case 8:
                                this.TapByText("\"ok\"", text, 0);
                                this.TapByText("another account", "", 5);
                                continue;
                            default:
                                if (this.CheckExistImage("DataClick\\image\\ok", null, 0) && !this.CheckExistImage("DataClick\\image\\facebook", null, 0))
                                {
                                    this.TapByImageWait("DataClick\\image\\ok", 0, 10);
                                    continue;
                                }
                                continue;
                        }
                    }
                    else
                    {
                        int num4 = this.CheckStatusLoginFacebookByActivity(activity);
                        if (num4 == 0 || num4 == -1)
                        {
                            bool flag = false;
                            Bitmap bitmap_screen = this.ScreenShoot();
                            string boundsByImage = this.GetBoundsByImage("DataClick\\image\\phoneoremail", bitmap_screen, 0);
                            string boundsByImage2 = this.GetBoundsByImage("DataClick\\image\\password", bitmap_screen, 0);
                            string boundsByImage3 = this.GetBoundsByImage("DataClick\\image\\login", bitmap_screen, 0);
                            if (boundsByImage != "" && boundsByImage2 != "" && boundsByImage3 != "")
                            {
                                flag = true;
                            }
                            if (!flag)
                            {
                                text = this.GetHtml();
                                num4 = this.CheckStatusLoginFacebookByText(text);
                                if (num4 != 0)
                                {
                                    num = num4;
                                    break;
                                }
                            }
                            if (!flag && this.CheckExistTexts(text, 0.0, new string[]
                            {
                                "session expired",
                                "please log in again."
                            }) != 0)
                            {
                                this.TapByText("\"ok\"", text, 0);
                                this.DelayTime(3.0);
                                continue;
                            }
                            if (!flag && this.CheckExistText("lúc khác", text, 0.0))
                            {
                                this.TapByText("\"ok\"", text, 0);
                                this.DelayTime(1.0);
                                continue;
                            }
                            if (!flag && this.CheckExistText("\"continue\"", text, 0.0))
                            {
                                this.TapByText("\"continue\"", text, 0);
                                this.DelayTime(1.0);
                                continue;
                            }
                            if (!flag && this.CheckExistImage("DataClick\\image\\ok", null, 0) && !this.CheckExistImage("DataClick\\image\\facebook", null, 0))
                            {
                                this.TapByImageWait("DataClick\\image\\ok", 0, 10);
                                this.DelayTime(1.0);
                                continue;
                            }
                            if (!flag)
                            {
                                bitmap_screen = this.ScreenShoot();
                                boundsByImage = this.GetBoundsByImage("DataClick\\image\\phoneoremail", bitmap_screen, 0);
                                boundsByImage2 = this.GetBoundsByImage("DataClick\\image\\password", bitmap_screen, 0);
                                boundsByImage3 = this.GetBoundsByImage("DataClick\\image\\login", bitmap_screen, 0);
                                if (boundsByImage == "" || boundsByImage2 == "" || boundsByImage3 == "")
                                {
                                    num4 = this.CheckStatusLoginFacebookByText("");
                                    if (num4 != 0)
                                    {
                                        num = num4;
                                        break;
                                    }
                                    this.CheckDevice(activity, text, "");
                                    break;
                                }
                            }
                            this.TapByBounds(boundsByImage, "");
                            this.InputText(username);
                            this.DelayTime(1.0);
                            this.TapByBounds(boundsByImage, "");
                            this.DelayTime(1.0);
                            this.TapByBounds(boundsByImage2, "");
                            this.InputTextWithUnicode(password, 0.0);
                            this.DelayTime(1.0);
                            this.TapByBounds(boundsByImage3, "");
                            Dictionary<int, List<string>> dic2 = new Dictionary<int, List<string>>
                            {
                                {
                                    18,
                                    new List<string>
                                    {
                                        "use this feature right now",
                                        "login failed"
                                    }
                                },
                                {
                                    1,
                                    new List<string>
                                    {
                                        "can't find account",
                                        "need help finding your account?"
                                    }
                                },
                                {
                                    17,
                                    new List<string>
                                    {
                                        "đã xảy ra lỗi khi đăng nhập. vui lòng thử lại sau."
                                    }
                                },
                                {
                                    2,
                                    new List<string>
                                    {
                                        "incorrect password",
                                        "mật khẩu cũ",
                                        "older password"
                                    }
                                },
                                {
                                    3,
                                    new List<string>
                                    {
                                        "login code required",
                                        "\"two-factor authentication",
                                        "\"authentication error\"",
                                        "yêu cầu xác thực hai yếu tố"
                                    }
                                },
                                {
                                    5,
                                    new List<string>
                                    {
                                        "logging in…",
                                        "loading…"
                                    }
                                },
                                {
                                    6,
                                    new List<string>
                                    {
                                        "kiểm tra kết nối"
                                    }
                                },
                                {
                                    7,
                                    new List<string>
                                    {
                                        "\"skip\""
                                    }
                                },
                                {
                                    11,
                                    new List<string>
                                    {
                                        "\"no thanks\""
                                    }
                                },
                                {
                                    8,
                                    new List<string>
                                    {
                                        "\"continue\""
                                    }
                                },
                                {
                                    9,
                                    new List<string>
                                    {
                                        "save your login info"
                                    }
                                },
                                {
                                    12,
                                    Device.GetListTextLoginedNoveryFacebook()
                                },
                                {
                                    13,
                                    Device.GetListTextLoginedFacebook()
                                },
                                {
                                    14,
                                    new List<string>
                                    {
                                        "tải danh bạ lên",
                                        "\"lúc khác\""
                                    }
                                },
                                {
                                    15,
                                    new List<string>
                                    {
                                        "tap to retry"
                                    }
                                },
                                {
                                    16,
                                    new List<string>
                                    {
                                        "thêm số di động vào tài khoản của bạn"
                                    }
                                },
                                {
                                    20,
                                    new List<string>
                                    {
                                        "confirm your identity"
                                    }
                                },
                                {
                                    21,
                                    new List<string>
                                    {
                                        "allow facebook to access your location?",
                                        "\"deny\""
                                    }
                                },
                                {
                                    22,
                                    new List<string>
                                    {
                                        "allow contacts access to find people to follow"
                                    }
                                },
                                {
                                    4,
                                    Device.GetListTextCheckpointFacebook()
                                }
                            };
                            while (Environment.TickCount - tickCount < timeOut * 1000)
                            {
                                if (!this.CheckIsLive())
                                {
                                    break;
                                }
                                text = this.GetHtml();
                                num4 = this.CheckExistTexts(text, 0.0, dic2);
                                switch (num4)
                                {
                                    case 1:
                                        num = 4;
                                        break;
                                    case 2:
                                        num = 5;
                                        break;
                                    case 3:
                                    case 8:
                                        if (num4 == 8 && !this.CheckExistText("\"login code\"", text, 0.0))
                                        {
                                            this.TapByText("\"continue\"", text, 0);
                                        }
                                        else if (string.IsNullOrEmpty(fa2))
                                        {
                                            num = 3;
                                        }
                                        else
                                        {
                                            num2++;
                                            if (num2 > 2)
                                            {
                                                return 6;
                                            }
                                            if (num4 == 3)
                                            {
                                                this.TapByText("\"ok\"", text, 0);
                                                text = this.GetHtml();
                                            }
                                            this.LoadStatusLD("Get 2FA...");
                                            string totp2 = Common.GetTotp(fa2);
                                            if (string.IsNullOrEmpty(totp2))
                                            {
                                                return 6;
                                            }
                                            this.TapByText("\"login code\"", text, 0);
                                            this.InputText(totp2);
                                            this.TapByText("\"continue\"", text, 0);
                                        }
                                        break;
                                    case 4:
                                        num = 2;
                                        break;
                                    case 5:
                                        break;
                                    case 6:
                                        num = 7;
                                        break;
                                    case 7:
                                        this.TapByText("\"skip\"", text, 0);
                                        break;
                                    case 9:
                                        this.TapByText("\"ok\"", text, 0);
                                        break;
                                    case 10:
                                    case 19:
                                        goto IL_E50;
                                    case 11:
                                        if (this.TapByText("\"no thanks\"", "", 0))
                                        {
                                            this.TapByImage("DataClick\\image\\sudungdulieu", null, 10);
                                        }
                                        break;
                                    case 12:
                                        num = 8;
                                        break;
                                    case 13:
                                        num = 1;
                                        break;
                                    case 14:
                                        this.TapByText("\"lúc khác\"", text, 0);
                                        break;
                                    case 15:
                                        if (num3 >= 3)
                                        {
                                            return num;
                                        }
                                        this.TapByText("\"tap to retry\"", text, 0);
                                        num3++;
                                        break;
                                    case 16:
                                        num = 9;
                                        break;
                                    case 17:
                                        return num;
                                    case 18:
                                        if (this.CheckExistText("please check your internet connection", text, 0.0))
                                        {
                                            this.TapByText("\"ok\"", text, 0);
                                            this.TapByImage("DataClick\\image\\login", null, 10);
                                        }
                                        else
                                        {
                                            num = 10;
                                        }
                                        break;
                                    case 20:
                                        if (this.CheckExistText("\"get started\"", "", 0.0))
                                        {
                                            this.TapByText("\"get started\"", text, 0);
                                        }
                                        else
                                        {
                                            num = 2;
                                        }
                                        break;
                                    case 21:
                                        if (this.CheckExistText("\"deny\"", text, 0.0))
                                        {
                                            this.TapByText("\"deny\"", text, 0);
                                        }
                                        else
                                        {
                                            this.TapByImage("DataClick\\image\\deny", null, 0);
                                        }
                                        break;
                                    case 22:
                                        this.TapByImage("DataClick\\image\\skip", null, 0);
                                        break;
                                    default:
                                        goto IL_E50;
                                }
                            IL_10EC:
                                if (num == 0)
                                {
                                    continue;
                                }
                                break;
                            IL_E50:
                                activity = this.GetActivity();
                                if (this.CheckExistImage("DataClick\\image\\sudungdulieu", null, 0))
                                {
                                    this.TapByImage("DataClick\\image\\sudungdulieu", null, 0);
                                    goto IL_10EC;
                                }
                                if (this.CheckExistImage("DataClick\\image\\skip", null, 0))
                                {
                                    this.TapByImage("DataClick\\image\\skip", null, 0);
                                    goto IL_10EC;
                                }
                                if (this.CheckExistImage("DataClick\\image\\ok", null, 0) && !this.CheckExistImage("DataClick\\image\\facebook", null, 0))
                                {
                                    this.TapByImageWait("DataClick\\image\\ok", 0, 10);
                                    goto IL_10EC;
                                }
                                if (this.CheckExistText("web view", text, 0.0) && (activity.Contains("activity.ImmersiveActivity") || activity.Contains("LoggedOutWebViewActivity")))
                                {
                                    num = 2;
                                    goto IL_10EC;
                                }
                                if (activity == "com.facebook.katana/com.facebook.location.optin.AccountLocationSettingsOptInActivity")
                                {
                                    if (this.CheckExistText("\"deny\"", text, 0.0))
                                    {
                                        this.TapByText("\"deny\"", text, 0);
                                        goto IL_10EC;
                                    }
                                    if (this.CheckExistImage("DataClick\\image\\notnow", null, 0))
                                    {
                                        this.TapByImage("DataClick\\image\\notnow", null, 0);
                                        goto IL_10EC;
                                    }
                                    if (this.CheckExistImage("DataClick\\image\\deny", null, 0))
                                    {
                                        this.TapByImage("DataClick\\image\\deny", null, 0);
                                        goto IL_10EC;
                                    }
                                    goto IL_10EC;
                                }
                                else
                                {
                                    text = this.GetHtml();
                                    List<string> listText = this.GetListText(text, 0);
                                    if (listText.Count == 1 && listText.Contains("get started"))
                                    {
                                        num = 2;
                                        goto IL_10EC;
                                    }
                                    goto IL_10EC;
                                }
                            }
                        }
                        else
                        {
                            num = num4;
                        }
                    }
                }
                return num;
            }
            return num;
        }

        public string GetTokenCookie()
        {
            string text = "";
            string text2 = "";
            try
            {
                string input = this.ReadFile("data/data/com.facebook.katana/app_light_prefs/com.facebook.katana/authentication");
                text = Regex.Match(input, "EAAAAU\\S+").Value;
                string value = Regex.Match(text, "\u0005(.*?)$").Value;
                text = text.Replace(value, "");
                string json = "{\"data\": [" + Regex.Match(input, "\\[(.*?)\\]").Groups[1].Value + "]}";
                JObject jobject = JObject.Parse(json);
                for (int i = 0; i < jobject["data"].Count<JToken>(); i++)
                {
                    text2 = string.Concat(new string[]
                    {
                        text2,
                        jobject["data"][i]["name"].ToString(),
                        "=",
                        jobject["data"][i]["value"].ToString(),
                        ";"
                    });
                }
            }
            catch
            {
            }
            return text + "|" + text2;
        }

        public bool WaitForLoaded(int timeWait_Second)
        {
            try
            {
                int tickCount = Environment.TickCount;
                while (this.CheckExistXpath("/[@class='android.widget.progressbar']", ""))
                {
                    if (Environment.TickCount - tickCount >= timeWait_Second * 1000)
                    {
                        goto IL_60;
                    }
                    this.DelayTime(1.0);
                }
                return true;
            }
            catch (Exception)
            {
            }
        IL_60:
            return false;
        }

        public void BackupConfigDevice(string uid)
        {
            Directory.CreateDirectory("device");
            Directory.CreateDirectory("device\\" + uid);
            File.Copy(this.PathLDPlayer + "\\vms\\config\\leidian" + this.IndexDevice.ToString() + ".config", "device\\" + uid + "\\config", true);
        }

        public void RestoreConfigDevice(string uid)
        {
            string sourceFileName = FileHelper.GetPathToCurrentFolder() + "\\device\\" + uid + "\\config";
            File.Copy(sourceFileName, this.PathLDPlayer + "\\vms\\config\\leidian" + this.IndexDevice.ToString() + ".config", true);
        }

        public void GotoNewFeedQuick()
        {
            this.ExecuteCMD(string.Format(ADBCommands.VIEW, this.link_feed, "com.facebook.katana"), 3);
        }

        public void GotoNotificationQuick()
        {
            this.ExecuteCMD(string.Format(ADBCommands.VIEW, this.link_notifications, "com.facebook.katana"), 10);
        }

        public void GotoSearchQuick()
        {
            this.ExecuteCMD(string.Format(ADBCommands.VIEW, this.link_search, "com.facebook.katana"), 10);
        }

        public void GotoWatchQuick()
        {
            this.ExecuteCMD(string.Format(ADBCommands.VIEW, this.link_watch, "com.facebook.katana"), 10);
        }

        public void GotoFriendsQuick()
        {
            this.ExecuteCMD(string.Format(ADBCommands.VIEW, this.link_friends, "com.facebook.katana"), 10);
        }

        public void GotoProfileQuick(string uid = "")
        {
            this.ExecuteCMD(string.Format(ADBCommands.VIEW, this.link_profile + ((uid == "") ? "" : ("/" + uid)), "com.facebook.katana"), 10);
        }

        public void GotoPageQuick(string id)
        {
            this.ExecuteCMD(string.Format(ADBCommands.VIEW, this.link_page + "/" + id, "com.facebook.katana"), 10);
        }

        public void GotoGroupQuick(string id)
        {
            this.ExecuteCMD(string.Format(ADBCommands.VIEW, this.link_group + "/" + id, "com.facebook.katana"), 10);
        }

        public void GotoSyncContactQuick()
        {
            this.ExecuteCMD(string.Format(ADBCommands.OPEN_LINK, this.link_syncContact), 10);
        }

        public void GotoBack(int times = 1, double timeDelay_seconds = 0.0)
        {
            for (int i = 0; i < times; i++)
            {
                this.InputKey(Device.KeyEvent.KEYCODE_BACK);
                this.DelayTime(timeDelay_seconds);
            }
        }

        public bool OpenLink(string link, int timeout = 10)
        {
            try
            {
                this.ExecuteCMD(string.Format(ADBCommands.VIEW, link.Replace("&", "\\&"), "com.facebook.katana"), timeout);
                if (link.StartsWith("fb://") || link.StartsWith("https://facebook.com") || link.StartsWith("https://www.facebook.com") || link.StartsWith("https://web.facebook.com"))
                {
                    for (int i = 0; i < 30; i++)
                    {
                        if (this.CheckExistImage("DataClick\\image\\openappagain", null, 0))
                        {
                            this.TapByImageWait("DataClick\\image\\openappagain", 0, 10);
                            break;
                        }
                        if (this.GetActivity().Contains("com.facebook.katana"))
                        {
                            break;
                        }
                        string html = this.GetHtml();
                        if (this.CheckExistText("open with", html, 0.0))
                        {
                            this.TapByText("facebook", html, 0);
                            this.DelayRandom(1.0, 1.5);
                            this.TapByText("always", html, 0);
                            this.DelayRandom(3.0, 5.0);
                        }
                    }
                }
                return true;
            }
            catch
            {
            }
            return false;
        }

        public bool OpenActivity(string activity)
        {
            try
            {
                this.ExecuteCMD(string.Format(ADBCommands.OPEN_LINK, activity), 3);
                return true;
            }
            catch
            {
            }
            return false;
        }

        public bool GotoListFriend()
        {
            this.GotoNewFeedQuick();
            int num = 1;
            bool flag;
            for (; ; )
            {
            IL_76:
                flag = this.TapByImage("DataClick\\image\\menu", null, 3);
                for (; ; )
                {
                    num++;
                    if (!flag)
                    {
                        goto Block_2;
                    }
                    switch (num)
                    {
                        case 1:
                            goto IL_76;
                        case 2:
                            flag = this.TapByImage("DataClick\\image\\banbe", null, 5);
                            continue;
                        case 3:
                            flag = this.TapByText("all friends", "", 3);
                            continue;
                    }
                    goto Block_1;
                }
            }
        Block_1:
            return flag;
        Block_2:
            this.GotoFriendsQuick();
            return flag;
        }

        public bool GotoListGroup()
        {
            this.GotoNewFeedQuick();
            int num = 1;
            bool flag;
            for (; ; )
            {
            IL_76:
                flag = this.TapByImage("DataClick\\image\\menu", null, 3);
                for (; ; )
                {
                    num++;
                    if (!flag)
                    {
                        return flag;
                    }
                    switch (num)
                    {
                        case 1:
                            goto IL_76;
                        case 2:
                            flag = this.TapByImage("DataClick\\image\\nhom", null, 5);
                            continue;
                        case 3:
                            flag = this.TapByText("your groups", "", 3);
                            continue;
                    }
                    return flag;
                }
            }
            return flag;
        }

        public bool GotoFriendSuggest()
        {
            this.GotoNewFeedQuick();
            int num = 1;
            bool flag;
            for (; ; )
            {
            IL_E4:
                flag = this.TapByImage("DataClick\\image\\menu", null, 5);
                for (; ; )
                {
                    num++;
                    if (!flag)
                    {
                        return flag;
                    }
                    switch (num)
                    {
                        case 1:
                            goto IL_E4;
                        case 2:
                            flag = this.TapByImage("DataClick\\image\\banbe", null, 5);
                            continue;
                        case 3:
                            {
                                int num2 = this.CheckExistTexts("", 5.0, new string[]
                                {
                            "\"suggestions\"",
                            "as a friend\""
                                });
                                int num3 = num2;
                                int num4 = num3;
                                if (num4 != 1)
                                {
                                    flag = (num4 == 2);
                                    continue;
                                }
                                flag = this.TapByText("\"suggestions\"", "", 0);
                                continue;
                            }
                    }
                    return flag;
                }
            }
            return flag;
        }

        public bool GotoAcceptFriend()
        {
            this.GotoNewFeedQuick();
            int num = 1;
            bool flag;
            for (; ; )
            {
            IL_E4:
                flag = this.TapByImage("DataClick\\image\\menu", null, 5);
                for (; ; )
                {
                    num++;
                    if (!flag)
                    {
                        return flag;
                    }
                    switch (num)
                    {
                        case 1:
                            goto IL_E4;
                        case 2:
                            flag = this.TapByImage("DataClick\\image\\banbe", null, 5);
                            continue;
                        case 3:
                            {
                                int num2 = this.CheckExistTexts("", 5.0, new string[]
                                {
                            "\"confirm",
                            "\"friend request\""
                                });
                                int num3 = num2;
                                int num4 = num3;
                                flag = (num4 == 1 || (num4 == 2 && this.TapByText("\"friend request\"", "", 0)));
                                continue;
                            }
                    }
                    return flag;
                }
            }
            return flag;
        }

        public bool GotoWall()
        {
            this.GotoNewFeedQuick();
            bool flag = false;
            int num = 1;
            do
            {
                int num2 = num;
                int num3 = num2;
                if (num3 != 1)
                {
                    if (num3 != 2)
                    {
                        return flag;
                    }
                    flag = this.TapByImage("DataClick\\image\\xemtrangcanhancuaban", null, 3);
                }
                else
                {
                    flag = this.TapByImage("DataClick\\image\\menu", null, 3);
                }
                num++;
            }
            while (flag);
            this.GotoProfileQuick("");
            flag = true;
            return flag;
        }

        public bool GotoWatch()
        {
            this.GotoNewFeedQuick();
            bool flag = false;
            int num = 1;
            do
            {
                int num2 = num;
                int num3 = num2;
                if (num3 != 1)
                {
                    if (num3 != 2)
                    {
                        return flag;
                    }
                    flag = this.TapByText("video trên watch", "", 3);
                }
                else
                {
                    flag = this.TapByImage("DataClick\\image\\menu", null, 3);
                }
                num++;
            }
            while (flag);
            this.GotoWatchQuick();
            flag = true;
            return flag;
        }

        public bool GotoSearch(string tuKhoa = "", string typeSearch = "mọi người")
        {
            this.GotoNewFeedQuick();
            bool flag;
            if (!(flag = this.TapByText("search", "", 0)))
            {
                this.GotoSearchQuick();
                flag = true;
            }
            if (flag && tuKhoa != "")
            {
                this.DelayRandom(1.0, 1.5);
                this.InputTextWithUnicode(tuKhoa, 0.1);
                this.DelayRandom(1.0, 1.5);
                this.InputKey(Device.KeyEvent.KEYCODE_ENTER);
                this.DelayRandom(1.5, 2.5);
                this.TapByText(typeSearch, "", 10);
            }
            return flag;
        }

        public string ClearDataAppFacebook()
        {
            string result;
            if (this.CheckExistImage("DataClick\\image\\phoneoremail", null, 0))
            {
                result = "";
            }
            else
            {
                result = this.ClearDataApp("com.facebook.katana");
            }
            return result;
        }

        public void GotoPageHead()
        {
            this.ScrollShort(300, 1);
            this.GotoBack(1, 0.0);
        }

        public string GetHtml()
        {
            string result = "";
            try
            {
                result = ADBHelper.GetXML(this.DeviceId);
            }
            catch
            {
            }
            return result;
        }

        public string ZipFile(string sourceFile, string toFile, int timeOut = 30)
        {
            return ADBHelper.ZipFile(this.DeviceId, sourceFile, toFile, timeOut);
        }

        public string UnZipFile(string filePath, int timeOut = 30)
        {
            return ADBHelper.UnZipFile(this.DeviceId, filePath, timeOut);
        }

        public string DumpScreen(string filePath)
        {
            return ADBHelper.DumpScreen(this.DeviceId, filePath);
        }

        public string ReadFile(string filePath)
        {
            return ADBHelper.ReadFile(this.DeviceId, filePath);
        }

        public string DeleteFile(string filePath)
        {
            return ADBHelper.DeleteFile(this.DeviceId, filePath);
        }

        public string DeleteFolder(string folderPath)
        {
            return ADBHelper.DeleteFolder(this.DeviceId, folderPath);
        }

        public string ClearDataApp(string package)
        {
            return ADBHelper.ClearDataApp(this.DeviceId, package);
        }

        public string PullFile(string fromFilePath, string toFilePath)
        {
            return ADBHelper.PullFile(this.DeviceId, fromFilePath, toFilePath);
        }

        public string PushFile(string fromFilePath, string toFilePath)
        {
            return ADBHelper.PushFile(this.DeviceId, fromFilePath, toFilePath);
        }

        public string ScreenCap(string filePath)
        {
            return ADBHelper.ScreenCap(this.DeviceId, filePath);
        }

        public string View(string link)
        {
            return ADBHelper.View(this.DeviceId, link);
        }

        public void ImportContact(List<string> lstPhoneNumber)
        {
            List<string> contents = this.ConvertLstPhoneNumberToLstContact(lstPhoneNumber);
            string text = this.CreateRandomString(10) + ".vcf";
            File.WriteAllLines(text, contents);
            try
            {
                this.ClearDataApp("com.android.contacts");
                this.ClearDataApp("com.android.providers.contacts");
                using (File.OpenRead(text))
                {
                    this.PushFile(text, "/sdcard/contact.vcf");
                }
                this.ExecuteCMD("shell pm grant com.android.contacts android.permission.READ_EXTERNAL_STORAGE", 10);
                ADBHelper.ImportContact(this.DeviceId, "/sdcard/contact.vcf");
                string html = "";
                if (this.CheckExistText("import contacts from vcard", ref html, 10.0))
                {
                    this.TapByText("\"ok\"", html, 0);
                }
                File.Delete(text);
            }
            catch
            {
            }
        }

        public void ImportContact(string filePath = "contact.vcf")
        {
            try
            {
                this.ClearDataApp("com.android.contacts");
                this.ClearDataApp("com.android.providers.contacts");
                using (File.OpenRead(filePath))
                {
                    this.PushFile(filePath, "/sdcard/contact.vcf");
                }
                ADBHelper.ImportContact(this.DeviceId, "/sdcard/contact.vcf");
                File.Delete(filePath);
            }
            catch
            {
            }
        }

        private List<string> ConvertLstPhoneNumberToLstContact(List<string> lstPhone)
        {
            Random random = new Random();
            List<string> list = new List<string>();
            try
            {
                lstPhone = Common.RemoveEmptyItems(lstPhone);
                string text = string.Concat(new string[]
                {
                    "BEGIN:VCARD",
                    Environment.NewLine,
                    "VERSION:3.0",
                    Environment.NewLine,
                    "N:{{fname}};;;",
                    Environment.NewLine,
                    "FN:{{lname}}",
                    Environment.NewLine,
                    "TEL;TYPE=CELL;TYPE=PREF:{{phone}}",
                    Environment.NewLine,
                    "END:VCARD"
                });
                for (int i = 0; i < lstPhone.Count; i++)
                {
                    string newValue = lstPhone[i];
                    string newValue2 = SetupFolder.first_name[random.Next(0, SetupFolder.first_name.Length)];
                    string newValue3 = SetupFolder.last_name[random.Next(0, SetupFolder.last_name.Length)];
                    string text2 = text;
                    text2 = text2.Replace("{{fname}}", newValue2);
                    text2 = text2.Replace("{{lname}}", newValue3);
                    text2 = text2.Replace("{{phone}}", newValue);
                    list.Add(text2);
                }
            }
            catch
            {
            }
            return list;
        }

        private string ConnectHttpProxy(string proxy)
        {
            return ADBHelper.ConnectHttpProxy(this.DeviceId, proxy);
        }

        public bool ConnectProxy(string proxy)
        {
            bool result;
            if (!this.CheckIsLive())
            {
                result = false;
            }
            else
            {
                bool flag = false;
                if (!string.IsNullOrEmpty(proxy.Trim()))
                {
                    int num = proxy.Split(new char[]
                    {
                        ':'
                    }).Count<string>();
                    int num2 = num;
                    int num3 = num2;
                    if (num3 != 2)
                    {
                        if (num3 != 4)
                        {
                        }
                    }
                    else
                    {
                        this.ConnectHttpProxy(proxy);
                        flag = true;
                    }
                }
                result = flag;
            }
            return result;
        }

        public void RemoveProxy()
        {
            if (this.CheckIsLive())
            {
                ADBHelper.RemoveHttpProxy(this.DeviceId);
            }
        }

        public bool CheckIsLive()
        {
            return this.process == null || !this.process.HasExited;
        }

        public void Swipe(int x1, int y1, int x2, int y2, int duration = 100)
        {
            ADBHelper.Swipe(this.DeviceId, x1, y1, x2, y2, duration);
        }

        public bool SwipeByBounds(string bounds1, string bounds2, int duration = 100)
        {
            try
            {
                Point locationFromBounds = this.GetLocationFromBounds(bounds1);
                int x = locationFromBounds.X;
                int y = locationFromBounds.Y;
                Point locationFromBounds2 = this.GetLocationFromBounds(bounds2);
                int x2 = locationFromBounds2.X;
                int y2 = locationFromBounds2.Y;
                this.Swipe(x, y, x2, y2, duration);
                return true;
            }
            catch
            {
            }
            return false;
        }

        public bool Scroll(int timeScroll = 1000, int typeScroll = 1)
        {
            bool result = false;
            try
            {
                string text = "[100,391][219,423]";
                string text2 = "[181,195][286,226]";
                if (typeScroll == 1)
                {
                    result = this.SwipeByBounds(text, text2, timeScroll);
                }
                else
                {
                    result = this.SwipeByBounds(text2, text, timeScroll);
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        public bool ScrollShort(int timeScroll = 1000, int typeScroll = 1)
        {
            bool result = false;
            try
            {
                string text = "[100,391][219,423]";
                string text2 = "[181,295][286,326]";
                if (typeScroll == 1)
                {
                    result = this.SwipeByBounds(text, text2, timeScroll);
                }
                else
                {
                    result = this.SwipeByBounds(text2, text, timeScroll);
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        public bool ScrollAndCheckScreenNotChange(int timeScroll = 1000, int typeScroll = 1, string bounds1 = "[100,391][219,423]", string bounds2 = "[181,195][286,226]", string boundsCheck = "[72,165][216,294]")
        {
            try
            {
                int num = 3;
                Bitmap bitmap = this.ScreenShoot();
                if (bitmap != null)
                {
                    Bitmap bitmap2 = this.Crop(bitmap, boundsCheck);
                    for (int i = 0; i < num; i++)
                    {
                        if (typeScroll == 1)
                        {
                            this.SwipeByBounds(bounds1, bounds2, timeScroll);
                        }
                        else
                        {
                            this.SwipeByBounds(bounds2, bounds1, timeScroll);
                        }
                        this.DelayTime(1.0);
                        string boundsByImage = this.GetBoundsByImage(bitmap2, null, 0);
                        if (boundsCheck != boundsByImage)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < num; j++)
                    {
                        string html = this.GetHtml();
                        if (typeScroll == 1)
                        {
                            this.SwipeByBounds(bounds1, bounds2, timeScroll);
                        }
                        else
                        {
                            this.SwipeByBounds(bounds2, bounds1, timeScroll);
                        }
                        string html2 = this.GetHtml();
                        if (html != html2)
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return true;
        }

        public Bitmap Crop(Bitmap bm, string bounds)
        {
            string[] array = bounds.Split(new char[]
            {
                '[',
                ',',
                ']'
            });
            return bm.Clone(new Rectangle(Convert.ToInt32(array[1]), Convert.ToInt32(array[2]), Convert.ToInt32(array[4]) - Convert.ToInt32(array[1]), Convert.ToInt32(array[5]) - Convert.ToInt32(array[2])), bm.PixelFormat);
        }

        private Point? FindOutPoint(Bitmap mainBitmap, Bitmap subBitmap, double percent = 0.9)
        {
            Point? result;
            try
            {
                var image = new Image<Bgr, byte>(mainBitmap);
                var image2 = new Image<Bgr, byte>(subBitmap);
                Point? point = null;
                using (var image3 = image.MatchTemplate(image2, (TemplateMatchingType)5))
                {
                    double[] array;
                    double[] array2;
                    Point[] array3;
                    Point[] array4;
                    image3.MinMax(out array, out array2, out array3, out array4);
                    if (array2[0] > percent) point = array4[0];
                }

                result = point;
            }
            catch (Exception ex)
            {
                result = null;
            }

            return result;
        }
        private List<Point> FindOutPoints(Bitmap mainBitmap, Bitmap subBitmap, double percent = 0.9)
        {
            Image<Bgr, byte> image = new Image<Bgr, byte>(mainBitmap);
            Image<Bgr, byte> image2 = new Image<Bgr, byte>(subBitmap);
            List<Point> list = new List<Point>();
            for (; ; )
            {
                using (Image<Gray, float> image3 = image.MatchTemplate(image2, TemplateMatchingType.CcoeffNormed))
                {
                    double[] array;
                    double[] array2;
                    Point[] array3;
                    Point[] array4;
                    image3.MinMax(out array, out array2, out array3, out array4);
                    if (array2[0] <= percent)
                    {
                        break;
                    }
                    Rectangle rect = new Rectangle(array4[0], image2.Size);
                    image.Draw(rect, new Bgr(Color.Blue), -1, LineType.EightConnected, 0);
                    list.Add(array4[0]);
                }
            }
            return list;
        }

        public string GetBoundsByImage(string ImagePath, Bitmap bitmap_screen = null, int timeWait_Second = 0)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(ImagePath);
                FileInfo[] files = directoryInfo.GetFiles();

                int timeStart = Environment.TickCount;
                while (true)
                {
                    if (bitmap_screen == null)
                        bitmap_screen = ScreenShoot();
                    Point? point = new Point?();
                    foreach (FileInfo fileInfo in files)
                    {
                        Bitmap subBitmap = (Bitmap)Image.FromFile(fileInfo.FullName);
                        point = FindOutPoint(bitmap_screen, subBitmap, 0.9);
                        if (point != null)
                        {
                            var value = point.Value;
                            if ((value.X != 0) && (value.Y != 0))
                                return $"[{value.X},{value.Y}][{value.X + subBitmap.Width},{value.Y + subBitmap.Height}]";
                        }
                    }
                    if (Environment.TickCount - timeStart >= timeWait_Second * 1000)
                        break;
                    DelayTime(1);
                    bitmap_screen = ScreenShoot();
                }
            }
            catch (Exception ex)
            {
            }

            return "";
        }

        public string GetBoundsByImage(Bitmap bitmap, Bitmap bitmap_screen = null, int timeWait_Second = 0)
        {
            string str;
            try
            {
                int tickCount = Environment.TickCount;
                while (true)
                {
                    if (bitmap_screen == null)
                    {
                        bitmap_screen = this.ScreenShoot();
                    }
                    Point? nullable = null;
                    nullable = this.FindOutPoint(bitmap_screen, bitmap, 0.9);
                    if (nullable.HasValue)
                    {
                        Point value = nullable.Value;
                        if ((value.X == 0 ? false : value.Y != 0))
                        {
                            str = string.Format("[{0},{1}][{2},{3}]", new object[] { value.X, value.Y, value.X + bitmap.Width, value.Y + bitmap.Height });
                            return str;
                        }
                    }
                    if (Environment.TickCount - tickCount >= timeWait_Second * 1000)
                    {
                        break;
                    }
                    this.DelayTime(1);
                    bitmap_screen = this.ScreenShoot();
                }
            }
            catch (Exception exception)
            {
            }
            str = "";
            return str;
        }

        public List<string> GetListBoundsByImage(string ImagePath, Bitmap bitmap_screen = null, int timeWait_Second = 0)
        {
            Point item;
            List<string> strs;
            bool y;
            List<string> strs1 = new List<string>();
            try
            {
                FileInfo[] files = (new DirectoryInfo(ImagePath)).GetFiles();
                int tickCount = Environment.TickCount;
                while (true)
                {
                    if (bitmap_screen == null)
                    {
                        bitmap_screen = this.ScreenShoot();
                    }
                    List<Point> points = new List<Point>();
                    FileInfo[] fileInfoArray = files;
                    int num = 0;
                    while (num < (int)fileInfoArray.Length)
                    {
                        FileInfo fileInfo = fileInfoArray[num];
                        Bitmap bitmap = (Bitmap)Image.FromFile(fileInfo.FullName);
                        points = this.FindOutPoints(bitmap_screen, bitmap, 0.9);
                        if (points.Count > 0)
                        {
                            for (int i = 0; i < points.Count; i++)
                            {
                                if (points[i].X == 0)
                                {
                                    y = false;
                                }
                                else
                                {
                                    item = points[i];
                                    y = item.Y != 0;
                                }
                                if (y)
                                {
                                    object[] x = new object[4];
                                    item = points[i];
                                    x[0] = item.X;
                                    item = points[i];
                                    x[1] = item.Y;
                                    item = points[i];
                                    x[2] = item.X + bitmap.Width;
                                    item = points[i];
                                    x[3] = item.Y + bitmap.Height;
                                    strs1.Add(string.Format("[{0},{1}][{2},{3}]", x));
                                }
                            }
                            strs = strs1;
                            return strs;
                        }
                        else
                        {
                            num++;
                        }
                    }
                    if (Environment.TickCount - tickCount >= timeWait_Second * 1000)
                    {
                        break;
                    }
                    this.DelayTime(1);
                    bitmap_screen = this.ScreenShoot();
                }
            }
            catch (Exception exception)
            {
            }
            strs = strs1;
            return strs;
        }

        public bool TapByImage(string imagePath, Bitmap bitmap_screen = null, int timeWait = 0)
        {
            try
            {
                this.LoadStatusLD("Find Img: " + imagePath.Substring(imagePath.LastIndexOf('\\') + 1));
                string boundsByImage = this.GetBoundsByImage(imagePath, bitmap_screen, timeWait);
                if (boundsByImage != "")
                {
                    this.LoadStatusLD("Click Img: " + imagePath.Substring(imagePath.LastIndexOf('\\') + 1));
                    return this.TapByBounds(boundsByImage, "");
                }
            }
            catch
            {
            }
            return false;
        }

        public bool TapByImageWait(string imagePath, int timeWaitSearch = 0, int times = 10)
        {
            bool flag;
            try
            {
                int tickCount = Environment.TickCount;
                while (this.CheckIsLive())
                {
                    this.LoadStatusLD(string.Concat("Find Img: ", imagePath.Substring(imagePath.LastIndexOf('\\') + 1)));
                    string boundsByImage = this.GetBoundsByImage(imagePath, null, 0);
                    if (boundsByImage != "")
                    {
                        this.LoadStatusLD(string.Concat("Click Img: ", imagePath.Substring(imagePath.LastIndexOf('\\') + 1)));
                        int num = 0;
                        while (num < times)
                        {
                            if (!this.CheckIsLive())
                            {
                                flag = false;
                                return flag;
                            }
                            else
                            {
                                this.TapByBounds(boundsByImage, "");
                                this.DelayTime(1);
                                string str = this.GetBoundsByImage(imagePath, null, 0);
                                if ((str == "" ? true : str != boundsByImage))
                                {
                                    flag = true;
                                    return flag;
                                }
                                else
                                {
                                    num++;
                                }
                            }
                        }
                    }
                    if (Environment.TickCount - tickCount >= timeWaitSearch * 1000)
                    {
                        flag = false;
                        return flag;
                    }
                    else
                    {
                        this.DelayTime(1);
                    }
                }
            }
            catch
            {
            }
            flag = false;
            return flag;
        }

        public bool WaitForImageAppear(string imagePath, double timeWait_Second = 0.0, bool touch = false)
        {
            try
            {
                int tickCount = Environment.TickCount;
                for (; ; )
                {
                    string boundsByImage = this.GetBoundsByImage(imagePath, null, 0);
                    if (boundsByImage != "")
                    {
                        if (touch == true)
                        {
                            this.TapByBounds(boundsByImage);
                        }

                        break;
                    }
                    if ((double)(Environment.TickCount - tickCount) >= timeWait_Second * 1000.0)
                    {
                        goto IL_65;
                    }
                    this.DelayTime(1.0);
                }
                return true;
            IL_65:;
            }
            catch (Exception)
            {
            }
            return false;
        }

        public bool WaitForImageDisappear(string imagePath, double timeWait_Second = 0.0)
        {
            try
            {
                int tickCount = Environment.TickCount;
                for (; ; )
                {
                    string boundsByImage = this.GetBoundsByImage(imagePath, null, 0);
                    if (boundsByImage == "")
                    {
                        break;
                    }
                    if ((double)(Environment.TickCount - tickCount) >= timeWait_Second * 1000.0)
                    {
                        goto IL_65;
                    }
                    this.DelayTime(1.0);
                }
                return true;
            IL_65:;
            }
            catch (Exception)
            {
            }
            return false;
        }

        public bool CheckExistImage(string imagePath, Bitmap bitmap_screen = null, int timeWait = 0)
        {
            try
            {
                string boundsByImage = this.GetBoundsByImage(imagePath, bitmap_screen, timeWait);
                return boundsByImage != "";
            }
            catch (Exception)
            {
            }
            return false;
        }

        public void InputKey(Device.KeyEvent key)
        {
            this.ExecuteCMD("shell input keyevent " + key.ToString(), 10);
        }

        public void InputText(string text)
        {
            this.LoadStatusLD("Send: " + text);
            ADBHelper.InputText(this.DeviceId, text);
        }

        public void InputTextWithUnicode(string text, double timeDelay = 0.0)
        {
            ADBHelper.SwitchAdbkeyboard(this.DeviceId);
            this.LoadStatusLD("Send: " + text);
            if (timeDelay == 0.0)
            {
                this.ExecuteCMD("shell am broadcast -a ADB_INPUT_B64 --es msg '" + Convert.ToBase64String(Encoding.UTF8.GetBytes(text.ToString())) + "'", 10);
            }
            else
            {
                timeDelay = ((timeDelay > 0.35) ? (timeDelay - 0.35) : 0.0);
                for (int i = 0; i < text.Length; i++)
                {
                    this.ExecuteCMD("shell am broadcast -a ADB_INPUT_B64 --es msg '" + Convert.ToBase64String(Encoding.UTF8.GetBytes(text[i].ToString())) + "'", 10);
                    this.DelayTime(timeDelay);
                }
            }
            ADBHelper.SwitchAndroidKeyboard(this.DeviceId, this.lstPackages);
        }

        public bool CheckExistText(string text, string html = "", double timeWait_Second = 0.0)
        {
            try
            {
                int tickCount = Environment.TickCount;
                for (; ; )
                {
                    if (html == "")
                    {
                        html = this.GetHtml();
                    }
                    if (Regex.IsMatch(html, text + "(.*?)/>"))
                    {
                        goto IL_87;
                    }
                    if ((double)(Environment.TickCount - tickCount) >= timeWait_Second * 1000.0)
                    {
                        break;
                    }
                    this.DelayTime(1.0);
                    html = this.GetHtml();
                }
                goto IL_92;
            IL_87:
                return true;
            }
            catch (Exception)
            {
            }
        IL_92:
            return false;
        }

        public bool CheckExistText(string text, ref string html, double timeWait_Second = 0.0)
        {
            try
            {
                int tickCount = Environment.TickCount;
                for (; ; )
                {
                    if (string.IsNullOrEmpty(html))
                    {
                        html = this.GetHtml();
                    }
                    if (Regex.IsMatch(html, text + "(.*?)/>"))
                    {
                        goto IL_67;
                    }
                    if ((double)(Environment.TickCount - tickCount) >= timeWait_Second * 1000.0)
                    {
                        break;
                    }
                    this.DelayTime(1.0);
                    html = this.GetHtml();
                }
                goto IL_6E;
            IL_67:
                return true;
            }
            catch
            {
            }
        IL_6E:
            return false;
        }

        public string CheckExistTextsV2(string html, double timeWait_Second, params string[] text)
        {
            try
            {
                int tickCount = Environment.TickCount;
                int i;
                for (; ; )
                {
                    if (html == "")
                    {
                        html = this.GetHtml();
                    }
                    for (i = 0; i < text.Length; i++)
                    {
                        if (Regex.IsMatch(html, text[i] + "(.*?)/>"))
                        {
                            goto IL_AD;
                        }
                    }
                    if ((double)(Environment.TickCount - tickCount) >= timeWait_Second * 1000.0)
                    {
                        break;
                    }
                    this.DelayTime(1.0);
                    html = this.GetHtml();
                }
                goto IL_BA;
            IL_AD:
                return text[i];
            }
            catch
            {
            }
        IL_BA:
            return "";
        }

        public int CheckExistTexts(ref string html, double timeWait_Second = 0.0, params string[] text)
        {
            int result = 0;
            try
            {
                int tickCount = Environment.TickCount;
                int i;
                for (; ; )
                {
                    if (html == "")
                    {
                        html = this.GetHtml();
                    }
                    for (i = 0; i < text.Length; i++)
                    {
                        if (Regex.IsMatch(html, text[i] + "(.*?)/>"))
                        {
                            goto IL_7F;
                        }
                    }
                    if ((double)(Environment.TickCount - tickCount) >= timeWait_Second * 1000.0)
                    {
                        break;
                    }
                    this.DelayTime(1.0);
                    html = this.GetHtml();
                }
                goto IL_89;
            IL_7F:
                return i + 1;
            }
            catch (Exception)
            {
            }
        IL_89:
            return result;
        }

        public int CheckExistTexts(string html = "", double timeWait_Second = 0.0, params string[] text)
        {
            int result = 0;
            try
            {
                int tickCount = Environment.TickCount;
                int i;
                for (; ; )
                {
                    if (html == "")
                    {
                        html = this.GetHtml();
                    }
                    for (i = 0; i < text.Length; i++)
                    {
                        if (Regex.IsMatch(html, text[i] + "(.*?)/>"))
                        {
                            goto IL_B2;
                        }
                    }
                    if ((double)(Environment.TickCount - tickCount) >= timeWait_Second * 1000.0)
                    {
                        break;
                    }
                    this.DelayTime(1.0);
                    html = this.GetHtml();
                }
                goto IL_C0;
            IL_B2:
                return i + 1;
            }
            catch
            {
            }
        IL_C0:
            return result;
        }

        public int CheckExistTexts(string html, double timeWait_Second, Dictionary<int, List<string>> dic)
        {
            int result = 0;
            try
            {
                int tickCount = Environment.TickCount;
                for (; ; )
                {
                    if (html == "")
                    {
                        html = this.GetHtml();
                    }
                    foreach (KeyValuePair<int, List<string>> keyValuePair in dic)
                    {
                        foreach (string str in keyValuePair.Value)
                        {
                            if (Regex.IsMatch(html, str + "(.*?)/>"))
                            {
                                return keyValuePair.Key;
                            }
                        }
                    }
                    if ((double)(Environment.TickCount - tickCount) >= timeWait_Second * 1000.0)
                    {
                        break;
                    }
                    this.DelayTime(1.0);
                    html = this.GetHtml();
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        public int CheckExistTexts(string html, double timeWait_Second, Dictionary<int, string> dic)
        {
            int result = 0;
            try
            {
                int tickCount = Environment.TickCount;
                for (; ; )
                {
                    if (html == "")
                    {
                        html = this.GetHtml();
                    }
                    foreach (KeyValuePair<int, string> keyValuePair in dic)
                    {
                        if (Regex.IsMatch(html, keyValuePair.Value + "(.*?)/>"))
                        {
                            return keyValuePair.Key;
                        }
                    }
                    if ((double)(Environment.TickCount - tickCount) >= timeWait_Second * 1000.0)
                    {
                        break;
                    }
                    this.DelayTime(1.0);
                    html = this.GetHtml();
                }
            }
            catch
            {
            }
            return result;
        }

        public bool WaitForTextAppear(double timeWait_Second = 0.0, params string[] text)
        {
            try
            {
                int tickCount = Environment.TickCount;
                for (; ; )
                {
                    string html = this.GetHtml();
                    for (int i = 0; i < text.Length; i++)
                    {
                        if (Regex.IsMatch(html, text[i] + "(.*?)/>"))
                        {
                            goto IL_81;
                        }
                    }
                    if ((double)(Environment.TickCount - tickCount) >= timeWait_Second * 1000.0)
                    {
                        break;
                    }
                    this.DelayTime(1.0);
                }
                goto IL_8D;
            IL_81:
                return true;
            }
            catch (Exception)
            {
            }
        IL_8D:
            return false;
        }

        public bool WaitForTextDisappear(double timeWait_Second = 0.0, params string[] text)
        {
            try
            {
                int tickCount = Environment.TickCount;
                for (; ; )
                {
                    string html = this.GetHtml();
                    for (int i = 0; i < text.Length; i++)
                    {
                        if (!Regex.IsMatch(html, text[i] + "(.*?)/>"))
                        {
                            goto IL_88;
                        }
                    }
                    if ((double)(Environment.TickCount - tickCount) >= timeWait_Second * 1000.0)
                    {
                        break;
                    }
                    this.DelayTime(1.0);
                }
                goto IL_94;
            IL_88:
                return true;
            }
            catch
            {
            }
        IL_94:
            return false;
        }

        public string GetBoundsByText(string text, string html = "", int timeWait_Second = 0)
        {
            try
            {
                this.LoadStatusLD("Find Text: " + text);
                int tickCount = Environment.TickCount;
                while (this.CheckIsLive())
                {
                    if (html == "")
                    {
                        html = this.GetHtml();
                    }
                    string value = Regex.Match(html, text.Replace("[", "\\[").Replace("]", "\\]") + "(.*?)/>").Groups[1].Value;
                    if (value != "")
                    {
                        return Regex.Match(value, "bounds=\"(.*?)\"").Groups[1].Value;
                    }
                    if (Environment.TickCount - tickCount >= timeWait_Second * 1000)
                    {
                        break;
                    }
                    this.DelayTime(1.0);
                    html = this.GetHtml();
                }
            }
            catch (Exception)
            {
            }
            return "";
        }

        public List<string> GetListBoundsByText(string text, string html = "", int timeWait_Second = 0)
        {
            List<string> list = new List<string>();
            try
            {
                int tickCount = Environment.TickCount;
                for (; ; )
                {
                    if (html == "")
                    {
                        html = this.GetHtml();
                    }
                    MatchCollection matchCollection = Regex.Matches(html, text.Replace("[", "\\[").Replace("]", "\\]") + "(.*?)/>");
                    for (int i = 0; i < matchCollection.Count; i++)
                    {
                        list.Add(Regex.Match(matchCollection[i].Value, "bounds=\"(.*?)\"").Groups[1].Value);
                    }
                    if (list.Count > 0 || Environment.TickCount - tickCount >= timeWait_Second * 1000)
                    {
                        break;
                    }
                    this.DelayTime(1.0);
                    html = this.GetHtml();
                }
            }
            catch (Exception)
            {
            }
            return list;
        }

        public bool CheckBoundsContainBounds(string bounds, string subBounds)
        {
            string[] array = subBounds.Split(new char[]
            {
                '[',
                ',',
                ']'
            });
            return this.CheckBoundsContainLocation(bounds, Convert.ToInt32(array[1]), Convert.ToInt32(array[2])) && this.CheckBoundsContainLocation(bounds, Convert.ToInt32(array[4]), Convert.ToInt32(array[5]));
        }

        public bool CheckBoundsContainLocation(string bounds, Point point)
        {
            int x = point.X;
            int y = point.Y;
            return this.CheckBoundsContainLocation(bounds, x, y);
        }

        public bool CheckBoundsContainLocation(string bounds, int x, int y)
        {
            string[] array = bounds.Split(new char[]
            {
                '[',
                ',',
                ']'
            });
            return x >= Convert.ToInt32(array[1]) && x <= Convert.ToInt32(array[4]) && y >= Convert.ToInt32(array[2]) && y <= Convert.ToInt32(array[5]);
        }

        public bool Tap(int x, int y, int delay = 1)
        {
            bool result;
            if (this.CheckBoundsContainLocation("[0,0][540,960]", x, y))
            {
                ADBHelper.Tap(this.DeviceId, x, y);
                this.DelayTime((double)delay);
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        public bool Tap(Point point, int delay = 1)
        {
            bool result;
            if (this.CheckBoundsContainLocation("[0,0][540,960]", point.X, point.Y))
            {
                ADBHelper.Tap(this.DeviceId, point.X, point.Y);
                this.DelayTime((double)delay);
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        public Point GetLocationFromBounds(string bounds)
        {
            try
            {
                string[] array = bounds.Split(new char[]
                {
                    '[',
                    ',',
                    ']'
                });
                int x = this.rd.Next(Convert.ToInt32(array[1]), Convert.ToInt32(array[4]));
                int y = this.rd.Next(Convert.ToInt32(array[2]), Convert.ToInt32(array[5]));
                return new Point(x, y);
            }
            catch
            {
            }
            return default(Point);
        }

        public bool TapByBounds(string bounds, string onlyInBounds = "")
        {
            try
            {
                Point locationFromBounds = this.GetLocationFromBounds(bounds);
                int x = locationFromBounds.X;
                int y = locationFromBounds.Y;
                if (onlyInBounds == "" || this.CheckBoundsContainLocation(onlyInBounds, x, y))
                {
                    return this.Tap(x, y, 1);
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        public bool TapByText(string text, string html = "", int timeWait_Second = 0)
        {
            string boundsByText = this.GetBoundsByText(text, html, timeWait_Second);
            bool result;
            if (!string.IsNullOrEmpty(boundsByText))
            {
                this.LoadStatusLD("Click Text: " + text);
                result = this.TapByBounds(boundsByText, "");
            }
            else
            {
                result = false;
            }
            return result;
        }

        public bool TapByXpath(string xPath, string html = "", int timeWait_Second = 0)
        {
            bool flag;
            string valueByXpath = "";
            int tickCount = Environment.TickCount;
            while (true)
            {
                if (this.CheckIsLive())
                {
                    if (html == "")
                    {
                        html = this.GetHtml();
                    }
                    valueByXpath = this.GetValueByXpath(html, xPath, "bounds");
                    if (valueByXpath != "" || Environment.TickCount - tickCount >= timeWait_Second * 1000)
                    {
                        if (string.IsNullOrEmpty(valueByXpath))
                        {
                            break;
                        }
                        flag = this.TapByBounds(valueByXpath, "");
                        return flag;
                    }
                    else
                    {
                        this.DelayTime(1);
                        html = this.GetHtml();
                    }
                }
                else
                {
                    break;
                }
            }
            flag = false;
            return flag;
        }

        public bool TapLong(int x, int y, int duration = -1)
        {
            bool result;
            if (this.CheckBoundsContainLocation("[0,0][540,960]", x, y))
            {
                if (duration == -1)
                {
                    duration = this.GetRandomInt(1000, 3000);
                }
                ADBHelper.TapLong(this.DeviceId, x, y, duration);
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        public bool TapLongByBounds(string bounds, string onlyInBounds = "")
        {
            try
            {
                Point locationFromBounds = this.GetLocationFromBounds(bounds);
                int x = locationFromBounds.X;
                int y = locationFromBounds.Y;
                if (onlyInBounds == "" || this.CheckBoundsContainLocation(onlyInBounds, x, y))
                {
                    return this.TapLong(x, y, 3000);
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        public bool TapLongByText(string text, string html = "", int timeWaitForSearch_Second = 0)
        {
            string boundsByText = this.GetBoundsByText(text, html, timeWaitForSearch_Second);
            return !string.IsNullOrEmpty(boundsByText) && this.TapLongByBounds(boundsByText, "");
        }

        public string GetValueByXpath(string xml, string xpath, string attribute)
        {
            string result = "";
            try
            {
                if (xml == "")
                {
                    xml = this.GetHtml();
                }
                xml = Regex.Match(xml, "<\\?xml(.*?)</hierarchy>").Value;
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xml);
                XmlNodeList xmlNodeList = xmlDocument.SelectNodes(xpath);
                result = xmlNodeList[xmlNodeList.Count - 1].Attributes[attribute].Value;
            }
            catch
            {
            }
            return result;
        }

        public bool CheckExistXpath(string xpath, string xml = "")
        {
            try
            {
                if (xml == "")
                {
                    xml = this.GetHtml();
                }
                xml = Regex.Match(xml, "<\\?xml(.*?)</hierarchy>").Value;
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xml);
                XmlNodeList xmlNodeList = xmlDocument.SelectNodes(xpath);
                if (xmlNodeList.Count > 0)
                {
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        public bool ClearText(string defaultValue = "", string xpath = "//*[@class='android.widget.edittext']", string attribute = "text")
        {
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    string html = this.GetHtml();
                    string valueByXpath = this.GetValueByXpath(html, xpath, attribute);
                    if (valueByXpath == defaultValue || valueByXpath.Length == 0)
                    {
                        if (defaultValue != "")
                        {
                            this.TapByText(defaultValue, html, 0);
                        }
                        break;
                    }
                    string[] array = this.GetValueByXpath(html, xpath, "bounds").Split(new char[]
                    {
                        '[',
                        ',',
                        ']'
                    });
                    Point point = new Point(int.Parse(array[4]) - 1, int.Parse(array[5]) - 1);
                    this.Tap(point, 1);
                    this.DelayTime(1.0);
                    int num = valueByXpath.Length / 30 + ((valueByXpath.Length % 30 > 0) ? 1 : 0);
                    string text = "";
                    for (int j = 0; j < 30; j++)
                    {
                        text += "input keyevent KEYCODE_DEL && ";
                    }
                    text = "shell \"" + text.TrimEnd(new char[]
                    {
                        ' ',
                        '&'
                    }) + "\"";
                    for (int k = 0; k < num; k++)
                    {
                        this.ExecuteCMD(text, 10);
                    }
                }
            }
            catch
            {
            }
            return false;
        }

        public string GetActivity()
        {
            try
            {
                if (!this.CheckIsLive())
                {
                    return "";
                }
                return ADBHelper.DumpActivity(this.DeviceId).Split(new char[]
                {
                    '{',
                    '}'
                })[1].Split(new char[]
                {
                    ' '
                })[2];
            }
            catch
            {
            }
            return "";
        }

        public void OpenAppChange(string package)
        {
            if (this.CheckIsLive())
            {
                ADBHelper.OpenAppChange(this.DeviceId, package);
            }
        }

        public void OpenApp(string package)
        {
            if (this.CheckIsLive())
            {
                ADBHelper.OpenApp(this.DeviceId, package);
            }
        }

        public void CloseApp(string package)
        {
            if (this.CheckIsLive())
            {
                ADBHelper.CloseApp(this.DeviceId, package);
            }
        }

        public bool InstallApp(string fileApkFromComputer)
        {
            bool result;
            if (!this.CheckIsLive())
            {
                result = false;
            }
            else
            {
                ADBHelper.InstallApp(this.DeviceId, fileApkFromComputer);
                result = true;
            }
            return result;
        }

        public bool UninstallApp(string package)
        {
            return this.CheckIsLive() && ADBHelper.UninstallApp(this.DeviceId, package).Trim().ToLower() == "success";
        }

        public List<string> GetListPackages()
        {
            return ADBHelper.GetListPackages(this.DeviceId);
        }

        public List<string> GetListText(string html = "", int type = 0)
        {
            if (html == "")
            {
                html = this.GetHtml();
            }
            List<string> listRegexGroup = this.GetListRegexGroup1(html, "text=\"(.*?)\"");
            List<string> listRegexGroup2 = this.GetListRegexGroup1(html, "content-desc=\"(.*?)\"");
            List<string> listRegexGroup3 = this.GetListRegexGroup1(html, "text='(.*?)'");
            List<string> listRegexGroup4 = this.GetListRegexGroup1(html, "content-desc='(.*?)'");
            List<string> list = new List<string>();
            switch (type)
            {
                case 0:
                    list.AddRange(listRegexGroup);
                    list.AddRange(listRegexGroup2);
                    list.AddRange(listRegexGroup3);
                    list.AddRange(listRegexGroup4);
                    break;
                case 1:
                    list.AddRange(listRegexGroup);
                    list.AddRange(listRegexGroup3);
                    break;
                case 2:
                    list.AddRange(listRegexGroup2);
                    list.AddRange(listRegexGroup4);
                    break;
            }
            return list;
        }

        private List<string> GetListRegexGroup1(string input, string pattern)
        {
            List<string> list = new List<string>();
            try
            {
                MatchCollection matchCollection = Regex.Matches(input, pattern);
                for (int i = 0; i < matchCollection.Count; i++)
                {
                    if (!string.IsNullOrEmpty(matchCollection[i].Groups[1].Value))
                    {
                        list.Add(matchCollection[i].Groups[1].Value);
                    }
                }
            }
            catch (Exception)
            {
            }
            return list;
        }

        public void InputKeyBackspace()
        {
            this.InputKey(Device.KeyEvent.KEYCODE_DEL);
        }

        public void InputKeyBackspace(int times = 1)
        {
            string text = "";
            for (int i = 0; i < times; i++)
            {
                text += "input keyevent KEYCODE_DEL && ";
            }
            text = "shell \"" + text.TrimEnd(new char[]
            {
                ' ',
                '&'
            }) + "\"";
            this.ExecuteCMD(text, 10);
        }

        public string ExecuteCMD(string cmd, int timeout = 10)
        {
            string result;
            if (!this.CheckIsLive())
            {
                result = "";
            }
            else
            {
                result = ADBHelper.RunCMD(this.DeviceId, cmd, timeout);
            }
            return result;
        }

        public void DelayTime(double second)
        {
            if (this.CheckIsLive())
            {
                Application.DoEvents();
                Thread.Sleep(Convert.ToInt32(second * 1000.0));
            }
        }

        public void DelayRandom(double timeFrom, double timeTo)
        {
            if (this.CheckIsLive())
            {
                Thread.Sleep(this.rd.Next(Convert.ToInt32(timeFrom * 1000.0), Convert.ToInt32(timeTo * 1000.0 + 1.0)));
                Application.DoEvents();
            }
        }

        public int GetRandomInt(int from, int to)
        {
            int result;
            try
            {
                result = this.rd.Next(from, to + 1);
            }
            catch
            {
                result = (from + to) / 2;
            }
            return result;
        }

        public string CreateRandomString(int lengText)
        {
            string text = "";
            string text2 = "abcdefghijklmnopqrstuvwxyz";
            for (int i = 0; i < lengText; i++)
            {
                text += text2[this.rd.Next(0, text2.Length)].ToString();
            }
            return text;
        }

        public string CreateRandomString(int lengText = 32, string format = "0_a_A", Random random = null)
        {
            string text = "";
            string[] source = format.Split(new char[]
            {
                '_'
            });
            if (source.Contains("A"))
            {
                text += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            }
            if (source.Contains("a"))
            {
                text += "abcdefghijklmnopqrstuvwxyz";
            }
            if (source.Contains("0"))
            {
                text += "0123456789";
            }
            char[] array = new char[lengText];
            if (random == null)
            {
                random = this.rd;
            }
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = text[random.Next(text.Length)];
            }
            return new string(array);
        }

        public bool SwipeAndCheckScreenChange(int x1, int y1, int x2, int y2, int duration, int timeDelaySwipe = 1000, int loop = 1)
        {
            bool result = false;
            for (int i = 0; i < loop; i++)
            {
                string html = this.GetHtml();
                ADBHelper.Swipe(this.DeviceId, x1, y1, x2, y2, Common.GetRandInt(500, 1000));
                Thread.Sleep(timeDelaySwipe);
                string html2 = this.GetHtml();
                if (html != html2)
                {
                    result = true;
                }
            }
            return result;
        }

        public void ExportError(Exception ex, string error = "")
        {
            try
            {
                if (this.CheckIsLive())
                {
                    string text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    string text2 = this.IndexDevice.ToString() + "_" + text.Replace("/", "_").Replace(" ", "_").Replace(":", "_");
                    Directory.CreateDirectory("log");
                    if (!string.IsNullOrEmpty(this.DeviceId))
                    {
                        File.WriteAllText("log\\" + text2 + ".xml", this.GetHtml());
                        this.ScreenShoot("log", text2 + ".png");
                    }
                    using (StreamWriter streamWriter = new StreamWriter("log\\log.txt", true))
                    {
                        streamWriter.WriteLine("-----------------------------------------------------------------------------");
                        streamWriter.WriteLine("Date: " + text);
                        streamWriter.WriteLine("LDPlayer: " + text2);
                        if (error != "")
                        {
                            streamWriter.WriteLine("Error: " + error);
                        }
                        streamWriter.WriteLine();
                        if (ex != null)
                        {
                            streamWriter.WriteLine("Type: " + ex.GetType().FullName);
                            streamWriter.WriteLine("Message: " + ex.Message);
                            streamWriter.WriteLine("StackTrace: " + ex.StackTrace);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public bool TapByTextWithPopupAppear(int countCheck, string textCheck, string[] arrText)
        {
            bool result = false;
            for (int i = 0; i < countCheck; i++)
            {
                string html = this.GetHtml();
                List<string> list = new List<string>
                {
                    textCheck
                };
                list.AddRange(arrText);
                int num = this.CheckExistTexts(html, 0.0, list.ToArray());
                if (num != 0)
                {
                    this.TapByText(list[num - 1], html, 0);
                }
                if (num == 1)
                {
                    result = true;
                    return result;
                }
                this.DelayTime(1.0);
            }
            return result;
        }

        public virtual void BackupTelegramApp(string packageName, string phone)
        {
            if (this.CheckIsLive() && !string.IsNullOrEmpty(phone))
            {
                string tmpPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                if (!Directory.Exists(tmpPath))
                {
                    Directory.CreateDirectory(tmpPath);
                }

                this.CloseApp("org.telegram.messenger");
                this.DelayTime(1.0);
                ADBHelper.RunCMD(this.PathLDPlayer + string.Format("\\dnconsole.exe killapp --index {0} --packagename org.telegram.messenger", this.IndexDevice), 10);
                this.DelayTime(1.0);

                string text2 = FileHelper.GetPathToCurrentFolder() + "\\output\\" + phone;

                string cmd = "adb pull /data/data/org.telegram.messenger/* \"" + tmpPath + "\"";
                ADBHelper.RunCMD(this.DeviceId, cmd, 10);
            }
        }

        public string RunCMD(string cmd)
        {
            return ADBHelper.RunCMD(this.DeviceId, cmd, 10);
        }

        public virtual void BackupApp(string packageName, string filePath)
        {
            try
            {
                string tmpPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                if (!Directory.Exists(tmpPath))
                {
                    Directory.CreateDirectory(tmpPath);
                }

                // Kill
                this.CloseApp(packageName);
                this.DelayTime(1.0);
                ADBHelper.RunCMD(this.PathLDPlayer + string.Format("\\dnconsole.exe killapp --index {0} --packagename {1}", this.IndexDevice, packageName), 10);
                this.DelayTime(1.0);

                //this.ExecuteCMD("shell \"su -c 'chmod 777 /data/data/" + packageName + "'\"");
                //this.ExecuteCMD("shell \"su -c 'chmod 777 /data/data/" + packageName + "/shared_prefs'\"");
                //this.ExecuteCMD("shell \"su -c 'chmod 777 /data/data/" + packageName + "/shared_prefs/*.*'\"");

                // Create
                Common.CreateFolder(tmpPath + "\\lib");
                Common.CreateFolder(tmpPath + "\\cache");
                Common.CreateFolder(tmpPath + "\\databases");
                Common.CreateFolder(tmpPath + "\\no_backup");
                Common.CreateFolder(tmpPath + "\\files");
                Common.CreateFolder(tmpPath + "\\shared_prefs");

                string cmd1 = "adb pull /data/data/org.telegram.messenger/lib \"" + tmpPath + "\\lib" + "\"";
                ADBHelper.RunCMD(this.DeviceId, cmd1, 10);
                string cmd2 = "adb pull /data/data/org.telegram.messenger/cache \"" + tmpPath + "\\cache" + "\"";
                ADBHelper.RunCMD(this.DeviceId, cmd2, 10);
                string cmd3 = "adb pull /data/data/org.telegram.messenger/databases \"" + tmpPath + "\\databases" + "\"";
                ADBHelper.RunCMD(this.DeviceId, cmd3, 10);
                string cmd4 = "adb pull /data/data/org.telegram.messenger/no_backup \"" + tmpPath + "\\no_backup" + "\"";
                ADBHelper.RunCMD(this.DeviceId, cmd4, 10);
                string cmd5 = "adb pull /data/data/org.telegram.messenger/files \"" + tmpPath + "\\files" + "\"";
                ADBHelper.RunCMD(this.DeviceId, cmd5, 10);
                string cmd6 = "adb pull /data/data/org.telegram.messenger/shared_prefs \"" + tmpPath + "\\shared_prefs" + "\"";
                ADBHelper.RunCMD(this.DeviceId, cmd6, 10);

                //this.PullFile("/data/data/" + packageName, tmpPath);
                File.WriteAllText(Path.Combine(tmpPath, "package.txt"), packageName);
                System.IO.Compression.ZipFile.CreateFromDirectory(tmpPath, filePath);
                Directory.Delete(tmpPath, true);
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "BackupApp()");
            }
        }

        public virtual void RestoreApp(string packageName, string filePath)
        {
            try
            {
                string tmpPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                string packageInfoPath = Path.Combine(tmpPath, "package.txt");
                System.IO.Compression.ZipFile.ExtractToDirectory(filePath, tmpPath);
                string oldpackage = packageName;
                if (File.Exists(packageInfoPath))
                {
                    oldpackage = File.ReadAllText(packageInfoPath);
                }
                this.ExecuteCMD("shell \"su -c 'chmod 777 /data/data/" + packageName + "'\"");
                this.ExecuteCMD("shell mkdir /data/data/" + packageName + "/shared_prefs");
                this.ExecuteCMD("shell \"su -c 'chmod 777 /data/data/" + packageName + "/shared_prefs'\"");
                foreach (string file in Directory.GetFiles(Path.Combine(tmpPath, "shared_prefs")))
                {
                    string fileName = Path.GetFileName(file);
                    if (fileName.Contains(oldpackage))
                    {
                        fileName = fileName.Replace(oldpackage, packageName);
                        string newPath = Path.Combine(tmpPath, "shared_prefs", fileName);
                        File.Move(file, newPath);
                    }
                }

                string cmd1 = "adb push \"" + tmpPath + "\\shared_prefs" + "\" /data/data/org.telegram.messenger/shared_prefs";
                string cmd2 = "adb push \"" + tmpPath + "\\databases" + "\" /data/data/org.telegram.messenger/databases";
                string cmd3 = "adb push \"" + tmpPath + "\\files" + "\" /data/data/org.telegram.messenger/files";
                string cmd4 = "adb push \"" + tmpPath + "\\no_backup" + "\" /data/data/org.telegram.messenger/no_backup";
                ADBHelper.RunCMD(this.DeviceId, cmd1, 10);
                ADBHelper.RunCMD(this.DeviceId, cmd2, 10);
                ADBHelper.RunCMD(this.DeviceId, cmd3, 10);
                ADBHelper.RunCMD(this.DeviceId, cmd4, 10);

                this.PushFile(tmpPath, "/data/data/" + packageName);
                Directory.Delete(tmpPath, true);
            }
            catch (Exception ex)
            {
                Common.ExportError(ex, "RestoreApp()");
            }

        }


        #region Hàm Click UI

        public bool TouchByUIElemnent(TouchLocation touchLocation, int sleep = 0)
        {
            bool flag;
            if (touchLocation != null)
            {
                if (sleep > 0)
                {
                    Thread.Sleep(sleep);
                }
                if (!string.IsNullOrEmpty(this.ExecuteCMD(string.Format("shell input tap {0} {1}", touchLocation.x, touchLocation.y))))
                {
                    flag = false;
                    return flag;
                }
                flag = true;
                return flag;
            }
            flag = false;
            return flag;
        }

        public bool TouchByUIElemnent(UiElement uiElement, bool randomTouch = false, int sleep = 0)
        {
            bool flag;
            if (!uiElement.IsVisible())
            {
                flag = false;
            }
            else
            {
                flag = (!randomTouch ? this.TouchByUIElemnent(uiElement.touchLocation, sleep) : this.TouchByUIElemnent(uiElement.GetRandomPoint(), sleep));
            }
            return flag;
        }

        public bool SendText(string inputText)
        {
            bool flag;
            flag = (string.IsNullOrEmpty(inputText) || !string.IsNullOrEmpty(this.ExecuteCMD(string.Concat("shell input text \"", inputText.Replace(" ", "%s"), "\""))) ? false : true);
            return flag;
        }

        public bool SendTextUnicode(string inputText)
        {
            bool flag;
            flag = (string.IsNullOrEmpty(inputText) || !string.IsNullOrEmpty(this.ExecuteCMD("shell am broadcast -a ADB_INPUT_TEXT --es msg '" + inputText + "'")) ? false : true);
            return flag;
        }

        public bool SetEditTextByUIElement(UiElement uiElement, string inputText, bool fakeUser = false)
        {
            bool flag;
            if ((!uiElement.IsVisible() ? true : string.IsNullOrEmpty(inputText)))
            {
                flag = false;
            }
            else if (!fakeUser)
            {
                flag = this.SendText(inputText);
            }
            else
            {
                string str = inputText.Substring(inputText.Length / 2);
                string str1 = inputText.Replace(str, "");
                string[] strArrays = new string[] { str1, str };
                for (int i = 0; i < (int)strArrays.Length; i++)
                {
                    this.SendText(strArrays[i]);
                    Thread.Sleep((new Random()).Next(200, 800));
                }
                flag = true;
            }
            return flag;
        }

        public UiElement FindNextButton()
        {
            return this.FindElementByTextClassPackage("Next", "android.widget.Button", Device.fbPackage, true);
        }

        public UiElement FindFirstNameInput()
        {
            UiElement uiElement;
            UiElement uiElement1 = null;
            UiElement uiElement2 = this.FindElementByClassPackage("android.widget.ScrollView", Device.fbPackage);
            if (uiElement2.IsVisible())
            {
                UiElement uiElement3 = uiElement2.FindElementByIndexClassPackage(0, "android.widget.RelativeLayout", Device.fbPackage);
                if (uiElement3.IsVisible())
                {
                    UiElement uiElement4 = uiElement3.FindElementByIndexClassPackage(0, "android.widget.LinearLayout", Device.fbPackage);
                    if (uiElement4.IsVisible())
                    {
                        UiElement uiElement5 = uiElement4.FindElementByIndexClassPackage(3, "android.widget.LinearLayout", Device.fbPackage);
                        if (uiElement5.IsVisible())
                        {
                            UiElement uiElement6 = uiElement5.FindElementByIndexClassPackage(0, "android.widget.LinearLayout", Device.fbPackage);
                            if (uiElement6.IsVisible())
                            {
                                UiElement uiElement7 = uiElement6.FindElementByIndexClassPackage(0, "android.widget.FrameLayout", Device.fbPackage);
                                if (!uiElement7.IsVisible())
                                {
                                    goto Label1;
                                }
                                uiElement = uiElement7.FindElementByClassPackage("android.widget.EditText", Device.fbPackage);
                                return uiElement;
                            }
                        }
                    }
                }
            Label1:
                if (uiElement1 == null)
                {
                    uiElement3 = uiElement2.FindElementByIndexClassPackage(0, "android.widget.RelativeLayout", Device.fbPackage);
                    if (uiElement3.IsVisible())
                    {
                        UiElement uiElement8 = uiElement3.FindElementByIndexClassPackage(0, "android.widget.LinearLayout", Device.fbPackage);
                        if (uiElement8.IsVisible())
                        {
                            UiElement uiElement9 = uiElement8.FindElementByIndexClassPackage(4, "android.widget.LinearLayout", Device.fbPackage);
                            if (uiElement9.IsVisible())
                            {
                                UiElement uiElement10 = uiElement9.FindElementByIndexClassPackage(0, "android.widget.LinearLayout", Device.fbPackage);
                                if (uiElement10.IsVisible())
                                {
                                    UiElement uiElement11 = uiElement10.FindElementByIndexClassPackage(0, "android.widget.FrameLayout", Device.fbPackage);
                                    if (!uiElement11.IsVisible())
                                    {
                                        uiElement = uiElement1;
                                        return uiElement;
                                    }
                                    uiElement = uiElement11.FindElementByClassPackage("android.widget.EditText", Device.fbPackage);
                                    return uiElement;
                                }
                            }
                        }
                    }
                }
            }
            uiElement = uiElement1;
            return uiElement;
        }

        public bool OpenFacebookAppWithRegistration()
        {
            try
            {
                this.ExecuteCMD(string.Concat("shell am start -n ", Device.fbPackage, "/com.facebook.registration.activity.AccountRegistrationActivity"));
                return true;
            }
            catch
            {
            }
            return false;
        }

        public bool CreateAccount()
        {
            bool flag;
            UiElement uiElement = this.FindElementByTextClassPackage("Skip", "android.widget.TextView", Device.fbPackage, true);
            if (!uiElement.IsVisible())
            {
                uiElement = this.FindElementByTextClassPackage("Skip", "android.widget.Button", Device.fbPackage, true);
                flag = (!uiElement.IsVisible() ? false : this.TouchByUIElemnent(uiElement, true, 0));
            }
            else
            {
                flag = this.TouchByUIElemnent(uiElement, true, 0);
            }
            return flag;
        }

        #endregion

        #region Thư viện

        public XmlNodeList GetXmlNodeList()
        {
            XmlNodeList elementsByTagName;
            string html = ADBHelper.GetXMLFull(this.DeviceId);
            if (!string.IsNullOrEmpty(html))
            {
                try
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(html);
                    elementsByTagName = xmlDocument.GetElementsByTagName("node");
                    return elementsByTagName;
                }
                catch (Exception exception)
                {
                }
            }
            elementsByTagName = null;
            return elementsByTagName;
        }

        public UiElement FindElementByTextClassPackage(string text, string className, string package, bool lowerText = false)
        {
            UiElement uiElement;

            XmlNodeList xmlNodeLists = this.GetXmlNodeList();
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

        #endregion

        public static string fbPackage = "com.facebook.katana";

        public int IndexDevice = -1;

        public bool isRunning = false;

        public int demDisconnect = 0;

        public int maxDisconnect = 3;

        public List<string> lstPackages = new List<string>();

        private string link_notifications = "fb://notifications";

        private string link_search = "fb://search";

        private string link_watch = "fb://watch";

        private string link_friends = "fb://friends";

        private string link_profile = "fb://profile";

        private string link_group = "fb://group";

        private string link_page = "fb://page";

        private string link_feed = "fb://feed";

        private string link_syncContact = "com.facebook.katana/com.facebook.growth.friendfinder.FriendFinderStartActivity";

        private int countOpenAppAgain = 0;

        public enum KeyEvent
        {
            // Token: 0x04000284 RID: 644
            KEYCODE_0,
            // Token: 0x04000285 RID: 645
            KEYCODE_SOFT_LEFT,
            // Token: 0x04000286 RID: 646
            KEYCODE_SOFT_RIGHT,
            // Token: 0x04000287 RID: 647
            KEYCODE_HOME,
            // Token: 0x04000288 RID: 648
            KEYCODE_BACK,
            // Token: 0x04000289 RID: 649
            KEYCODE_CALL,
            // Token: 0x0400028A RID: 650
            KEYCODE_ENDCALL,
            // Token: 0x0400028B RID: 651
            KEYCODE_0_,
            // Token: 0x0400028C RID: 652
            KEYCODE_1,
            // Token: 0x0400028D RID: 653
            KEYCODE_2,
            // Token: 0x0400028E RID: 654
            KEYCODE_3,
            // Token: 0x0400028F RID: 655
            KEYCODE_4,
            // Token: 0x04000290 RID: 656
            KEYCODE_5,
            // Token: 0x04000291 RID: 657
            KEYCODE_6,
            // Token: 0x04000292 RID: 658
            KEYCODE_7,
            // Token: 0x04000293 RID: 659
            KEYCODE_8,
            // Token: 0x04000294 RID: 660
            KEYCODE_9,
            // Token: 0x04000295 RID: 661
            KEYCODE_STAR,
            // Token: 0x04000296 RID: 662
            KEYCODE_POUND,
            // Token: 0x04000297 RID: 663
            KEYCODE_DPAD_UP,
            // Token: 0x04000298 RID: 664
            KEYCODE_DPAD_DOWN,
            // Token: 0x04000299 RID: 665
            KEYCODE_DPAD_LEFT,
            // Token: 0x0400029A RID: 666
            KEYCODE_DPAD_RIGHT,
            // Token: 0x0400029B RID: 667
            KEYCODE_DPAD_CENTER,
            // Token: 0x0400029C RID: 668
            KEYCODE_VOLUME_UP,
            // Token: 0x0400029D RID: 669
            KEYCODE_VOLUME_DOWN,
            // Token: 0x0400029E RID: 670
            KEYCODE_POWER,
            // Token: 0x0400029F RID: 671
            KEYCODE_CAMERA,
            // Token: 0x040002A0 RID: 672
            KEYCODE_CLEAR,
            // Token: 0x040002A1 RID: 673
            KEYCODE_A,
            // Token: 0x040002A2 RID: 674
            KEYCODE_B,
            // Token: 0x040002A3 RID: 675
            KEYCODE_C,
            // Token: 0x040002A4 RID: 676
            KEYCODE_D,
            // Token: 0x040002A5 RID: 677
            KEYCODE_E,
            // Token: 0x040002A6 RID: 678
            KEYCODE_F,
            // Token: 0x040002A7 RID: 679
            KEYCODE_G,
            // Token: 0x040002A8 RID: 680
            KEYCODE_H,
            // Token: 0x040002A9 RID: 681
            KEYCODE_I,
            // Token: 0x040002AA RID: 682
            KEYCODE_J,
            // Token: 0x040002AB RID: 683
            KEYCODE_K,
            // Token: 0x040002AC RID: 684
            KEYCODE_L,
            // Token: 0x040002AD RID: 685
            KEYCODE_M,
            // Token: 0x040002AE RID: 686
            KEYCODE_N,
            // Token: 0x040002AF RID: 687
            KEYCODE_O,
            // Token: 0x040002B0 RID: 688
            KEYCODE_P,
            // Token: 0x040002B1 RID: 689
            KEYCODE_Q,
            // Token: 0x040002B2 RID: 690
            KEYCODE_R,
            // Token: 0x040002B3 RID: 691
            KEYCODE_S,
            // Token: 0x040002B4 RID: 692
            KEYCODE_T,
            // Token: 0x040002B5 RID: 693
            KEYCODE_U,
            // Token: 0x040002B6 RID: 694
            KEYCODE_V,
            // Token: 0x040002B7 RID: 695
            KEYCODE_W,
            // Token: 0x040002B8 RID: 696
            KEYCODE_X,
            // Token: 0x040002B9 RID: 697
            KEYCODE_Y,
            // Token: 0x040002BA RID: 698
            KEYCODE_Z,
            // Token: 0x040002BB RID: 699
            KEYCODE_COMMA,
            // Token: 0x040002BC RID: 700
            KEYCODE_PERIOD,
            // Token: 0x040002BD RID: 701
            KEYCODE_ALT_LEFT,
            // Token: 0x040002BE RID: 702
            KEYCODE_ALT_RIGHT,
            // Token: 0x040002BF RID: 703
            KEYCODE_SHIFT_LEFT,
            // Token: 0x040002C0 RID: 704
            KEYCODE_SHIFT_RIGHT,
            // Token: 0x040002C1 RID: 705
            KEYCODE_TAB,
            // Token: 0x040002C2 RID: 706
            KEYCODE_SPACE,
            // Token: 0x040002C3 RID: 707
            KEYCODE_SYM,
            // Token: 0x040002C4 RID: 708
            KEYCODE_EXPLORER,
            // Token: 0x040002C5 RID: 709
            KEYCODE_ENVELOPE,
            // Token: 0x040002C6 RID: 710
            KEYCODE_ENTER,
            // Token: 0x040002C7 RID: 711
            KEYCODE_DEL,
            // Token: 0x040002C8 RID: 712
            KEYCODE_GRAVE,
            // Token: 0x040002C9 RID: 713
            KEYCODE_MINUS,
            // Token: 0x040002CA RID: 714
            KEYCODE_EQUALS,
            // Token: 0x040002CB RID: 715
            KEYCODE_LEFT_BRACKET,
            // Token: 0x040002CC RID: 716
            KEYCODE_RIGHT_BRACKET,
            // Token: 0x040002CD RID: 717
            KEYCODE_BACKSLASH,
            // Token: 0x040002CE RID: 718
            KEYCODE_SEMICOLON,
            // Token: 0x040002CF RID: 719
            KEYCODE_APOSTROPHE,
            // Token: 0x040002D0 RID: 720
            KEYCODE_SLASH,
            // Token: 0x040002D1 RID: 721
            KEYCODE_AT,
            // Token: 0x040002D2 RID: 722
            KEYCODE_NUM,
            // Token: 0x040002D3 RID: 723
            KEYCODE_HEADSETHOOK,
            // Token: 0x040002D4 RID: 724
            KEYCODE_FOCUS,
            // Token: 0x040002D5 RID: 725
            KEYCODE_PLUS,
            // Token: 0x040002D6 RID: 726
            KEYCODE_MENU,
            // Token: 0x040002D7 RID: 727
            KEYCODE_NOTIFICATION,
            KEYCODE_MOVE_END,
            // Token: 0x040002D8 RID: 728
            KEYCODE_APP_SWITCH = 187
        }
    }
}
