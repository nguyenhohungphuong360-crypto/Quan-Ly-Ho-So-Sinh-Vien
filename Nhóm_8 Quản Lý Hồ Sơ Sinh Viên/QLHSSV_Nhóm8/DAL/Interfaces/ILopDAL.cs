using System.Data;
using QLHSSV_Nhóm8.DTO;

namespace QLHSSV_Nhóm8.DAL.Interfaces
{
    public interface ILopDAL
    {
        DataTable GetAll();
        bool KiemTraTonTai(string maLop);
        bool Insert(LopDTO lop);
        bool Update(LopDTO lop);
        bool Delete(string maLop);
        DataTable Search(string keyword);
        string ChuyenSinhVienVaXoaLop(string maLopCu, string maLopMoi);
    }
}

