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
    public partial class StudentForm : Form
    {
        private String connectionString = @"Data Source=LAPTOP-LJP88J8V;Initial Catalog=QLHoctap;Integrated Security=True;TrustServerCertificate=True";
        private ContextMenuStrip contextMenu;

        public StudentForm()
        {
            InitializeComponent();
            CustomizeDataGridView();
            SetupContextMenu();
            InitializeUTF8Text();
        }

        private void InitializeUTF8Text()
        {
            // Set UTF-8 button texts with emojis
            btnHome.Text = "🏠 Trang chủ";
            btnTeacherInfo.Text = "👨‍🏫 Thông tin giảng viên";
            btnStudentInfo.Text = "👨‍🎓 Thông tin sinh viên";
            btnAddStudent.Text = "➕ Thêm sinh viên mới";

            // Set chart title
            lblChartTitle.Text = "Phân bổ sinh viên theo khoảng GPA";
        }

        private void SetupContextMenu()
        {
            contextMenu = new ContextMenuStrip();

            ToolStripMenuItem addItem = new ToolStripMenuItem("Thêm mới");
            addItem.Click += (s, e) => AddNewStudent();

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

        private void StudentForm_Load(object sender, EventArgs e)
        {
            LoadStudentData();
            dataGridView1.CellDoubleClick += (s, ev) => { if (ev.RowIndex >= 0) HandleEdit(); };
            dataGridView1.CellClick += DataGridView1_CellClick;
            panelChartArea.Invalidate();
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (e.ColumnIndex == 0 || e.ColumnIndex == 1)
            {
                string masv = dataGridView1.Rows[e.RowIndex].Cells["colStudentID"].Value?.ToString();
                if (string.IsNullOrEmpty(masv)) return;

                if (e.ColumnIndex == 0)
                    EditStudent(masv);
                else if (e.ColumnIndex == 1)
                    DeleteStudent(masv);
            }
        }

        private void HandleEdit()
        {
            if (dataGridView1.SelectedRows.Count == 0) return;
            string masv = dataGridView1.SelectedRows[0].Cells["colStudentID"].Value?.ToString();
            if (!string.IsNullOrEmpty(masv))
                EditStudent(masv);
        }

        private void HandleDelete()
        {
            if (dataGridView1.SelectedRows.Count == 0) return;
            string masv = dataGridView1.SelectedRows[0].Cells["colStudentID"].Value?.ToString();
            if (!string.IsNullOrEmpty(masv))
                DeleteStudent(masv);
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

        private void LoadStudentData()
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

            dataGridView1.Columns.Add("colStudentID", "Mã SV");
            dataGridView1.Columns.Add("colFullName", "Họ và tên");
            dataGridView1.Columns.Add("colClass", "Lớp");
            dataGridView1.Columns.Add("colMajor", "Ngành");
            dataGridView1.Columns.Add("colGPA", "Điểm TB");
            dataGridView1.Columns.Add("colGender", "Giới tính");
            dataGridView1.Columns.Add("colPhone", "Điện thoại");
            dataGridView1.Columns.Add("colAdvisor", "GVHD");

            dataGridView1.Columns[2].Width = 100;
            dataGridView1.Columns[3].Width = 200;
            dataGridView1.Columns[4].Width = 120;
            dataGridView1.Columns[5].Width = 180;
            dataGridView1.Columns[6].Width = 80;
            dataGridView1.Columns[7].Width = 80;
            dataGridView1.Columns[8].Width = 120;
            dataGridView1.Columns[9].Width = 150;

            dataGridView1.Rows.Clear();
            string query = @"SELECT s.masv, s.hoten, s.lophoc, s.chuyennganh, s.diemtb, 
       s.gioitinh, s.dienthoai, g.hotengv
 FROM sinhvien s 
   LEFT JOIN giangvien g ON s.magvhd = g.magv
      ORDER BY s.masv";
            DataTable dt = ExecuteQuery(query);

            foreach (DataRow row in dt.Rows)
            {
                dataGridView1.Rows.Add(
           null, null,
                row["masv"].ToString(),
              row["hoten"].ToString(),
          row["lophoc"].ToString(),
              row["chuyennganh"].ToString(),
            row["diemtb"] != DBNull.Value ? Convert.ToDouble(row["diemtb"]).ToString("0.00") : "N/A",
               row["gioitinh"].ToString(),
              row["dienthoai"].ToString(),
           row["hotengv"].ToString()
            );
            }
        }

        private void EditStudent(string masv)
        {
            string query = $"SELECT * FROM sinhvien WHERE masv = '{masv}'";
            DataTable dt = ExecuteQuery(query);
            if (dt.Rows.Count == 0) return;
            DataRow row = dt.Rows[0];

            DataTable dtAdvisors = ExecuteQuery("SELECT magv, hotengv FROM giangVien ORDER BY magv");

            Form editForm = new Form();
            editForm.Text = "Chỉnh sửa thông tin sinh viên";
            editForm.Size = new Size(550, 650);
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

            mainPanel.Controls.Add(CreateLabel("Mã sinh viên:", yPos));
            TextBox txtMasv = CreateTextBox(yPos, row["masv"].ToString(), true);
            mainPanel.Controls.Add(txtMasv);
            yPos += spacing;

            mainPanel.Controls.Add(CreateLabel("Họ và tên:", yPos));
            TextBox txtHoten = CreateTextBox(yPos, row["hoten"].ToString());
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
            TextBox txtDienthoai = CreateTextBox(yPos, row["dienthoai"].ToString());
            mainPanel.Controls.Add(txtDienthoai);
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

            mainPanel.Controls.Add(CreateLabel("Lớp học:", yPos));
            TextBox txtLophoc = CreateTextBox(yPos, row["lophoc"].ToString());
            mainPanel.Controls.Add(txtLophoc);
            yPos += spacing;

            mainPanel.Controls.Add(CreateLabel("Chuyên ngành:", yPos));
            TextBox txtChuyennganh = CreateTextBox(yPos, row["chuyennganh"].ToString());
            mainPanel.Controls.Add(txtChuyennganh);
            yPos += spacing;

            mainPanel.Controls.Add(CreateLabel("Bậc đào tạo:", yPos));
            TextBox txtBacdaotao = CreateTextBox(yPos, row["bacdaotao"].ToString());
            mainPanel.Controls.Add(txtBacdaotao);
            yPos += spacing;

            mainPanel.Controls.Add(CreateLabel("Điểm TB:", yPos));
            TextBox txtDiemtb = CreateTextBox(yPos, row["diemtb"] != DBNull.Value ? row["diemtb"].ToString() : "");
            mainPanel.Controls.Add(txtDiemtb);
            yPos += spacing;

            mainPanel.Controls.Add(CreateLabel("Giảng viên HD:", yPos));
            ComboBox cboGVHD = new ComboBox
            {
                Left = labelWidth + 10,
                Top = yPos,
                Width = textBoxWidth,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F)
            };
            foreach (DataRow advisorRow in dtAdvisors.Rows)
            {
                cboGVHD.Items.Add($"{advisorRow["magv"]} - {advisorRow["hotengv"]}");
            }
            if (row["magvhd"] != DBNull.Value)
            {
                string currentAdvisor = row["magvhd"].ToString();
                for (int i = 0; i < cboGVHD.Items.Count; i++)
                {
                    if (cboGVHD.Items[i].ToString().StartsWith(currentAdvisor))
                    {
                        cboGVHD.SelectedIndex = i;
                        break;
                    }
                }
            }
            mainPanel.Controls.Add(cboGVHD);
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
            btnSave.Click += (s, ev) =>
            {
                if (string.IsNullOrWhiteSpace(txtHoten.Text))
                {
                    MessageBox.Show("Vui lòng nhập họ tên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string magvhd = cboGVHD.SelectedItem?.ToString().Split('-')[0].Trim();

                string updateQuery = @"UPDATE sinhvien SET hoten = @hoten, ngaysinh = @ngaysinh, 
           gioitinh = @gioitinh, dienthoai = @dienthoai, email = @email, 
       lophoc = @lophoc, chuyennganh = @chuyennganh, bacdaotao = @bacdaotao, 
     diemtb = @diemtb, magvhd = @magvhd 
    WHERE masv = @masv";

                SqlParameter[] parameters = {
     new SqlParameter("@hoten", txtHoten.Text.Trim()),
   new SqlParameter("@ngaysinh", dtpNgaysinh.Value),
        new SqlParameter("@gioitinh", cboGioitinh.SelectedItem?.ToString() ?? "Nam"),
            new SqlParameter("@dienthoai", txtDienthoai.Text.Trim()),
  new SqlParameter("@email", txtEmail.Text.Trim()),
             new SqlParameter("@lophoc", txtLophoc.Text.Trim()),
  new SqlParameter("@chuyennganh", txtChuyennganh.Text.Trim()),
          new SqlParameter("@bacdaotao", txtBacdaotao.Text.Trim()),
        new SqlParameter("@diemtb", string.IsNullOrWhiteSpace(txtDiemtb.Text) ? (object)DBNull.Value : Convert.ToDouble(txtDiemtb.Text)),
        new SqlParameter("@magvhd", magvhd ?? (object)DBNull.Value),
         new SqlParameter("@masv", masv)
     };

                if (ExecuteNonQuery(updateQuery, parameters))
                {
                    MessageBox.Show("Cập nhật thông tin sinh viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    editForm.Close();
                    LoadStudentData();
                    panelChartArea.Invalidate();
                }
            };

            Button btnCancel = new Button
            {
                Text = "❌ Hủy bỏ",
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
            btnCancel.Click += (s, ev) => editForm.Close();

            buttonPanel.Controls.Add(btnSave);
            buttonPanel.Controls.Add(btnCancel);

            editForm.ShowDialog();
        }

        private void DeleteStudent(string masv)
        {
            DialogResult result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa sinh viên mã {masv}?\n\nLưu ý: Dữ liệu sẽ bị xóa vĩnh viễn và không thể khôi phục!",
                     "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                string deleteRelatedQuery = $"DELETE FROM ketqua WHERE masv = '{masv}'";
                ExecuteNonQuery(deleteRelatedQuery);

                string query = $"DELETE FROM sinhvien WHERE masv = '{masv}'";
                if (ExecuteNonQuery(query))
                {
                    MessageBox.Show("Xóa sinh viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadStudentData();
                    panelChartArea.Invalidate();
                }
            }
        }

        private void BtnAddStudent_Click(object sender, EventArgs e)
        {
            AddNewStudent();
        }

        private void AddNewStudent()
        {
            DataTable dtAdvisors = ExecuteQuery("SELECT magv, hotengv FROM giangVien ORDER BY magv");

            Form addForm = new Form();
            addForm.Text = "Thêm sinh viên mới";
            addForm.Size = new Size(550, 650);
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

            mainPanel.Controls.Add(CreateLabel("Mã sinh viên:", yPos));
            TextBox txtMasv = CreateTextBox(yPos);
            mainPanel.Controls.Add(txtMasv);
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

            mainPanel.Controls.Add(CreateLabel("Điện thoại:", yPos));
            TextBox txtDienthoai = CreateTextBox(yPos);
            mainPanel.Controls.Add(txtDienthoai);
            yPos += spacing;

            mainPanel.Controls.Add(CreateLabel("Email:", yPos));
            TextBox txtEmail = CreateTextBox(yPos);
            mainPanel.Controls.Add(txtEmail);
            yPos += spacing;

            mainPanel.Controls.Add(CreateLabel("Lớp học:", yPos));
            TextBox txtLophoc = CreateTextBox(yPos);
            mainPanel.Controls.Add(txtLophoc);
            yPos += spacing;

            mainPanel.Controls.Add(CreateLabel("Chuyên ngành:", yPos));
            TextBox txtChuyennganh = CreateTextBox(yPos);
            mainPanel.Controls.Add(txtChuyennganh);
            yPos += spacing;

            mainPanel.Controls.Add(CreateLabel("Bậc đào tạo:", yPos));
            TextBox txtBacdaotao = CreateTextBox(yPos, "Đại học");
            mainPanel.Controls.Add(txtBacdaotao);
            yPos += spacing;

            mainPanel.Controls.Add(CreateLabel("Điểm TB:", yPos));
            TextBox txtDiemtb = CreateTextBox(yPos);
            mainPanel.Controls.Add(txtDiemtb);
            yPos += spacing;

            mainPanel.Controls.Add(CreateLabel("Giảng viên HD:", yPos));
            ComboBox cboGVHD = new ComboBox
            {
                Left = labelWidth + 10,
                Top = yPos,
                Width = textBoxWidth,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F)
            };
            foreach (DataRow row in dtAdvisors.Rows)
            {
                cboGVHD.Items.Add($"{row["magv"]} - {row["hotengv"]}");
            }
            if (cboGVHD.Items.Count > 0) cboGVHD.SelectedIndex = 0;
            mainPanel.Controls.Add(cboGVHD);
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
                if (string.IsNullOrWhiteSpace(txtMasv.Text) || string.IsNullOrWhiteSpace(txtHoten.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ Mã SV và Họ tên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string magvhd = cboGVHD.SelectedItem?.ToString().Split('-')[0].Trim();

                string insertQuery = @"INSERT INTO sinhvien (masv, hoten, ngaysinh, gioitinh, dienthoai, email, lophoc, chuyennganh, bacdaotao, anhdaidien, diemtb, magvhd) 
               VALUES (@masv, @hoten, @ngaysinh, @gioitinh, @dienthoai, @email, @lophoc, @chuyennganh, @bacdaotao, @anhdaidien, @diemtb, @magvhd)";

                SqlParameter[] parameters = {
        new SqlParameter("@masv", txtMasv.Text.Trim()),
      new SqlParameter("@hoten", txtHoten.Text.Trim()),
         new SqlParameter("@ngaysinh", dtpNgaysinh.Value),
            new SqlParameter("@gioitinh", cboGioitinh.SelectedItem?.ToString() ?? "Nam"),
              new SqlParameter("@dienthoai", txtDienthoai.Text.Trim()),
                new SqlParameter("@email", txtEmail.Text.Trim()),
     new SqlParameter("@lophoc", txtLophoc.Text.Trim()),
  new SqlParameter("@chuyennganh", txtChuyennganh.Text.Trim()),
        new SqlParameter("@bacdaotao", txtBacdaotao.Text.Trim()),
    new SqlParameter("@anhdaidien", DBNull.Value),
             new SqlParameter("@diemtb", string.IsNullOrWhiteSpace(txtDiemtb.Text) ? (object)DBNull.Value : Convert.ToDouble(txtDiemtb.Text)),
  new SqlParameter("@magvhd", magvhd ?? (object)DBNull.Value)
     };

                if (ExecuteNonQuery(insertQuery, parameters))
                {
                    MessageBox.Show("Thêm sinh viên mới thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    addForm.Close();
                    LoadStudentData();
                }
            };

            Button btnCancel = new Button
            {
                Text = "❌ Hủy bỏ",
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

        private void btnTeacherInfo_Click(object sender, EventArgs e)
        {
            TeacherForm teacherForm = new TeacherForm();
            teacherForm.Show();
            this.Hide();
        }

        private void panelChartArea_Paint(object sender, PaintEventArgs e)
        {
            DrawStudentGPAChart(e.Graphics, panelChartArea.ClientRectangle);
        }

        private void DrawStudentGPAChart(Graphics g, Rectangle bounds)
        {
            if (bounds.Width < 100 || bounds.Height < 100) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            string query = "SELECT diemtb FROM sinhvien WHERE diemtb IS NOT NULL";
            DataTable dt = ExecuteQuery(query);

            int[] studentCounts = new int[5];
            double totalGPA = 0;
            int studentCount = 0;

            foreach (DataRow row in dt.Rows)
            {
                double gpa = Convert.ToDouble(row["diemtb"]);
                totalGPA += gpa;
                studentCount++;

                if (gpa < 2.0) studentCounts[0]++;
                else if (gpa < 2.5) studentCounts[1]++;
                else if (gpa < 3.2) studentCounts[2]++;
                else if (gpa < 3.6) studentCounts[3]++;
                else studentCounts[4]++;
            }

            double avgGPA = studentCount > 0 ? totalGPA / studentCount : 0;

            string[] gpaRanges = { "< 2.0\nYếu", "2.0-2.5\nTrung bình", "2.5-3.2\nKhá", "3.2-3.6\nGiỏi", "3.6-4.0\nXuất sắc" };
            string[] gpaLabels = { "< 2.0", "2.0-2.5", "2.5-3.2", "3.2-3.6", "3.6-4.0" };

            int margin = 60;
            int chartWidth = Math.Max(100, bounds.Width - 2 * margin);
            int chartHeight = Math.Max(100, bounds.Height - 2 * margin - 80);
            int barWidth = Math.Max(10, chartWidth / (gpaLabels.Length * 2));
            int maxCount = studentCounts.Max() > 0 ? studentCounts.Max() + 2 : 6;

            Color[] barColorsStart = {
          Color.FromArgb(239, 68, 68),
                Color.FromArgb(249, 115, 22),
    Color.FromArgb(234, 179, 8),
      Color.FromArgb(34, 197, 94),
      Color.FromArgb(16, 185, 129)
   };

            Color[] barColorsEnd = {
                Color.FromArgb(220, 38, 38),
      Color.FromArgb(234, 88, 12),
      Color.FromArgb(202, 138, 4),
    Color.FromArgb(22, 163, 74),
      Color.FromArgb(5, 150, 105)
        };

            using (Font titleFont = new Font("Segoe UI", 10F, FontStyle.Bold))
            using (Brush titleBrush = new SolidBrush(Color.FromArgb(16, 185, 129)))
            using (Font labelFont = new Font("Segoe UI", 9F))
            using (Brush labelBrush = new SolidBrush(Color.FromArgb(107, 114, 128)))
            using (Font countFont = new Font("Segoe UI", 9F, FontStyle.Bold))
            {
                g.DrawString("Phân bổ sinh viên theo khoảng GPA", titleFont, titleBrush, margin, 10);

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

                for (int i = 0; i < gpaLabels.Length; i++)
                {
                    int x = margin + (chartWidth / gpaLabels.Length) * i + barWidth / 2;
                    int bottomY = bounds.Bottom - margin - 60;
                    int barHeight = maxCount > 0 ? Math.Max(0, (int)(studentCounts[i] / (float)maxCount * chartHeight)) : 0;

                    if (barHeight > 0)
                    {
                        Rectangle bar = new Rectangle(x, bottomY - barHeight, barWidth, barHeight);
                        using (LinearGradientBrush barBrush = new LinearGradientBrush(
                           new Point(bar.X, bar.Y),
                                new Point(bar.X, bar.Bottom),
                    barColorsStart[i],
                              barColorsEnd[i]))
                        {
                            g.FillRectangle(barBrush, bar);
                        }

                        string countText = studentCounts[i].ToString();
                        SizeF textSize = g.MeasureString(countText, countFont);
                        g.DrawString(countText, countFont, Brushes.Black,
                     x + barWidth / 2 - textSize.Width / 2, bottomY - barHeight - 20);
                    }

                    using (StringFormat sf = new StringFormat()
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    })
                    {
                        string[] labelParts = gpaRanges[i].Split('\n');
                        g.DrawString(labelParts[0], labelFont, labelBrush, x + barWidth / 2, bottomY + 15, sf);
                        using (Font smallFont = new Font("Segoe UI", 8F))
                        {
                            g.DrawString(labelParts[1], smallFont, labelBrush, x + barWidth / 2, bottomY + 32, sf);
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
                    g.DrawString($"Tổng số sinh viên: {studentCount} người  |  GPA trung bình: {avgGPA:F2}",
                          legendFont, labelBrush, margin, bounds.Bottom - 30);
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
