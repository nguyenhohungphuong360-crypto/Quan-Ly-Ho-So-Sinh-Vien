using System.Data;
using QLHSSV_Nhóm8.DAL;
using QLHSSV_Nhóm8.DAL.Interfaces;
using QLHSSV_Nhóm8.DTO;

namespace QLHSSV_Nhóm8.BUS
{
    public class NganhBUS
    {
        private readonly INganhDAL dal;

        public NganhBUS() : this(new NganhDAL())
        {
        }

        public NganhBUS(INganhDAL dal)
        {
            this.dal = dal;
        }

        // Lấy danh sách ngành
        public DataTable GetAll()
        {
            return dal.GetAll();
        }

        // Thêm ngành
        public bool Them(NganhDTO nganh)
        {
            if (!DuLieuHopLe(nganh))
                return false;

            if (dal.KiemTraTonTai(nganh.MaNganh))
                return false;

            return dal.Insert(nganh);
        }

        // Sửa ngành
        public bool Sua(NganhDTO nganh)
        {
            if (!DuLieuHopLe(nganh))
                return false;

            if (!dal.KiemTraTonTai(nganh.MaNganh))
                return false;

            return dal.Update(nganh);
        }

        // Xóa ngành
        public bool Xoa(string maNganh)
        {
            if (string.IsNullOrWhiteSpace(maNganh))
                return false;

            return dal.Delete(maNganh);
        }

        // Tìm kiếm ngành
        public DataTable TimKiem(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return dal.GetAll();

            return dal.Search(keyword.Trim());
        }

        // Kiểm tra dữ liệu bắt buộc
        private bool DuLieuHopLe(NganhDTO nganh)
        {
            if (nganh == null)
                return false;

            return
                !string.IsNullOrWhiteSpace(nganh.MaNganh) &&
                !string.IsNullOrWhiteSpace(nganh.TenNganh) &&
                !string.IsNullOrWhiteSpace(nganh.MaKhoa);
        }
    }
}
