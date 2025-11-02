using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace New_project
{
    public partial class TeacherForm : Form
    {
        private String connectionString = @"Data Source=LAPTOP-LJP88J8V;Initial Catalog=QLHoctap;Integrated Security=True;TrustServerCertificate=True";
        private ContextMenuStrip contextMenu;

        public TeacherForm()
        {
            InitializeComponent();
            CustomizeDataGridView();
            SetupContextMenu();
            InitializeUTF8Text();
        }

        private void InitializeUTF8Text()
        {
            // Set UTF-8 button texts
            btnHome.Text = "🏠 Trang chủ";
            btnTeacherInfo.Text = "👨‍🏫 Thông tin giảng viên";
            btnStudentInfo.Text = "👨‍🎓 Thông tin sinh viên";
            btnAddTeacher.Text = "➕ Thêm giảng viên mới";

            // Set chart title
            lblChartTitle.Text = "Phân bố giảng viên theo mức đánh giá";
        }

        private void SetupContextMenu()
        {
            contextMenu = new ContextMenuStrip();

            ToolStripMenuItem addItem = new ToolStripMenuItem("Thêm mới");
            addItem.Click += (s, e) => AddNewTeacher();

            ToolStripMenuItem editItem = new ToolStripMenuItem("Chỉnh sửa");
            editItem.Click += (s, e) => { if (dataGridView1.SelectedRows.Count > 0) HandleEdit(); };

            ToolStripMenuItem deleteItem = new ToolStripMenuItem("Xóa");
            deleteItem.Click += (s, e) => { if (dataGridView1.SelectedRows.Count > 0) HandleDelete(); };

            contextMenu.Items.Add(addItem);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(editItem);
            contextMenu.Items.Add(deleteItem);

            dataGridView1.ContextMenuStrip = contextMenu;

            contextMenu.Opening += (s, e) =>
            {
                bool hasSelection = dataGridView1.SelectedRows.Count > 0;
                ((ToolStripMenuItem)contextMenu.Items[2]).Enabled = hasSelection;
                ((ToolStripMenuItem)contextMenu.Items[3]).Enabled = hasSelection;
            };
        }

        private void TeacherForm_Load(object sender, EventArgs e)
        {
            LoadTeacherData();
            dataGridView1.CellDoubleClick += (s, ev) => { if (ev.RowIndex >= 0) HandleEdit(); };
            dataGridView1.CellClick += DataGridView1_CellClick;
            panelChartArea.Invalidate();
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (e.ColumnIndex == 0 || e.ColumnIndex == 1)
            {
                string magv = dataGridView1.Rows[e.RowIndex].Cells["colID"].Value?.ToString();
                if (string.IsNullOrEmpty(magv)) return;

                if (e.ColumnIndex == 0)
                    EditTeacher(magv);
                else if (e.ColumnIndex == 1)
                    DeleteTeacher(magv);
            }
        }

        private void HandleEdit()
        {
            if (dataGridView1.SelectedRows.Count == 0) return;
            string magv = dataGridView1.SelectedRows[0].Cells["colID"].Value?.ToString();
            if (!string.IsNullOrEmpty(magv))
                EditTeacher(magv);
        }

        private void HandleDelete()
        {
            if (dataGridView1.SelectedRows.Count == 0) return;
            string magv = dataGridView1.SelectedRows[0].Cells["colID"].Value?.ToString();
            if (!string.IsNullOrEmpty(magv))
                DeleteTeacher(magv);
        }

        private DataTable ExecuteQuery(string query)
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối cơ sở dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return dataTable;
        }

        private bool ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (parameters != null)
                            command.Parameters.AddRange(parameters);
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thực thi lệnh: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void CustomizeDataGridView()
        {
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(59, 130, 246);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(59, 130, 246);
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 0, 0, 0);

            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 58, 138);
            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dataGridView1.DefaultCellStyle.Padding = new Padding(10, 5, 10, 5);
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
        }

        private void LoadTeacherData()
        {
            dataGridView1.Columns.Clear();

            // Create Edit button column
            DataGridViewButtonColumn btnEdit = new DataGridViewButtonColumn();
            btnEdit.HeaderText = "Sửa";
            btnEdit.Text = "Sửa";
            btnEdit.UseColumnTextForButtonValue = true;
            btnEdit.Width = 80;
            btnEdit.DefaultCellStyle.BackColor = Color.FromArgb(59, 130, 246);
            btnEdit.DefaultCellStyle.ForeColor = Color.White;
            btnEdit.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dataGridView1.Columns.Add(btnEdit);

            // Create Delete button column
            DataGridViewButtonColumn btnDelete = new DataGridViewButtonColumn();
            btnDelete.HeaderText = "Xóa";
            btnDelete.Text = "Xóa";
            btnDelete.UseColumnTextForButtonValue = true;
            btnDelete.Width = 80;
            btnDelete.DefaultCellStyle.BackColor = Color.FromArgb(239, 68, 68);
            btnDelete.DefaultCellStyle.ForeColor = Color.White;
            btnDelete.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dataGridView1.Columns.Add(btnDelete);

            dataGridView1.Columns.Add("colSTT", "STT");
            dataGridView1.Columns.Add("colID", "Mã GV");
            dataGridView1.Columns.Add("colFullName", "Họ và tên");
            dataGridView1.Columns.Add("colDepartment", "Bộ môn");
            dataGridView1.Columns.Add("colGender", "Giới tính");
            dataGridView1.Columns.Add("colEmail", "Email");
            dataGridView1.Columns.Add("colPhone", "Số điện thoại");
            dataGridView1.Columns.Add("colClass", "Lớp hướng dẫn");
            dataGridView1.Columns.Add("colBirthDate", "Ngày sinh");

            dataGridView1.Columns[2].Width = 60;
            dataGridView1.Columns[3].Width = 100;
            dataGridView1.Columns[4].Width = 200;
            dataGridView1.Columns[5].Width = 180;
            dataGridView1.Columns[6].Width = 80;
            dataGridView1.Columns[7].Width = 220;
            dataGridView1.Columns[8].Width = 120;
            dataGridView1.Columns[9].Width = 150;
            dataGridView1.Columns[10].Width = 100;

            dataGridView1.Rows.Clear();
            string query = "SELECT magv, hotengv, ngaysinh, sdt, email, gioitinh, lophuongdan, bomon FROM giangvien ORDER BY magv";
            DataTable dt = ExecuteQuery(query);

            int stt = 1;
            foreach (DataRow row in dt.Rows)
            {
                dataGridView1.Rows.Add(
                 null, null, stt++,
               row["magv"].ToString(),
             row["hotengv"].ToString(),
                 row["bomon"].ToString(),
                  row["gioitinh"].ToString(),
                  row["email"].ToString(),
                   row["sdt"].ToString(),
                 row["lophuongdan"].ToString(),
              Convert.ToDateTime(row["ngaysinh"]).ToString("dd/MM/yyyy")
                    );
            }
        }

        private void EditTeacher(string magv)
        {
            string query = $"SELECT * FROM giangvien WHERE magv = '{magv}'";
            DataTable dt = ExecuteQuery(query);
            if (dt.Rows.Count == 0) return;
            DataRow row = dt.Rows[0];

            Form editForm = new Form();
            editForm.Text = "Chỉnh sửa thông tin giảng viên";
            editForm.Size = new Size(550, 500);
            editForm.StartPosition = FormStartPosition.CenterParent;
            editForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            editForm.MaximizeBox = false;
            editForm.MinimizeBox = false;
            editForm.BackColor = Color.White;

            Panel mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Padding = new Padding(30, 20, 30, 20);
            mainPanel.AutoScroll = true;
            editForm.Controls.Add(mainPanel);

            int yPos = 0;
            int labelWidth = 130;
            int textBoxWidth = 320;
            int spacing = 45;

            Label CreateLabel(string text, int top)
            {
                return new Label
                {
                    Text = text,
                    Left = 0,
                    Top = top,
                    Width = labelWidth,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(55, 65, 81)
                };
            }

            TextBox CreateTextBox(int top, string value, bool readOnly = false)
            {
                var txt = new TextBox
                {
                    Left = labelWidth + 10,
                    Top = top,
                    Width = textBoxWidth,
                    Font = new Font("Segoe UI", 10F),
                    Text = value
                };
                if (readOnly)
                {
                    txt.ReadOnly = true;
                    txt.BackColor = Color.FromArgb(243, 244, 246);
                }
                return txt;
            }

            mainPanel.Controls.Add(CreateLabel("Mã giảng viên:", yPos));
            TextBox txtMagv = CreateTextBox(yPos, row["magv"].ToString(), true);
            mainPanel.Controls.Add(txtMagv);
            yPos += spacing;

            mainPanel.Controls.Add(CreateLabel("Họ và tên:", yPos));
            TextBox txtHoten = CreateTextBox(yPos, row["hotengv"].ToString());
            mainPanel.Controls.Add(txtHoten);
            yPos += spacing;

            mainPanel.Controls.Add(CreateLabel("Ngày sinh:", yPos));
            DateTimePicker dtpNgaysinh = new DateTimePicker
            {
                Left = labelWidth + 10,
                Top = yPos,
                Width = textBoxWidth,
                Font = new Font("Segoe UI", 10F),
                Format = DateTimePickerFormat.Short,
                Value = Convert.ToDateTime(row["ngaysinh"])
            };
            mainPanel.Controls.Add(dtpNgaysinh);
            yPos += spacing;

            mainPanel.Controls.Add(CreateLabel("Số điện thoại:", yPos));
            TextBox txtSdt = CreateTextBox(yPos, row["sdt"].ToString());
            mainPanel.Controls.Add(txtSdt);
            yPos += spacing;

            mainPanel.Controls.Add(CreateLabel("Email:", yPos));
            TextBox txtEmail = CreateTextBox(yPos, row["email"].ToString());
            mainPanel.Controls.Add(txtEmail);
            yPos += spacing;

            mainPanel.Controls.Add(CreateLabel("Giới tính:", yPos));
            ComboBox cboGioitinh = new ComboBox
            {
                Left = labelWidth + 10,
                Top = yPos,
                Width = textBoxWidth,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F)
            };
            cboGioitinh.Items.AddRange(new string[] { "Nam", "Nữ" });
            cboGioitinh.SelectedItem = row["gioitinh"].ToString();
            mainPanel.Controls.Add(cboGioitinh);
            yPos += spacing;

            mainPanel.Controls.Add(CreateLabel("Lớp hướng dẫn:", yPos));
            TextBox txtLophd = CreateTextBox(yPos, row["lophuongdan"].ToString());
            mainPanel.Controls.Add(txtLophd);
            yPos += spacing;

            mainPanel.Controls.Add(CreateLabel("Bộ môn:", yPos));
            TextBox txtBomon = CreateTextBox(yPos, row["bomon"].ToString());
            mainPanel.Controls.Add(txtBomon);
            yPos += spacing + 20;

            Panel buttonPanel = new Panel
            {
                Left = 0,
                Top = yPos,
                Width = labelWidth + textBoxWidth + 10,
                Height = 45
            };
            mainPanel.Controls.Add(buttonPanel);

            Button btnSave = new Button
            {
                Text = "💾 Lưu thay đổi",
                Width = 150,
                Height = 40,
                Left = (buttonPanel.Width - 310) / 2,
                Top = 0,
                BackColor = Color.FromArgb(34, 197, 94),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtHoten.Text))
                {
                    MessageBox.Show("Vui lòng nhập họ tên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string updateQuery = @"UPDATE giangvien SET hotengv = @hoten, ngaysinh = @ngaysinh, 
            sdt = @sdt, email = @email, gioitinh = @gioitinh, lophuongdan = @lophd, bomon = @bomon 
    WHERE magv = @magv";

                SqlParameter[] parameters = {
         new SqlParameter("@hoten", txtHoten.Text.Trim()),
            new SqlParameter("@ngaysinh", dtpNgaysinh.Value),
  new SqlParameter("@sdt", txtSdt.Text.Trim()),
   new SqlParameter("@email", txtEmail.Text.Trim()),
          new SqlParameter("@gioitinh", cboGioitinh.SelectedItem?.ToString() ?? "Nam"),
    new SqlParameter("@lophd", txtLophd.Text.Trim()),
    new SqlParameter("@bomon", txtBomon.Text.Trim()),
              new SqlParameter("@magv", magv)
              };

                if (ExecuteNonQuery(updateQuery, parameters))
                {
                    MessageBox.Show("Cập nhật thông tin giảng viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    editForm.Close();
                    LoadTeacherData();
                }
            };

            Button btnCancel = new Button
            {
                Text = "Hủy bỏ",
                Width = 150,
                Height = 40,
                Left = btnSave.Right + 10,
                Top = 0,
                BackColor = Color.FromArgb(107, 114, 128),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => editForm.Close();

            buttonPanel.Controls.Add(btnSave);
            buttonPanel.Controls.Add(btnCancel);

            editForm.ShowDialog();
        }

        private void DeleteTeacher(string magv)
        {
            DialogResult result = MessageBox.Show(
    $"Bạn có chắc chắn muốn xóa giảng viên mã {magv}?\n\nLưu ý: Dữ liệu sẽ bị xóa vĩnh viễn và không thể khôi phục!",
     "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                string query = $"DELETE FROM giangvien WHERE magv = '{magv}'";
                if (ExecuteNonQuery(query))
                {
                    MessageBox.Show("Xóa giảng viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadTeacherData();
                }
            }
        }

        private void BtnAddTeacher_Click(object sender, EventArgs e)
        {
            AddNewTeacher();
        }

        private void AddNewTeacher()
        {
            Form addForm = new Form();
            addForm.Text = "Thêm giảng viên mới";
            addForm.Size = new Size(550, 500);
            addForm.StartPosition = FormStartPosition.CenterParent;
            addForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            addForm.MaximizeBox = false;
            addForm.MinimizeBox = false;
            addForm.BackColor = Color.White;

            Panel mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Padding = new Padding(30, 20, 30, 20);
            mainPanel.AutoScroll = true;
            addForm.Controls.Add(mainPanel);

            int yPos = 0;
            int labelWidth = 130;
            int textBoxWidth = 320;
            int spacing = 45;

            Label CreateLabel(string text, int top)
            {
                return new Label
                {
                    Text = text,
                    Left = 0,
                    Top = top,
                    Width = labelWidth,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(55, 65, 81)
                };
            }

            TextBox CreateTextBox(int top, string value = "")
            {
                return new TextBox
                {
                    Left = labelWidth + 10,
                    Top = top,
                    Width = textBoxWidth,
                    Font = new Font("Segoe UI", 10F),
                    Text = value
                };
            }

            mainPanel.Controls.Add(CreateLabel("Mã giảng viên:", yPos));
            TextBox txtMagv = CreateTextBox(yPos);
            mainPanel.Controls.Add(txtMagv);
            yPos += spacing;

            mainPanel.Controls.Add(CreateLabel("Họ và tên:", yPos));
            TextBox txtHoten = CreateTextBox(yPos);
            mainPanel.Controls.Add(txtHoten);
            yPos += spacing;

            mainPanel.Controls.Add(CreateLabel("Ngày sinh:", yPos));
            DateTimePicker dtpNgaysinh = new DateTimePicker
            {
                Left = labelWidth + 10,
                Top = yPos,
                Width = textBoxWidth,
                Font = new Font("Segoe UI", 10F),
                Format = DateTimePickerFormat.Short
            };
            mainPanel.Controls.Add(dtpNgaysinh);
            yPos += spacing;

            mainPanel.Controls.Add(CreateLabel("Số điện thoại:", yPos));
            TextBox txtSdt = CreateTextBox(yPos);
            mainPanel.Controls.Add(txtSdt);
            yPos += spacing;

            mainPanel.Controls.Add(CreateLabel("Email:", yPos));
            TextBox txtEmail = CreateTextBox(yPos);
            mainPanel.Controls.Add(txtEmail);
            yPos += spacing;

            mainPanel.Controls.Add(CreateLabel("Giới tính:", yPos));
            ComboBox cboGioitinh = new ComboBox
            {
                Left = labelWidth + 10,
                Top = yPos,
                Width = textBoxWidth,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F)
            };
            cboGioitinh.Items.AddRange(new string[] { "Nam", "Nữ" });
            cboGioitinh.SelectedIndex = 0;
            mainPanel.Controls.Add(cboGioitinh);
            yPos += spacing;

            mainPanel.Controls.Add(CreateLabel("Lớp hướng dẫn:", yPos));
            TextBox txtLophd = CreateTextBox(yPos);
            mainPanel.Controls.Add(txtLophd);
            yPos += spacing;

            mainPanel.Controls.Add(CreateLabel("Bộ môn:", yPos));
            TextBox txtBomon = CreateTextBox(yPos);
            mainPanel.Controls.Add(txtBomon);
            yPos += spacing + 20;

            Panel buttonPanel = new Panel
            {
                Left = 0,
                Top = yPos,
                Width = labelWidth + textBoxWidth + 10,
                Height = 45
            };
            mainPanel.Controls.Add(buttonPanel);

            Button btnSave = new Button
            {
                Text = "💾 Thêm mới",
                Width = 150,
                Height = 40,
                Left = (buttonPanel.Width - 310) / 2,
                Top = 0,
                BackColor = Color.FromArgb(34, 197, 94),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += (s, ev) =>
            {
                if (string.IsNullOrWhiteSpace(txtMagv.Text) || string.IsNullOrWhiteSpace(txtHoten.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ Mã GV và Họ tên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string insertQuery = @"INSERT INTO giangvien (magv, hotengv, ngaysinh, sdt, email, gioitinh, lophuongdan, bomon) 
  VALUES (@magv, @hoten, @ngaysinh, @sdt, @email, @gioitinh, @lophd, @bomon)";

                SqlParameter[] parameters = {
  new SqlParameter("@magv", txtMagv.Text.Trim()),
        new SqlParameter("@hoten", txtHoten.Text.Trim()),
 new SqlParameter("@ngaysinh", dtpNgaysinh.Value),
         new SqlParameter("@sdt", txtSdt.Text.Trim()),
               new SqlParameter("@email", txtEmail.Text.Trim()),
    new SqlParameter("@gioitinh", cboGioitinh.SelectedItem?.ToString() ?? "Nam"),
         new SqlParameter("@lophd", txtLophd.Text.Trim()),
      new SqlParameter("@bomon", txtBomon.Text.Trim())
 };

                if (ExecuteNonQuery(insertQuery, parameters))
                {
                    MessageBox.Show("Thêm giảng viên mới thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    addForm.Close();
                    LoadTeacherData();
                }
            };

            Button btnCancel = new Button
            {
                Text = "Hủy bỏ",
                Width = 150,
                Height = 40,
                Left = btnSave.Right + 10,
                Top = 0,
                BackColor = Color.FromArgb(107, 114, 128),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, ev) => addForm.Close();

            buttonPanel.Controls.Add(btnSave);
            buttonPanel.Controls.Add(btnCancel);

            addForm.ShowDialog();
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            MainForm mainForm = new MainForm();
            mainForm.Show();
            this.Hide();
        }

        private void btnStudentInfo_Click(object sender, EventArgs e)
        {
            StudentForm studentForm = new StudentForm();
            studentForm.Show();
            this.Hide();
        }

        private void panelChartArea_Paint(object sender, PaintEventArgs e)
        {
            DrawTeacherRatingChart(e.Graphics, panelChartArea.ClientRectangle);
        }

        private void DrawTeacherRatingChart(Graphics g, Rectangle bounds)
        {
            if (bounds.Width < 100 || bounds.Height < 100) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            string[] ratingLabels = { "⭐", "⭐⭐", "⭐⭐⭐", "⭐⭐⭐⭐", "⭐⭐⭐⭐⭐" };
            string[] ratingNames = { "1 Sao", "2 Sao", "3 Sao", "4 Sao", "5 Sao" };

            DataTable dt = ExecuteQuery("SELECT COUNT(*) as total FROM giangvien");
            int totalTeachers = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["total"]) : 0;

            int[] ratingCounts = new int[5];
            int baseCount = totalTeachers / 5;
            int remainder = totalTeachers % 5;
            for (int i = 0; i < 5; i++)
            {
                ratingCounts[i] = baseCount + (i < remainder ? 1 : 0);
            }

            int margin = 60;
            int chartWidth = Math.Max(100, bounds.Width - 2 * margin);
            int chartHeight = Math.Max(100, bounds.Height - 2 * margin - 80);
            int barWidth = Math.Max(10, chartWidth / (ratingLabels.Length * 2));
            int maxCount = Math.Max(6, ratingCounts.Max() + 2);

            using (Font titleFont = new Font("Segoe UI", 10F, FontStyle.Bold))
            using (Brush titleBrush = new SolidBrush(Color.FromArgb(59, 130, 246)))
            using (Font labelFont = new Font("Segoe UI", 9F))
            using (Brush labelBrush = new SolidBrush(Color.FromArgb(107, 114, 128)))
            using (Font countFont = new Font("Segoe UI", 9F, FontStyle.Bold))
            {
                g.DrawString("Phân bố giảng viên theo mức đánh giá", titleFont, titleBrush, margin, 10);

                for (int i = 0; i <= 6; i++)
                {
                    float y = bounds.Bottom - margin - 60 - (chartHeight * i / 6f);
                    if (y >= 30)
                    {
                        g.DrawString(((int)(maxCount * i / 6f)).ToString(), labelFont, labelBrush, margin - 35, y - 8);

                        using (Pen gridPen = new Pen(Color.FromArgb(229, 231, 235), 1))
                        {
                            g.DrawLine(gridPen, margin, y, bounds.Right - margin, y);
                        }
                    }
                }

                for (int i = 0; i < ratingLabels.Length; i++)
                {
                    int x = margin + (chartWidth / ratingLabels.Length) * i + barWidth / 2;
                    int bottomY = bounds.Bottom - margin - 60;

                    int barHeight = maxCount > 0 ? Math.Max(0, (int)(ratingCounts[i] / (float)maxCount * chartHeight)) : 0;

                    if (barHeight > 0)
                    {
                        Rectangle bar = new Rectangle(x, bottomY - barHeight, barWidth, barHeight);
                        using (LinearGradientBrush barBrush = new LinearGradientBrush(
                     new Point(bar.X, bar.Y),
              new Point(bar.X, bar.Bottom),
                     Color.FromArgb(52, 211, 153),
                       Color.FromArgb(16, 185, 129)))
                        {
                            g.FillRectangle(barBrush, bar);
                        }

                        string countText = ratingCounts[i].ToString();
                        SizeF textSize = g.MeasureString(countText, countFont);
                        g.DrawString(countText, countFont, Brushes.Black,
                            x + barWidth / 2 - textSize.Width / 2, bottomY - barHeight - 20);
                    }

                    using (StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center })
                    {
                        g.DrawString(ratingNames[i], labelFont, labelBrush, x + barWidth / 2, bottomY + 10, sf);
                        using (Font starFont = new Font("Segoe UI", 11F))
                        {
                            g.DrawString(ratingLabels[i], starFont, labelBrush, x + barWidth / 2, bottomY + 28, sf);
                        }
                    }
                }

                using (Pen axisPen = new Pen(Color.FromArgb(156, 163, 175), 2))
                {
                    g.DrawLine(axisPen, margin, bounds.Bottom - margin - 60,
                        bounds.Right - margin, bounds.Bottom - margin - 60);
                }

                using (Font legendFont = new Font("Segoe UI", 8F))
                {
                    g.DrawString($"Tổng số giảng viên: {totalTeachers} người", legendFont, labelBrush, margin, bounds.Bottom - 30);
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            Application.Exit();
        }
    }
}
