using QLHSSV_Nhóm8.DAL;
using QLHSSV_Nhóm8.DAL.Interfaces;

namespace QLHSSV_Nhóm8.BUS
{
    public class BaoCaoThongKeBUS
    {
        private readonly IBaoCaoThongKeDAL dal;

        public BaoCaoThongKeBUS() : this(new BaoCaoThongKeDAL())
        {
        }

        public BaoCaoThongKeBUS(IBaoCaoThongKeDAL dal)
        {
            this.dal = dal;
        }

        // Đếm sinh viên trong khoa
        public int DemSinhVienKhoa(string maKhoa)
        {
            if (string.IsNullOrWhiteSpace(maKhoa))
                return 0;

            return dal.DemSinhVienKhoa(maKhoa.Trim());
        }

        // Đếm số ngành trong khoa
        public int DemSoNganhKhoa(string maKhoa)
        {
            if (string.IsNullOrWhiteSpace(maKhoa))
                return 0;

            return dal.DemSoNganhKhoa(maKhoa.Trim());
        }

        // Đếm sinh viên theo CTDT
        public int DemSinhVienCTDT(string maCTDT)
        {
            if (string.IsNullOrWhiteSpace(maCTDT))
                return 0;

            return dal.DemSinhVienCTDT(maCTDT.Trim());
        }
    }
}
