using System;
using System.Drawing;
using System.Windows.Forms;
using QLHSSV_Nhóm8.Utils;

namespace QLHSSV_Nhóm8.GUI
{
    public class FMain : Form
    {
        private Label lblTieuDe;
        private Label lblTaiKhoan;

        private Button btnSinhVien;
        private Button btnLop;
        private Button btnNganh;
        private Button btnKhoa;
        private Button btnCTDT;
        private Button btnNguoiThan;
        private Button btnTaiKhoan;
        private Button btnDemoGiaoTac;
        private Button btnDemoDeadLock;
        private Button btnDangXuat;
        private Button btnBaoCao;

        private TableLayoutPanel pnlMenu;

        public FMain()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // =========================
            // FORM
            // =========================
            Text = "Hệ thống quản lý hồ sơ sinh viên";
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 247, 250);
            // =========================
            // CONTAINER GIỮA
            // =========================
            Panel pnlContent = new Panel();
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.Padding = new Padding(100, 40, 100, 40);
            pnlContent.BackColor = Color.FromArgb(245, 247, 250);
            Controls.Add(pnlContent);
            // =========================
            // HEADER
            // =========================
            Panel pnlHeader = new Panel();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 140;
            pnlHeader.BackColor = Color.White;
            Controls.Add(pnlHeader);
            lblTieuDe = new Label();
            lblTieuDe.Text = "HỆ THỐNG QUẢN LÝ HỒ SƠ SINH VIÊN";
            lblTieuDe.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lblTieuDe.AutoSize = true;
            lblTieuDe.Location = new Point(40, 35);
            pnlHeader.Controls.Add(lblTieuDe);
            lblTaiKhoan = new Label();
            lblTaiKhoan.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            lblTaiKhoan.ForeColor = Color.DimGray;
            lblTaiKhoan.AutoSize = true;
            lblTaiKhoan.Location = new Point(45, 95);
            pnlHeader.Controls.Add(lblTaiKhoan);
            // =========================
            // TABLE MENU
            // =========================
            pnlMenu = new TableLayoutPanel();
            pnlMenu.Dock = DockStyle.Top;
            pnlMenu.Height = 500;
            pnlMenu.ColumnCount = 3;
            pnlMenu.RowCount = 4;
            pnlMenu.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            pnlMenu.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            pnlMenu.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            pnlMenu.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            pnlMenu.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            pnlMenu.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            pnlMenu.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            pnlContent.Controls.Add(pnlMenu);
            // =========================
            // BUTTON
            // =========================
            btnSinhVien = TaoButton("Quản lý sinh viên");
            btnLop = TaoButton("Quản lý lớp");
            btnNganh = TaoButton("Quản lý ngành");
            btnKhoa = TaoButton("Quản lý khoa");
            btnCTDT = TaoButton("Chương trình đào tạo");
            btnNguoiThan = TaoButton("Quản lý người thân");
            btnTaiKhoan = TaoButton("Quản lý tài khoản");
            btnDangXuat = TaoButton("Đăng xuất");
            btnDangXuat.ForeColor = Color.Firebrick;
            btnBaoCao = TaoButton("Báo cáo - Thống kê");
            btnDemoGiaoTac = TaoButton("Demo Giao Tác");
            btnDemoDeadLock = TaoButton("Demo Deadlock");
            // Dòng 1
            pnlMenu.Controls.Add(btnSinhVien, 0, 0);
            pnlMenu.Controls.Add(btnLop, 1, 0);
            pnlMenu.Controls.Add(btnNganh, 2, 0);
            // Dòng 2
            pnlMenu.Controls.Add(btnKhoa, 0, 1);
            pnlMenu.Controls.Add(btnCTDT, 1, 1);
            pnlMenu.Controls.Add(btnNguoiThan, 2, 1);
            // Dòng 3
            pnlMenu.Controls.Add(btnTaiKhoan, 0, 2);
            pnlMenu.Controls.Add(btnBaoCao, 1, 2);
            pnlMenu.Controls.Add(btnDemoGiaoTac, 2, 2);

