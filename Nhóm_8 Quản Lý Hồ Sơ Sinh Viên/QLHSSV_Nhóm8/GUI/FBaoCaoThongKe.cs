using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QLHSSV_Nhóm8.BUS;

namespace QLHSSV_Nhóm8.GUI
{
    public class FBaoCaoThongKe : Form
    {
        private Label lblTieuDe;

        private Label lblTongSinhVien;
        private Label lblTongLop;
        private Label lblTongNganh;
        private Label lblTongKhoa;

        private ComboBox cboKhoa;
        private ComboBox cboCTDT;

        private Button btnThongKeKhoa;
        private Button btnThongKeCTDT;
        private Button btnTheoLop;
        private Button btnTheoGioiTinh;
        private Button btnLamMoi;

        private DataGridView dgvThongKe;
        private TableLayoutPanel pnlTongQuan;

        private readonly SinhVienBUS sinhVienBUS =
            new SinhVienBUS();

        private readonly LopBUS lopBUS =
            new LopBUS();

        private readonly NganhBUS nganhBUS =
            new NganhBUS();

        private readonly KhoaBUS khoaBUS =
            new KhoaBUS();

        private readonly ChuongTrinhDaoTaoBUS ctdtBUS =
            new ChuongTrinhDaoTaoBUS();

        private readonly BaoCaoThongKeBUS baoCaoBUS =
            new BaoCaoThongKeBUS();

        public FBaoCaoThongKe()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Form
            Text = "Báo cáo - Thống kê";
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 247, 250);

