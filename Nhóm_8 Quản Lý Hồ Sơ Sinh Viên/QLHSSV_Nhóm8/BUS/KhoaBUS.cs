using System.Data;
using QLHSSV_Nhóm8.DAL;
using QLHSSV_Nhóm8.DAL.Interfaces;
using QLHSSV_Nhóm8.DTO;

namespace QLHSSV_Nhóm8.BUS
{
    public class KhoaBUS
    {
        private readonly IKhoaDAL dal;

        public KhoaBUS() : this(new KhoaDAL())
        {
        }

        public KhoaBUS(IKhoaDAL dal)
        {
            this.dal = dal;
        }

        // Lấy danh sách khoa
        public DataTable GetAll()
        {
            return dal.GetAll();
        }

        // Thêm khoa
        public bool Them(KhoaDTO khoa)
        {
            if (!DuLieuHopLe(khoa))
                return false;

            if (dal.KiemTraTonTai(khoa.MaKhoa))
                return false;

            return dal.Insert(khoa);
        }

        // Sửa khoa
        public bool Sua(KhoaDTO khoa)
        {
            if (!DuLieuHopLe(khoa))
                return false;

            if (!dal.KiemTraTonTai(khoa.MaKhoa))
                return false;

            return dal.Update(khoa);
        }

        // Xóa khoa
        public bool Xoa(string maKhoa)
        {
            if (string.IsNullOrWhiteSpace(maKhoa))
                return false;

            return dal.Delete(maKhoa);
        }

        // Tìm kiếm khoa
        public DataTable TimKiem(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return dal.GetAll();

            return dal.Search(keyword.Trim());
        }

        // Kiểm tra dữ liệu bắt buộc
        private bool DuLieuHopLe(KhoaDTO khoa)
        {
            if (khoa == null)
                return false;

            return
                !string.IsNullOrWhiteSpace(khoa.MaKhoa) &&
                !string.IsNullOrWhiteSpace(khoa.TenKhoa);
        }
    }
}
