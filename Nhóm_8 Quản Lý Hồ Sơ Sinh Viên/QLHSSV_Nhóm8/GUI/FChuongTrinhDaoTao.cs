using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QLHSSV_Nhóm8.BUS;
using QLHSSV_Nhóm8.DTO;

namespace QLHSSV_Nhóm8.GUI
{
    public class FChuongTrinhDaoTao : Form
    {
        private Label lblTieuDe;

        private TextBox txtMaCTDT;
        private TextBox txtTenCTDT;
        private TextBox txtTimKiem;

        private ComboBox cboTrinhDo;
        private ComboBox cboMaNganh;

        private NumericUpDown numThoiGian;
        private NumericUpDown numTinChi;
        private NumericUpDown numNamBatDau;
        private NumericUpDown numNamKetThuc;

        private Button btnThem;
        private Button btnSua;
        private Button btnXoa;
        private Button btnLamMoi;
        private Button btnTimKiem;

        private DataGridView dgvCTDT;

        private readonly ChuongTrinhDaoTaoBUS ctdtBUS =
            new ChuongTrinhDaoTaoBUS();

        private readonly NganhBUS nganhBUS =
            new NganhBUS();

        public FChuongTrinhDaoTao()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Form
            this.Text = "Quản lý chương trình đào tạo";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.White;

            // Tiêu đề
            lblTieuDe = new Label
            {
                Text = "QUẢN LÝ CHƯƠNG TRÌNH ĐÀO TẠO",
                Font = new Font("Arial", 20, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(400, 20)
            };
            this.Controls.Add(lblTieuDe);

            // Cột trái
            TaoLabel("Mã CTĐT:", 50, 90);
            TaoLabel("Tên CTĐT:", 50, 140);
            TaoLabel("Trình độ:", 50, 190);
            TaoLabel("Thời gian đào tạo:", 50, 240);

            txtMaCTDT = TaoTextBox(190, 88, 300);
            txtTenCTDT = TaoTextBox(190, 138, 300);

            cboTrinhDo = new ComboBox
            {
                Location = new Point(190, 188),
                Size = new Size(300, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboTrinhDo.Items.AddRange(
                new object[] { "Đại học", "Cao đẳng" });
            this.Controls.Add(cboTrinhDo);

            numThoiGian =
                TaoNumeric(190, 238, 1, 10);

            // Cột phải
            TaoLabel("Tổng tín chỉ:", 570, 90);
            TaoLabel("Năm bắt đầu:", 570, 140);
            TaoLabel("Năm kết thúc:", 570, 190);
            TaoLabel("Ngành:", 570, 240);

            numTinChi =
                TaoNumeric(700, 88, 1, 300);

            numNamBatDau =
                TaoNumeric(700, 138, 2000, 2100);

            numNamKetThuc =
                TaoNumeric(700, 188, 2000, 2100);

            cboMaNganh = new ComboBox
            {
                Location = new Point(700, 238),
                Size = new Size(300, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.Controls.Add(cboMaNganh);

            // Button
            btnThem = TaoButton("Thêm", 280, 310);
            btnSua = TaoButton("Sửa", 400, 310);
            btnXoa = TaoButton("Xóa", 520, 310);
            btnLamMoi = TaoButton("Làm mới", 640, 310);

            // Tìm kiếm
            TaoLabel("Tìm kiếm:", 50, 375);
            txtTimKiem = TaoTextBox(130, 373, 350);
            btnTimKiem = TaoButton("Tìm", 500, 365);

            // Bảng dữ liệu
            dgvCTDT = new DataGridView
            {
                Location = new Point(30, 425),
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

            dgvCTDT.Columns.Add("MaCTDT", "Mã CTĐT");
            dgvCTDT.Columns.Add("TenCTDT", "Tên CTĐT");
            dgvCTDT.Columns.Add("TrinhDo", "Trình độ");
            dgvCTDT.Columns.Add(
                "ThoiGianDaoTao", "Thời gian");
            dgvCTDT.Columns.Add(
                "TongTinChi", "Tổng tín chỉ");
            dgvCTDT.Columns.Add(
                "NamBatDau", "Năm bắt đầu");
            dgvCTDT.Columns.Add(
                "NamKetThuc", "Năm kết thúc");
            dgvCTDT.Columns.Add(
                "MaNganh", "Mã ngành");

            this.Controls.Add(dgvCTDT);

            // Event
            this.Load += FChuongTrinhDaoTao_Load;
            this.Resize += FChuongTrinhDaoTao_Resize;

            btnThem.Click += btnThem_Click;
            btnSua.Click += btnSua_Click;
            btnXoa.Click += btnXoa_Click;
            btnLamMoi.Click += btnLamMoi_Click;
            btnTimKiem.Click += btnTimKiem_Click;

            dgvCTDT.CellClick += dgvCTDT_CellClick;
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

        private NumericUpDown TaoNumeric(
            int x,
            int y,
            int min,
            int max)
        {
            NumericUpDown num =
                new NumericUpDown
                {
                    Location = new Point(x, y),
                    Size = new Size(300, 25),
                    Minimum = min,
                    Maximum = max
                };

            this.Controls.Add(num);
            return num;
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

        private void FChuongTrinhDaoTao_Load(
            object sender,
            EventArgs e)
        {
            LoadNganh();
            LoadData();
            XoaTrang();
            CanGiuaTieuDe();
        }

        // Load ngành vào ComboBox
        private void LoadNganh()
        {
            DataTable dt = nganhBUS.GetAll();

            cboMaNganh.DataSource = dt;
            cboMaNganh.DisplayMember = "TenNganh";
            cboMaNganh.ValueMember = "MaNganh";
            cboMaNganh.SelectedIndex = -1;
        }

        private void LoadData()
        {
            HienThiDuLieu(ctdtBUS.GetAll());
        }

        // Hiển thị dữ liệu
        private void HienThiDuLieu(DataTable dt)
        {
            dgvCTDT.Rows.Clear();

            foreach (DataRow row in dt.Rows)
            {
                dgvCTDT.Rows.Add(
                    row["MaCTDT"],
                    row["TenCTDT"],
                    row["TrinhDo"],
                    row["ThoiGianDaoTao"],
                    row["TongTinChi"],
                    row["NamBatDau"],
                    row["NamKetThuc"],
                    row["MaNganh"]
                );
            }

            dgvCTDT.ClearSelection();
        }

        // Lấy dữ liệu từ Form
        private ChuongTrinhDaoTaoDTO LayDuLieu()
        {
            return new ChuongTrinhDaoTaoDTO
            {
                MaCTDT = txtMaCTDT.Text.Trim(),
                TenCTDT = txtTenCTDT.Text.Trim(),
                TrinhDo = cboTrinhDo.Text,
                ThoiGianDaoTao =
                    (int)numThoiGian.Value,
                TongTinChi =
                    (int)numTinChi.Value,
                NamBatDau =
                    (int)numNamBatDau.Value,
                NamKetThuc =
                    (int)numNamKetThuc.Value,
                MaNganh =
                    cboMaNganh.SelectedValue?
                    .ToString() ?? ""
            };
        }

        private void btnThem_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                ChuongTrinhDaoTaoDTO ct =
                    LayDuLieu();

                if (!ctdtBUS.Them(ct))
                {
                    MessageBox.Show(
                        "Thêm thất bại! Kiểm tra dữ liệu hoặc mã đã tồn tại.");
                    return;
                }

                MessageBox.Show(
                    "Thêm chương trình đào tạo thành công!");

                LoadData();
                XoaTrang();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể thêm chương trình đào tạo!\n" +
                    ex.Message);
            }
        }

        private void btnSua_Click(
            object sender,
            EventArgs e)
        {
            if (dgvCTDT.CurrentRow == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn chương trình cần sửa!");
                return;
            }

            try
            {
                ChuongTrinhDaoTaoDTO ct =
                    LayDuLieu();

                if (!ctdtBUS.Sua(ct))
                {
                    MessageBox.Show(
                        "Cập nhật thất bại! Kiểm tra dữ liệu.");
                    return;
                }

                MessageBox.Show(
                    "Cập nhật chương trình đào tạo thành công!");

                LoadData();
                XoaTrang();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể cập nhật chương trình đào tạo!\n" +
                    ex.Message);
            }
        }

        private void btnXoa_Click(
            object sender,
            EventArgs e)
        {
            if (dgvCTDT.CurrentRow == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn chương trình cần xóa!");
                return;
            }

            string ma =
                dgvCTDT.CurrentRow
                .Cells["MaCTDT"]
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
                if (!ctdtBUS.Xoa(ma))
                {
                    MessageBox.Show(
                        "Xóa chương trình đào tạo thất bại!");
                    return;
                }

                MessageBox.Show(
                    "Xóa chương trình đào tạo thành công!");

                LoadData();
                XoaTrang();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể xóa chương trình đào tạo!\n" +
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
                    ctdtBUS.TimKiem(
                        txtTimKiem.Text.Trim()));
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tìm kiếm!\n" +
                    ex.Message);
            }
        }

