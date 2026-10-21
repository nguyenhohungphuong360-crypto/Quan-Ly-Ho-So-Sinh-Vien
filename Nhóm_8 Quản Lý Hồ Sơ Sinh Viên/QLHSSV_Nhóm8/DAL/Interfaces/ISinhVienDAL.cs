using System.Data;
using QLHSSV.DTO;
using QLHSSV_Nhóm8.DTO;

namespace QLHSSV_Nhóm8.DAL.Interfaces
{
    public interface ISinhVienDAL
    {
        DataTable GetAll();
        bool Insert(SinhVienDTO sv);
        bool Update(SinhVienDTO sv);
        bool Delete(string maSV);
        DataTable Search(string keyword);
        bool KiemTraTonTai(string maSV);
        DataTable ThongKeTheoLop();
        DataTable ThongKeTheoGioiTinh();
        int DemSinhVienTheoLop(string maLop);
        DataTable GetLichSuXoa();
        bool ChuyenLop(string maSV, string maLopMoi);
    }
}

