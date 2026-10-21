using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QLHSSV_Nhóm8.BUS;
using QLHSSV_Nhóm8.DTO;

namespace QLHSSV_Nhóm8.GUI
{
    public class FNguoiThan : Form
    {
        private Label lblTieuDe;

        private TextBox txtMaNT;
        private TextBox txtHoTen;
        private TextBox txtNgheNghiep;
        private TextBox txtSDT;
        private TextBox txtDiaChi;
        private TextBox txtDanToc;
        private TextBox txtTimKiem;

        private ComboBox cboMoiQuanHe;
        private ComboBox cboGioiTinh;
        private ComboBox cboMaSV;

        private DateTimePicker dtpNgaySinh;

        private Button btnThem;
        private Button btnSua;
        private Button btnXoa;
        private Button btnLamMoi;
        private Button btnTimKiem;

        private DataGridView dgvNguoiThan;

        private readonly NguoiThanBUS nguoiThanBUS =
            new NguoiThanBUS();

        private readonly SinhVienBUS sinhVienBUS =
            new SinhVienBUS();

        public FNguoiThan()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Form
            this.Text = "Quản lý người thân";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.White;

            // Tiêu đề
            lblTieuDe = new Label
            {
                Text = "QUẢN LÝ NGƯỜI THÂN",
                Font = new Font("Arial", 20, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(500, 20)
            };
            this.Controls.Add(lblTieuDe);

            // Cột trái
            TaoLabel("Mã người thân:", 50, 90);
            TaoLabel("Họ tên:", 50, 135);
            TaoLabel("Mối quan hệ:", 50, 180);
            TaoLabel("Nghề nghiệp:", 50, 225);
            TaoLabel("SĐT:", 50, 270);

            txtMaNT = TaoTextBox(170, 88, 300);
            txtHoTen = TaoTextBox(170, 133, 300);

            cboMoiQuanHe = new ComboBox
            {
                Location = new Point(170, 178),
                Size = new Size(300, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboMoiQuanHe.Items.AddRange(
                new object[] { "Ba", "Mẹ", "Anh", "Chị", "Em", "Khác" });
            this.Controls.Add(cboMoiQuanHe);

            txtNgheNghiep = TaoTextBox(170, 223, 300);
            txtSDT = TaoTextBox(170, 268, 300);

            // Cột phải
            TaoLabel("Ngày sinh:", 560, 90);
            TaoLabel("Địa chỉ:", 560, 135);
            TaoLabel("Dân tộc:", 560, 180);
            TaoLabel("Giới tính:", 560, 225);
            TaoLabel("Sinh viên:", 560, 270);

            dtpNgaySinh = new DateTimePicker
            {
                Location = new Point(660, 88),
                Size = new Size(300, 25),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy"
            };
            this.Controls.Add(dtpNgaySinh);

            txtDiaChi = TaoTextBox(660, 133, 300);
            txtDanToc = TaoTextBox(660, 178, 300);

            cboGioiTinh = new ComboBox
            {
                Location = new Point(660, 223),
                Size = new Size(300, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboGioiTinh.Items.AddRange(
                new object[] { "Nam", "Nữ" });
            this.Controls.Add(cboGioiTinh);

            cboMaSV = new ComboBox
            {
                Location = new Point(660, 268),
                Size = new Size(300, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.Controls.Add(cboMaSV);

            // Button
            btnThem = TaoButton("Thêm", 280, 330);
            btnSua = TaoButton("Sửa", 400, 330);
            btnXoa = TaoButton("Xóa", 520, 330);
            btnLamMoi = TaoButton("Làm mới", 640, 330);

            // Tìm kiếm
            TaoLabel("Tìm kiếm:", 50, 395);
            txtTimKiem = TaoTextBox(130, 393, 350);
            btnTimKiem = TaoButton("Tìm", 500, 385);

            // DataGridView
            dgvNguoiThan = new DataGridView
            {
                Location = new Point(30, 445),
                Size = new Size(1100, 300),
                Anchor = AnchorStyles.Top |
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

            dgvNguoiThan.Columns.Add("MaNT", "Mã NT");
            dgvNguoiThan.Columns.Add("HoTen", "Họ tên");
            dgvNguoiThan.Columns.Add("MoiQuanHe", "Mối quan hệ");
            dgvNguoiThan.Columns.Add("NgheNghiep", "Nghề nghiệp");
            dgvNguoiThan.Columns.Add("SDT", "SĐT");
            dgvNguoiThan.Columns.Add("NgaySinh", "Ngày sinh");
            dgvNguoiThan.Columns.Add("DiaChi", "Địa chỉ");
            dgvNguoiThan.Columns.Add("DanToc", "Dân tộc");
            dgvNguoiThan.Columns.Add("GioiTinh", "Giới tính");
            dgvNguoiThan.Columns.Add("MaSV", "Mã SV");

            this.Controls.Add(dgvNguoiThan);

            // Event
            this.Load += FNguoiThan_Load;
            this.Resize += FNguoiThan_Resize;

            btnThem.Click += btnThem_Click;
            btnSua.Click += btnSua_Click;
            btnXoa.Click += btnXoa_Click;
            btnLamMoi.Click += btnLamMoi_Click;
            btnTimKiem.Click += btnTimKiem_Click;

            dgvNguoiThan.CellClick += dgvNguoiThan_CellClick;
        }

        private Label TaoLabel(string text, int x, int y)
        {
            Label lbl = new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Arial", 10),
                Location = new Point(x, y)
            };

            this.Controls.Add(lbl);
            return lbl;
        }

        private TextBox TaoTextBox(int x, int y, int width)
        {
            TextBox txt = new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(width, 25)
            };

            this.Controls.Add(txt);
            return txt;
        }

        private Button TaoButton(string text, int x, int y)
        {
            Button btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(100, 40),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            this.Controls.Add(btn);
            return btn;
        }

        private void FNguoiThan_Load(object sender, EventArgs e)
        {
            LoadSinhVien();
            LoadData();
            XoaTrang();
        }

        // Load danh sách sinh viên cho ComboBox
        private void LoadSinhVien()
        {
            DataTable dt = sinhVienBUS.GetAll();

            cboMaSV.DataSource = dt;
            cboMaSV.DisplayMember = "HoTen";
            cboMaSV.ValueMember = "MaSV";
            cboMaSV.SelectedIndex = -1;
        }

        private void LoadData()
        {
            HienThiDuLieu(nguoiThanBUS.GetAll());
        }

        // Hiển thị dữ liệu lên bảng
        private void HienThiDuLieu(DataTable dt)
        {
            dgvNguoiThan.Rows.Clear();

            foreach (DataRow row in dt.Rows)
            {
                dgvNguoiThan.Rows.Add(
                    row["MaNT"],
                    row["HoTen"],
                    row["MoiQuanHe"],
                    row["NgheNghiep"],
                    row["SDT"],
                    Convert.ToDateTime(row["NgaySinh"])
                        .ToString("dd/MM/yyyy"),
                    row["DiaChi"],
                    row["DanToc"],
                    row["GioiTinh"],
                    row["MaSV"]
                );
            }

            dgvNguoiThan.ClearSelection();
        }

        // Lấy dữ liệu từ Form
        private NguoiThanDTO LayDuLieu()
        {
            return new NguoiThanDTO
            {
                MaNT = txtMaNT.Text.Trim(),
                HoTen = txtHoTen.Text.Trim(),
                MoiQuanHe = cboMoiQuanHe.Text,
                NgheNghiep = txtNgheNghiep.Text.Trim(),
                SDT = txtSDT.Text.Trim(),
                NgaySinh = dtpNgaySinh.Value,
                DiaChi = txtDiaChi.Text.Trim(),
                DanToc = txtDanToc.Text.Trim(),
                GioiTinh = cboGioiTinh.Text,
                MaSV = cboMaSV.SelectedValue?.ToString() ?? ""
            };
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                NguoiThanDTO nt = LayDuLieu();

                if (!nguoiThanBUS.Them(nt))
                {
                    MessageBox.Show(
                        "Thêm thất bại! Kiểm tra dữ liệu hoặc mã đã tồn tại.");
                    return;
                }

                MessageBox.Show("Thêm người thân thành công!");
                LoadData();
                XoaTrang();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể thêm người thân!\n" + ex.Message);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvNguoiThan.CurrentRow == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn người thân cần sửa!");
                return;
            }

            try
            {
                NguoiThanDTO nt = LayDuLieu();

                if (!nguoiThanBUS.Sua(nt))
                {
                    MessageBox.Show(
                        "Cập nhật thất bại! Kiểm tra dữ liệu.");
                    return;
                }

                MessageBox.Show(
                    "Cập nhật người thân thành công!");
                LoadData();
                XoaTrang();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể cập nhật người thân!\n" + ex.Message);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvNguoiThan.CurrentRow == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn người thân cần xóa!");
                return;
            }

            string ma = dgvNguoiThan.CurrentRow
                .Cells["MaNT"]
                .Value?.ToString() ?? "";

            if (MessageBox.Show(
                    "Bạn có chắc muốn xóa " + ma + "?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question)
                != DialogResult.Yes)
            {
                return;
            }

            try
            {
                if (!nguoiThanBUS.Xoa(ma))
                {
                    MessageBox.Show("Xóa người thân thất bại!");
                    return;
                }

                MessageBox.Show("Xóa người thân thành công!");
                LoadData();
                XoaTrang();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể xóa người thân!\n" + ex.Message);
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            try
            {
                HienThiDuLieu(
                    nguoiThanBUS.TimKiem(
                        txtTimKiem.Text.Trim()));
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tìm kiếm!\n" + ex.Message);
            }
        }

        private void dgvNguoiThan_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row =
                dgvNguoiThan.Rows[e.RowIndex];

            txtMaNT.Text =
                row.Cells["MaNT"].Value?.ToString() ?? "";

            txtHoTen.Text =
                row.Cells["HoTen"].Value?.ToString() ?? "";

            cboMoiQuanHe.Text =
                row.Cells["MoiQuanHe"].Value?.ToString() ?? "";

            txtNgheNghiep.Text =
                row.Cells["NgheNghiep"].Value?.ToString() ?? "";

            txtSDT.Text =
                row.Cells["SDT"].Value?.ToString() ?? "";

            if (DateTime.TryParse(
                row.Cells["NgaySinh"].Value?.ToString(),
                out DateTime ngaySinh))
            {
                dtpNgaySinh.Value = ngaySinh;
            }

            txtDiaChi.Text =
                row.Cells["DiaChi"].Value?.ToString() ?? "";

            txtDanToc.Text =
                row.Cells["DanToc"].Value?.ToString() ?? "";

            cboGioiTinh.Text =
                row.Cells["GioiTinh"].Value?.ToString() ?? "";

            cboMaSV.SelectedValue =
                row.Cells["MaSV"].Value?.ToString() ?? "";
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            XoaTrang();
            LoadData();
        }

        private void XoaTrang()
        {
            txtMaNT.Clear();
            txtHoTen.Clear();
            txtNgheNghiep.Clear();
            txtSDT.Clear();
            txtDiaChi.Clear();
            txtDanToc.Clear();
            txtTimKiem.Clear();

            cboMoiQuanHe.SelectedIndex = -1;
            cboGioiTinh.SelectedIndex = -1;
            cboMaSV.SelectedIndex = -1;

            dtpNgaySinh.Value =
                new DateTime(1975, 1, 1);

            dgvNguoiThan.ClearSelection();
        }

        private void FNguoiThan_Resize(object sender, EventArgs e)
        {
            lblTieuDe.Left =
                (this.ClientSize.Width - lblTieuDe.Width) / 2;

            dgvNguoiThan.Left = 30;
            dgvNguoiThan.Width =
                this.ClientSize.Width - 60;

            dgvNguoiThan.Height =
                this.ClientSize.Height -
                dgvNguoiThan.Top - 30;
        }
    }
}
