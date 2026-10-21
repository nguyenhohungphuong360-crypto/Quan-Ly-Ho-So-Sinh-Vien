using System.Data;
using QLHSSV_Nhóm8.DAL;
using QLHSSV_Nhóm8.DAL.Interfaces;
using QLHSSV_Nhóm8.DTO;

namespace QLHSSV_Nhóm8.BUS
{
    public class NguoiThanBUS
    {
        private readonly INguoiThanDAL dal;

        public NguoiThanBUS() : this(new NguoiThanDAL())
        {
        }

        public NguoiThanBUS(INguoiThanDAL dal)
        {
            this.dal = dal;
        }

        // Lấy danh sách người thân
        public DataTable GetAll()
        {
            return dal.GetAll();
        }

        // Thêm người thân
        public bool Them(NguoiThanDTO nt)
        {
            if (!DuLieuHopLe(nt))
                return false;

            if (dal.KiemTraTonTai(nt.MaNT))
                return false;

            return dal.Insert(nt);
        }

        // Sửa người thân
        public bool Sua(NguoiThanDTO nt)
        {
            if (!DuLieuHopLe(nt))
                return false;

            return dal.Update(nt);
        }

        // Xóa người thân
        public bool Xoa(string maNT)
        {
            if (string.IsNullOrWhiteSpace(maNT))
                return false;

            return dal.Delete(maNT);
        }

        // Tìm kiếm người thân
        public DataTable TimKiem(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return dal.GetAll();

            return dal.Search(keyword.Trim());
        }

        // Kiểm tra dữ liệu bắt buộc
        private bool DuLieuHopLe(NguoiThanDTO nt)
        {
            if (nt == null)
                return false;

            return !string.IsNullOrWhiteSpace(nt.MaNT)
                && !string.IsNullOrWhiteSpace(nt.HoTen)
                && !string.IsNullOrWhiteSpace(nt.MaSV);
        }
    }
}
