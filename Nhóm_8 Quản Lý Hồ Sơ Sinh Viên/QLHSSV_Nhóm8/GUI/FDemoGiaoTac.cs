using System;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using QLHSSV_Nhóm8.BUS;

namespace QLHSSV_Nhóm8.GUI
{
    public class FDemoGiaoTac : Form
    {
        private readonly DemoGiaoTacBUS bus =
            new DemoGiaoTacBUS();

        private Guid runId = Guid.Empty;

        private string scenario = "DR";
        private string mode = "ERROR";

        private Task<DataTable>? taskT1Runner;
        private Task<DataTable>? taskT2Runner;

        private long lastLogId = 0;
        private bool polling = false;

        private readonly System.Windows.Forms.Timer timerLog =
            new System.Windows.Forms.Timer();

        // Header
        private Label lblTieuDe = null!;
        private Label lblTinhHuong = null!;
        private Label lblTrangThaiChung = null!;

        // Chọn lỗi / chế độ
        private Button btnLostUpdate = null!;
        private Button btnDirtyRead = null!;
        private Button btnNonRepeatable = null!;
        private Button btnPhantom = null!;
        private Button btnLoi = null!;
        private Button btnFix = null!;
        private Button btnReset = null!;

        // T1 / T2
        private GroupBox grpT1 = null!;
        private GroupBox grpT2 = null!;

        private readonly Button[] t1Buttons =
            new Button[4];

        private readonly Button[] t2Buttons =
            new Button[4];

        private Label lblT1 = null!;
        private Label lblT2 = null!;

        private RichTextBox txtT1 = null!;
        private RichTextBox txtT2 = null!;

        // Dữ liệu + giải thích
        private DataGridView dgvInspector = null!;
        private RichTextBox txtGiaiThich = null!;

        public FDemoGiaoTac()
        {
            KhoiTaoGiaoDien();

            timerLog.Interval = 250;
            timerLog.Tick += TimerLog_Tick;

            this.Load += async (s, e) =>
                await KhoiTaoKichBanAsync();

            this.FormClosing +=
                FDemoGiaoTac_FormClosing;
        }

        // =====================================================
        // GIAO DIỆN
        // =====================================================
        private void KhoiTaoGiaoDien()
        {
            Text = "Demo các vấn đề giao tác";
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 10);

            TableLayoutPanel root =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 6
                };

