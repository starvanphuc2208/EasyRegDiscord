namespace easy
{
    public class LicenseDto
    {
        public int error { get; set; }
        public string ngay_dang_ky { get; set; }
        public string ngay_het_han { get; set; }
        public string param_01 { get; set; }
        public string param_02 { get; set; }
        public string param_03 { get; set; }
        public string param_04 { get; set; }
        public string param_05 { get; set; }
        public string param_06 { get; set; }
        public string param_07 { get; set; }
        public string thong_bao { get; set; }
    }

    public class VersionDto
    {
        public string name { get; set; }
        public string version { get; set; }
    }


    public class RegisterDto
    {
        public int error { get; set; }

        public string thong_bao { get; set; }
    }

}
