using System;
using System.Data;
using System.Text.RegularExpressions;
using QLHSSV.DTO;
using QLHSSV_Nhóm8.Core.Common;
using QLHSSV_Nhóm8.DAL;
using QLHSSV_Nhóm8.DAL.Interfaces;
using QLHSSV_Nhóm8.DTO;
using Serilog;

namespace QLHSSV_Nhóm8.BUS
{
    public class SinhVienBUS
    {
        private readonly ISinhVienDAL _dal;

        public SinhVienBUS() : this(new SinhVienDAL())
        {
        }

        public SinhVienBUS(ISinhVienDAL dal)
        {
            _dal = dal;
        }

        public DataTable GetAll()
        {
            return _dal.GetAll();
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ dữ liệu hồ sơ sinh viên
        /// </summary>
        public bool KiemTraHopLe(SinhVienDTO sv, out string error)
        {
            error = string.Empty;

            if (sv == null)
            {
                error = "Dữ liệu sinh viên không được rỗng.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(sv.MaSV))
            {
                error = "Mã sinh viên không được để trống.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(sv.HoTen))
            {
                error = "Họ tên sinh viên không được để trống.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(sv.MaLop))
            {
                error = "Vui lòng chọn lớp học cho sinh viên.";
                return false;
            }

            // Kiểm tra tuổi (từ 15 đến 80 tuổi)
            var today = DateTime.Today;
            var age = today.Year - sv.NgaySinh.Year;
            if (sv.NgaySinh.Date > today.AddYears(-age)) age--;

            if (age < 15 || age > 80)
            {
                error = $"Ngày sinh không hợp lệ: Sinh viên phải từ 15 đến 80 tuổi (hiện tại tính ra {age} tuổi).";
                return false;
            }

            // Kiểm tra Email định dạng chuẩn nếu có nhập
            if (!string.IsNullOrWhiteSpace(sv.Email))
            {
                var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(sv.Email.Trim(), emailPattern, RegexOptions.IgnoreCase))
                {
                    error = "Định dạng Email không hợp lệ (ví dụ: student@school.edu.vn).";
                    return false;
                }
            }

            // Kiểm tra SĐT (10 số bắt đầu bằng 0)
            if (!string.IsNullOrWhiteSpace(sv.SDT))
            {
                var phonePattern = @"^0[0-9]{9,10}$";
                if (!Regex.IsMatch(sv.SDT.Trim(), phonePattern))
                {
                    error = "Số điện thoại phải từ 10-11 chữ số và bắt đầu bằng số 0.";
                    return false;
                }
            }

            // Kiểm tra CCCD (9 hoặc 12 số)
            if (!string.IsNullOrWhiteSpace(sv.CCCD))
            {
                var cccdPattern = @"^([0-9]{9}|[0-9]{12})$";
                if (!Regex.IsMatch(sv.CCCD.Trim(), cccdPattern))
                {
                    error = "Số CCCD/CMND phải bao gồm 9 hoặc 12 chữ số.";
                    return false;
                }
            }

            return true;
        }

        public Result ThemResult(SinhVienDTO sv)
        {
            if (!KiemTraHopLe(sv, out string error))
            {
                return Result.Failure(error);
            }

            if (_dal.KiemTraTonTai(sv.MaSV))
            {
                return Result.Failure($"Mã sinh viên '{sv.MaSV}' đã tồn tại trong hệ thống.");
            }

            bool success = _dal.Insert(sv);
            if (success)
            {
                Log.Information("Thêm thành công sinh viên: {MaSV} - {HoTen}", sv.MaSV, sv.HoTen);
                return Result.Success();
            }

            return Result.Failure("Không thể thêm sinh viên vào cơ sở dữ liệu.");
        }

        public bool Them(SinhVienDTO sv)
        {
            var res = ThemResult(sv);
            return res.IsSuccess;
        }

        public Result SuaResult(SinhVienDTO sv)
        {
            if (!KiemTraHopLe(sv, out string error))
            {
                return Result.Failure(error);
            }

            if (!_dal.KiemTraTonTai(sv.MaSV))
            {
                return Result.Failure($"Không tìm thấy sinh viên có mã '{sv.MaSV}' để cập nhật.");
            }

            bool success = _dal.Update(sv);
            if (success)
            {
                Log.Information("Cập nhật thành công sinh viên: {MaSV}", sv.MaSV);
                return Result.Success();
            }

            return Result.Failure("Cập nhật thông tin sinh viên thất bại.");
        }

        public bool Sua(SinhVienDTO sv)
        {
            var res = SuaResult(sv);
            return res.IsSuccess;
        }

        public bool Xoa(string maSV)
        {
            if (string.IsNullOrWhiteSpace(maSV))
                return false;

            maSV = maSV.Trim();
            bool success = _dal.Delete(maSV);
            if (success)
            {
                Log.Information("Đã xóa sinh viên: {MaSV}", maSV);
            }
            return success;
        }

        public DataTable TimKiem(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return _dal.GetAll();

            return _dal.Search(keyword.Trim());
        }

        public DataTable ThongKeTheoLop()
        {
            return _dal.ThongKeTheoLop();
        }

        public DataTable ThongKeTheoGioiTinh()
        {
            return _dal.ThongKeTheoGioiTinh();
        }

        public int DemSinhVienTheoLop(string maLop)
        {
            if (string.IsNullOrWhiteSpace(maLop))
                return 0;

            return _dal.DemSinhVienTheoLop(maLop.Trim());
        }

        public bool ChuyenLop(string maSV, string maLopMoi)
        {
            if (string.IsNullOrWhiteSpace(maSV) || string.IsNullOrWhiteSpace(maLopMoi))
                return false;

            return _dal.ChuyenLop(maSV.Trim(), maLopMoi.Trim());
        }

        public DataTable GetLichSuXoa()
        {
            return _dal.GetLichSuXoa();
        }
    }
}
