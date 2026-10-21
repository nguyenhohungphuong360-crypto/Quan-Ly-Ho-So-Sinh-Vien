using System;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using QLHSSV_Nhóm8.DAL;

namespace QLHSSV_Nhóm8.GUI
{
    public class FDemoDeadLock : Form
    {
        private Button btnMoPhong;
        private Button btnXuLyDeadlock;
        private Button btnKhacPhuc;
        private Button btnReset;

        private Label lblMoTa;
        private Label lblTrangThaiT1;
        private Label lblTrangThaiT2;
        private Label lblChiTietT1;
        private Label lblChiTietT2;
        private Label lblCheDo;

        private ProgressBar prgT1;
        private ProgressBar prgT2;

        private RichTextBox txtLogT1;
        private RichTextBox txtLogT2;
        private RichTextBox txtGiaiThich;

        private DataGridView dgvLop;
        private DataGridView dgvSinhVien;

        private readonly DemoDeadLockDAL dal =
            new DemoDeadLockDAL();

        private readonly System.Windows.Forms.Timer timerKet =
            new System.Windows.Forms.Timer();

        private readonly System.Windows.Forms.Timer timerKhacPhuc =
            new System.Windows.Forms.Timer();

        private DateTime thoiDiemBatDau;
        private DateTime thoiDiemKhacPhuc;

        public FDemoDeadLock()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Form
            Text = "Demo Deadlock";
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(246, 248, 251);
            Font = new Font("Segoe UI", 10);

            TableLayoutPanel root =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 6,
                    Padding = new Padding(20)
                };

