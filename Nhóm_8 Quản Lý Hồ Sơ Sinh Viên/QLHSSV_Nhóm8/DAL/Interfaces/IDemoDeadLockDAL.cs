using System.Data;
using System.Threading.Tasks;

namespace QLHSSV_Nhóm8.DAL.Interfaces
{
    public interface IDemoDeadLockDAL
    {
        Task<string[]> BatDauMoPhongTheoSlideAsync();
        Task RollbackCaHaiAsync();
        Task<string[]> ChayNganDeadlockAsync();
        DataTable GetLopDemo();
        DataTable GetSinhVienDemo();
        string KiemTraDuLieuDemo();
    }
}

