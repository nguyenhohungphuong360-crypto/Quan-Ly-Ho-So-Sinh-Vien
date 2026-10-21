using System.Data;
using QLHSSV_Nhóm8.DAL;
using QLHSSV_Nhóm8.DTO;

namespace QLHSSV_Nhóm8.BUS
{
    public class ChuongTrinhDaoTaoBUS
    {
        private readonly ChuongTrinhDaoTaoDAL dal =
            new ChuongTrinhDaoTaoDAL();

        // Lấy danh sách CTĐT
        public DataTable GetAll()
        {
            return dal.GetAll();
        }

        // Thêm CTĐT
        public bool Them(ChuongTrinhDaoTaoDTO ct)
        {
            if (!DuLieuHopLe(ct))
                return false;

            if (dal.KiemTraTonTai(ct.MaCTDT))
                return false;

            return dal.Insert(ct);
        }

        // Sửa CTĐT
        public bool Sua(ChuongTrinhDaoTaoDTO ct)
        {
            if (!DuLieuHopLe(ct))
                return false;

            if (!dal.KiemTraTonTai(ct.MaCTDT))
                return false;

            return dal.Update(ct);
        }

        // Xóa CTĐT
        public bool Xoa(string maCTDT)
        {
            if (string.IsNullOrWhiteSpace(maCTDT))
                return false;

            return dal.Delete(maCTDT);
        }

        // Tìm kiếm CTĐT
        public DataTable TimKiem(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return dal.GetAll();

            return dal.Search(keyword.Trim());
        }

        // Kiểm tra dữ liệu
        private bool DuLieuHopLe(
            ChuongTrinhDaoTaoDTO ct)
        {
            if (ct == null)
                return false;

            if (string.IsNullOrWhiteSpace(ct.MaCTDT) ||
                string.IsNullOrWhiteSpace(ct.TenCTDT) ||
                string.IsNullOrWhiteSpace(ct.TrinhDo) ||
                string.IsNullOrWhiteSpace(ct.MaNganh))
            {
                return false;
            }

            if (ct.ThoiGianDaoTao <= 0 ||
                ct.TongTinChi <= 0)
            {
                return false;
            }

            if (ct.NamBatDau <= 0 ||
                ct.NamKetThuc <= 0 ||
                ct.NamKetThuc < ct.NamBatDau)
            {
                return false;
            }

            return true;
        }
    }
}
