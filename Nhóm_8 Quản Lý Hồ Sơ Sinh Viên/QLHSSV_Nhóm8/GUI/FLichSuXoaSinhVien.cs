using QLHSSV_Nhóm8.BUS;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace QLHSSV_Nhóm8.GUI
{
    public class FLichSuXoaSinhVien : Form
    {
        // BUS dùng để lấy dữ liệu lịch sử
        private readonly SinhVienBUS sinhVienBUS;

        // Các control trên Form
        private Label lblTieuDe;
        private DataGridView dgvLichSu;

        // ==================================
        // CONSTRUCTOR
        // ==================================
        public FLichSuXoaSinhVien()
        {
            // Khởi tạo BUS
            sinhVienBUS = new SinhVienBUS();

            // Tạo giao diện
            InitializeComponent();
        }

        // ==================================
        // KHỞI TẠO GIAO DIỆN
        // ==================================
        // ==================================
        // KHỞI TẠO GIAO DIỆN
        // ==================================
        private void InitializeComponent()
        {
            // ==============================
            // FORM
            // ==============================
            this.Text = "Lịch sử xóa sinh viên";

            // Mở Form ở giữa màn hình
            this.StartPosition =
                FormStartPosition.CenterScreen;

            this.Size =
                new Size(950, 600);

            this.BackColor =
                Color.White;


            // ==============================
            // TIÊU ĐỀ
            // ==============================
            lblTieuDe =
                new Label();

            lblTieuDe.Text =
                "LỊCH SỬ XÓA SINH VIÊN";

            lblTieuDe.Font =
                new Font(
                    "Arial",
                    18,
                    FontStyle.Bold
                );

            // Không dùng AutoSize để có thể
            // căn giữa theo chiều rộng Form
            lblTieuDe.AutoSize =
                false;

            // Cho tiêu đề nằm ở phía trên Form
            lblTieuDe.Dock =
                DockStyle.Top;

            lblTieuDe.Height =
                90;

            // Căn chữ chính giữa
            lblTieuDe.TextAlign =
                ContentAlignment.MiddleCenter;

            this.Controls.Add(lblTieuDe);


            // ==============================
            // DATAGRIDVIEW LỊCH SỬ
            // ==============================
            dgvLichSu =
                new DataGridView();

            // Đưa bảng lên trên vì đã bỏ
            // nút Làm mới và Đóng
            dgvLichSu.Location =
                new Point(30, 100);

            dgvLichSu.Size =
                new Size(875, 420);

            // Các cột tự chia đều chiều rộng
            dgvLichSu.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            // Không cho người dùng thêm dòng
            // trực tiếp trên DataGridView
            dgvLichSu.AllowUserToAddRows =
                false;

            dgvLichSu.AllowUserToDeleteRows =
                false;

            // Chỉ dùng bảng để xem dữ liệu
            dgvLichSu.ReadOnly =
                true;

            // Khi click sẽ chọn nguyên dòng
            dgvLichSu.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            // Tự co giãn khi thay đổi kích thước Form
            dgvLichSu.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Bottom |
                AnchorStyles.Left |
                AnchorStyles.Right;

            this.Controls.Add(dgvLichSu);


            // ==============================
            // EVENT LOAD FORM
            // ==============================
            // Khi mở Form sẽ tự động
            // lấy lịch sử mới nhất từ Database
            this.Load +=
                FLichSuXoaSinhVien_Load;
        }
        // ==================================
        // LOAD FORM LỊCH SỬ XÓA
        // ==================================
        private void FLichSuXoaSinhVien_Load(
            object sender,
            EventArgs e)
        {
            // Khi Form vừa mở
            // tự động lấy lịch sử xóa mới nhất từ Database
            LoadData();
        }
        // ==================================
        // LOAD DỮ LIỆU LỊCH SỬ XÓA
        // ==================================
        private void LoadData()
        {
            try
            {
                // GUI gọi BUS
                // BUS gọi DAL
                // DAL đọc dữ liệu từ View trong SQL Server
                DataTable dt =
                    sinhVienBUS.GetLichSuXoa();

                // Hiển thị dữ liệu lên DataGridView
                dgvLichSu.DataSource = dt;

                // Đổi tên các cột cho dễ nhìn
                if (dgvLichSu.Columns["Id"] != null)
                    dgvLichSu.Columns["Id"].HeaderText =
                        "STT";

                if (dgvLichSu.Columns["MaSV"] != null)
                    dgvLichSu.Columns["MaSV"].HeaderText =
                        "Mã SV";

                if (dgvLichSu.Columns["HoTen"] != null)
                    dgvLichSu.Columns["HoTen"].HeaderText =
                        "Họ tên";

                if (dgvLichSu.Columns["CCCD"] != null)
                    dgvLichSu.Columns["CCCD"].HeaderText =
                        "CCCD";

                if (dgvLichSu.Columns["MaLop"] != null)
                    dgvLichSu.Columns["MaLop"].HeaderText =
                        "Mã lớp";

                if (dgvLichSu.Columns["ThoiGianXoa"] != null)
                {
                    dgvLichSu.Columns["ThoiGianXoa"].HeaderText =
                        "Thời gian xóa";
                }

                // Không tự chọn dòng đầu tiên
                dgvLichSu.ClearSelection();
            }
            catch (Exception ex)
            {
                // Hiển thị lỗi nếu không lấy được dữ liệu
                MessageBox.Show(
                    "Không thể tải lịch sử xóa!\n" +
                    ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

    }
}