using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient; // Hanya menggunakan MySQL

namespace TebakAngka
{
    public partial class Form1 : Form
    {
        // Sesuaikan dengan koneksi XAMPP Anda
        string connectionString = "Server=localhost;Database=sistem_parkir;Uid=root;Pwd=;";

        public Form1()
        {
            InitializeComponent();
            BuildInterface();
            LoadActiveVehicles(); // Langsung memuat data saat form dibuat
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CekKoneksiDatabase();
        }

        private void CekKoneksiDatabase()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Anda bisa menghapus MessageBox ini nanti jika dirasa mengganggu setiap kali aplikasi dibuka
                    MessageBox.Show("Koneksi ke database db_siparkir BERHASIL!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Koneksi GAGAL: Pastikan XAMPP MySQL menyala.\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private Panel header;
        private RadioButton btnMobil;
        private RadioButton btnMotor;
        private TextBox txtPlat;
        private DataGridView grid;
        private Label lblClock;
        private Timer clockTimer;
        private string jenisTerpilih = "Mobil";

        private readonly Color blue = Color.FromArgb(134, 196, 236);
        private readonly Color darkBlue = Color.FromArgb(17, 47, 72);
        private readonly Color green = Color.FromArgb(20, 166, 76);
        private readonly Color red = Color.FromArgb(220, 42, 42);

        private void BuildInterface()
        {
            BackColor = Color.White;
            MinimumSize = new Size(714, 416);
            Font = new Font("Segoe UI", 10F);

            header = new Panel { Dock = DockStyle.Top, Height = 102, BackColor = blue };
            Controls.Add(header);
            header.Paint += Header_Paint;
            header.Resize += delegate
            {
                if (lblClock != null)
                    lblClock.Left = header.ClientSize.Width - 145;
            };

            var title = new Label { Text = "Sistem Parkir", ForeColor = Color.Black, Font = new Font("Segoe UI", 27F, FontStyle.Bold), Location = new Point(170, 17), AutoSize = true };
            var subtitle = new Label { Text = "Selamat Datang", ForeColor = Color.FromArgb(67, 102, 127), Font = new Font("Segoe UI", 19F, FontStyle.Bold), Location = new Point(171, 63), AutoSize = true };
            header.Controls.Add(title);
            header.Controls.Add(subtitle);
            lblClock = new Label
            {
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                Location = new Point(header.ClientSize.Width - 145, 29),
                AutoSize = true
            };
            header.Controls.Add(lblClock);
            clockTimer = new Timer { Interval = 1000 };
            clockTimer.Tick += delegate { UpdateClock(); };
            clockTimer.Start();
            UpdateClock();

            var inputPanel = new Panel
            {
                Location = new Point(20, 125),
                Width = ClientSize.Width - 40,
                Height = 82,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.FromArgb(239, 239, 239),
                BorderStyle = BorderStyle.FixedSingle
            };
            Controls.Add(inputPanel);
            inputPanel.Resize += delegate { LayoutInput(inputPanel); };
            inputPanel.Paint += InputPanel_Paint;

            var vehicleLabel = new Label { Text = "Jenis Kendaraan :", Font = new Font("Segoe UI", 10F, FontStyle.Bold), Location = new Point(18, 8), AutoSize = true };
            inputPanel.Controls.Add(vehicleLabel);
            btnMobil = CreateVehicleButton("🚗  Mobil", true);
            btnMotor = CreateVehicleButton("🏍  Motor", false);
            inputPanel.Controls.Add(btnMobil);
            inputPanel.Controls.Add(btnMotor);
            btnMobil.CheckedChanged += VehicleChanged;
            btnMotor.CheckedChanged += VehicleChanged;

            var plateLabel = new Label { Text = "Plat Nomor :", Font = new Font("Segoe UI", 10F, FontStyle.Bold), AutoSize = true };
            inputPanel.Controls.Add(plateLabel);
            txtPlat = new TextBox { Font = new Font("Segoe UI", 14F), CharacterCasing = CharacterCasing.Upper, MaxLength = 12 };
            inputPanel.Controls.Add(txtPlat);
            var masuk = CreateActionButton("⇥  Masuk", green);
            var keluar = CreateActionButton("⇤  Keluar", red);
            inputPanel.Controls.Add(masuk);
            inputPanel.Controls.Add(keluar);
            masuk.Click += Masuk_Click;
            keluar.Click += Keluar_Click;
            LayoutInput(inputPanel);

            var sectionTitle = new Label
            {
                Text = "Kendaraan Aktif",
                BackColor = blue,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 17F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(18, 0, 0, 0),
                Location = new Point(20, 230),
                Width = ClientSize.Width - 40,
                Height = 38,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            Controls.Add(sectionTitle);
            sectionTitle.Resize += delegate { if (grid != null) grid.Width = sectionTitle.Width; };

            grid = new DataGridView
            {
                Location = new Point(20, 268),
                Width = ClientSize.Width - 40,
                Height = 130,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                GridColor = Color.Gray
            };
            grid.ColumnHeadersHeight = 35;
            grid.RowTemplate.Height = 32;
            grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(220, 220, 220),
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };
            grid.DefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Segoe UI", 10F),
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                SelectionBackColor = Color.FromArgb(190, 225, 248),
                SelectionForeColor = Color.Black
            };
            grid.Columns.Add("No", "NO");
            grid.Columns.Add("Jenis", "JENIS");
            grid.Columns.Add("Plat", "PLAT NOMOR");
            grid.Columns.Add("Masuk", "WAKTU MASUK");
            grid.Columns.Add("Estimasi", "ESTIMASI BIAYA");
            grid.Columns[0].FillWeight = 35;
            Controls.Add(grid);
        }

