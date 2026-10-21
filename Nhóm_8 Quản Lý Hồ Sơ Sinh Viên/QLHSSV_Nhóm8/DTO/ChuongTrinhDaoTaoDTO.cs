namespace QLHSSV_Nhóm8.DTO
{
    public class ChuongTrinhDaoTaoDTO
    {
        public string MaCTDT { get; set; } = "";
        public string TenCTDT { get; set; } = "";
        public string TrinhDo { get; set; } = "";

        public int ThoiGianDaoTao { get; set; }
        public int TongTinChi { get; set; }
        public int NamBatDau { get; set; }
        public int NamKetThuc { get; set; }

        public string MaNganh { get; set; } = "";
    }
}