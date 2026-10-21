namespace QLHSSV_Nhóm8.DAL.Interfaces
{
    public interface IBaoCaoThongKeDAL
    {
        int DemSinhVienKhoa(string maKhoa);
        int DemSoNganhKhoa(string maKhoa);
        int DemSinhVienCTDT(string maCTDT);
    }
}

