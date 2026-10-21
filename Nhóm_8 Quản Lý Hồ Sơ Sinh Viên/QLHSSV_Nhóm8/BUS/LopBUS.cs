using System.Collections.Generic;
using System.Data;
using QLHSSV_Nhóm8.DAL;
using QLHSSV_Nhóm8.DAL.Interfaces;
using QLHSSV_Nhóm8.DTO;

namespace QLHSSV_Nhóm8.BUS
{
    public class LopBUS
    {
        private readonly ILopDAL dal;

        public LopBUS() : this(new LopDAL())
        {
        }

        public LopBUS(ILopDAL dal)
        {
            this.dal = dal;
        }

        // Lấy danh sách lớp
        public DataTable GetAll()
        {
            return dal.GetAll();
        }

        // Thêm lớp
        public bool Them(LopDTO lop)
        {
            if (!DuLieuHopLe(lop))
                return false;

            if (dal.KiemTraTonTai(lop.MaLop))
                return false;

            return dal.Insert(lop);
        }

        // Sửa lớp
        public bool Sua(LopDTO lop)
        {
            if (!DuLieuHopLe(lop))
                return false;

            if (!dal.KiemTraTonTai(lop.MaLop))
                return false;

            return dal.Update(lop);
        }

        // Xóa lớp
        public bool Xoa(string maLop)
        {
            if (string.IsNullOrWhiteSpace(maLop))
                return false;

            return dal.Delete(maLop);
        }

        // Transaction chuyển SV và xóa lớp
        public string ChuyenSinhVienVaXoaLop(
            string maLopCu,
            string maLopMoi)
        {
            if (string.IsNullOrWhiteSpace(maLopCu))
                return "Vui lòng chọn lớp cũ!";

            if (string.IsNullOrWhiteSpace(maLopMoi))
                return "Vui lòng chọn lớp mới!";

            if (maLopCu.Trim() == maLopMoi.Trim())
                return "Lớp cũ và lớp mới không được giống nhau!";

            return dal.ChuyenSinhVienVaXoaLop(
                maLopCu.Trim(),
                maLopMoi.Trim());
        }

        // Tìm kiếm lớp
        public List<LopDTO> TimKiem(string keyword)
        {
            DataTable dt;

            if (string.IsNullOrWhiteSpace(keyword))
                dt = dal.GetAll();
            else
                dt = dal.Search(keyword.Trim());

            List<LopDTO> ketQua =
                new List<LopDTO>();

            foreach (DataRow row in dt.Rows)
            {
                ketQua.Add(new LopDTO
                {
                    MaLop =
                        row["MaLop"].ToString() ?? "",

                    TenLop =
                        row["TenLop"].ToString() ?? "",

                    NienKhoa =
                        row["NienKhoa"].ToString() ?? "",

                    MaNganh =
                        row["MaNganh"].ToString() ?? ""
                });
            }

            return ketQua;
        }

        // Kiểm tra dữ liệu
        private bool DuLieuHopLe(LopDTO lop)
        {
            if (lop == null)
                return false;

            return
                !string.IsNullOrWhiteSpace(lop.MaLop) &&
                !string.IsNullOrWhiteSpace(lop.TenLop) &&
                !string.IsNullOrWhiteSpace(lop.NienKhoa) &&
                !string.IsNullOrWhiteSpace(lop.MaNganh);
        }
    }
}
