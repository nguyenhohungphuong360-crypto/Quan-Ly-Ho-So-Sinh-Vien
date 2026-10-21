using System.Data;
using QLHSSV_Nhóm8.DTO;

namespace QLHSSV_Nhóm8.DAL.Interfaces
{
    public interface IKhoaDAL
    {
        DataTable GetAll();
        bool KiemTraTonTai(string maKhoa);
        bool Insert(KhoaDTO khoa);
        bool Update(KhoaDTO khoa);
        bool Delete(string maKhoa);
        DataTable Search(string keyword);
    }
}

