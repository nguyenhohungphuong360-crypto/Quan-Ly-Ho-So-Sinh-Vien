using System;
using System.Data;
using System.Threading.Tasks;
using QLHSSV_Nhóm8.DAL;

namespace QLHSSV_Nhóm8.BUS
{
    public class DemoGiaoTacBUS
    {
        private readonly DemoGiaoTacDAL dal =
            new DemoGiaoTacDAL();

        public Task InitAsync(
            Guid runId,
            string scenario,
            string mode)
        {
            return dal.InitAsync(
                runId,
                scenario,
                mode);
        }

        public Task SetStepAsync(
            Guid runId,
            string giaoTac,
            int step,
            string? action = null)
        {
            return dal.SetStepAsync(
                runId,
                giaoTac,
                step,
                action);
        }

        public Task CancelAsync(Guid runId)
        {
            return dal.CancelAsync(runId);
        }

        public Task<DataTable> GetLogAsync(Guid runId)
        {
            return dal.GetLogAsync(runId);
        }

        public Task<DataTable> InspectorAsync(string scenario)
        {
            return dal.InspectorAsync(scenario);
        }

        public Task<DataTable> RunAsync(
            string procedureName,
            Guid runId)
        {
            return dal.RunAsync(
                procedureName,
                runId);
        }
    }
}