        private void LayoutInput(Panel p)
        {
            int right = p.ClientSize.Width;
            btnMobil.Location = new Point(18, 34); btnMobil.Size = new Size(105, 42);
            btnMotor.Location = new Point(127, 34); btnMotor.Size = new Size(105, 42);
            txtPlat.Location = new Point(275, 28); txtPlat.Size = new Size(Math.Max(150, right - 585), 42);
            foreach (Control c in p.Controls)
                if (c is Label && c.Text == "Plat Nomor :") c.Location = new Point(275, 8);
            Control masuk = p.Controls[p.Controls.Count - 2];
            Control keluar = p.Controls[p.Controls.Count - 1];
            keluar.Location = new Point(right - 100, 19); keluar.Size = new Size(90, 50);
            masuk.Location = new Point(right - 202, 19); masuk.Size = new Size(90, 50);
        }

        private RadioButton CreateVehicleButton(string text, bool selected)
        {
            return new RadioButton { Text = text, Appearance = Appearance.Button, TextAlign = ContentAlignment.MiddleCenter, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(37, 153, 222), ForeColor = Color.Black, Font = new Font("Segoe UI", 10F, FontStyle.Bold), Checked = selected, TabStop = false };
        }

        private Button CreateActionButton(string text, Color color)
        {
            return new Button { Text = text, BackColor = color, ForeColor = Color.Black, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10F, FontStyle.Bold), Cursor = Cursors.Hand };
        }

        private void UpdateClock()
        {
            if (lblClock == null) return;
            lblClock.Text = DateTime.Now.ToString("HH : mm : ss") + Environment.NewLine + DateTime.Now.ToString("dd/MM/yyyy");
        }

        private void InputPanel_Paint(object sender, PaintEventArgs e)
        {
            using (var pen = new Pen(Color.FromArgb(180, 180, 180), 1))
            {
                e.Graphics.DrawLine(pen, 257, 0, 257, ((Control)sender).ClientSize.Height);
            }
        }

