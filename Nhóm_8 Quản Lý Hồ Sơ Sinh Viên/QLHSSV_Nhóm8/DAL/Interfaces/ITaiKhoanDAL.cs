using System.Data;
using QLHSSV_Nhóm8.DTO;

namespace QLHSSV_Nhóm8.DAL.Interfaces
{
    public interface ITaiKhoanDAL
    {
        DataTable GetAll();
        bool KiemTraTonTai(string tenDangNhap);
        bool Insert(TaiKhoanDTO tk);
        bool Update(TaiKhoanDTO tk);
        bool Delete(string tenDangNhap);
        DataTable Search(string keyword);
        TaiKhoanDTO? DangNhap(string tenDangNhap, string matKhau);
        TaiKhoanDTO? GetByUsername(string tenDangNhap);
        bool UpdatePassword(string tenDangNhap, string newHashedPassword);
    }
}