            root.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 100));
            root.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 62));
            root.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 58));
            root.RowStyles.Add(
                new RowStyle(SizeType.Percent, 58));
            root.RowStyles.Add(
                new RowStyle(SizeType.Percent, 42));
            root.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 34));

            Controls.Add(root);

            // HEADER
            Panel header =
                new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.White
                };

            root.Controls.Add(header, 0, 0);

            lblTieuDe =
                new Label
                {
                    Text = "DEMO CÁC VẤN ĐỀ GIAO TÁC",
                    Font = new Font(
                        "Segoe UI",
                        22,
                        FontStyle.Bold),
                    AutoSize = true,
                    Top = 18
                };

            Label sub =
                new Label
                {
                    Text =
                        "Lost Update • Dirty Read • " +
                        "Non-repeatable Read • Phantom Read",
                    AutoSize = true,
                    ForeColor = Color.DimGray,
                    Top = 66
                };

            header.Controls.Add(lblTieuDe);
            header.Controls.Add(sub);

            header.Resize += (s, e) =>
            {
                lblTieuDe.Left =
                    (header.ClientSize.Width -
                     lblTieuDe.Width) / 2;

                sub.Left =
                    (header.ClientSize.Width -
                     sub.Width) / 2;
            };

            // MENU
            FlowLayoutPanel menu =
                new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    WrapContents = false,
                    Padding = new Padding(
                        25, 6, 5, 5)
                };

            root.Controls.Add(menu, 0, 1);

            btnLostUpdate =
                TaoNutMenu("Lost Update");

            btnDirtyRead =
                TaoNutMenu("Dirty Read");

            btnNonRepeatable =
                TaoNutMenu("Non-repeatable Read");

            btnPhantom =
                TaoNutMenu("Phantom Read");

            btnLoi =
                TaoNutMenu("Mô phỏng lỗi");

            btnFix =
                TaoNutMenu("Khắc phục");

            btnReset =
                TaoNutMenu("Reset");

            menu.Controls.Add(btnLostUpdate);
            menu.Controls.Add(btnDirtyRead);
            menu.Controls.Add(btnNonRepeatable);
            menu.Controls.Add(btnPhantom);
            menu.Controls.Add(btnLoi);
            menu.Controls.Add(btnFix);
            menu.Controls.Add(btnReset);

            btnLostUpdate.Click +=
                async (s, e) =>
                await ChonKichBanAsync("LU");

            btnDirtyRead.Click +=
                async (s, e) =>
                await ChonKichBanAsync("DR");

            btnNonRepeatable.Click +=
                async (s, e) =>
                await ChonKichBanAsync("NRR");

            btnPhantom.Click +=
                async (s, e) =>
                await ChonKichBanAsync("PR");

            btnLoi.Click +=
                async (s, e) =>
                await ChonCheDoAsync("ERROR");

            btnFix.Click +=
                async (s, e) =>
                await ChonCheDoAsync("FIX");

            btnReset.Click +=
                async (s, e) =>
                await KhoiTaoKichBanAsync();

            // TÌNH HUỐNG
            Panel pTinhHuong =
                new Panel
                {
                    Dock = DockStyle.Fill,
                    Padding = new Padding(
                        25, 4, 25, 4)
                };

            root.Controls.Add(
                pTinhHuong,
                0,
                2);

            lblTinhHuong =
                new Label
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.White,
                    BorderStyle =
                        BorderStyle.FixedSingle,
                    TextAlign =
                        ContentAlignment.MiddleLeft,
                    Padding =
                        new Padding(14, 0, 0, 0),
                    Font =
                        new Font(
                            "Segoe UI",
                            10,
                            FontStyle.Bold)
                };

            pTinhHuong.Controls.Add(
                lblTinhHuong);

            // TRANSACTIONS
            TableLayoutPanel transactions =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 2,
                    RowCount = 1,
                    Padding =
                        new Padding(
                            22, 4, 22, 4)
                };

            transactions.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 50));

            transactions.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 50));

            root.Controls.Add(
                transactions,
                0,
                3);

            grpT1 =
                TaoKhungGiaoTac(
                    "TRANSACTION 1");

            grpT2 =
                TaoKhungGiaoTac(
                    "TRANSACTION 2");

            grpT1.Margin =
                new Padding(0, 0, 8, 0);

            grpT2.Margin =
                new Padding(8, 0, 0, 0);

            transactions.Controls.Add(
                grpT1,
                0,
                0);

            transactions.Controls.Add(
                grpT2,
                1,
                0);

            TaoNoiDungGiaoTac(
                grpT1,
                t1Buttons,
                out lblT1,
                out txtT1,
                "T1");

            TaoNoiDungGiaoTac(
                grpT2,
                t2Buttons,
                out lblT2,
                out txtT2,
                "T2");

            // BOTTOM
            TableLayoutPanel bottom =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 2,
                    RowCount = 1,
                    Padding =
                        new Padding(
                            22, 4, 22, 4)
                };

            bottom.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 62));

            bottom.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 38));

            root.Controls.Add(
                bottom,
                0,
                4);

            GroupBox gInspector =
                new GroupBox
                {
                    Text =
                        "DATABASE INSPECTOR - DỮ LIỆU THỰC TẾ",
                    Dock = DockStyle.Fill,
                    Font =
                        new Font(
                            "Segoe UI",
                            10,
                            FontStyle.Bold),
                    Margin =
                        new Padding(0, 0, 8, 0)
                };

            dgvInspector =
                new DataGridView
                {
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    RowHeadersVisible = false,
                    AutoSizeColumnsMode =
                        DataGridViewAutoSizeColumnsMode.Fill,
                    BackgroundColor = Color.White,
                    SelectionMode =
                        DataGridViewSelectionMode.FullRowSelect
                };

            gInspector.Controls.Add(
                dgvInspector);

            GroupBox gExplain =
                new GroupBox
                {
                    Text =
                        "GIẢI THÍCH HIỆN TƯỢNG / CƠ CHẾ",
                    Dock = DockStyle.Fill,
                    Font =
                        new Font(
                            "Segoe UI",
                            10,
                            FontStyle.Bold),
                    Margin =
                        new Padding(8, 0, 0, 0)
                };

            txtGiaiThich =
                new RichTextBox
                {
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    BorderStyle =
                        BorderStyle.None,
                    BackColor =
                        Color.FromArgb(
                            255, 252, 235),
                    Font =
                        new Font(
                            "Segoe UI",
                            10)
                };

            gExplain.Controls.Add(
                txtGiaiThich);

            bottom.Controls.Add(
                gInspector,
                0,
                0);

            bottom.Controls.Add(
                gExplain,
                1,
                0);

            lblTrangThaiChung =
                new Label
                {
                    Dock = DockStyle.Fill,
                    TextAlign =
                        ContentAlignment.MiddleLeft,
                    Padding =
                        new Padding(23, 0, 0, 0),
                    Font =
                        new Font(
                            "Segoe UI",
                            9,
                            FontStyle.Bold)
                };

            root.Controls.Add(
                lblTrangThaiChung,
                0,
                5);
        }

        private GroupBox TaoKhungGiaoTac(
            string text)
        {
            return new GroupBox
            {
                Text = text,
                Dock = DockStyle.Fill,
                Font =
                    new Font(
                        "Segoe UI",
                        10,
                        FontStyle.Bold)
            };
        }

        private void TaoNoiDungGiaoTac(
            GroupBox group,
            Button[] buttons,
            out Label status,
            out RichTextBox log,
            string side)
        {
            TableLayoutPanel layout =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 3
                };

            layout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 150));

            layout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 38));

            layout.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent, 100));

            FlowLayoutPanel steps =
                new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    FlowDirection =
                        FlowDirection.TopDown,
                    WrapContents = false,
                    Padding =
                        new Padding(8, 5, 8, 5)
                };

            for (int i = 0; i < 4; i++)
            {
                Button b =
                    TaoNutBuoc();

                b.Tag = side + "-" + (i + 1);

                b.Click +=
                    StepButton_Click;

                buttons[i] = b;

                steps.Controls.Add(b);
            }

            status =
                new Label
                {
                    Dock = DockStyle.Fill,
                    TextAlign =
                        ContentAlignment.MiddleLeft,
                    Padding =
                        new Padding(8, 0, 0, 0),
                    Text =
                        "Trạng thái: Chờ thực thi"
                };

            log =
                new RichTextBox
                {
                    Dock = DockStyle.Fill,
                    ReadOnly = true,

                    BackColor = Color.White,
                    ForeColor = Color.Black,

                    BorderStyle =
                        BorderStyle.FixedSingle,

                    Font =
                        new Font(
                            "Consolas",
                            9.5f)
                };

            layout.Controls.Add(
                steps,
                0,
                0);

            layout.Controls.Add(
                status,
                0,
                1);

            layout.Controls.Add(
                log,
                0,
                2);

            group.Controls.Add(layout);
        }

        private Button TaoNutMenu(
            string text)
        {
            return new Button
            {
                Text = text,
                Size = new Size(170, 40),
                Margin = new Padding(5),
                Font =
                    new Font(
                        "Segoe UI",
                        9.5f,
                        FontStyle.Bold),
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
        }

        private Button TaoNutBuoc()
        {
            Button b =
                new Button
                {
                    Width = 560,
                    Height = 30,
                    TextAlign =
                        ContentAlignment.MiddleLeft,
                    Font =
                        new Font(
                            "Segoe UI",
                            9,
                            FontStyle.Bold),
                    BackColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Margin =
                        new Padding(2, 2, 2, 3)
                };

            b.FlatAppearance.BorderColor =
                Color.LightGray;

            return b;
        }

        // =====================================================
        // KHỞI TẠO KỊCH BẢN
        // =====================================================
        private async Task ChonKichBanAsync(
            string newScenario)
        {
            await HuyPhienCuAsync();

            scenario = newScenario;

            await KhoiTaoKichBanAsync();
        }

        private async Task ChonCheDoAsync(
            string newMode)
        {
            await HuyPhienCuAsync();

            mode = newMode;

            await KhoiTaoKichBanAsync();
        }

        private async Task HuyPhienCuAsync()
        {
            timerLog.Stop();

            if (runId != Guid.Empty)
            {
                try
                {
                    await bus.CancelAsync(
                        runId);

                    await Task.Delay(250);
                }
                catch
                {
                }
            }
        }

        private async Task KhoiTaoKichBanAsync()
        {
            timerLog.Stop();

            runId = Guid.NewGuid();
            lastLogId = 0;

            txtT1.Clear();
            txtT2.Clear();

            lblT1.Text =
                "Trạng thái: Chờ thực thi";

            lblT2.Text =
                "Trạng thái: Chờ thực thi";

            lblT1.ForeColor =
                Color.Black;

            lblT2.ForeColor =
                Color.Black;

            await bus.InitAsync(
                runId,
                scenario,
                mode);

            CauHinhGiaoDien();

            KhoiDongRunner();

            await LoadInspectorAsync();

            timerLog.Start();

            lblTrangThaiChung.Text =
                mode == "ERROR"
                ? "Chế độ: MÔ PHỎNG LỖI"
                : "Chế độ: KHẮC PHỤC";
        }

        private void KhoiDongRunner()
        {
            taskT1Runner = null;
            taskT2Runner = null;

            if (scenario == "DR")
            {
                taskT1Runner =
                    bus.RunAsync(
                        "sp_DR_T1_Runner",
                        runId);
            }
            else if (scenario == "NRR")
            {
                taskT1Runner =
                    bus.RunAsync(
                        "sp_NRR_T1_Runner",
                        runId);
            }
            else if (scenario == "LU")
            {
                taskT1Runner =
                    bus.RunAsync(
                        "sp_LU_T1_Runner",
                        runId);

                taskT2Runner =
                    bus.RunAsync(
                        "sp_LU_T2_Runner",
                        runId);
            }
            else if (scenario == "PR")
            {
                taskT1Runner =
                    bus.RunAsync(
                        "sp_PR_T1_Runner",
                        runId);
            }

            if (taskT1Runner != null)
                _ = TheoDoiRunnerAsync(
                    taskT1Runner,
                    "T1");

            if (taskT2Runner != null)
                _ = TheoDoiRunnerAsync(
                    taskT2Runner,
                    "T2");
        }

        private async Task TheoDoiRunnerAsync(
            Task<DataTable> task,
            string side)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                if (!IsDisposed)
                {
                    BeginInvoke(
                        new Action(() =>
                        {
                            GhiLogLocal(
                                side == "T1"
                                    ? txtT1
                                    : txtT2,
                                "Lỗi runner: " +
                                ex.Message);
                        }));
                }
            }
        }

        // =====================================================
        // CẤU HÌNH UI THEO 4 LỖI / 4 FIX
        // =====================================================
        private void CauHinhGiaoDien()
        {
            foreach (Button b in t1Buttons)
            {
                b.Visible = false;
                b.Enabled = false;
                b.BackColor = Color.White;
                b.ForeColor = Color.Black;
            }

            foreach (Button b in t2Buttons)
            {
                b.Visible = false;
                b.Enabled = false;
                b.BackColor = Color.White;
                b.ForeColor = Color.Black;
            }

            btnLoi.BackColor =
                mode == "ERROR"
                ? Color.FromArgb(
                    255, 220, 220)
                : Color.White;

            btnFix.BackColor =
                mode == "FIX"
                ? Color.FromArgb(
                    220, 245, 225)
                : Color.White;

            if (scenario == "DR")
                CauHinhDirtyRead();
            else if (scenario == "NRR")
                CauHinhNonRepeatable();
            else if (scenario == "LU")
                CauHinhLostUpdate();
            else
                CauHinhPhantom();
        }

        private void CauHinhDirtyRead()
        {
            lblTieuDe.Text =
                mode == "ERROR"
                ? "DEMO DIRTY READ"
                : "KHẮC PHỤC DIRTY READ";

            grpT1.Text =
                "TRANSACTION 1 - USER 1";

            grpT2.Text =
                "TRANSACTION 2 - USER 2";

            lblTinhHuong.Text =
                "SV01: Đang học → Bảo lưu nhưng T1 chưa COMMIT. " +
                (mode == "ERROR"
                    ? "T2 dùng READ UNCOMMITTED để đọc dữ liệu chưa xác nhận."
                    : "T2 dùng READ COMMITTED nên phải chờ T1 COMMIT/ROLLBACK.");

            t1Buttons[0].Text =
                "1. Bắt đầu giao tác";
            t1Buttons[1].Text =
                "2. Cập nhật SV01 → Bảo lưu (CHƯA COMMIT)";
            t1Buttons[2].Text =
                "3a. ✓ COMMIT";
            t1Buttons[3].Text =
                "3b. ✖ ROLLBACK";

            for (int i = 0; i < 4; i++)
                t1Buttons[i].Visible = true;

            t1Buttons[0].Enabled = true;

            t1Buttons[3].BackColor =
                Color.FromArgb(
                    255, 215, 215);
            t1Buttons[3].ForeColor =
                Color.Firebrick;

            t2Buttons[0].Visible = true;

            t2Buttons[0].Text =
                mode == "ERROR"
                ? "🔎 Tra cứu SV01 - READ UNCOMMITTED"
                : "🛡 Tra cứu SV01 - READ COMMITTED";

            t2Buttons[0].BackColor =
                mode == "ERROR"
                ? Color.FromArgb(
                    255, 225, 225)
                : Color.FromArgb(
                    220, 245, 225);

            txtGiaiThich.Text =
                mode == "ERROR"
                ?
                "DIRTY READ - ĐỌC DỮ LIỆU RÁC\r\n\r\n" +

                "• Hiện tượng:\r\n" +
                "Phòng Đào tạo cập nhật trạng thái của SV01 từ \"Đang học\" " +
                "thành \"Bảo lưu\" nhưng Transaction 1 chưa COMMIT.\r\n\r\n" +

                "Trong thời điểm đó, Transaction 2 sử dụng READ UNCOMMITTED " +
                "và vẫn đọc được trạng thái \"Bảo lưu\".\r\n\r\n" +

                "Sau đó Transaction 1 ROLLBACK nên dữ liệu thật của SV01 " +
                "trở lại \"Đang học\".\r\n\r\n" +

                "→ Như vậy giá trị \"Bảo lưu\" mà T2 đã đọc là dữ liệu chưa " +
                "được xác nhận, tức dữ liệu rác.\r\n\r\n" +

                "• Nguyên nhân:\r\n" +
                "READ UNCOMMITTED cho phép một giao tác đọc dữ liệu đang được " +
                "giao tác khác thay đổi nhưng chưa COMMIT."

                :

                "KHẮC PHỤC DIRTY READ\r\n\r\n" +

                "• Sử dụng READ COMMITTED.\r\n\r\n" +

                "Khi T1 đang thay đổi trạng thái SV01 nhưng chưa COMMIT hoặc " +
                "ROLLBACK, T2 không được phép đọc giá trị đang thay đổi mà " +
                "phải chờ T1 kết thúc.\r\n\r\n" +

                "Nếu T1 ROLLBACK, T2 chỉ đọc được trạng thái \"Đang học\".\r\n\r\n" +

                "→ T2 không còn đọc phải dữ liệu chưa được xác nhận.";
        }
        private void CauHinhNonRepeatable()
        {
            lblTieuDe.Text =
                mode == "ERROR"
                ? "DEMO NON-REPEATABLE READ"
                : "KHẮC PHỤC NON-REPEATABLE READ";

            grpT1.Text =
                "TRANSACTION 1 - USER 1";

            grpT2.Text =
                "TRANSACTION 2 - USER 2";

            lblTinhHuong.Text =
                "T1 đọc SDT khoa CNTT hai lần. " +
                (mode == "ERROR"
                    ? "T2 UPDATE + COMMIT giữa hai lần đọc."
                    : "T1 dùng REPEATABLE READ để giữ khóa đọc.");

            t1Buttons[0].Text =
                "1. Bắt đầu giao tác";
            t1Buttons[1].Text =
                "2. Đọc SDT CNTT lần 1";
            t1Buttons[2].Text =
                "3. Đọc SDT CNTT lần 2";
            t1Buttons[3].Text =
                "4. ✓ COMMIT";

            for (int i = 0; i < 4; i++)
                t1Buttons[i].Visible = true;

            t1Buttons[0].Enabled = true;

            t2Buttons[0].Visible = true;
            t2Buttons[0].Text =
                "🔧 UPDATE SDT → 0289999888 + COMMIT";

            t2Buttons[0].BackColor =
                mode == "ERROR"
                ? Color.FromArgb(
                    255, 225, 225)
                : Color.FromArgb(
                    220, 235, 250);

            txtGiaiThich.Text =
                mode == "ERROR"
                ?
                "NON-REPEATABLE READ - ĐỌC KHÔNG LẶP LẠI\r\n\r\n" +

                "• Hiện tượng:\r\n" +
                "Transaction 1 đọc thông tin khoa CNTT và nhận được " +
                "SDT = 0281234567.\r\n\r\n" +

                "Trong khi T1 vẫn chưa kết thúc, Transaction 2 cập nhật SDT " +
                "của khoa thành 0289999888 và COMMIT.\r\n\r\n" +

                "Khi T1 đọc lại cùng bản ghi lần thứ hai, kết quả trở thành " +
                "0289999888.\r\n\r\n" +

                "→ Hai lần SELECT trong cùng một giao tác cho ra hai giá trị khác nhau.\r\n\r\n" +

                "• Nguyên nhân:\r\n" +
                "Ở mức READ COMMITTED, Shared Lock được giải phóng sau khi " +
                "mỗi câu SELECT kết thúc nên T2 có thể chen vào UPDATE."

                :

                "KHẮC PHỤC NON-REPEATABLE READ\r\n\r\n" +

                "• Nâng mức cô lập lên REPEATABLE READ.\r\n\r\n" +

                "Sau khi T1 đọc bản ghi khoa CNTT lần thứ nhất, Shared Lock " +
                "được giữ cho tới khi toàn bộ Transaction 1 kết thúc.\r\n\r\n" +

                "Nếu T2 yêu cầu UPDATE SDT trong thời gian này, T2 phải chờ.\r\n\r\n" +

                "Do đó lần đọc thứ hai của T1 vẫn nhận SDT = 0281234567.\r\n\r\n" +

                "→ Hai lần đọc trong cùng T1 được đảm bảo nhất quán."; }

        private void CauHinhLostUpdate()
        {
            lblTieuDe.Text =
                mode == "ERROR"
                ? "DEMO LOST UPDATE"
                : "KHẮC PHỤC LOST UPDATE";

            grpT1.Text =
                "TRANSACTION 1 - USER 1";

            grpT2.Text =
                "TRANSACTION 2 - USER 2";

            lblTinhHuong.Text =
                "T1 và T2 cùng mở NT01. T1 đổi SDT thành 0901234111; " +
                "T2 sửa địa chỉ và lưu toàn bộ dữ liệu đang giữ. " +
                (mode == "FIX"
                    ? "Chế độ fix dùng UPDLOCK ngay từ bước đọc."
                    : "");

            t1Buttons[0].Text =
                mode == "ERROR"
                ? "1. T1 mở NT01"
                : "1. T1 mở NT01 bằng UPDLOCK";

            t1Buttons[1].Text =
                "2. T1 lưu SDT 0901234111 + COMMIT";

            t1Buttons[0].Visible = true;
            t1Buttons[1].Visible = true;
            t1Buttons[0].Enabled = true;

            t2Buttons[0].Text =
                mode == "ERROR"
                ? "1. T2 mở NT01 từ dữ liệu cũ"
                : "1. T2 mở NT01 bằng UPDLOCK";

            t2Buttons[1].Text =
                "2. T2 lưu địa chỉ Bình Dương";

            t2Buttons[0].Visible = true;
            t2Buttons[1].Visible = true;

            t2Buttons[1].BackColor =
                mode == "ERROR"
                ? Color.FromArgb(
                    255, 215, 215)
                : Color.FromArgb(
                    220, 245, 225);

            txtGiaiThich.Text =
                mode == "ERROR"
                ?
                "LOST UPDATE - MẤT DỮ LIỆU CẬP NHẬT\r\n\r\n" +

                "• Hiện tượng:\r\n" +
                "Sinh viên và Chuyên viên nhập liệu cùng mở thông tin " +
                "người thân NT01 khi SDT hiện tại là 0901234568.\r\n\r\n" +

                "Sinh viên cập nhật SDT thành 0901234111 và lưu thành công. " +
                "Tuy nhiên Chuyên viên vẫn đang giữ dữ liệu cũ có SDT = " +
                "0901234568. Khi Chuyên viên sửa địa chỉ thành Bình Dương " +
                "và lưu toàn bộ dòng, SDT cũ 0901234568 được ghi trở lại.\r\n\r\n" +

                "→ Kết quả: thay đổi SDT 0901234111 của Sinh viên bị mất.\r\n\r\n" +

                "• Nguyên nhân:\r\n" +
                "Hai giao tác cùng đọc dữ liệu ban đầu nhưng không giữ khóa " +
                "cập nhật ngay từ bước đọc. Giao tác lưu sau ghi đè kết quả " +
                "của giao tác lưu trước."

                :

                "KHẮC PHỤC LOST UPDATE\r\n\r\n" +

                "• Sử dụng UPDLOCK khi đọc thông tin NT01.\r\n\r\n" +

                "Khi Transaction 1 đã mở NT01 bằng UPDLOCK, Transaction 2 " +
                "không thể đồng thời lấy khóa cập nhật trên cùng bản ghi mà " +
                "phải chờ Transaction 1 hoàn tất.\r\n\r\n" +

                "Sau khi T1 COMMIT SDT = 0901234111, T2 mới được đọc dữ liệu " +
                "mới nhất rồi sửa địa chỉ thành Bình Dương.\r\n\r\n" +

                "→ Kết quả cuối: SDT 0901234111 vẫn được giữ, chỉ DiaChi thay đổi.\r\n\r\n" +

                "• Cơ chế:\r\n" +
                "UPDLOCK giúp ngăn hai giao tác cùng đọc dữ liệu để chuẩn bị " +
                "cập nhật tại cùng một thời điểm.";
        }

        private void CauHinhPhantom()
        {
            lblTieuDe.Text =
                mode == "ERROR"
                ? "DEMO PHANTOM READ"
                : "KHẮC PHỤC PHANTOM READ";

            grpT1.Text =
                "TRANSACTION 1 - USER 1";

            grpT2.Text =
                "TRANSACTION 2 - USER 2";

            lblTinhHuong.Text =
                "T1 đếm CTĐT ngành HHTT hai lần. " +
                (mode == "ERROR"
                    ? "T2 INSERT CT99 giữa hai lần đọc."
                    : "T1 dùng SERIALIZABLE để khóa phạm vi MaNganh = HHTT.");

            t1Buttons[0].Text =
                "1. Bắt đầu giao tác";
            t1Buttons[1].Text =
                "2. Đếm CTĐT HHTT lần 1";
            t1Buttons[2].Text =
                "3. Đếm CTĐT HHTT lần 2";
            t1Buttons[3].Text =
                "4. ✓ COMMIT";

            for (int i = 0; i < 4; i++)
                t1Buttons[i].Visible = true;

            t1Buttons[0].Enabled = true;

            t2Buttons[0].Visible = true;
            t2Buttons[0].Text =
                "➕ INSERT CT99 vào ngành HHTT + COMMIT";

            t2Buttons[0].BackColor =
                mode == "ERROR"
                ? Color.FromArgb(
                    255, 225, 225)
                : Color.FromArgb(
                    220, 235, 250);

            txtGiaiThich.Text =
                mode == "ERROR"
                ?
                "PHANTOM READ - ĐỌC BÓNG MA\r\n\r\n" +

                "• Hiện tượng:\r\n" +
                "Transaction 1 truy vấn số lượng chương trình đào tạo thuộc " +
                "một ngành và nhận được một số lượng ban đầu.\r\n\r\n" +

                "Trong lúc T1 vẫn đang mở, Transaction 2 INSERT thêm một " +
                "chương trình đào tạo mới thuộc cùng ngành và COMMIT.\r\n\r\n" +

                "Khi T1 thực hiện lại cùng truy vấn, số lượng chương trình " +
                "tăng lên và xuất hiện thêm một dòng mới.\r\n\r\n" +

                "→ Dòng mới xuất hiện giữa hai lần truy vấn được gọi là " +
                "dòng \"bóng ma\".\r\n\r\n" +

                "• Nguyên nhân:\r\n" +
                "Mức cô lập hiện tại chỉ bảo vệ các dòng đã tồn tại, " +
                "không khóa toàn bộ phạm vi dữ liệu thỏa điều kiện truy vấn."

                :

                "KHẮC PHỤC PHANTOM READ\r\n\r\n" +

                "• Nâng mức cô lập lên SERIALIZABLE.\r\n\r\n" +

                "SERIALIZABLE tạo Range Lock trên phạm vi dữ liệu được T1 " +
                "truy vấn.\r\n\r\n" +

                "Khi T2 cố INSERT một chương trình mới vào cùng phạm vi, " +
                "T2 phải chờ cho đến khi Transaction 1 COMMIT.\r\n\r\n" +

                "Vì vậy lần truy vấn thứ hai của T1 vẫn cho kết quả giống " +
                "lần thứ nhất.\r\n\r\n" +

                "→ Không còn xuất hiện dòng bóng ma trong Transaction 1.";
        }

        // =====================================================
        // CLICK CÁC NÚT BƯỚC
        // =====================================================
        private async void StepButton_Click(
            object? sender,
            EventArgs e)
        {
            if (sender is not Button b ||
                b.Tag is not string tag)
                return;

            try
            {
                if (scenario == "DR")
                    await XuLyDirtyAsync(tag);
                else if (scenario == "NRR")
                    await XuLyNRRAsync(tag);
                else if (scenario == "LU")
                    await XuLyLUAsync(tag);
                else
                    await XuLyPhantomAsync(tag);

                await LoadInspectorAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Lỗi thực thi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private async Task XuLyDirtyAsync(
            string tag)
        {
            if (tag == "T1-1")
            {
                await bus.SetStepAsync(
                    runId, "T1", 1);

                t1Buttons[0].Enabled = false;
                t1Buttons[1].Enabled = true;
            }
            else if (tag == "T1-2")
            {
                await bus.SetStepAsync(
                    runId, "T1", 2);

                t1Buttons[1].Enabled = false;
                t1Buttons[2].Enabled = true;
                t1Buttons[3].Enabled = true;
                t2Buttons[0].Enabled = true;

                await Task.Delay(250);
            }
            else if (tag == "T1-3")
            {
                await bus.SetStepAsync(
                    runId,
                    "T1",
                    3,
                    "COMMIT");

                t1Buttons[2].Enabled = false;
                t1Buttons[3].Enabled = false;
            }
            else if (tag == "T1-4")
            {
                await bus.SetStepAsync(
                    runId,
                    "T1",
                    3,
                    "ROLLBACK");

                t1Buttons[2].Enabled = false;
                t1Buttons[3].Enabled = false;
            }
            else if (tag == "T2-1")
            {
                t2Buttons[0].Enabled = false;

                DataTable dt =
                    await bus.RunAsync(
                        "sp_DR_T2_Read",
                        runId);

                HienThiKetQua(
                    txtT2,
                    dt);

                if (mode == "ERROR")
                    t2Buttons[0].Enabled = true;
            }
        }

        private async Task XuLyNRRAsync(
            string tag)
        {
            if (tag == "T1-1")
            {
                await bus.SetStepAsync(
                    runId, "T1", 1);

                t1Buttons[0].Enabled = false;
                t1Buttons[1].Enabled = true;
            }
            else if (tag == "T1-2")
            {
                await bus.SetStepAsync(
                    runId, "T1", 2);

                t1Buttons[1].Enabled = false;
                t2Buttons[0].Enabled = true;

                await Task.Delay(250);
            }
            else if (tag == "T2-1")
            {
                t2Buttons[0].Enabled = false;

                // Ở FIX, UPDATE sẽ bị block.
                // Cho phép T1 đọc lần 2 ngay trong lúc T2 đang chờ.
                if (mode == "FIX")
                    t1Buttons[2].Enabled = true;

                DataTable dt =
                    await bus.RunAsync(
                        "sp_NRR_T2_Update",
                        runId);

                HienThiKetQua(
                    txtT2,
                    dt);

                if (mode == "ERROR")
                    t1Buttons[2].Enabled = true;
            }
            else if (tag == "T1-3")
            {
                await bus.SetStepAsync(
                    runId, "T1", 3);

                t1Buttons[2].Enabled = false;
                t1Buttons[3].Enabled = true;

                await Task.Delay(250);
            }
            else if (tag == "T1-4")
            {
                await bus.SetStepAsync(
                    runId,
                    "T1",
                    4,
                    "COMMIT");

                t1Buttons[3].Enabled = false;
            }
        }

        private async Task XuLyLUAsync(
            string tag)
        {
            if (tag == "T1-1")
            {
                await bus.SetStepAsync(
                    runId, "T1", 1);

                t1Buttons[0].Enabled = false;
                t2Buttons[0].Enabled = true;

                await Task.Delay(250);
            }
            else if (tag == "T2-1")
            {
                t2Buttons[0].Enabled = false;

                await bus.SetStepAsync(
                    runId, "T2", 1);

                // Ở FIX, T2 sẽ block ở UPDLOCK.
                // Phải cho phép T1 lưu để giải phóng khóa.
                t1Buttons[1].Enabled = true;

                if (mode == "ERROR")
                {
                    await Task.Delay(300);
                }
            }
            else if (tag == "T1-2")
            {
                await bus.SetStepAsync(
                    runId, "T1", 2);

                t1Buttons[1].Enabled = false;

                if (mode == "ERROR")
                {
                    t2Buttons[1].Enabled = true;
                }
                else
                {
                    // Cho T2 thời gian nhận khóa và đọc dữ liệu mới.
                    await Task.Delay(450);
                    t2Buttons[1].Enabled = true;
                }
            }
            else if (tag == "T2-2")
            {
                await bus.SetStepAsync(
                    runId, "T2", 2);

                t2Buttons[1].Enabled = false;
            }
        }

        private async Task XuLyPhantomAsync(
            string tag)
        {
            if (tag == "T1-1")
            {
                await bus.SetStepAsync(
                    runId, "T1", 1);

                t1Buttons[0].Enabled = false;
                t1Buttons[1].Enabled = true;
            }
            else if (tag == "T1-2")
            {
                await bus.SetStepAsync(
                    runId, "T1", 2);

                t1Buttons[1].Enabled = false;
                t2Buttons[0].Enabled = true;

                await Task.Delay(250);
            }
            else if (tag == "T2-1")
            {
                t2Buttons[0].Enabled = false;

                if (mode == "FIX")
                    t1Buttons[2].Enabled = true;

                DataTable dt =
                    await bus.RunAsync(
                        "sp_PR_T2_Insert",
                        runId);

                HienThiKetQua(
                    txtT2,
                    dt);

                if (mode == "ERROR")
                    t1Buttons[2].Enabled = true;
            }
            else if (tag == "T1-3")
            {
                await bus.SetStepAsync(
                    runId, "T1", 3);

                t1Buttons[2].Enabled = false;
                t1Buttons[3].Enabled = true;

                await Task.Delay(250);
            }
            else if (tag == "T1-4")
            {
                await bus.SetStepAsync(
                    runId,
                    "T1",
                    4,
                    "COMMIT");

                t1Buttons[3].Enabled = false;
            }
        }

        // =====================================================
        // LOG REALTIME
        // =====================================================
        private async void TimerLog_Tick(
            object? sender,
            EventArgs e)
        {
            if (polling ||
                runId == Guid.Empty)
                return;

            polling = true;

            try
            {
                DataTable dt =
                    await bus.GetLogAsync(
                        runId);

                foreach (DataRow row
                         in dt.Rows)
                {
                    long id =
                        Convert.ToInt64(
                            row["Id"]);

                    if (id <= lastLogId)
                        continue;

                    string side =
                        row["GiaoTac"]
                        ?.ToString()
                        ?? "";

                    string loai =
                        row["Loai"]
                        ?.ToString()
                        ?? "INFO";

                    string noiDung =
                        row["NoiDung"]
                        ?.ToString()
                        ?? "";

                    DateTime time =
                        Convert.ToDateTime(
                            row["ThoiGian"]);

                    RichTextBox box =
                        side == "T1"
                        ? txtT1
                        : txtT2;

                    Label status =
                        side == "T1"
                        ? lblT1
                        : lblT2;

                    GhiLogMau(
                        box,
                        time,
                        noiDung,
                        loai);

                    status.Text =
                        "Trạng thái: " +
                        noiDung;

                    status.ForeColor =
                        loai == "DANGER"
                        ? Color.Firebrick
                        : loai == "SUCCESS"
                            ? Color.DarkGreen
                            : loai == "WAIT"
                                ? Color.DarkOrange
                                : Color.Black;

                    lastLogId =
                        Math.Max(
                            lastLogId,
                            id);
                }

                await LoadInspectorAsync();
            }
            catch
            {
                // Poll log không làm văng form.
            }
            finally
            {
                polling = false;
            }
        }

        private void GhiLogMau(
            RichTextBox box,
            DateTime time,
            string text,
            string loai)
        {
            box.SelectionStart =
                box.TextLength;

            box.SelectionLength = 0;

            box.SelectionColor =
                loai == "DANGER"
                ? Color.Firebrick

                : loai == "SUCCESS"
                    ? Color.DarkGreen

                    : loai == "WAIT"
                        ? Color.DarkOrange

                        : Color.Black;

            box.AppendText(
                "[" +
                time.ToString(
                    "HH:mm:ss.fff") +
                "] " +
                text +
                Environment.NewLine);

            box.SelectionStart =
                box.TextLength;

            box.ScrollToCaret();
        }

        private void GhiLogLocal(
            RichTextBox box,
            string text)
        {
            box.AppendText(
                "[" +
                DateTime.Now.ToString(
                    "HH:mm:ss") +
                "] " +
                text +
                Environment.NewLine);
        }

        // =====================================================
        // INSPECTOR / KẾT QUẢ
        // =====================================================
        private async Task LoadInspectorAsync()
        {
            DataTable dt =
                await bus.InspectorAsync(
                    scenario);

            dgvInspector.DataSource = dt;
            dgvInspector.ClearSelection();
        }

        private void HienThiKetQua(
            RichTextBox box,
            DataTable dt)
        {
            box.AppendText(
                Environment.NewLine +
                "----- KẾT QUẢ STORED PROCEDURE -----" +
                Environment.NewLine);

            foreach (DataRow row
                     in dt.Rows)
            {
                foreach (DataColumn col
                         in dt.Columns)
                {
                    box.AppendText(
                        col.ColumnName +
                        ": " +
                        (row[col]?.ToString()
                         ?? "") +
                        Environment.NewLine);
                }

                box.AppendText(
                    "------------------------------" +
                    Environment.NewLine);
            }

            box.SelectionStart =
                box.TextLength;

            box.ScrollToCaret();
        }

        // =====================================================
        // ĐÓNG FORM
        // =====================================================
        private async void FDemoGiaoTac_FormClosing(
            object? sender,
            FormClosingEventArgs e)
        {
            timerLog.Stop();

            if (runId == Guid.Empty)
                return;

            try
            {
                await bus.CancelAsync(
                    runId);
            }
            catch
            {
            }
        }
    }
}
