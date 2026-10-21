using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QLHSSV_Nhóm8.BUS;
using QLHSSV_Nhóm8.DTO;

namespace QLHSSV_Nhóm8.GUI
{
    public class FQuanLyTaiKhoan : Form
    {
        private Label lblTieuDe;

        private TextBox txtTenDangNhap;
        private TextBox txtMatKhau;
        private TextBox txtTimKiem;

        private ComboBox cboVaiTro;
        private CheckBox chkTrangThai;

        private Button btnThem;
        private Button btnSua;
        private Button btnXoa;
        private Button btnLamMoi;
        private Button btnTimKiem;

        private DataGridView dgvTaiKhoan;

        private readonly TaiKhoanBUS taiKhoanBUS =
            new TaiKhoanBUS();

        public FQuanLyTaiKhoan()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Form
            this.Text = "Quản lý tài khoản";
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // Tiêu đề
            lblTieuDe = new Label
            {
                Text = "QUẢN LÝ TÀI KHOẢN",
                Font = new Font("Arial", 20, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(500, 20)
            };
            this.Controls.Add(lblTieuDe);

            // Thông tin tài khoản
            TaoLabel("Tên đăng nhập:", 50, 100);
            TaoLabel("Mật khẩu:", 50, 150);
            TaoLabel("Vai trò:", 560, 100);
            TaoLabel("Trạng thái:", 560, 150);

            txtTenDangNhap =
                TaoTextBox(180, 98, 300);

            txtMatKhau =
                TaoTextBox(180, 148, 300);

            txtMatKhau.UseSystemPasswordChar = true;

            cboVaiTro = new ComboBox
            {
                Location = new Point(660, 98),
                Size = new Size(300, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            cboVaiTro.Items.AddRange(
                new object[]
                {
                    "Admin",
                    "Nhân viên"
                });

            this.Controls.Add(cboVaiTro);

            chkTrangThai = new CheckBox
            {
                Text = "Hoạt động",
                Location = new Point(660, 148),
                AutoSize = true,
                Checked = true
            };
            this.Controls.Add(chkTrangThai);

            // Button
            btnThem = TaoButton("Thêm", 250, 220);
            btnSua = TaoButton("Sửa", 370, 220);
            btnXoa = TaoButton("Xóa", 490, 220);
            btnLamMoi = TaoButton("Làm mới", 610, 220);

            // Tìm kiếm
            TaoLabel("Tìm kiếm:", 50, 290);

            txtTimKiem =
                TaoTextBox(130, 288, 350);

            btnTimKiem =
                TaoButton("Tìm", 500, 280);

            // Bảng dữ liệu
            dgvTaiKhoan = new DataGridView
            {
                Location = new Point(30, 340),
                Size = new Size(1000, 350),
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

            dgvTaiKhoan.Columns.Add(
                "TenDangNhap",
                "Tên đăng nhập");

            dgvTaiKhoan.Columns.Add(
                "VaiTro",
                "Vai trò");

            dgvTaiKhoan.Columns.Add(
                "TrangThai",
                "Trạng thái");

            this.Controls.Add(dgvTaiKhoan);

            // Event
            this.Load += FQuanLyTaiKhoan_Load;
            this.Resize += FQuanLyTaiKhoan_Resize;

            btnThem.Click += btnThem_Click;
            btnSua.Click += btnSua_Click;
            btnXoa.Click += btnXoa_Click;
            btnLamMoi.Click += btnLamMoi_Click;
            btnTimKiem.Click += btnTimKiem_Click;

            dgvTaiKhoan.CellClick +=
                dgvTaiKhoan_CellClick;
        }

        private Label TaoLabel(
            string text,
            int x,
            int y)
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

        private TextBox TaoTextBox(
            int x,
            int y,
            int width)
        {
            TextBox txt = new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(width, 25),
                Font = new Font("Arial", 10)
            };

            this.Controls.Add(txt);
            return txt;
        }

        private Button TaoButton(
            string text,
            int x,
            int y)
        {
            Button btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(100, 40),
                Font = new Font(
                    "Arial",
                    10,
                    FontStyle.Bold)
            };

            this.Controls.Add(btn);
            return btn;
        }

        private void FQuanLyTaiKhoan_Load(
            object sender,
            EventArgs e)
        {
            LoadData();
            XoaTrang();
            CanGiuaTieuDe();
        }

        private void LoadData()
        {
            HienThiDuLieu(
                taiKhoanBUS.GetAll());
        }

        // Hiển thị dữ liệu
        private void HienThiDuLieu(DataTable dt)
        {
            dgvTaiKhoan.Rows.Clear();

            foreach (DataRow row in dt.Rows)
            {
                bool trangThai =
                    Convert.ToBoolean(
                        row["TrangThai"]);

                dgvTaiKhoan.Rows.Add(
                    row["TenDangNhap"],
                    row["VaiTro"],
                    trangThai
                        ? "Hoạt động"
                        : "Đã khóa"
                );
            }

            dgvTaiKhoan.ClearSelection();
        }

        // Lấy dữ liệu từ Form
        private TaiKhoanDTO LayDuLieu()
        {
            return new TaiKhoanDTO
            {
                TenDangNhap =
                    txtTenDangNhap.Text.Trim(),

                MatKhau =
                    txtMatKhau.Text,

                VaiTro =
                    cboVaiTro.Text,

                TrangThai =
                    chkTrangThai.Checked
            };
        }

        private void btnThem_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                TaiKhoanDTO tk =
                    LayDuLieu();

                if (!taiKhoanBUS.Them(tk))
                {
                    MessageBox.Show(
                        "Thêm tài khoản thất bại! " +
                        "Kiểm tra dữ liệu hoặc tên đăng nhập đã tồn tại.");
                    return;
                }

                MessageBox.Show(
                    "Thêm tài khoản thành công!");

                LoadData();
                XoaTrang();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể thêm tài khoản!\n" +
                    ex.Message);
            }
        }

        private void btnSua_Click(
            object sender,
            EventArgs e)
        {
            if (dgvTaiKhoan.CurrentRow == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn tài khoản cần sửa!");
                return;
            }

            if (string.IsNullOrWhiteSpace(
                txtMatKhau.Text))
            {
                MessageBox.Show(
                    "Vui lòng nhập mật khẩu mới!");

                txtMatKhau.Focus();
                return;
            }

            try
            {
                TaiKhoanDTO tk =
                    LayDuLieu();

                if (!taiKhoanBUS.Sua(tk))
                {
                    MessageBox.Show(
                        "Cập nhật tài khoản thất bại!");
                    return;
                }

                MessageBox.Show(
                    "Cập nhật tài khoản thành công!");

                LoadData();
                XoaTrang();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể cập nhật tài khoản!\n" +
                    ex.Message);
            }
        }

        private void btnXoa_Click(
            object sender,
            EventArgs e)
        {
            if (dgvTaiKhoan.CurrentRow == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn tài khoản cần xóa!");
                return;
            }

            string tenDangNhap =
                dgvTaiKhoan.CurrentRow
                .Cells["TenDangNhap"]
                .Value?.ToString() ?? "";

            if (MessageBox.Show(
                    "Bạn có chắc muốn xóa tài khoản " +
                    tenDangNhap + "?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question)
                != DialogResult.Yes)
            {
                return;
            }

            try
            {
                if (!taiKhoanBUS.Xoa(
                    tenDangNhap))
                {
                    MessageBox.Show(
                        "Không thể xóa tài khoản này. " +
                        "Tài khoản admin chính không được phép xóa.");
                    return;
                }

                MessageBox.Show(
                    "Xóa tài khoản thành công!");

                LoadData();
                XoaTrang();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể xóa tài khoản!\n" +
                    ex.Message);
            }
        }

        private void btnTimKiem_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                HienThiDuLieu(
                    taiKhoanBUS.TimKiem(
                        txtTimKiem.Text.Trim()));
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tìm kiếm!\n" +
                    ex.Message);
            }
        }

        private void dgvTaiKhoan_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row =
                dgvTaiKhoan.Rows[e.RowIndex];

            txtTenDangNhap.Text =
                row.Cells["TenDangNhap"]
                .Value?.ToString() ?? "";

            cboVaiTro.Text =
                row.Cells["VaiTro"]
                .Value?.ToString() ?? "";

            string trangThai =
                row.Cells["TrangThai"]
                .Value?.ToString() ?? "";

            chkTrangThai.Checked =
                trangThai == "Hoạt động";

            // Không hiện mật khẩu cũ
            txtMatKhau.Clear();

            // Không sửa tên đăng nhập
            txtTenDangNhap.ReadOnly = true;
        }

        private void btnLamMoi_Click(
            object sender,
            EventArgs e)
        {
            XoaTrang();
            LoadData();
        }

        private void XoaTrang()
        {
            txtTenDangNhap.ReadOnly = false;

            txtTenDangNhap.Clear();
            txtMatKhau.Clear();
            txtTimKiem.Clear();

            cboVaiTro.SelectedIndex = -1;
            chkTrangThai.Checked = true;

            dgvTaiKhoan.ClearSelection();
        }

        private void FQuanLyTaiKhoan_Resize(
            object sender,
            EventArgs e)
        {
            CanGiuaTieuDe();

            dgvTaiKhoan.Left = 30;
            dgvTaiKhoan.Width =
                this.ClientSize.Width - 60;

            dgvTaiKhoan.Height =
                this.ClientSize.Height -
                dgvTaiKhoan.Top - 30;
        }

        private void CanGiuaTieuDe()
        {
            lblTieuDe.Left =
                (this.ClientSize.Width -
                 lblTieuDe.Width) / 2;
        }
    }
}
