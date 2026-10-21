using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace QLHSSV_Nhóm8.DAL
{
    public class DemoGiaoTacDAL : QLHSSV_Nhóm8.DAL.Interfaces.IDemoGiaoTacDAL
    {
        private async Task<DataTable> ExecuteProcedureAsync(
            string procedureName,
            Action<SqlParameterCollection>? addParameters = null,
            int timeout = 0)
        {
            DataTable dt = new DataTable();

            using SqlConnection conn = Database.GetConnection();
            await conn.OpenAsync();

            using SqlCommand cmd = new SqlCommand(procedureName, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = timeout;

            addParameters?.Invoke(cmd.Parameters);

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (reader.FieldCount > 0)
                dt.Load(reader);

            return dt;
        }

        public Task InitAsync(
            Guid runId,
            string scenario,
            string mode)
        {
            return ExecuteProcedureAsync(
                "sp_Demo_Init",
                p =>
                {
                    p.Add("@RunId", SqlDbType.UniqueIdentifier).Value = runId;
                    p.Add("@Scenario", SqlDbType.VarChar, 20).Value = scenario;
                    p.Add("@Mode", SqlDbType.VarChar, 10).Value = mode;
                });
        }

        public Task SetStepAsync(
            Guid runId,
            string giaoTac,
            int step,
            string? action = null)
        {
            return ExecuteProcedureAsync(
                "sp_Demo_SetStep",
                p =>
                {
                    p.Add("@RunId", SqlDbType.UniqueIdentifier).Value = runId;
                    p.Add("@GiaoTac", SqlDbType.VarChar, 5).Value = giaoTac;
                    p.Add("@Step", SqlDbType.Int).Value = step;

                    SqlParameter pa = p.Add("@Action", SqlDbType.VarChar, 20);
                    pa.Value = string.IsNullOrWhiteSpace(action)
                        ? DBNull.Value
                        : action;
                });
        }

        public Task CancelAsync(Guid runId)
        {
            return ExecuteProcedureAsync(
                "sp_Demo_Cancel",
                p => p.Add(
                    "@RunId",
                    SqlDbType.UniqueIdentifier
                ).Value = runId);
        }

        public Task<DataTable> GetLogAsync(Guid runId)
        {
            return ExecuteProcedureAsync(
                "sp_Demo_GetLog",
                p => p.Add(
                    "@RunId",
                    SqlDbType.UniqueIdentifier
                ).Value = runId);
        }

        public Task<DataTable> InspectorAsync(string scenario)
        {
            return ExecuteProcedureAsync(
                "sp_Demo_Inspector",
                p => p.Add(
                    "@Scenario",
                    SqlDbType.VarChar,
                    20
                ).Value = scenario);
        }

        public Task<DataTable> RunAsync(
            string procedureName,
            Guid runId)
        {
            return ExecuteProcedureAsync(
                procedureName,
                p => p.Add(
                    "@RunId",
                    SqlDbType.UniqueIdentifier
                ).Value = runId,
                0);
        }
    }
}
