using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QLHSSV_Nhóm8.BUS;
using QLHSSV_Nhóm8.DTO;

namespace QLHSSV_Nhóm8.GUI
{
    public class FNganh : Form
    {
        private Label lblTieuDe;

        private TextBox txtMaNganh;
        private TextBox txtTenNganh;
        private TextBox txtTimKiem;

        private ComboBox cboMaKhoa;

        private Button btnThem;
        private Button btnSua;
        private Button btnXoa;
        private Button btnLamMoi;
        private Button btnTimKiem;

        private DataGridView dgvNganh;

        private readonly NganhBUS nganhBUS = new NganhBUS();
        private readonly KhoaBUS khoaBUS = new KhoaBUS();

        public FNganh()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Form
            this.Text = "Quản lý ngành";
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // Tiêu đề
            lblTieuDe = new Label
            {
                Text = "QUẢN LÝ NGÀNH",
                Font = new Font("Arial", 20, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(500, 20)
            };
            this.Controls.Add(lblTieuDe);

            // Thông tin ngành
            TaoLabel("Mã ngành:", 50, 90);
            TaoLabel("Tên ngành:", 50, 140);
            TaoLabel("Khoa:", 550, 90);

            txtMaNganh = TaoTextBox(150, 88, 300);
            txtTenNganh = TaoTextBox(150, 138, 300);

            cboMaKhoa = new ComboBox
            {
                Location = new Point(650, 88),
                Size = new Size(300, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.Controls.Add(cboMaKhoa);

            // Button
            btnThem = TaoButton("Thêm", 250, 220);
            btnSua = TaoButton("Sửa", 370, 220);
            btnXoa = TaoButton("Xóa", 490, 220);
            btnLamMoi = TaoButton("Làm mới", 610, 220);

            // Tìm kiếm
            TaoLabel("Tìm kiếm:", 50, 290);
            txtTimKiem = TaoTextBox(130, 288, 350);
            btnTimKiem = TaoButton("Tìm", 500, 280);

            // Bảng dữ liệu
            dgvNganh = new DataGridView
            {
                Location = new Point(30, 340),
                Size = new Size(1000, 350),
                Anchor = AnchorStyles.Top |
                         AnchorStyles.Bottom |
                         AnchorStyles.Left |
                         AnchorStyles.Right,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            dgvNganh.Columns.Add("MaNganh", "Mã ngành");
            dgvNganh.Columns.Add("TenNganh", "Tên ngành");
            dgvNganh.Columns.Add("MaKhoa", "Mã khoa");

            this.Controls.Add(dgvNganh);

            // Event
            this.Load += FNganh_Load;
            this.Resize += FNganh_Resize;

            btnThem.Click += btnThem_Click;
            btnSua.Click += btnSua_Click;
            btnXoa.Click += btnXoa_Click;
            btnLamMoi.Click += btnLamMoi_Click;
            btnTimKiem.Click += btnTimKiem_Click;

            dgvNganh.CellClick += dgvNganh_CellClick;
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
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            this.Controls.Add(button);
            return button;
        }

        private void FNganh_Load(object sender, EventArgs e)
        {
            LoadKhoa();
            LoadData();
            CanGiuaTieuDe();
        }

        // Load khoa vào ComboBox
        private void LoadKhoa()
        {
            DataTable dt = khoaBUS.GetAll();

            cboMaKhoa.DataSource = dt;
            cboMaKhoa.DisplayMember = "TenKhoa";
            cboMaKhoa.ValueMember = "MaKhoa";
            cboMaKhoa.SelectedIndex = -1;
        }

        private void LoadData()
        {
            HienThiDuLieu(nganhBUS.GetAll());
        }

        // Hiển thị dữ liệu lên bảng
        private void HienThiDuLieu(DataTable dt)
        {
            dgvNganh.Rows.Clear();

            foreach (DataRow row in dt.Rows)
            {
                dgvNganh.Rows.Add(
                    row["MaNganh"],
                    row["TenNganh"],
                    row["MaKhoa"]
                );
            }

            dgvNganh.ClearSelection();
        }

        // Lấy dữ liệu từ Form
        private NganhDTO LayDuLieu()
        {
            return new NganhDTO
            {
                MaNganh = txtMaNganh.Text.Trim(),
                TenNganh = txtTenNganh.Text.Trim(),
                MaKhoa = cboMaKhoa.SelectedValue?.ToString() ?? ""
            };
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                NganhDTO nganh = LayDuLieu();

                if (!nganhBUS.Them(nganh))
                {
                    MessageBox.Show(
                        "Thêm ngành thất bại! Kiểm tra dữ liệu hoặc mã ngành bị trùng.");
                    return;
                }

                MessageBox.Show("Thêm ngành thành công!");
                LoadData();
                XoaTrang();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể thêm ngành!\n" + ex.Message);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvNganh.CurrentRow == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn ngành cần sửa!");
                return;
            }

            try
            {
                NganhDTO nganh = LayDuLieu();

                if (!nganhBUS.Sua(nganh))
                {
                    MessageBox.Show(
                        "Cập nhật ngành thất bại!");
                    return;
                }

                MessageBox.Show("Cập nhật ngành thành công!");
                LoadData();
                XoaTrang();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể cập nhật ngành!\n" + ex.Message);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvNganh.CurrentRow == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn ngành cần xóa!");
                return;
            }

            string maNganh = dgvNganh.CurrentRow
                .Cells["MaNganh"]
                .Value?.ToString() ?? "";

            if (MessageBox.Show(
                    "Bạn có chắc muốn xóa ngành " + maNganh + "?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question)
                != DialogResult.Yes)
            {
                return;
            }

            try
            {
                if (!nganhBUS.Xoa(maNganh))
                {
                    MessageBox.Show("Xóa ngành thất bại!");
                    return;
                }

                MessageBox.Show("Xóa ngành thành công!");
                LoadData();
                XoaTrang();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể xóa ngành!\n" + ex.Message);
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            try
            {
                string keyword = txtTimKiem.Text.Trim();

                if (string.IsNullOrWhiteSpace(keyword))
                {
                    LoadData();
                    return;
                }

                HienThiDuLieu(
                    nganhBUS.TimKiem(keyword));
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tìm kiếm!\n" + ex.Message);
            }
        }

        private void dgvNganh_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row =
                dgvNganh.Rows[e.RowIndex];

            txtMaNganh.Text =
                row.Cells["MaNganh"].Value?.ToString() ?? "";

            txtTenNganh.Text =
                row.Cells["TenNganh"].Value?.ToString() ?? "";

            cboMaKhoa.SelectedValue =
                row.Cells["MaKhoa"].Value?.ToString() ?? "";
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            XoaTrang();
            LoadData();
        }

        private void XoaTrang()
        {
            txtMaNganh.Clear();
            txtTenNganh.Clear();
            txtTimKiem.Clear();

            cboMaKhoa.SelectedIndex = -1;
            dgvNganh.ClearSelection();
        }

        private void FNganh_Resize(object sender, EventArgs e)
        {
            CanGiuaTieuDe();

            dgvNganh.Left = 30;
            dgvNganh.Width = this.ClientSize.Width - 60;
            dgvNganh.Height =
                this.ClientSize.Height -
                dgvNganh.Top - 30;
        }

        private void CanGiuaTieuDe()
        {
            lblTieuDe.Left =
                (this.ClientSize.Width -
                 lblTieuDe.Width) / 2;
        }
    }
}
