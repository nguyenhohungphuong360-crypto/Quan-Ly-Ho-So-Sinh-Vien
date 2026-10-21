using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QLHSSV_Nhóm8.BUS;
using QLHSSV_Nhóm8.DTO;

namespace QLHSSV_Nhóm8.GUI
{
    public class FKhoa : Form
    {
        private Label lblTieuDe;

        private TextBox txtMaKhoa;
        private TextBox txtTenKhoa;
        private TextBox txtSDT;
        private TextBox txtEmail;
        private TextBox txtDiaChi;
        private TextBox txtTimKiem;

        private Button btnThem;
        private Button btnSua;
        private Button btnXoa;
        private Button btnLamMoi;
        private Button btnTimKiem;

        private DataGridView dgvKhoa;

        private readonly KhoaBUS khoaBUS =
            new KhoaBUS();

        public FKhoa()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Form
            this.Text = "Quản lý khoa";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.White;

            // Tiêu đề
            lblTieuDe = new Label
            {
                Text = "QUẢN LÝ KHOA",
                Font = new Font("Arial", 20, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(500, 20)
            };
            this.Controls.Add(lblTieuDe);

            // Thông tin khoa
            TaoLabel("Mã khoa:", 50, 90);
            TaoLabel("Tên khoa:", 50, 140);
            TaoLabel("SĐT:", 50, 190);
            TaoLabel("Email:", 550, 90);
            TaoLabel("Địa chỉ:", 550, 140);

            txtMaKhoa = TaoTextBox(150, 88, 300);
            txtTenKhoa = TaoTextBox(150, 138, 300);
            txtSDT = TaoTextBox(150, 188, 300);
            txtEmail = TaoTextBox(650, 88, 300);
            txtDiaChi = TaoTextBox(650, 138, 300);

            // Button
            btnThem = TaoButton("Thêm", 250, 240);
            btnSua = TaoButton("Sửa", 370, 240);
            btnXoa = TaoButton("Xóa", 490, 240);
            btnLamMoi = TaoButton("Làm mới", 610, 240);

            // Tìm kiếm
            TaoLabel("Tìm kiếm:", 50, 310);
            txtTimKiem = TaoTextBox(130, 308, 350);
            btnTimKiem = TaoButton("Tìm", 500, 300);

            // Bảng dữ liệu
            dgvKhoa = new DataGridView
            {
                Location = new Point(30, 360),
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

            dgvKhoa.Columns.Add("MaKhoa", "Mã khoa");
            dgvKhoa.Columns.Add("TenKhoa", "Tên khoa");
            dgvKhoa.Columns.Add("SDT", "Số điện thoại");
            dgvKhoa.Columns.Add("Email", "Email");
            dgvKhoa.Columns.Add("DiaChi", "Địa chỉ");

            this.Controls.Add(dgvKhoa);

            // Event
            this.Load += FKhoa_Load;
            this.Resize += FKhoa_Resize;

            btnThem.Click += btnThem_Click;
            btnSua.Click += btnSua_Click;
            btnXoa.Click += btnXoa_Click;
            btnLamMoi.Click += btnLamMoi_Click;
            btnTimKiem.Click += btnTimKiem_Click;

            dgvKhoa.CellClick += dgvKhoa_CellClick;
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

        private void FKhoa_Load(object sender, EventArgs e)
        {
            LoadData();
            CanGiuaTieuDe();
        }

        private void LoadData()
        {
            HienThiDuLieu(khoaBUS.GetAll());
        }

        // Hiển thị dữ liệu lên bảng
        private void HienThiDuLieu(DataTable dt)
        {
            dgvKhoa.Rows.Clear();

            foreach (DataRow row in dt.Rows)
            {
                dgvKhoa.Rows.Add(
                    row["MaKhoa"],
                    row["TenKhoa"],
                    row["SDT"],
                    row["Email"],
                    row["DiaChi"]
                );
            }

            dgvKhoa.ClearSelection();
        }

        // Lấy dữ liệu từ Form
        private KhoaDTO LayDuLieu()
        {
            return new KhoaDTO
            {
                MaKhoa = txtMaKhoa.Text.Trim(),
                TenKhoa = txtTenKhoa.Text.Trim(),
                SDT = txtSDT.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                DiaChi = txtDiaChi.Text.Trim()
            };
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                KhoaDTO khoa = LayDuLieu();

                if (!khoaBUS.Them(khoa))
                {
                    MessageBox.Show(
                        "Thêm khoa thất bại! Kiểm tra dữ liệu hoặc mã khoa bị trùng.");
                    return;
                }

                MessageBox.Show("Thêm khoa thành công!");
                LoadData();
                XoaTrang();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể thêm khoa!\n" + ex.Message);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvKhoa.CurrentRow == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn khoa cần sửa!");
                return;
            }

            try
            {
                KhoaDTO khoa = LayDuLieu();

                if (!khoaBUS.Sua(khoa))
                {
                    MessageBox.Show(
                        "Cập nhật khoa thất bại!");
                    return;
                }

                MessageBox.Show(
                    "Cập nhật khoa thành công!");
                LoadData();
                XoaTrang();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể cập nhật khoa!\n" + ex.Message);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvKhoa.CurrentRow == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn khoa cần xóa!");
                return;
            }

            string maKhoa = dgvKhoa.CurrentRow
                .Cells["MaKhoa"]
                .Value?.ToString() ?? "";

            if (MessageBox.Show(
                    "Bạn có chắc muốn xóa khoa " + maKhoa + "?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question)
                != DialogResult.Yes)
            {
                return;
            }

            try
            {
                if (!khoaBUS.Xoa(maKhoa))
                {
                    MessageBox.Show(
                        "Xóa khoa thất bại!");
                    return;
                }

                MessageBox.Show(
                    "Xóa khoa thành công!");
                LoadData();
                XoaTrang();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể xóa khoa!\n" + ex.Message);
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            try
            {
                HienThiDuLieu(
                    khoaBUS.TimKiem(
                        txtTimKiem.Text.Trim()));
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tìm kiếm!\n" + ex.Message);
            }
        }

        private void dgvKhoa_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row =
                dgvKhoa.Rows[e.RowIndex];

            txtMaKhoa.Text =
                row.Cells["MaKhoa"].Value?.ToString() ?? "";

            txtTenKhoa.Text =
                row.Cells["TenKhoa"].Value?.ToString() ?? "";

            txtSDT.Text =
                row.Cells["SDT"].Value?.ToString() ?? "";

            txtEmail.Text =
                row.Cells["Email"].Value?.ToString() ?? "";

            txtDiaChi.Text =
                row.Cells["DiaChi"].Value?.ToString() ?? "";
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            XoaTrang();
            LoadData();
        }

        private void XoaTrang()
        {
            txtMaKhoa.Clear();
            txtTenKhoa.Clear();
            txtSDT.Clear();
            txtEmail.Clear();
            txtDiaChi.Clear();
            txtTimKiem.Clear();

            dgvKhoa.ClearSelection();
        }

        private void FKhoa_Resize(object sender, EventArgs e)
        {
            CanGiuaTieuDe();

            dgvKhoa.Left = 30;
            dgvKhoa.Width =
                this.ClientSize.Width - 60;

            dgvKhoa.Height =
                this.ClientSize.Height -
                dgvKhoa.Top - 30;
        }

        private void CanGiuaTieuDe()
        {
            lblTieuDe.Left =
                (this.ClientSize.Width -
                 lblTieuDe.Width) / 2;
        }
    }
}