        private void Header_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            using (var pen = new Pen(Color.Black, 2))
            {
                graphics.DrawLine(pen, 150, 8, 150, 112);
            }
            DrawCarIcon(graphics, new Point(43, 35));
            int clockLeft = header.ClientSize.Width - 185;
            using (var pen = new Pen(Color.Black, 3))
            {
                graphics.DrawEllipse(pen, clockLeft, 29, 40, 40);
                graphics.DrawLine(pen, clockLeft + 20, 49, clockLeft + 20, 37);
                graphics.DrawLine(pen, clockLeft + 20, 49, clockLeft + 30, 57);
            }
        }

        private void DrawCarIcon(Graphics graphics, Point location)
        {
            int x = location.X;
            int y = location.Y;
            using (var pen = new Pen(Color.Black, 4))
            using (var brush = new SolidBrush(Color.Black))
            {
                graphics.DrawLine(pen, x + 7, y + 23, x + 14, y + 5);
                graphics.DrawLine(pen, x + 14, y + 5, x + 48, y + 5);
                graphics.DrawLine(pen, x + 48, y + 5, x + 56, y + 23);
                graphics.DrawLine(pen, x + 2, y + 23, x + 61, y + 23);
                graphics.DrawLine(pen, x + 3, y + 23, x + 3, y + 42);
                graphics.DrawLine(pen, x + 60, y + 23, x + 60, y + 42);
                graphics.FillRectangle(brush, x + 10, y + 38, 10, 7);
                graphics.FillRectangle(brush, x + 48, y + 38, 10, 7);
                graphics.FillEllipse(Brushes.White, x + 14, y + 26, 10, 10);
                graphics.FillEllipse(Brushes.White, x + 39, y + 26, 10, 10);
                graphics.DrawRectangle(pen, x + 64, y - 15, 21, 25);
            }
            using (var font = new Font("Segoe UI", 14F, FontStyle.Bold))
            {
                graphics.DrawString("P", font, Brushes.Black, x + 67, y - 14);
            }
        }

        private void VehicleChanged(object sender, EventArgs e)
        {
            if (btnMobil.Checked)
            {
                jenisTerpilih = "Mobil";
                btnMobil.BackColor = Color.FromArgb(37, 153, 222);
                btnMotor.BackColor = Color.FromArgb(205, 205, 205);
            }
            else if (btnMotor.Checked)
            {
                jenisTerpilih = "Motor";
                btnMobil.BackColor = Color.FromArgb(205, 205, 205);
                btnMotor.BackColor = Color.FromArgb(37, 153, 222);
            }
        }

        // =========================================================================
        // BAGIAN KODE MYSQL UNTUK DATABASE SIPARKIR 
        // =========================================================================

        private void Masuk_Click(object sender, EventArgs e)
        {
            string plat = txtPlat.Text.Trim();
            if (plat.Length == 0) { MessageBox.Show("Masukkan plat nomor kendaraan.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Information); txtPlat.Focus(); return; }

            // Konversi UI jenisTerpilih ke id_jenis pada database (berdasarkan soal, 1: Motor, 2: Mobil)
            int idJenis = (jenisTerpilih == "Mobil") ? 2 : 1;

            // Membuat no_tiket otomatis (misal T-jammenitdetik)
            string noTiket = "T-" + DateTime.Now.ToString("HHmmss");

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                // Disematkan id_area = 1 dan id_user = 1 (karena di form ini tidak ada dropdown area dan user login)
                string query = "INSERT INTO transaksi_parkir (no_tiket, plat_nomor, id_jenis, id_area, id_user, status) " +
                               "VALUES (@tiket, @plat, @jenis, 1, 1, 'Parkir')";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@tiket", noTiket);
                    cmd.Parameters.AddWithValue("@plat", plat);
                    cmd.Parameters.AddWithValue("@jenis", idJenis);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Gagal menyimpan data masuk: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            txtPlat.Clear();
            LoadActiveVehicles(); // Segarkan Grid
        }

        private void Keluar_Click(object sender, EventArgs e)
        {
            string plat = txtPlat.Text.Trim();
            if (plat.Length == 0) { MessageBox.Show("Masukkan plat nomor kendaraan yang keluar.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Information); txtPlat.Focus(); return; }

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                // Cari kendaraan yang sedang parkir dengan plat tersebut
                string querySelect = "SELECT id_transaksi, id_jenis, waktu_masuk FROM transaksi_parkir WHERE plat_nomor = @plat AND status = 'Parkir' LIMIT 1";

                int idTransaksi = 0;
                int idJenis = 0;
                DateTime waktuMasuk = DateTime.MinValue;

                using (MySqlCommand cmdSelect = new MySqlCommand(querySelect, conn))
                {
                    cmdSelect.Parameters.AddWithValue("@plat", plat);
                    conn.Open();
                    using (MySqlDataReader r = cmdSelect.ExecuteReader())
                    {
                        if (!r.Read())
                        {
                            MessageBox.Show("Kendaraan aktif dengan plat tersebut tidak ditemukan atau sudah keluar.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        idTransaksi = r.GetInt32(0);
                        idJenis = r.GetInt32(1);
                        waktuMasuk = r.GetDateTime(2);
                    }
                }

                // Kalkulasi Durasi dan Biaya sesuai Rumus di PDF (durasi_jam = CEILING(selisih menit / 60))
                DateTime waktuKeluar = DateTime.Now;
                TimeSpan selisih = waktuKeluar - waktuMasuk;
                int durasiJam = (int)Math.Ceiling(selisih.TotalMinutes / 60.0);
                if (durasiJam < 1) durasiJam = 1; // Minimal 1 jam

                decimal tarifPerJam = (idJenis == 2) ? 5000 : 2000; // 2=Mobil, 1=Motor
                decimal tarifMaks = (idJenis == 2) ? 25000 : 10000;

                decimal totalBiaya = durasiJam * tarifPerJam;
                if (totalBiaya > tarifMaks) totalBiaya = tarifMaks; // Pembatasan maksimal harian

                // Update data transaksi menjadi Selesai
                string queryUpdate = "UPDATE transaksi_parkir SET waktu_keluar = @keluar, durasi_jam = @durasi, total_biaya = @biaya, status = 'Selesai' WHERE id_transaksi = @id";
                using (MySqlCommand cmdUpdate = new MySqlCommand(queryUpdate, conn))
                {
                    cmdUpdate.Parameters.AddWithValue("@keluar", waktuKeluar);
                    cmdUpdate.Parameters.AddWithValue("@durasi", durasiJam);
                    cmdUpdate.Parameters.AddWithValue("@biaya", totalBiaya);
                    cmdUpdate.Parameters.AddWithValue("@id", idTransaksi);
                    cmdUpdate.ExecuteNonQuery();
                }

                MessageBox.Show($"Kendaraan keluar.\nDurasi: {durasiJam} Jam\nTotal Biaya: Rp {totalBiaya:N0}", "Transaksi Selesai", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            txtPlat.Clear();
            LoadActiveVehicles(); // Segarkan Grid
        }

        private void LoadActiveVehicles()
        {
            if (grid == null) return;
            try
            {
                grid.Rows.Clear();
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    // Relasi JOIN ke tabel jenis_kendaraan untuk mendapatkan nama jenisnya (Mobil/Motor)
                    string query = "SELECT jk.nama_jenis, tp.plat_nomor, tp.waktu_masuk, jk.tarif_per_jam " +
                                   "FROM transaksi_parkir tp " +
                                   "JOIN jenis_kendaraan jk ON tp.id_jenis = jk.id_jenis " +
                                   "WHERE tp.status = 'Parkir' ORDER BY tp.waktu_masuk";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        conn.Open();
                        using (MySqlDataReader r = cmd.ExecuteReader())
                        {
                            int no = 1;
                            while (r.Read())
                            {
                                string namaJenis = r.GetString(0);
                                string platNomor = r.GetString(1);
                                DateTime masuk = r.GetDateTime(2);
                                decimal tarifPerJam = r.GetDecimal(3);

                                grid.Rows.Add(
                                    no++,
                                    namaJenis,
                                    platNomor,
                                    masuk.ToString("dd/MM/yyyy HH:mm"),
                                    "Rp " + tarifPerJam.ToString("N0") + " /jam"); // Menampilkan tarif dasar sebagai estimasi
                            }
                        }
                    }
                }

                // Menjaga agar tampilan minimal ada 4 baris kosong (untuk desain)
                while (grid.Rows.Count < 4)
                {
                    grid.Rows.Add("", "", "", "", "");
                }
            }
            catch (Exception ex)
            {
                // Jangan munculkan pesan error terus menerus saat form loading jika XAMPP mati
                Console.WriteLine(ex.Message);
            }
        }
    }
}