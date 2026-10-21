using System;
using System.Data;
using System.Threading.Tasks;

namespace QLHSSV_Nhóm8.DAL.Interfaces
{
    public interface IDemoGiaoTacDAL
    {
        Task InitAsync(Guid runId, string scenario, string mode);
        Task SetStepAsync(Guid runId, string tx, int step, string action);
        Task CancelAsync(Guid runId);
        Task<DataTable> GetLogAsync(Guid runId);
        Task<DataTable> InspectorAsync(string scenario);
        Task<DataTable> RunAsync(string procedureName, Guid runId);
    }
}

