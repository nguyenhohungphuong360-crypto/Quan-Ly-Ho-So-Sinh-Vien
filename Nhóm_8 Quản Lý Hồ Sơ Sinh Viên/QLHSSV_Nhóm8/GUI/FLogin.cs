using System;
using System.Drawing;
using System.Windows.Forms;
using QLHSSV_Nhóm8.BUS;
using QLHSSV_Nhóm8.DTO;
using QLHSSV_Nhóm8.Utils;

namespace QLHSSV_Nhóm8.GUI
{
    public class FLogin : Form
    {
        private Label lblTieuDe;

        private TextBox txtTenDangNhap;
        private TextBox txtMatKhau;

        private CheckBox chkHienMatKhau;

        private Button btnDangNhap;
        private Button btnThoat;

        private readonly TaiKhoanBUS taiKhoanBUS =
            new TaiKhoanBUS();

        public FLogin()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Form
            this.Text = "Đăng nhập hệ thống";
            this.ClientSize = new Size(620, 430);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Font = new Font("Segoe UI", 10);

            // Tiêu đề
            lblTieuDe = new Label
            {
                Text = "ĐĂNG NHẬP",
                Font = new Font("Segoe UI", 25, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(0, 45)
            };
            this.Controls.Add(lblTieuDe);

            // Khu nhập liệu
            TableLayoutPanel pnlInput = new TableLayoutPanel
            {
                Location = new Point(85, 140),
                Size = new Size(450, 150),
                ColumnCount = 2,
                RowCount = 3
            };

            pnlInput.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 145));

            pnlInput.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));

            pnlInput.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 55));

            pnlInput.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 55));

            pnlInput.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 40));

            this.Controls.Add(pnlInput);

            // Tên đăng nhập
            Label lblTenDangNhap = new Label
            {
                Text = "Tên đăng nhập:",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Font = new Font("Segoe UI", 10)
            };

            txtTenDangNhap = new TextBox
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5, 10, 5, 10),
                Font = new Font("Segoe UI", 11)
            };

            pnlInput.Controls.Add(lblTenDangNhap, 0, 0);
            pnlInput.Controls.Add(txtTenDangNhap, 1, 0);

            // Mật khẩu
            Label lblMatKhau = new Label
            {
                Text = "Mật khẩu:",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Font = new Font("Segoe UI", 10)
            };

            txtMatKhau = new TextBox
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5, 10, 5, 10),
                Font = new Font("Segoe UI", 11),
                UseSystemPasswordChar = true
            };

            pnlInput.Controls.Add(lblMatKhau, 0, 1);
            pnlInput.Controls.Add(txtMatKhau, 1, 1);

            // Hiện mật khẩu
            chkHienMatKhau = new CheckBox
            {
                Text = "Hiện mật khẩu",
                AutoSize = true,
                Anchor = AnchorStyles.Left
            };

            pnlInput.Controls.Add(chkHienMatKhau, 1, 2);

            // Khu button
            FlowLayoutPanel pnlButton = new FlowLayoutPanel
            {
                Size = new Size(300, 60),
                Location = new Point(160, 315),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0)
            };
            this.Controls.Add(pnlButton);

            btnDangNhap = new Button
            {
                Text = "Đăng nhập",
                Size = new Size(130, 44),
                Margin = new Padding(0, 0, 20, 0),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(45, 95, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnDangNhap.FlatAppearance.BorderSize = 0;

            btnThoat = new Button
            {
                Text = "Thoát",
                Size = new Size(130, 44),
                Margin = new Padding(0),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(50, 50, 50),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnThoat.FlatAppearance.BorderColor = Color.LightGray;

            pnlButton.Controls.Add(btnDangNhap);
            pnlButton.Controls.Add(btnThoat);

            // Event
            btnDangNhap.Click += btnDangNhap_Click;
            btnThoat.Click += btnThoat_Click;
            chkHienMatKhau.CheckedChanged +=
                chkHienMatKhau_CheckedChanged;

            this.AcceptButton = btnDangNhap;
            this.CancelButton = btnThoat;
            this.Shown += FLogin_Shown;
        }

        private void FLogin_Shown(
            object? sender,
            EventArgs e)
        {
            CanGiuaTieuDe();
            txtTenDangNhap.Focus();
        }

        // Đăng nhập
        private void DangNhap()
        {
            string tenDangNhap =
                txtTenDangNhap.Text.Trim();

            string matKhau =
                txtMatKhau.Text;

            if (string.IsNullOrWhiteSpace(tenDangNhap))
            {
                MessageBox.Show(
                    "Vui lòng nhập tên đăng nhập!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtTenDangNhap.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(matKhau))
            {
                MessageBox.Show(
                    "Vui lòng nhập mật khẩu!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtMatKhau.Focus();
                return;
            }

            try
            {
                TaiKhoanDTO? taiKhoan =
                    taiKhoanBUS.DangNhap(
                        tenDangNhap,
                        matKhau);

                if (taiKhoan == null)
                {
                    MessageBox.Show(
                        "Tên đăng nhập hoặc mật khẩu không đúng,\n" +
                        "hoặc tài khoản đã bị khóa!",
                        "Đăng nhập thất bại",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    txtMatKhau.Clear();
                    txtMatKhau.Focus();
                    return;
                }

                Session.TenDangNhap =
                    taiKhoan.TenDangNhap;

                Session.VaiTro =
                    taiKhoan.VaiTro;

                using (FMain form = new FMain())
                {
                    this.Hide();
                    form.ShowDialog();
                    this.Show();
                }

                XoaDangNhap();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể đăng nhập!\n" +
                    ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnDangNhap_Click(
            object? sender,
            EventArgs e)
        {
            DangNhap();
        }

        private void chkHienMatKhau_CheckedChanged(
            object? sender,
            EventArgs e)
        {
            txtMatKhau.UseSystemPasswordChar =
                !chkHienMatKhau.Checked;
        }

        private void btnThoat_Click(
            object? sender,
            EventArgs e)
        {
            if (MessageBox.Show(
                    "Bạn có muốn thoát chương trình?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question)
                == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void XoaDangNhap()
        {
            txtTenDangNhap.Clear();
            txtMatKhau.Clear();

            chkHienMatKhau.Checked = false;

            txtTenDangNhap.Focus();
        }

        private void CanGiuaTieuDe()
        {
            lblTieuDe.Left =
                (this.ClientSize.Width -
                 lblTieuDe.Width) / 2;
        }
    }
}