            // Dòng 4
            pnlMenu.Controls.Add(btnDemoDeadLock, 0, 3);
            pnlMenu.Controls.Add(btnDangXuat, 1, 3);
            // =========================
            // EVENT
            // =========================
            Load += FMain_Load;
            btnSinhVien.Click += btnSinhVien_Click;
            btnLop.Click += btnLop_Click;
            btnNganh.Click += btnNganh_Click;
            btnKhoa.Click += btnKhoa_Click;
            btnCTDT.Click += btnCTDT_Click;
            btnNguoiThan.Click += btnNguoiThan_Click;
            btnTaiKhoan.Click += btnTaiKhoan_Click;
            btnBaoCao.Click += btnBaoCao_Click;
            btnDemoGiaoTac.Click += btnDemoGiaoTac_Click;
            btnDemoDeadLock.Click += btnDemoDeadLock_Click;
            btnDangXuat.Click += btnDangXuat_Click;


        }

        // =========================
        // STYLE BUTTON
        // =========================
        private Button TaoButton(
            string text)
        {
            Button btn = new Button();

            btn.Text = text;

            btn.Dock =
                DockStyle.Fill;

            btn.Margin =
                new Padding(12);

            btn.Font =
                new Font(
                    "Segoe UI",
                    12,
                    FontStyle.Bold
                );

            btn.BackColor =
                Color.White;

            btn.ForeColor =
                Color.FromArgb(40, 40, 40);

            btn.FlatStyle =
                FlatStyle.Flat;

            btn.FlatAppearance.BorderSize = 1;

            btn.FlatAppearance.BorderColor =
                Color.FromArgb(220, 225, 230);

            btn.Cursor =
                Cursors.Hand;

            // Hover
            btn.MouseEnter += (s, e) =>
            {
                btn.BackColor =
                    Color.FromArgb(
                        235, 242, 255);
            };

            btn.MouseLeave += (s, e) =>
            {
                btn.BackColor =
                    Color.White;
            };

            return btn;
        }

        // =========================
        // LOAD
        // =========================
        private void FMain_Load(
            object sender,
            EventArgs e)
        {
            lblTaiKhoan.Text =
                "Tài khoản: "
                + Session.TenDangNhap
                + "     |     Vai trò: "
                + Session.VaiTro;

            // Chỉ Admin thấy quản lý tài khoản
            bool laAdmin =
                Session.VaiTro.Equals(
                    "Admin",
                    StringComparison.OrdinalIgnoreCase
                );

            btnTaiKhoan.Visible =
                laAdmin;

            // Căn giữa tiêu đề
            lblTieuDe.Left =
                (this.ClientSize.Width -
                 lblTieuDe.Width) / 2;
        }

        // =========================
        // SINH VIÊN
        // =========================
        private void btnSinhVien_Click(
            object sender,
            EventArgs e)
        {
            using (FSinhVien f =
                   new FSinhVien())
            {
                f.ShowDialog();
            }
        }

        // =========================
        // LỚP
        // =========================
        private void btnLop_Click(
            object sender,
            EventArgs e)
        {
            using (FLop f =
                   new FLop())
            {
                f.ShowDialog();
            }
        }

        // =========================
        // NGÀNH
        // =========================
        private void btnNganh_Click(
            object sender,
            EventArgs e)
        {
            using (FNganh f =
                   new FNganh())
            {
                f.ShowDialog();
            }
        }

        // =========================
        // KHOA
        // =========================
        private void btnKhoa_Click(
            object sender,
            EventArgs e)
        {
            using (FKhoa f =
                   new FKhoa())
            {
                f.ShowDialog();
            }
        }

        // =========================
        // CTĐT
        // =========================
        private void btnCTDT_Click(
            object sender,
            EventArgs e)
        {
            using (FChuongTrinhDaoTao f =
                   new FChuongTrinhDaoTao())
            {
                f.ShowDialog();
            }
        }

        // =========================
        // NGƯỜI THÂN
        // =========================
        private void btnNguoiThan_Click(
            object sender,
            EventArgs e)
        {
            using (FNguoiThan f =
                   new FNguoiThan())
            {
                f.ShowDialog();
            }
        }

        // =========================
        // TÀI KHOẢN
        // =========================
        private void btnTaiKhoan_Click(
            object sender,
            EventArgs e)
        {
            if (Session.VaiTro != "Admin")
            {
                MessageBox.Show(
                    "Bạn không có quyền sử dụng chức năng này!"
                );

                return;
            }

            using (FQuanLyTaiKhoan f =
                   new FQuanLyTaiKhoan())
            {
                f.ShowDialog();
            }
        }

        // =========================
        // ĐĂNG XUẤT
        // =========================
        private void btnDangXuat_Click(
            object sender,
            EventArgs e)
        {
            DialogResult result =
                MessageBox.Show(
                    "Bạn có chắc muốn đăng xuất?",
                    "Đăng xuất",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

            if (result != DialogResult.Yes)
                return;

            Session.DangXuat();

            this.Close();
        }
        private void btnBaoCao_Click(
            object sender,
            EventArgs e)
        {
            using (FBaoCaoThongKe form =
                   new FBaoCaoThongKe())
            {
                form.ShowDialog();
            }
        }
        // =========================
        // DEMO GIAO TÁC
        // =========================
        private void btnDemoGiaoTac_Click(
            object sender,
            EventArgs e)
        {
            using (FDemoGiaoTac form =
                   new FDemoGiaoTac())
            {
                form.ShowDialog();
            }
        }

        // =========================
        // DEMO DEADLOCK
        // =========================
        private void btnDemoDeadLock_Click(
            object sender,
            EventArgs e)
        {
            using (FDemoDeadLock form =
                   new FDemoDeadLock())
            {
                form.ShowDialog();
            }
        }
    }
}