using QLHSSV.DTO;
using QLHSSV_Nhóm8.BUS;
using QLHSSV_Nhóm8.DTO;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace QLHSSV_Nhóm8.GUI
{
    public partial class FSinhVien : Form
    {
        private Label lblTieuDe;
        private Label lblMaSV;
        private Label lblHoTen;
        private Label lblCCCD;
        private Label lblNgaySinh;
        private Label lblGioiTinh;
        private Label lblSDT;
        private Label lblEmail;
        private Label lblDiaChi;
        private Label lblDanToc;
        private Label lblTrangThai;
        private Label lblMaLop;
        private Label lblTimKiem;

        private TextBox txtMaSV;
        private TextBox txtHoTen;
        private TextBox txtCCCD;
        private TextBox txtSDT;
        private TextBox txtEmail;
        private TextBox txtDiaChi;
        private TextBox txtDanToc;
        private TextBox txtTimKiem;

        private DateTimePicker dtpNgaySinh;

        private ComboBox cboGioiTinh;
        private ComboBox cboTrangThai;
        private ComboBox cboMaLop;

        private Button btnThem;
        private Button btnSua;
        private Button btnXoa;
        private Button btnLamMoi;
        private Button btnDemSinhVien;
        private Button btnLichSuXoa;
        private Button btnTimKiem;

        private DataGridView dgvSinhVien;

        private readonly SinhVienBUS sinhVienBUS =
            new SinhVienBUS();

        private readonly LopBUS lopBUS =
            new LopBUS();

        public FSinhVien()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Form
            Text = "Quản lý hồ sơ sinh viên";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            BackColor = Color.White;

            // Tiêu đề
            lblTieuDe = new Label
            {
                Text = "QUẢN LÝ HỒ SƠ SINH VIÊN",
                Font = new Font("Arial", 20, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(350, 20)
            };
            Controls.Add(lblTieuDe);

            // Label
            lblMaSV = TaoLabel("Mã sinh viên:", 30, 80);
            lblHoTen = TaoLabel("Họ tên:", 30, 120);
            lblCCCD = TaoLabel("CCCD:", 30, 160);
            lblNgaySinh = TaoLabel("Ngày sinh:", 30, 200);
            lblGioiTinh = TaoLabel("Giới tính:", 30, 240);
            lblSDT = TaoLabel("Số điện thoại:", 30, 280);

            lblEmail = TaoLabel("Email:", 570, 80);
            lblDiaChi = TaoLabel("Địa chỉ:", 570, 120);
            lblDanToc = TaoLabel("Dân tộc:", 570, 160);
            lblTrangThai = TaoLabel("Trạng thái học tập:", 570, 200);
            lblMaLop = TaoLabel("Lớp:", 570, 240);

            // TextBox
            txtMaSV = TaoTextBox(160, 78, 300);
            txtHoTen = TaoTextBox(160, 118, 300);
            txtCCCD = TaoTextBox(160, 158, 300);
            txtSDT = TaoTextBox(160, 278, 300);

            txtEmail = TaoTextBox(720, 78, 300);
            txtDiaChi = TaoTextBox(720, 118, 300);
            txtDanToc = TaoTextBox(720, 158, 300);

            // Ngày sinh
            dtpNgaySinh = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Location = new Point(160, 198),
                Size = new Size(300, 25)
            };
            Controls.Add(dtpNgaySinh);

            // Giới tính
            cboGioiTinh = new ComboBox
            {
                Location = new Point(160, 238),
                Size = new Size(300, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboGioiTinh.Items.AddRange(
                new object[] { "Nam", "Nữ" });
            Controls.Add(cboGioiTinh);

            // Trạng thái
            cboTrangThai = new ComboBox
            {
                Location = new Point(720, 198),
                Size = new Size(300, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboTrangThai.Items.AddRange(
                new object[]
                {
                    "Đang học",
                    "Bảo lưu",
                    "Đã tốt nghiệp",
                    "Đình chỉ"
                });
            Controls.Add(cboTrangThai);

            // Lớp
            cboMaLop = new ComboBox
            {
                Location = new Point(720, 238),
                Size = new Size(300, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            Controls.Add(cboMaLop);

            // Button
            btnThem = TaoButton("Thêm", 200, 330);
            btnSua = TaoButton("Sửa", 320, 330);
            btnXoa = TaoButton("Xóa", 440, 330);
            btnLamMoi = TaoButton("Làm mới", 560, 330);

            btnDemSinhVien =
                TaoButton("Số SV trong lớp", 680, 330);
            btnDemSinhVien.Size =
                new Size(150, 40);

            btnLichSuXoa =
                TaoButton("Lịch sử xóa", 850, 330);
            btnLichSuXoa.Size =
                new Size(130, 40);

            // Tìm kiếm
            lblTimKiem =
                TaoLabel("Tìm kiếm:", 30, 390);

            txtTimKiem =
                TaoTextBox(110, 388, 350);

            btnTimKiem =
                TaoButton("Tìm", 480, 380);

            // DataGridView
            dgvSinhVien = new DataGridView
            {
                Location = new Point(30, 450),
                Size = new Size(1000, 220),
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
                    DataGridViewSelectionMode.FullRowSelect
            };

            dgvSinhVien.Columns.Add("MaSV", "Mã SV");
            dgvSinhVien.Columns.Add("HoTen", "Họ tên");
            dgvSinhVien.Columns.Add("CCCD", "CCCD");
            dgvSinhVien.Columns.Add("NgaySinh", "Ngày sinh");
            dgvSinhVien.Columns.Add("GioiTinh", "Giới tính");
            dgvSinhVien.Columns.Add("SDT", "SĐT");
            dgvSinhVien.Columns.Add("Email", "Email");
            dgvSinhVien.Columns.Add("DiaChi", "Địa chỉ");
            dgvSinhVien.Columns.Add("DanToc", "Dân tộc");
            dgvSinhVien.Columns.Add(
                "TrangThaiHocTap",
                "Trạng thái");
            dgvSinhVien.Columns.Add("MaLop", "Mã lớp");

            Controls.Add(dgvSinhVien);

            // Event
            Load += FSinhVien_Load;
            Resize += FSinhVien_Resize;

            btnThem.Click += btnThem_Click;
            btnSua.Click += btnSua_Click;
            btnXoa.Click += btnXoa_Click;
            btnLamMoi.Click += btnLamMoi_Click;
            btnTimKiem.Click += btnTimKiem_Click;
            btnDemSinhVien.Click +=
                btnDemSinhVien_Click;
            btnLichSuXoa.Click +=
                btnLichSuXoa_Click;

            dgvSinhVien.CellClick +=
                dgvSinhVien_CellClick;
        }

        private Label TaoLabel(
            string text,
            int x,
            int y)
        {
            Label label = new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Arial", 10),
                Location = new Point(x, y)
            };

            Controls.Add(label);
            return label;
        }

        private TextBox TaoTextBox(
            int x,
            int y,
            int width)
        {
            TextBox textBox = new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(width, 25),
                Font = new Font("Arial", 10)
            };

            Controls.Add(textBox);
            return textBox;
        }

        private Button TaoButton(
            string text,
            int x,
            int y)
        {
            Button button = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(100, 40),
                Font = new Font(
                    "Arial",
                    10,
                    FontStyle.Bold)
            };

            Controls.Add(button);
            return button;
        }

        private void FSinhVien_Load(
            object sender,
            EventArgs e)
        {
            dtpNgaySinh.Value =
                new DateTime(2005, 1, 1);

            cboGioiTinh.SelectedIndex = -1;
            cboTrangThai.SelectedIndex = -1;

            LoadLop();
            LoadData();
            CanGiuaTieuDe();
        }

        private void LoadLop()
        {
            DataTable dt =
                lopBUS.GetAll();

            cboMaLop.DataSource = dt;
            cboMaLop.DisplayMember = "TenLop";
            cboMaLop.ValueMember = "MaLop";
            cboMaLop.SelectedIndex = -1;
        }

        private SinhVienDTO LayDuLieu()
        {
            return new SinhVienDTO
            {
                MaSV =
                    txtMaSV.Text.Trim(),

                HoTen =
                    txtHoTen.Text.Trim(),

                CCCD =
                    txtCCCD.Text.Trim(),

                NgaySinh =
                    dtpNgaySinh.Value,

                GioiTinh =
                    cboGioiTinh.Text,

                SDT =
                    txtSDT.Text.Trim(),

                Email =
                    txtEmail.Text.Trim(),

                DiaChi =
                    txtDiaChi.Text.Trim(),

                DanToc =
                    txtDanToc.Text.Trim(),

                TrangThaiHocTap =
                    cboTrangThai.Text,

                MaLop =
                    cboMaLop.SelectedValue?
                    .ToString() ?? ""
            };
        }

        private void btnThem_Click(
            object sender,
            EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(
                txtMaSV.Text))
            {
                MessageBox.Show(
                    "Vui lòng nhập mã sinh viên!");
                txtMaSV.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(
                txtHoTen.Text))
            {
                MessageBox.Show(
                    "Vui lòng nhập họ tên!");
                txtHoTen.Focus();
                return;
            }

            try
            {
                SinhVienDTO sv =
                    LayDuLieu();

                if (!sinhVienBUS.Them(sv))
                {
                    MessageBox.Show(
                        "Thêm sinh viên thất bại!\n" +
                        "Mã sinh viên có thể đã tồn tại " +
                        "hoặc dữ liệu chưa hợp lệ.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show(
                    "Thêm sinh viên thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadData();
                XoaTrang();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể thêm sinh viên!\n\n" +
                    ex.Message,
                    "Dữ liệu không hợp lệ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                LoadData();
            }
        }

        private void btnSua_Click(
            object sender,
            EventArgs e)
        {
            if (dgvSinhVien.CurrentRow == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn sinh viên cần sửa!");
                return;
            }

            try
            {
                string maLopCu =
                    dgvSinhVien.CurrentRow
                    .Cells["MaLop"]
                    .Value?.ToString() ?? "";

                string maLopMoi =
                    cboMaLop.SelectedValue?
                    .ToString() ?? "";

                if (string.IsNullOrWhiteSpace(
                    maLopMoi))
                {
                    MessageBox.Show(
                        "Vui lòng chọn lớp!");
                    return;
                }

                SinhVienDTO sv =
                    LayDuLieu();

                // Sửa thông tin, giữ lớp cũ
                sv.MaLop = maLopCu;

                if (!sinhVienBUS.Sua(sv))
                {
                    MessageBox.Show(
                        "Cập nhật sinh viên thất bại!");
                    return;
                }

                // Đổi lớp bằng sp_ChuyenLop
                if (maLopCu != maLopMoi)
                {
                    if (!sinhVienBUS.ChuyenLop(
                        sv.MaSV,
                        maLopMoi))
                    {
                        MessageBox.Show(
                            "Thông tin đã cập nhật nhưng chuyển lớp thất bại!");
                        LoadData();
                        return;
                    }

                    MessageBox.Show(
                        $"Cập nhật thành công!\n" +
                        $"Sinh viên {sv.MaSV} đã chuyển " +
                        $"từ lớp {maLopCu} sang {maLopMoi}.");
                }
                else
                {
                    MessageBox.Show(
                        "Cập nhật sinh viên thành công!");
                }

                LoadData();
                XoaTrang();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể cập nhật sinh viên!\n" +
                    ex.Message);
            }
        }

        private void btnXoa_Click(
            object sender,
            EventArgs e)
        {
            if (dgvSinhVien.CurrentRow == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn sinh viên cần xóa!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            string maSV =
                dgvSinhVien.CurrentRow
                .Cells["MaSV"]
                .Value?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(maSV))
            {
                MessageBox.Show(
                    "Không xác định được mã sinh viên!");
                return;
            }

            DialogResult result =
                MessageBox.Show(
                    $"Bạn có chắc muốn xóa sinh viên {maSV}?\n\n" +
                    "Toàn bộ dữ liệu liên quan của sinh viên cũng sẽ bị xóa.",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            try
            {
                if (!sinhVienBUS.Xoa(maSV))
                {
                    LoadData();
                    XoaTrang();

                    MessageBox.Show(
                        "Không thể xóa sinh viên hoặc sinh viên không còn tồn tại!");
                    return;
                }

                LoadData();
                XoaTrang();

                MessageBox.Show(
                    "Xóa sinh viên và toàn bộ dữ liệu liên quan thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Xóa sinh viên thất bại!\n" +
                    ex.Message);
            }
        }

        private void btnDemSinhVien_Click(
            object sender,
            EventArgs e)
        {
            if (cboMaLop.SelectedValue == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn sinh viên hoặc chọn lớp!");
                return;
            }

            string maLop =
                cboMaLop.SelectedValue
                .ToString() ?? "";

            try
            {
                int soLuong =
                    sinhVienBUS
                    .DemSinhVienTheoLop(maLop);

                MessageBox.Show(
                    $"Lớp {maLop} hiện có {soLuong} sinh viên.",
                    "Thống kê sinh viên",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể thống kê sinh viên!\n" +
                    ex.Message);
            }
        }

        private void btnLichSuXoa_Click(
            object sender,
            EventArgs e)
        {
            using (FLichSuXoaSinhVien form =
                   new FLichSuXoaSinhVien())
            {
                form.ShowDialog();
            }
        }

        private void btnLamMoi_Click(
            object sender,
            EventArgs e)
        {
            XoaTrang();
            LoadData();
        }

        private void dgvSinhVien_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row =
                dgvSinhVien.Rows[e.RowIndex];

            txtMaSV.Text =
                row.Cells["MaSV"]
                .Value?.ToString() ?? "";

            txtHoTen.Text =
                row.Cells["HoTen"]
                .Value?.ToString() ?? "";

            txtCCCD.Text =
                row.Cells["CCCD"]
                .Value?.ToString() ?? "";

            txtSDT.Text =
                row.Cells["SDT"]
                .Value?.ToString() ?? "";

            txtEmail.Text =
                row.Cells["Email"]
                .Value?.ToString() ?? "";

            txtDiaChi.Text =
                row.Cells["DiaChi"]
                .Value?.ToString() ?? "";

            txtDanToc.Text =
                row.Cells["DanToc"]
                .Value?.ToString() ?? "";

            cboGioiTinh.Text =
                row.Cells["GioiTinh"]
                .Value?.ToString() ?? "";

            cboTrangThai.Text =
                row.Cells["TrangThaiHocTap"]
                .Value?.ToString() ?? "";

            cboMaLop.SelectedValue =
                row.Cells["MaLop"]
                .Value?.ToString() ?? "";

            string ngay =
                row.Cells["NgaySinh"]
                .Value?.ToString() ?? "";

            if (DateTime.TryParse(
                ngay,
                out DateTime ngaySinh))
            {
                dtpNgaySinh.Value =
                    ngaySinh;
            }
        }

        private void btnTimKiem_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                DataTable danhSach =
                    sinhVienBUS.TimKiem(
                        txtTimKiem.Text.Trim());

                HienThiDuLieu(danhSach);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tìm kiếm!\n" +
                    ex.Message);
            }
        }

        private void LoadData()
        {
            HienThiDuLieu(
                sinhVienBUS.GetAll());
        }

        private void HienThiDuLieu(
            DataTable dt)
        {
            dgvSinhVien.Rows.Clear();

            foreach (DataRow row in dt.Rows)
            {
                string ngaySinh = "";

                if (row["NgaySinh"] != DBNull.Value)
                {
                    ngaySinh =
                        Convert.ToDateTime(
                            row["NgaySinh"])
                        .ToString("dd/MM/yyyy");
                }

                dgvSinhVien.Rows.Add(
                    row["MaSV"].ToString(),
                    row["HoTen"].ToString(),
                    row["CCCD"].ToString(),
                    ngaySinh,
                    row["GioiTinh"].ToString(),
                    row["SDT"].ToString(),
                    row["Email"].ToString(),
                    row["DiaChi"].ToString(),
                    row["DanToc"].ToString(),
                    row["TrangThaiHocTap"].ToString(),
                    row["MaLop"].ToString());
            }

            dgvSinhVien.ClearSelection();
        }

        private void XoaTrang()
        {
            txtMaSV.Clear();
            txtHoTen.Clear();
            txtCCCD.Clear();
            txtSDT.Clear();
            txtEmail.Clear();
            txtDiaChi.Clear();
            txtDanToc.Clear();
            txtTimKiem.Clear();

            dtpNgaySinh.Value =
                new DateTime(2005, 1, 1);

            cboGioiTinh.SelectedIndex = -1;
            cboTrangThai.SelectedIndex = -1;
            cboMaLop.SelectedIndex = -1;

            dgvSinhVien.ClearSelection();
        }

        private void FSinhVien_Resize(
            object sender,
            EventArgs e)
        {
            CanGiuaTieuDe();

            dgvSinhVien.Width =
                ClientSize.Width - 60;

            dgvSinhVien.Height =
                ClientSize.Height - 480;
        }

        private void CanGiuaTieuDe()
        {
            lblTieuDe.Left =
                (ClientSize.Width -
                 lblTieuDe.Width) / 2;
        }
    }
}