            root.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 85));

            root.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 52));

            root.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 50));

            root.RowStyles.Add(
                new RowStyle(SizeType.Percent, 44));

            root.RowStyles.Add(
                new RowStyle(SizeType.Percent, 56));

            root.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 35));

            Controls.Add(root);

            // Header
            Panel pnlHeader =
                new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.White
                };

            Label lblTieuDe =
                new Label
                {
                    Text = "DEMO DEADLOCK",
                    Dock = DockStyle.Top,
                    Height = 50,
                    TextAlign =
                        ContentAlignment.BottomCenter,
                    Font = new Font(
                        "Segoe UI",
                        23,
                        FontStyle.Bold)
                };

            Label lblPhu =
                new Label
                {
                    Text =
                        "Deadlock • Ordering All the Items Protocol",
                    Dock = DockStyle.Bottom,
                    Height = 30,
                    TextAlign =
                        ContentAlignment.TopCenter,
                    ForeColor = Color.DimGray
                };

            pnlHeader.Controls.Add(lblTieuDe);
            pnlHeader.Controls.Add(lblPhu);

            root.Controls.Add(
                pnlHeader,
                0,
                0);

            // Menu
            FlowLayoutPanel pnlMenu =
                new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    FlowDirection =
                        FlowDirection.LeftToRight,
                    WrapContents = false,
                    Padding =
                        new Padding(5, 7, 5, 5)
                };

            Button btnTab =
                TaoButtonMenu(
                    "Deadlock",
                    140);

            btnMoPhong =
                TaoButtonMenu(
                    "Mô phỏng lỗi",
                    160);

            btnXuLyDeadlock =
                TaoButtonMenu(
                    "Xử lý Deadlock",
                    165);

            btnKhacPhuc =
                TaoButtonMenu(
                    "Khắc phục",
                    145);

            btnReset =
                TaoButtonMenu(
                    "Reset",
                    120);

            btnTab.Enabled = false;
            btnTab.BackColor =
                Color.FromArgb(230, 230, 230);

            btnMoPhong.BackColor =
                Color.FromArgb(255, 225, 225);

            btnXuLyDeadlock.Enabled = false;

            pnlMenu.Controls.Add(btnTab);
            pnlMenu.Controls.Add(btnMoPhong);
            pnlMenu.Controls.Add(btnXuLyDeadlock);
            pnlMenu.Controls.Add(btnKhacPhuc);
            pnlMenu.Controls.Add(btnReset);

            root.Controls.Add(
                pnlMenu,
                0,
                1);

            // Mô tả
            lblMoTa =
                new Label
                {
                    Text =
                        "Bước 1: T1 giữ L01, T2 giữ SV01. Hai giao tác đi vào trạng thái bị kẹt và chờ tài nguyên của nhau.",
                    Dock = DockStyle.Fill,
                    TextAlign =
                        ContentAlignment.MiddleLeft,
                    BorderStyle =
                        BorderStyle.FixedSingle,
                    BackColor = Color.White,
                    Padding =
                        new Padding(12, 0, 8, 0),
                    Font = new Font(
                        "Segoe UI",
                        10,
                        FontStyle.Bold)
                };

            root.Controls.Add(
                lblMoTa,
                0,
                2);

            // Transaction
            TableLayoutPanel pnlTransaction =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 2,
                    RowCount = 1
                };

            pnlTransaction.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    50));

            pnlTransaction.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    50));

            pnlTransaction.Controls.Add(
                TaoKhungTransaction(
                    "TRANSACTION 1 - USER 1",
                    out lblTrangThaiT1,
                    out lblChiTietT1,
                    out prgT1,
                    out txtLogT1),
                0,
                0);

            pnlTransaction.Controls.Add(
                TaoKhungTransaction(
                    "TRANSACTION 2 - USER 2",
                    out lblTrangThaiT2,
                    out lblChiTietT2,
                    out prgT2,
                    out txtLogT2),
                1,
                0);

            root.Controls.Add(
                pnlTransaction,
                0,
                3);

            // Bottom
            TableLayoutPanel pnlBottom =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 2,
                    RowCount = 1
                };

            pnlBottom.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    62));

            pnlBottom.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    38));

            pnlBottom.Controls.Add(
                TaoDatabaseInspector(),
                0,
                0);

            pnlBottom.Controls.Add(
                TaoKhungGiaiThich(),
                1,
                0);

            root.Controls.Add(
                pnlBottom,
                0,
                4);

            lblCheDo =
                new Label
                {
                    Text =
                        "Chế độ: MÔ PHỎNG THEO SLIDE",
                    Dock = DockStyle.Fill,
                    TextAlign =
                        ContentAlignment.MiddleLeft,
                    Font = new Font(
                        "Segoe UI",
                        9,
                        FontStyle.Bold)
                };

            root.Controls.Add(
                lblCheDo,
                0,
                5);

            // Timer
            timerKet.Interval = 250;
            timerKet.Tick += timerKet_Tick;

            timerKhacPhuc.Interval = 250;
            timerKhacPhuc.Tick +=
                timerKhacPhuc_Tick;

            // Event
            Load += FDemoDeadLock_Load;
            FormClosing += FDemoDeadLock_FormClosing;

            btnMoPhong.Click += btnMoPhong_Click;
            btnXuLyDeadlock.Click +=
                btnXuLyDeadlock_Click;

            btnKhacPhuc.Click +=
                btnKhacPhuc_Click;

            btnReset.Click += btnReset_Click;
        }

        private Control TaoKhungTransaction(
            string tieuDe,
            out Label lblTrangThai,
            out Label lblChiTiet,
            out ProgressBar progress,
            out RichTextBox txtLog)
        {
            GroupBox group =
                new GroupBox
                {
                    Text = tieuDe,
                    Dock = DockStyle.Fill,
                    Font = new Font(
                        "Segoe UI",
                        10,
                        FontStyle.Bold),
                    Padding = new Padding(10)
                };

            TableLayoutPanel pnl =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 4
                };

            pnl.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    42));

            pnl.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    78));

            pnl.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    30));

            pnl.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent,
                    100));

            lblTrangThai =
                new Label
                {
                    Text = "Trạng thái: Chờ thực thi",
                    Dock = DockStyle.Fill,
                    TextAlign =
                        ContentAlignment.MiddleLeft,
                    BackColor =
                        Color.FromArgb(240, 240, 240),
                    Padding =
                        new Padding(10, 0, 6, 0),
                    Font = new Font(
                        "Segoe UI",
                        10,
                        FontStyle.Bold)
                };

            lblChiTiet =
                new Label
                {
                    Text =
                        "Chưa giữ khóa | Chưa chờ tài nguyên",
                    Dock = DockStyle.Fill,
                    TextAlign =
                        ContentAlignment.MiddleLeft,
                    BackColor = Color.White,
                    BorderStyle =
                        BorderStyle.FixedSingle,
                    Padding =
                        new Padding(10, 5, 8, 5),
                    Font = new Font(
                        "Segoe UI",
                        9)
                };

            progress =
                new ProgressBar
                {
                    Dock = DockStyle.Fill,
                    Minimum = 0,
                    Maximum = 100,
                    Value = 0,
                    Margin =
                        new Padding(0, 5, 0, 5)
                };

            txtLog =
                new RichTextBox
                {
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    BackColor = Color.White,
                    Font = new Font(
                        "Consolas",
                        9)
                };

            pnl.Controls.Add(
                lblTrangThai,
                0,
                0);

            pnl.Controls.Add(
                lblChiTiet,
                0,
                1);

            pnl.Controls.Add(
                progress,
                0,
                2);

            pnl.Controls.Add(
                txtLog,
                0,
                3);

            group.Controls.Add(pnl);

            return group;
        }

        private Control TaoDatabaseInspector()
        {
            GroupBox group =
                new GroupBox
                {
                    Text =
                        "DATABASE INSPECTOR - DỮ LIỆU THỰC TẾ",
                    Dock = DockStyle.Fill,
                    Font = new Font(
                        "Segoe UI",
                        10,
                        FontStyle.Bold)
                };

            TableLayoutPanel pnl =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 4,
                    Padding = new Padding(5)
                };

            pnl.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    28));

            pnl.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent,
                    50));

            pnl.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    28));

            pnl.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent,
                    50));

            Label lblLop =
                new Label
                {
                    Text = "Lớp L01",
                    Dock = DockStyle.Fill
                };

            Label lblSV =
                new Label
                {
                    Text = "Sinh viên SV01",
                    Dock = DockStyle.Fill
                };

            dgvLop = TaoGrid();
            dgvSinhVien = TaoGrid();

            pnl.Controls.Add(lblLop, 0, 0);
            pnl.Controls.Add(dgvLop, 0, 1);
            pnl.Controls.Add(lblSV, 0, 2);
            pnl.Controls.Add(dgvSinhVien, 0, 3);

            group.Controls.Add(pnl);

            return group;
        }

        private Control TaoKhungGiaiThich()
        {
            GroupBox group =
                new GroupBox
                {
                    Text =
                        "GIẢI THÍCH HIỆN TƯỢNG / CƠ CHẾ",
                    Dock = DockStyle.Fill,
                    Font = new Font(
                        "Segoe UI",
                        10,
                        FontStyle.Bold)
                };

            txtGiaiThich =
                new RichTextBox
                {
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    BackColor =
                        Color.FromArgb(
                            255,
                            252,
                            225),
                    BorderStyle =
                        BorderStyle.None,
                    Font = new Font(
                        "Segoe UI",
                        10)
                };

            group.Controls.Add(txtGiaiThich);

            return group;
        }

        private Button TaoButtonMenu(
            string text,
            int width)
        {
            Button btn =
                new Button
                {
                    Text = text,
                    Width = width,
                    Height = 36,
                    Margin =
                        new Padding(5, 0, 5, 0),
                    BackColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font(
                        "Segoe UI",
                        9,
                        FontStyle.Bold),
                    Cursor = Cursors.Hand
                };

            btn.FlatAppearance.BorderColor =
                Color.Gray;

            return btn;
        }

        private DataGridView TaoGrid()
        {
            return new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White
            };
        }

        private void FDemoDeadLock_Load(
            object sender,
            EventArgs e)
        {
            ResetGiaoDien();
            LoadData();
        }

        // =========================
        // MÔ PHỎNG DEADLOCK
        // =========================

        private async void btnMoPhong_Click(
            object sender,
            EventArgs e)
        {
            string loi =
                dal.KiemTraDuLieuDemo();

            if (!string.IsNullOrWhiteSpace(loi))
            {
                MessageBox.Show(
                    loi,
                    "Thiếu dữ liệu demo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            timerKhacPhuc.Stop();
            HienThiGiaiThichMoPhong();

            SetDangChay(true);

            try
            {
                XoaLog();

                string[] ketQua =
                    await dal
                        .BatDauMoPhongTheoSlideAsync();

                GhiLog(txtLogT1, ketQua[0]);
                GhiLog(txtLogT2, ketQua[1]);

                thoiDiemBatDau =
                    DateTime.Now;

                lblMoTa.Text =
                    "Bước 1: Hai giao tác đang bị kẹt. T1 chờ SV01 và T2 chờ L01.";

                lblCheDo.Text =
                    "Chế độ: BƯỚC 1 - TRẠNG THÁI KẸT";

                prgT1.Style =
                    ProgressBarStyle.Marquee;

                prgT2.Style =
                    ProgressBarStyle.Marquee;

                timerKet.Start();

                SetTrangThaiKet(
                    lblTrangThaiT1,
                    lblChiTietT1,
                    "Đang giữ: L01 | Đang chờ: SV01");

                SetTrangThaiKet(
                    lblTrangThaiT2,
                    lblChiTietT2,
                    "Đang giữ: SV01 | Đang chờ: L01");

                GhiLog(
                    txtLogT1,
                    "T1 bị kẹt: giữ L01, chờ SV01.");

                GhiLog(
                    txtLogT2,
                    "T2 bị kẹt: giữ SV01, chờ L01.");

                btnXuLyDeadlock.Enabled = false;
            }
            catch (Exception ex)
            {
                await dal.RollbackCaHaiAsync();

                MessageBox.Show(
                    "Không thể bắt đầu mô phỏng!\n" +
                    ex.Message);
            }
            finally
            {
                btnReset.Enabled = true;
            }
        }

        private void timerKet_Tick(
            object sender,
            EventArgs e)
        {
            double giay =
                (DateTime.Now -
                 thoiDiemBatDau)
                .TotalSeconds;

            int soGiay =
                Math.Max(
                    0,
                    (int)giay);

            lblTrangThaiT1.Text =
                "Trạng thái: BỊ KẸT - ĐANG CHỜ TÀI NGUYÊN (" +
                soGiay +
                "s)";

            lblTrangThaiT2.Text =
                "Trạng thái: BỊ KẸT - ĐANG CHỜ TÀI NGUYÊN (" +
                soGiay +
                "s)";

            if (giay >= 5)
            {
                btnXuLyDeadlock.Enabled = true;

                lblMoTa.Text =
                    "Bước 2: Deadlock đã được xác định. Nhấn \"Xử lý Deadlock\" để hủy các giao tác liên quan.";
            }
        }

        private async void btnXuLyDeadlock_Click(
            object sender,
            EventArgs e)
        {
            timerKet.Stop();

            btnXuLyDeadlock.Enabled = false;

            lblMoTa.Text =
                "Bước 3: Hệ thống đang rollback toàn bộ các giao tác liên quan đến khóa chéo.";

            lblCheDo.Text =
                "Chế độ: BƯỚC 3 - ROLLBACK TOÀN BỘ";

            lblTrangThaiT1.Text =
                "Trạng thái: Đang ROLLBACK...";

            lblTrangThaiT2.Text =
                "Trạng thái: Đang ROLLBACK...";

            await dal.RollbackCaHaiAsync();

            prgT1.Style =
                ProgressBarStyle.Blocks;

            prgT2.Style =
                ProgressBarStyle.Blocks;

            prgT1.Value = 100;
            prgT2.Value = 100;

            SetRollback(
                lblTrangThaiT1,
                lblChiTietT1);

            SetRollback(
                lblTrangThaiT2,
                lblChiTietT2);

            const string thongBao =
                "Toàn bộ các giao tác liên quan đến khóa chéo đã được hủy bỏ để đảm bảo tính nhất quán tuyệt đối.";

            GhiLog(
                txtLogT1,
                "T1: DEADLOCK VICTIM - ROLLBACK.");

            GhiLog(
                txtLogT2,
                "T2: DEADLOCK VICTIM - ROLLBACK.");

            GhiLog(
                txtLogT1,
                thongBao);

            GhiLog(
                txtLogT2,
                thongBao);

            lblMoTa.Text =
                thongBao;

            LoadData();
            SetDangChay(false);
        }

        // =========================
        // KHẮC PHỤC
        // =========================

        private async void btnKhacPhuc_Click(
            object sender,
            EventArgs e)
        {
            timerKet.Stop();

            await dal.RollbackCaHaiAsync();

            ChonCheDoKhacPhuc();
            HienThiGiaiThichKhacPhuc();

            SetDangChay(true);
            XoaLog();

            lblCheDo.Text =
                "Chế độ: KHẮC PHỤC - ORDERING ALL THE ITEMS";

            lblMoTa.Text =
                "T1 và T2 cùng tuân theo một thứ tự khóa: L01 → SV01. T2 chờ L01 thay vì giữ SV01 rồi chờ ngược lại.";

            thoiDiemKhacPhuc =
                DateTime.Now;

            BatTrangThaiKhacPhuc();

            timerKhacPhuc.Start();

            GhiLog(
                txtLogT1,
                "BEGIN TRANSACTION → yêu cầu khóa L01.");

            GhiLog(
                txtLogT2,
                "BEGIN TRANSACTION → cũng yêu cầu khóa L01.");

            try
            {
                string[] ketQua =
                    await dal
                        .ChayNganDeadlockAsync();

                timerKhacPhuc.Stop();

                CapNhatKetQuaKhacPhuc(
                    lblTrangThaiT1,
                    lblChiTietT1,
                    prgT1,
                    ketQua[0]);

                CapNhatKetQuaKhacPhuc(
                    lblTrangThaiT2,
                    lblChiTietT2,
                    prgT2,
                    ketQua[1]);

                GhiLog(
                    txtLogT1,
                    ketQua[0]);

                GhiLog(
                    txtLogT2,
                    ketQua[1]);

                lblMoTa.Text =
                    "Hoàn tất: cả T1 và T2 đều COMMIT. Có blocking nhưng không hình thành vòng chờ nên không xảy ra deadlock.";

                LoadData();
            }
            catch (Exception ex)
            {
                timerKhacPhuc.Stop();

                MessageBox.Show(
                    "Lỗi khi chạy khắc phục!\n" +
                    ex.Message);
            }
            finally
            {
                SetDangChay(false);
            }
        }

        private void BatTrangThaiKhacPhuc()
        {
            prgT1.Style =
                ProgressBarStyle.Blocks;

            prgT2.Style =
                ProgressBarStyle.Blocks;

            prgT1.Value = 5;
            prgT2.Value = 5;

            lblTrangThaiT1.Text =
                "Trạng thái: T1 ĐÃ LẤY KHÓA L01";

            lblTrangThaiT1.BackColor =
                Color.FromArgb(255, 245, 205);

            lblChiTietT1.Text =
                "BEGIN → UPDATE L01 thành công → đang giữ X-Lock trên L01 → WAITFOR 10 giây → chưa yêu cầu SV01.";

            lblTrangThaiT2.Text =
                "Trạng thái: T2 BLOCKING - ĐANG CHỜ L01";

            lblTrangThaiT2.BackColor =
                Color.FromArgb(255, 245, 205);

            lblChiTietT2.Text =
                "BEGIN → UPDATE L01 chưa thực hiện được vì T1 đang giữ khóa → T2 chưa giữ SV01 → không tạo khóa chéo.";
        }

        private void timerKhacPhuc_Tick(
            object sender,
            EventArgs e)
        {
            double giay =
                (DateTime.Now -
                 thoiDiemKhacPhuc)
                .TotalSeconds;

            if (giay < 10)
            {
                int conLai =
                    Math.Max(
                        0,
                        10 - (int)giay);

                lblTrangThaiT1.Text =
                    "Trạng thái: T1 GIỮ L01 - WAITFOR " +
                    conLai +
                    "s";

                lblChiTietT1.Text =
                    "T1 đang giữ khóa L01 | Sau WAITFOR sẽ UPDATE SV01 | Sau đó COMMIT và giải phóng khóa.";

                prgT1.Value =
                    Math.Min(
                        48,
                        5 + (int)(giay * 4.3));

                lblTrangThaiT2.Text =
                    "Trạng thái: T2 BLOCKING - CHỜ T1 NHẢ L01";

                lblChiTietT2.Text =
                    "T2 đang đứng tại UPDATE Lop L01 | Chưa lấy được L01 | Chưa chạy WAITFOR | Chưa chạm tới SV01.";

                prgT2.Value = 10;
            }
            else if (giay < 20)
            {
                int conLai =
                    Math.Max(
                        0,
                        20 - (int)giay);

                lblTrangThaiT1.Text =
                    "Trạng thái: T1 ĐÃ COMMIT - ĐÃ GIẢI PHÓNG KHÓA";

                lblTrangThaiT1.BackColor =
                    Color.FromArgb(220, 245, 220);

                lblChiTietT1.Text =
                    "T1 đã UPDATE SV01 → COMMIT → giải phóng L01 và SV01. Vòng chờ không tồn tại.";

                prgT1.Value = 100;

                lblTrangThaiT2.Text =
                    "Trạng thái: T2 ĐÃ LẤY ĐƯỢC L01 - WAITFOR " +
                    conLai +
                    "s";

                lblTrangThaiT2.BackColor =
                    Color.FromArgb(255, 245, 205);

                lblChiTietT2.Text =
                    "Sau khi T1 COMMIT, T2 lấy được L01 → giữ L01 → WAITFOR 10 giây → sau đó UPDATE SV01.";

                prgT2.Value =
                    Math.Min(
                        95,
                        50 + (int)((giay - 10) * 4.5));
            }
            else
            {
                lblTrangThaiT1.Text =
                    "Trạng thái: T1 COMMIT THÀNH CÔNG";

                lblChiTietT1.Text =
                    "T1 hoàn tất theo đúng thứ tự L01 → SV01.";

                prgT1.Value = 100;

                lblTrangThaiT2.Text =
                    "Trạng thái: T2 ĐANG UPDATE SV01 / COMMIT";

                lblChiTietT2.Text =
                    "T2 tiếp tục UPDATE SV01 → chuẩn bị COMMIT. Không có giao tác nào chờ ngược lại.";

                prgT2.Value = 98;
            }
        }

        // =========================
        // GIẢI THÍCH
        // =========================

        private void HienThiGiaiThichMoPhong()
        {
            txtGiaiThich.Text =
                "MÔ PHỎNG DEADLOCK THEO SLIDE\n\n" +
                "Bước 1 - Trạng thái kẹt:\n" +
                "T1 giữ L01 và chờ SV01.\n" +
                "T2 giữ SV01 và chờ L01.\n\n" +
                "=> Xuất hiện vòng chờ: T1 → T2 → T1.\n\n" +
                "Bước 2 - Xử lý Deadlock:\n" +
                "Người dùng hoặc hệ thống kích hoạt hành động xử lý.\n\n" +
                "Bước 3 - Rollback toàn bộ:\n" +
                "Cả T1 và T2 được rollback để minh họa theo slide.\n\n" +
                "Lưu ý: SQL Server thực tế thường tự chọn một deadlock victim, không rollback cả hai.";
        }

        private void HienThiGiaiThichKhacPhuc()
        {
            txtGiaiThich.Text =
                "KHẮC PHỤC - ORDERING ALL THE ITEMS PROTOCOL\n\n" +
                "Quy tắc chung:\n" +
                "Mọi transaction phải xin khóa theo CÙNG MỘT THỨ TỰ: L01 → SV01.\n\n" +
                "1. T1 xin L01 trước và lấy được khóa.\n" +
                "2. T2 cũng xin L01 trước nhưng L01 đang do T1 giữ nên T2 bị BLOCKING.\n" +
                "3. Điểm quan trọng: T2 CHƯA được phép giữ SV01 trong lúc chờ L01.\n" +
                "4. T1 WAITFOR 10 giây → UPDATE SV01 → COMMIT → nhả toàn bộ khóa.\n" +
                "5. T2 lúc này mới lấy được L01 → WAITFOR 10 giây → UPDATE SV01 → COMMIT.\n\n" +
                "Kết quả:\n" +
                "Có chờ (blocking), nhưng KHÔNG có vòng chờ.\n" +
                "Không tồn tại T1 chờ T2 đồng thời T2 chờ T1.\n" +
                "=> Không xảy ra deadlock, cả hai transaction đều COMMIT.";
        }

        // =========================
        // TRẠNG THÁI / RESET
        // =========================

        private void SetTrangThaiKet(
            Label trangThai,
            Label chiTiet,
            string noiDung)
        {
            trangThai.Text =
                "Trạng thái: BỊ KẸT - ĐANG CHỜ TÀI NGUYÊN";

            trangThai.BackColor =
                Color.FromArgb(255, 240, 190);

            chiTiet.Text =
                noiDung +
                " | Loading...";
        }

        private void SetRollback(
            Label trangThai,
            Label chiTiet)
        {
            trangThai.Text =
                "Trạng thái: DEADLOCK VICTIM - ROLLBACK";

            trangThai.BackColor =
                Color.FromArgb(255, 215, 215);

            chiTiet.Text =
                "Transaction đã bị hủy và toàn bộ khóa đã được giải phóng.";
        }

        private void CapNhatKetQuaKhacPhuc(
            Label trangThai,
            Label chiTiet,
            ProgressBar progress,
            string ketQua)
        {
            progress.Style =
                ProgressBarStyle.Blocks;

            progress.Value = 100;

            if (ketQua.Contains("COMMIT"))
            {
                trangThai.Text =
                    "Trạng thái: COMMIT THÀNH CÔNG";

                trangThai.BackColor =
                    Color.FromArgb(220, 245, 220);

                chiTiet.Text =
                    "Transaction hoàn tất. Khóa được giải phóng. Không xảy ra deadlock.";
            }
            else
            {
                trangThai.Text =
                    "Trạng thái: Có lỗi";

                trangThai.BackColor =
                    Color.FromArgb(255, 225, 225);

                chiTiet.Text =
                    ketQua;
            }
        }

        private void btnReset_Click(
            object sender,
            EventArgs e)
        {
            ResetAsync();
        }

        private async void ResetAsync()
        {
            timerKet.Stop();
            timerKhacPhuc.Stop();

            await dal.RollbackCaHaiAsync();

            ResetGiaoDien();
            LoadData();
        }

        private void ResetGiaoDien()
        {
            ChonCheDoLoi();

            btnXuLyDeadlock.Enabled = false;

            lblCheDo.Text =
                "Chế độ: MÔ PHỎNG THEO SLIDE";

            lblMoTa.Text =
                "Bước 1: T1 giữ L01, T2 giữ SV01. Hai giao tác đi vào trạng thái bị kẹt và chờ tài nguyên của nhau.";

            HienThiGiaiThichMoPhong();

            DatCho(
                lblTrangThaiT1,
                lblChiTietT1,
                prgT1);

            DatCho(
                lblTrangThaiT2,
                lblChiTietT2,
                prgT2);

            XoaLog();
            SetDangChay(false);
        }

        private void DatCho(
            Label trangThai,
            Label chiTiet,
            ProgressBar progress)
        {
            trangThai.Text =
                "Trạng thái: Chờ thực thi";

            trangThai.BackColor =
                Color.FromArgb(240, 240, 240);

            chiTiet.Text =
                "Chưa giữ khóa | Chưa chờ tài nguyên";

            progress.Style =
                ProgressBarStyle.Blocks;

            progress.Value = 0;
        }

        private void LoadData()
        {
            try
            {
                dgvLop.DataSource =
                    dal.GetLopDemo();

                dgvSinhVien.DataSource =
                    dal.GetSinhVienDemo();

                dgvLop.ClearSelection();
                dgvSinhVien.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không tải được dữ liệu!\n" +
                    ex.Message);
            }
        }

        private void ChonCheDoLoi()
        {
            btnMoPhong.BackColor =
                Color.FromArgb(255, 225, 225);

            btnKhacPhuc.BackColor =
                Color.White;
        }

        private void ChonCheDoKhacPhuc()
        {
            btnMoPhong.BackColor =
                Color.White;

            btnKhacPhuc.BackColor =
                Color.FromArgb(220, 245, 220);
        }

        private void SetDangChay(
            bool dangChay)
        {
            btnMoPhong.Enabled =
                !dangChay;

            btnKhacPhuc.Enabled =
                !dangChay;

            btnReset.Enabled = true;
        }

        private void XoaLog()
        {
            txtLogT1.Clear();
            txtLogT2.Clear();
        }

        private void GhiLog(
            RichTextBox box,
            string noiDung)
        {
            box.AppendText(
                "[" +
                DateTime.Now.ToString("HH:mm:ss") +
                "] " +
                noiDung +
                Environment.NewLine);

            box.ScrollToCaret();
        }

        private async void FDemoDeadLock_FormClosing(
            object? sender,
            FormClosingEventArgs e)
        {
            timerKet.Stop();
            timerKhacPhuc.Stop();

            await dal.RollbackCaHaiAsync();
        }
    }
}