        private void dgvCTDT_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row =
                dgvCTDT.Rows[e.RowIndex];

            txtMaCTDT.Text =
                row.Cells["MaCTDT"]
                .Value?.ToString() ?? "";

            txtTenCTDT.Text =
                row.Cells["TenCTDT"]
                .Value?.ToString() ?? "";

            cboTrinhDo.Text =
                row.Cells["TrinhDo"]
                .Value?.ToString() ?? "";

            numThoiGian.Value =
                Convert.ToDecimal(
                    row.Cells["ThoiGianDaoTao"].Value);

            numTinChi.Value =
                Convert.ToDecimal(
                    row.Cells["TongTinChi"].Value);

            numNamBatDau.Value =
                Convert.ToDecimal(
                    row.Cells["NamBatDau"].Value);

            numNamKetThuc.Value =
                Convert.ToDecimal(
                    row.Cells["NamKetThuc"].Value);

            cboMaNganh.SelectedValue =
                row.Cells["MaNganh"]
                .Value?.ToString() ?? "";
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
            txtMaCTDT.Clear();
            txtTenCTDT.Clear();
            txtTimKiem.Clear();

            cboTrinhDo.SelectedIndex = -1;
            cboMaNganh.SelectedIndex = -1;

            numThoiGian.Value = 1;
            numTinChi.Value = 1;
            numNamBatDau.Value = 2024;
            numNamKetThuc.Value = 2028;

            dgvCTDT.ClearSelection();
        }

        private void FChuongTrinhDaoTao_Resize(
            object sender,
            EventArgs e)
        {
            CanGiuaTieuDe();

            dgvCTDT.Left = 30;
            dgvCTDT.Width =
                this.ClientSize.Width - 60;

            dgvCTDT.Height =
                this.ClientSize.Height -
                dgvCTDT.Top - 30;
        }

        private void CanGiuaTieuDe()
        {
            lblTieuDe.Left =
                (this.ClientSize.Width -
                 lblTieuDe.Width) / 2;
        }
    }
}