            // Header
            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 110,
                BackColor = Color.White
            };
            Controls.Add(pnlHeader);

            lblTieuDe = new Label
            {
                Text = "BÁO CÁO - THỐNG KÊ",
                Font = new Font(
                    "Segoe UI",
                    24,
                    FontStyle.Bold),
                AutoSize = true,
                Location = new Point(0, 35)
            };
            pnlHeader.Controls.Add(lblTieuDe);

            // Tổng quan
            pnlTongQuan =
                new TableLayoutPanel
                {
                    Location = new Point(60, 140),
                    Size = new Size(1000, 120),
                    ColumnCount = 4,
                    RowCount = 1,
                    GrowStyle = TableLayoutPanelGrowStyle.FixedSize
                };

            pnlTongQuan.ColumnStyles.Clear();
            pnlTongQuan.RowStyles.Clear();

            for (int i = 0; i < 4; i++)
            {
                pnlTongQuan.ColumnStyles.Add(
                    new ColumnStyle(
                        SizeType.Percent,
                        25F));
            }

            pnlTongQuan.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent,
                    100F));

            Controls.Add(pnlTongQuan);

            lblTongSinhVien =
                TaoTheThongKe(
                    "TỔNG SINH VIÊN",
                    "0");

            lblTongLop =
                TaoTheThongKe(
                    "TỔNG LỚP",
                    "0");

            lblTongNganh =
                TaoTheThongKe(
                    "TỔNG NGÀNH",
                    "0");

            lblTongKhoa =
                TaoTheThongKe(
                    "TỔNG KHOA",
                    "0");

            pnlTongQuan.Controls.Add(
                lblTongSinhVien,
                0,
                0);

            pnlTongQuan.Controls.Add(
                lblTongLop,
                1,
                0);

            pnlTongQuan.Controls.Add(
                lblTongNganh,
                2,
                0);

            pnlTongQuan.Controls.Add(
                lblTongKhoa,
                3,
                0);

            // Khoa
            Label lblKhoa = new Label
            {
                Text = "Khoa:",
                Location = new Point(60, 295),
                AutoSize = true,
                Font = new Font(
                    "Segoe UI",
                    10,
                    FontStyle.Bold)
            };
            Controls.Add(lblKhoa);

            cboKhoa = new ComboBox
            {
                Location = new Point(115, 290),
                Size = new Size(250, 30),
                DropDownStyle =
                    ComboBoxStyle.DropDownList
            };
            Controls.Add(cboKhoa);

            btnThongKeKhoa =
                TaoButton("Thống kê khoa");

            btnThongKeKhoa.Location =
                new Point(385, 282);

            Controls.Add(btnThongKeKhoa);

            // CTĐT
            Label lblCTDT = new Label
            {
                Text = "CTĐT:",
                Location = new Point(650, 295),
                AutoSize = true,
                Font = new Font(
                    "Segoe UI",
                    10,
                    FontStyle.Bold)
            };
            Controls.Add(lblCTDT);

            cboCTDT = new ComboBox
            {
                Location = new Point(710, 290),
                Size = new Size(300, 30),
                DropDownStyle =
                    ComboBoxStyle.DropDownList
            };
            Controls.Add(cboCTDT);

            btnThongKeCTDT =
                TaoButton("Thống kê CTĐT");

            btnThongKeCTDT.Location =
                new Point(1030, 282);

            Controls.Add(btnThongKeCTDT);

            // Các nút cũ
            btnTheoLop =
                TaoButton("Sinh viên theo lớp");

            btnTheoLop.Location =
                new Point(250, 345);

            Controls.Add(btnTheoLop);

            btnTheoGioiTinh =
                TaoButton(
                    "Sinh viên theo giới tính");

            btnTheoGioiTinh.Location =
                new Point(530, 345);

            Controls.Add(btnTheoGioiTinh);

            btnLamMoi =
                TaoButton("Làm mới");

            btnLamMoi.Location =
                new Point(810, 345);

            Controls.Add(btnLamMoi);

            // DataGridView
            dgvThongKe =
                new DataGridView
                {
                    Location =
                        new Point(60, 420),

                    Size =
                        new Size(1400, 360),

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

                    BackgroundColor = Color.White,
                    BorderStyle =
                        BorderStyle.FixedSingle
                };

            Controls.Add(dgvThongKe);

            // Event
            Load += FBaoCaoThongKe_Load;
            Resize += FBaoCaoThongKe_Resize;

            btnThongKeKhoa.Click +=
                btnThongKeKhoa_Click;

            btnThongKeCTDT.Click +=
                btnThongKeCTDT_Click;

            btnTheoLop.Click +=
                btnTheoLop_Click;

            btnTheoGioiTinh.Click +=
                btnTheoGioiTinh_Click;

            btnLamMoi.Click +=
                btnLamMoi_Click;
        }

        private Label TaoTheThongKe(
            string tieuDe,
            string giaTri)
        {
            return new Label
            {
                Text =
                    tieuDe +
                    Environment.NewLine +
                    giaTri,

                Dock = DockStyle.Fill,
                Margin = new Padding(12),

                TextAlign =
                    ContentAlignment.MiddleCenter,

                Font = new Font(
                    "Segoe UI",
                    13,
                    FontStyle.Bold),

                BackColor = Color.White,

                ForeColor =
                    Color.FromArgb(
                        45,
                        70,
                        110),

                BorderStyle =
                    BorderStyle.FixedSingle
            };
        }

        private Button TaoButton(
            string text)
        {
            Button btn = new Button
            {
                Text = text,
                Size = new Size(230, 48),

                Font = new Font(
                    "Segoe UI",
                    10,
                    FontStyle.Bold),

                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            btn.FlatAppearance.BorderColor =
                Color.LightGray;

            return btn;
        }

        private void FBaoCaoThongKe_Load(
            object sender,
            EventArgs e)
        {
            LoadDanhMuc();
            LoadTongQuan();
            LoadTheoLop();
            CanGiuaTieuDe();
        }

        private void LoadDanhMuc()
        {
            DataTable dtKhoa =
                khoaBUS.GetAll();

            cboKhoa.DataSource = dtKhoa;
            cboKhoa.DisplayMember = "TenKhoa";
            cboKhoa.ValueMember = "MaKhoa";
            cboKhoa.SelectedIndex = -1;

            DataTable dtCTDT =
                ctdtBUS.GetAll();

            cboCTDT.DataSource = dtCTDT;
            cboCTDT.DisplayMember = "TenCTDT";
            cboCTDT.ValueMember = "MaCTDT";
            cboCTDT.SelectedIndex = -1;
        }

        private void LoadTongQuan()
        {
            int tongSinhVien =
                sinhVienBUS
                    .GetAll()
                    .Rows
                    .Count;

            int tongLop =
                lopBUS
                    .GetAll()
                    .Rows
                    .Count;

            int tongNganh =
                nganhBUS
                    .GetAll()
                    .Rows
                    .Count;

            int tongKhoa =
                khoaBUS
                    .GetAll()
                    .Rows
                    .Count;

            lblTongSinhVien.Text =
                "TỔNG SINH VIÊN" +
                Environment.NewLine +
                tongSinhVien;

            lblTongLop.Text =
                "TỔNG LỚP" +
                Environment.NewLine +
                tongLop;

            lblTongNganh.Text =
                "TỔNG NGÀNH" +
                Environment.NewLine +
                tongNganh;

            lblTongKhoa.Text =
                "TỔNG KHOA" +
                Environment.NewLine +
                tongKhoa;
        }

        // Gọi fn_DemSSVKhoa + fn_DemSoNganhKhoa
        private void btnThongKeKhoa_Click(
            object sender,
            EventArgs e)
        {
            if (cboKhoa.SelectedValue == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn khoa!");
                return;
            }

            try
            {
                string maKhoa =
                    cboKhoa.SelectedValue
                    .ToString() ?? "";

                int soSinhVien =
                    baoCaoBUS
                    .DemSinhVienKhoa(maKhoa);

                int soNganh =
                    baoCaoBUS
                    .DemSoNganhKhoa(maKhoa);

                DataTable dt =
                    new DataTable();

                dt.Columns.Add(
                    "MaKhoa",
                    typeof(string));

                dt.Columns.Add(
                    "TenKhoa",
                    typeof(string));

                dt.Columns.Add(
                    "SoSinhVien",
                    typeof(int));

                dt.Columns.Add(
                    "SoNganh",
                    typeof(int));

                dt.Rows.Add(
                    maKhoa,
                    cboKhoa.Text,
                    soSinhVien,
                    soNganh);

                dgvThongKe.DataSource = dt;

                dgvThongKe.Columns[
                    "MaKhoa"
                ].HeaderText =
                    "Mã khoa";

                dgvThongKe.Columns[
                    "TenKhoa"
                ].HeaderText =
                    "Tên khoa";

                dgvThongKe.Columns[
                    "SoSinhVien"
                ].HeaderText =
                    "Số sinh viên";

                dgvThongKe.Columns[
                    "SoNganh"
                ].HeaderText =
                    "Số ngành";

                dgvThongKe.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể thống kê khoa!\n" +
                    ex.Message);
            }
        }

        // Gọi fn_DemSVCTDT
        private void btnThongKeCTDT_Click(
            object sender,
            EventArgs e)
        {
            if (cboCTDT.SelectedValue == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn chương trình đào tạo!");
                return;
            }

            try
            {
                string maCTDT =
                    cboCTDT.SelectedValue
                    .ToString() ?? "";

                int soSinhVien =
                    baoCaoBUS
                    .DemSinhVienCTDT(maCTDT);

                DataTable dt =
                    new DataTable();

                dt.Columns.Add(
                    "MaCTDT",
                    typeof(string));

                dt.Columns.Add(
                    "TenCTDT",
                    typeof(string));

                dt.Columns.Add(
                    "SoSinhVien",
                    typeof(int));

                dt.Rows.Add(
                    maCTDT,
                    cboCTDT.Text,
                    soSinhVien);

                dgvThongKe.DataSource = dt;

                dgvThongKe.Columns[
                    "MaCTDT"
                ].HeaderText =
                    "Mã CTĐT";

                dgvThongKe.Columns[
                    "TenCTDT"
                ].HeaderText =
                    "Tên CTĐT";

                dgvThongKe.Columns[
                    "SoSinhVien"
                ].HeaderText =
                    "Số sinh viên";

                dgvThongKe.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể thống kê CTĐT!\n" +
                    ex.Message);
            }
        }

        private void LoadTheoLop()
        {
            DataTable dt =
                sinhVienBUS
                    .ThongKeTheoLop();

            dgvThongKe.DataSource = dt;

            if (dgvThongKe.Columns[
                "MaLop"] != null)
            {
                dgvThongKe.Columns[
                    "MaLop"
                ].HeaderText =
                    "Mã lớp";
            }

            if (dgvThongKe.Columns[
                "TenLop"] != null)
            {
                dgvThongKe.Columns[
                    "TenLop"
                ].HeaderText =
                    "Tên lớp";
            }

            if (dgvThongKe.Columns[
                "SoLuongSinhVien"] != null)
            {
                dgvThongKe.Columns[
                    "SoLuongSinhVien"
                ].HeaderText =
                    "Số lượng sinh viên";
            }

            dgvThongKe.ClearSelection();
        }

        private void btnTheoLop_Click(
            object sender,
            EventArgs e)
        {
            LoadTheoLop();
        }

        private void btnTheoGioiTinh_Click(
            object sender,
            EventArgs e)
        {
            DataTable dt =
                sinhVienBUS
                    .ThongKeTheoGioiTinh();

            dgvThongKe.DataSource = dt;

            if (dgvThongKe.Columns[
                "GioiTinh"] != null)
            {
                dgvThongKe.Columns[
                    "GioiTinh"
                ].HeaderText =
                    "Giới tính";
            }

            if (dgvThongKe.Columns[
                "SoLuongSinhVien"] != null)
            {
                dgvThongKe.Columns[
                    "SoLuongSinhVien"
                ].HeaderText =
                    "Số lượng sinh viên";
            }

            dgvThongKe.ClearSelection();
        }

        private void btnLamMoi_Click(
            object sender,
            EventArgs e)
        {
            LoadDanhMuc();
            LoadTongQuan();
            LoadTheoLop();
        }

        private void FBaoCaoThongKe_Resize(
            object sender,
            EventArgs e)
        {
            CanGiuaTieuDe();

            pnlTongQuan.Left = 60;
            pnlTongQuan.Width =
                Math.Max(
                    600,
                    ClientSize.Width - 120);

            dgvThongKe.Left = 60;

            dgvThongKe.Width =
                Math.Max(
                    600,
                    ClientSize.Width - 120);

            dgvThongKe.Height =
                Math.Max(
                    200,
                    ClientSize.Height -
                    dgvThongKe.Top - 40);
        }

        private void CanGiuaTieuDe()
        {
            lblTieuDe.Left =
                (ClientSize.Width -
                 lblTieuDe.Width) / 2;
        }
    }
}
