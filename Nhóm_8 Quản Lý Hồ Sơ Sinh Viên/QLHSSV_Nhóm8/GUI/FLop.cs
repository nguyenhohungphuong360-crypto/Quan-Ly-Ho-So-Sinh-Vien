using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QLHSSV_Nhóm8.BUS;
using QLHSSV_Nhóm8.DTO;

namespace QLHSSV_Nhóm8.GUI
{
    public partial class FLop : Form
    {
        private Label lblTieuDe;

        private TextBox txtMaLop;
        private TextBox txtTenLop;
        private TextBox txtNienKhoa;
        private TextBox txtTimKiem;

        private ComboBox cboMaNganh;
        private ComboBox cboLopMoi;

        private Button btnThem;
        private Button btnSua;
        private Button btnXoa;
        private Button btnLamMoi;
        private Button btnTimKiem;
        private Button btnChuyenVaXoaLop;

        private DataGridView dgvLop;

        private readonly LopBUS lopBUS = new LopBUS();
        private readonly NganhBUS nganhBUS = new NganhBUS();

        public FLop()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Form
            this.Text = "Quản lý lớp";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.White;

            // Tiêu đề
            lblTieuDe = new Label
            {
                Text = "QUẢN LÝ LỚP",
                Font = new Font("Arial", 20, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(500, 20)
            };

            this.Controls.Add(lblTieuDe);

            // Thông tin lớp
            TaoLabel("Mã lớp:", 50, 90);
            TaoLabel("Tên lớp:", 50, 140);
            TaoLabel("Niên khóa:", 50, 190);
            TaoLabel("Mã ngành:", 550, 90);
            TaoLabel("Lớp chuyển đến:", 550, 140);

            txtMaLop = TaoTextBox(150, 88, 300);
            txtTenLop = TaoTextBox(150, 138, 300);
            txtNienKhoa = TaoTextBox(150, 188, 300);

            // Ngành
            cboMaNganh = new ComboBox
            {
                Location = new Point(680, 88),
                Size = new Size(300, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Arial", 10)
            };

            this.Controls.Add(cboMaNganh);

            // Lớp chuyển đến
            cboLopMoi = new ComboBox
            {
                Location = new Point(680, 138),
                Size = new Size(300, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Arial", 10)
            };

            this.Controls.Add(cboLopMoi);

            // Nút chức năng
            btnThem = TaoButton("Thêm", 250, 260);
            btnSua = TaoButton("Sửa", 370, 260);
            btnXoa = TaoButton("Xóa", 490, 260);
            btnLamMoi = TaoButton("Làm mới", 610, 260);

            btnChuyenVaXoaLop =
                TaoButton("Chuyển SV và Xóa Lớp", 730, 260);

            btnChuyenVaXoaLop.Size =
                new Size(210, 40);

            // Tìm kiếm
            TaoLabel("Tìm kiếm:", 50, 320);
            txtTimKiem = TaoTextBox(130, 318, 350);
            btnTimKiem = TaoButton("Tìm", 500, 310);

            // Bảng dữ liệu
            dgvLop = new DataGridView
            {
                Location = new Point(30, 370),
                Size = new Size(1000, 300),

                Anchor =
                    AnchorStyles.Top |
                    AnchorStyles.Bottom |
                    AnchorStyles.Left |
                    AnchorStyles.Right,

                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill,

                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,

                SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect,

                MultiSelect = false,
                RowHeadersVisible = true
            };

            dgvLop.Columns.Add("MaLop", "Mã lớp");
            dgvLop.Columns.Add("TenLop", "Tên lớp");
            dgvLop.Columns.Add("NienKhoa", "Niên khóa");
            dgvLop.Columns.Add("MaNganh", "Mã ngành");

            this.Controls.Add(dgvLop);

            // Sự kiện
            this.Load += FLop_Load;
            this.Resize += FLop_Resize;

            btnThem.Click += btnThem_Click;
            btnSua.Click += btnSua_Click;
            btnXoa.Click += btnXoa_Click;
            btnLamMoi.Click += btnLamMoi_Click;
            btnTimKiem.Click += btnTimKiem_Click;

            btnChuyenVaXoaLop.Click +=
                btnChuyenVaXoaLop_Click;

            dgvLop.CellClick += dgvLop_CellClick;
        }

        private Label TaoLabel(string text, int x, int y)
        {
            Label label = new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Arial", 10),
                Location = new Point(x, y)
            };

            this.Controls.Add(label);
            return label;
        }

        private TextBox TaoTextBox(int x, int y, int width)
        {
            TextBox textBox = new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(width, 25),
                Font = new Font("Arial", 10)
            };

            this.Controls.Add(textBox);
            return textBox;
        }

