using System.Data;
using QLHSSV_Nhóm8.DTO;

namespace QLHSSV_Nhóm8.DAL.Interfaces
{
    public interface INganhDAL
    {
        DataTable GetAll();
        bool KiemTraTonTai(string maNganh);
        bool Insert(NganhDTO nganh);
        bool Update(NganhDTO nganh);
        bool Delete(string maNganh);
        DataTable Search(string keyword);
    }
}

