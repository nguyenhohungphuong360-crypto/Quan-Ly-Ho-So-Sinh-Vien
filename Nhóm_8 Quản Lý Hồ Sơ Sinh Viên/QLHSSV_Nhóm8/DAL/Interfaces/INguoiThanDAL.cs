using System.Data;
using QLHSSV_Nhóm8.DTO;

namespace QLHSSV_Nhóm8.DAL.Interfaces
{
    public interface INguoiThanDAL
    {
        DataTable GetAll();
        bool KiemTraTonTai(string maNT);
        bool Insert(NguoiThanDTO nt);
        bool Update(NguoiThanDTO nt);
        bool Delete(string maNT);
        DataTable Search(string keyword);
    }
}