        private Button TaoButton(string text, int x, int y)
        {
            Button button = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(100, 40),
                Font = new Font("Arial", 10, FontStyle.Bold),
                UseVisualStyleBackColor = true
            };

            this.Controls.Add(button);
            return button;
        }

        // Load form
        private void FLop_Load(object sender, EventArgs e)
        {
            LoadNganh();
            LoadLopMoi();
            LoadData();
            CanGiuaTieuDe();
        }

        // Load ngành
        private void LoadNganh()
        {
            DataTable dt = nganhBUS.GetAll();

            foreach (DataRow row in dt.Rows)
            {
                if (row["MaNganh"] != DBNull.Value)
                    row["MaNganh"] = row["MaNganh"].ToString().Trim();

                if (row["TenNganh"] != DBNull.Value)
                    row["TenNganh"] = row["TenNganh"].ToString().Trim();
            }

            cboMaNganh.DataSource = dt;
            cboMaNganh.DisplayMember = "TenNganh";
            cboMaNganh.ValueMember = "MaNganh";
            cboMaNganh.SelectedIndex = -1;
        }

        // Load lớp chuyển
        private void LoadLopMoi()
        {
            DataTable dt = lopBUS.GetAll();

            foreach (DataRow row in dt.Rows)
            {
                if (row["MaLop"] != DBNull.Value)
                    row["MaLop"] = row["MaLop"].ToString().Trim();

                if (row["TenLop"] != DBNull.Value)
                    row["TenLop"] = row["TenLop"].ToString().Trim();
            }

            cboLopMoi.DataSource = dt;
            cboLopMoi.DisplayMember = "TenLop";
            cboLopMoi.ValueMember = "MaLop";
            cboLopMoi.SelectedIndex = -1;
        }

        // Load dữ liệu
        private void LoadData()
        {
            HienThiDuLieu(lopBUS.GetAll());
        }

        // Hiển thị dữ liệu
        private void HienThiDuLieu(DataTable dt)
        {
            dgvLop.Rows.Clear();

            foreach (DataRow row in dt.Rows)
            {
                string maLop =
                    row["MaLop"]?.ToString().Trim() ?? "";

                string tenLop =
                    row["TenLop"]?.ToString().Trim() ?? "";

                string nienKhoa =
                    row["NienKhoa"]?.ToString().Trim() ?? "";

                string maNganh =
                    row["MaNganh"]?.ToString().Trim() ?? "";

                dgvLop.Rows.Add(
                    maLop,
                    tenLop,
                    nienKhoa,
                    maNganh
                );
            }

            dgvLop.ClearSelection();
        }

        // Hiển thị tìm kiếm
        private void HienThiDuLieu(List<LopDTO> danhSach)
        {
            dgvLop.Rows.Clear();

            foreach (LopDTO lop in danhSach)
            {
                dgvLop.Rows.Add(
                    lop.MaLop?.Trim() ?? "",
                    lop.TenLop?.Trim() ?? "",
                    lop.NienKhoa?.Trim() ?? "",
                    lop.MaNganh?.Trim() ?? ""
                );
            }

            dgvLop.ClearSelection();
        }

        // Lấy dữ liệu
        private LopDTO LayDuLieu()
        {
            return new LopDTO
            {
                MaLop = txtMaLop.Text.Trim(),
                TenLop = txtTenLop.Text.Trim(),
                NienKhoa = txtNienKhoa.Text.Trim(),

                MaNganh =
                    cboMaNganh.SelectedValue?
                    .ToString()
                    .Trim() ?? ""
            };
        }

        // Thêm lớp
        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                LopDTO lop = LayDuLieu();

                if (!lopBUS.Them(lop))
                {
                    MessageBox.Show(
                        "Thêm thất bại! Kiểm tra dữ liệu hoặc mã lớp bị trùng."
                    );

                    return;
                }

                MessageBox.Show("Thêm lớp thành công!");

                LoadData();
                LoadLopMoi();
                XoaTrang();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể thêm lớp!\n" +
                    ex.Message
                );
            }
        }

        // Sửa lớp
        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvLop.CurrentRow == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn lớp cần sửa!"
                );

                return;
            }

            try
            {
                LopDTO lop = LayDuLieu();

                if (!lopBUS.Sua(lop))
                {
                    MessageBox.Show(
                        "Cập nhật lớp thất bại!"
                    );

                    return;
                }

                MessageBox.Show(
                    "Cập nhật lớp thành công!"
                );

                LoadData();
                LoadLopMoi();
                XoaTrang();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể cập nhật lớp!\n" +
                    ex.Message
                );
            }
        }

        // Xóa lớp
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvLop.CurrentRow == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn lớp cần xóa!"
                );

                return;
            }

            string maLop =
                dgvLop.CurrentRow
                .Cells["MaLop"]
                .Value?
                .ToString()
                .Trim() ?? "";

            if (MessageBox.Show(
                    "Bạn có chắc muốn xóa lớp " + maLop + "?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question)
                != DialogResult.Yes)
            {
                return;
            }

            try
            {
                if (!lopBUS.Xoa(maLop))
                {
                    MessageBox.Show(
                        "Xóa lớp thất bại!"
                    );

                    return;
                }

                MessageBox.Show(
                    "Xóa lớp thành công!"
                );

                LoadData();
                LoadLopMoi();
                XoaTrang();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể xóa lớp!\n" +
                    ex.Message
                );
            }
        }

        // Chuyển SV và xóa lớp
        private void btnChuyenVaXoaLop_Click(
            object sender,
            EventArgs e)
        {
            if (dgvLop.CurrentRow == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn lớp cũ cần xóa!"
                );

                return;
            }

            if (cboLopMoi.SelectedValue == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn lớp chuyển đến!"
                );

                return;
            }

            string maLopCu =
                dgvLop.CurrentRow
                .Cells["MaLop"]
                .Value?
                .ToString()
                .Trim() ?? "";

            string maLopMoi =
                cboLopMoi.SelectedValue
                .ToString()
                .Trim();

            if (maLopCu == maLopMoi)
            {
                MessageBox.Show(
                    "Lớp cũ và lớp mới không được giống nhau!"
                );

                return;
            }

            DialogResult xacNhan =
                MessageBox.Show(
                    $"Chuyển toàn bộ sinh viên từ lớp {maLopCu} " +
                    $"sang lớp {maLopMoi} và xóa lớp {maLopCu}?",
                    "Xác nhận transaction",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

            if (xacNhan != DialogResult.Yes)
                return;

            try
            {
                string ketQua =
                    lopBUS.ChuyenSinhVienVaXoaLop(
                        maLopCu,
                        maLopMoi
                    );

                MessageBox.Show(
                    ketQua,
                    "Transaction thành công",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                LoadData();
                LoadLopMoi();
                XoaTrang();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Transaction thất bại!\n" +
                    "Dữ liệu đã được ROLLBACK.\n\n" +
                    ex.Message,
                    "Rollback",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                LoadData();
                LoadLopMoi();
            }
        }

        // Tìm kiếm
        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            try
            {
                string tuKhoa =
                    txtTimKiem.Text.Trim();

                if (string.IsNullOrWhiteSpace(tuKhoa))
                {
                    LoadData();
                    return;
                }

                HienThiDuLieu(
                    lopBUS.TimKiem(tuKhoa)
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tìm kiếm!\n" +
                    ex.Message
                );
            }
        }

        // Chọn lớp
        private void dgvLop_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row =
                dgvLop.Rows[e.RowIndex];

            txtMaLop.Text =
                row.Cells["MaLop"]
                .Value?
                .ToString()
                .Trim() ?? "";

            txtTenLop.Text =
                row.Cells["TenLop"]
                .Value?
                .ToString()
                .Trim() ?? "";

            txtNienKhoa.Text =
                row.Cells["NienKhoa"]
                .Value?
                .ToString()
                .Trim() ?? "";

            string maNganh =
                row.Cells["MaNganh"]
                .Value?
                .ToString()
                .Trim() ?? "";

            cboMaNganh.SelectedValue = maNganh;
            cboLopMoi.SelectedIndex = -1;
        }

        // Làm mới
        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            XoaTrang();
            LoadData();
            LoadLopMoi();
        }

        // Xóa trắng
        private void XoaTrang()
        {
            txtMaLop.Clear();
            txtTenLop.Clear();
            txtNienKhoa.Clear();
            txtTimKiem.Clear();

            cboMaNganh.SelectedIndex = -1;
            cboLopMoi.SelectedIndex = -1;

            dgvLop.ClearSelection();
        }

        // Co giãn bảng
        private void FLop_Resize(object sender, EventArgs e)
        {
            CanGiuaTieuDe();

            dgvLop.Left = 30;
            dgvLop.Width = this.ClientSize.Width - 60;

            dgvLop.Height =
                this.ClientSize.Height -
                dgvLop.Top - 30;
        }

        // Căn giữa tiêu đề
        private void CanGiuaTieuDe()
        {
            lblTieuDe.Left =
                (this.ClientSize.Width -
                 lblTieuDe.Width) / 2;
        }
    }
}