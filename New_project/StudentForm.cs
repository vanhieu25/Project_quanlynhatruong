using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace New_project
{
    public partial class StudentForm : Form
    {
        private readonly DatabaseHelper dbHelper = new DatabaseHelper();
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
            contextMenu.Items.AddRange(new ToolStripItem[] {
        CreateMenuItem("Thêm mới", (s, e) => AddNewStudent()),
                new ToolStripSeparator(),
      CreateMenuItem("Chỉnh sửa", (s, e) => HandleEdit(), true),
       CreateMenuItem("Xóa", (s, e) => HandleDelete(), true)
  });

            dataGridView1.ContextMenuStrip = contextMenu;
            contextMenu.Opening += (s, e) =>
            {
        bool hasSelection = dataGridView1.SelectedRows.Count > 0;
    contextMenu.Items[2].Enabled = hasSelection;
    contextMenu.Items[3].Enabled = hasSelection;
     };
        }

        private ToolStripMenuItem CreateMenuItem(string text, EventHandler onClick, bool needsSelection = false)
        {
            var item = new ToolStripMenuItem(text);
       item.Click += onClick;
 return item;
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
  if (e.RowIndex < 0 || e.ColumnIndex > 1) return;

     string masv = dataGridView1.Rows[e.RowIndex].Cells["colStudentID"].Value?.ToString();
            if (string.IsNullOrEmpty(masv)) return;

            if (e.ColumnIndex == 0) EditStudent(masv);
      else if (e.ColumnIndex == 1) DeleteStudent(masv);
        }

 private void HandleEdit()
        {
            if (dataGridView1.SelectedRows.Count == 0) return;
  string masv = dataGridView1.SelectedRows[0].Cells["colStudentID"].Value?.ToString();
        if (!string.IsNullOrEmpty(masv)) EditStudent(masv);
      }

        private void HandleDelete()
  {
if (dataGridView1.SelectedRows.Count == 0) return;
            string masv = dataGridView1.SelectedRows[0].Cells["colStudentID"].Value?.ToString();
    if (!string.IsNullOrEmpty(masv)) DeleteStudent(masv);
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

      // Add button columns
  dataGridView1.Columns.Add(CreateButtonColumn("Sửa", Color.FromArgb(59, 130, 246)));
      dataGridView1.Columns.Add(CreateButtonColumn("Xóa", Color.FromArgb(239, 68, 68)));

   // Add data columns
       string[] headers = { "Mã SV", "Họ và tên", "Lớp", "Ngành", "Điểm TB", "Giới tính", "Điện thoại", "GVHD" };
 int[] widths = { 100, 200, 120, 180, 80, 80, 120, 150 };
    
     for (int i = 0; i < headers.Length; i++)
         {
   string colName = i == 0 ? "colStudentID" : $"col{headers[i].Replace(" ", "")}";
           int index = dataGridView1.Columns.Add(colName, headers[i]);
     dataGridView1.Columns[index].Width = widths[i];
    }

            dataGridView1.Rows.Clear();
        DataTable dt = dbHelper.ExecuteStoredProcedure("sp_GetAllStudents");

            foreach (DataRow row in dt.Rows)
        {
  dataGridView1.Rows.Add(null, null,
    row["masv"], row["hoten"], row["lophoc"], row["chuyennganh"],
    row["diemtb"] != DBNull.Value ? Convert.ToDouble(row["diemtb"]).ToString("0.00") : "N/A",
          row["gioitinh"], row["dienthoai"], row["hotengv"]);
     }
        }

        private DataGridViewButtonColumn CreateButtonColumn(string text, Color bgColor)
   {
            return new DataGridViewButtonColumn
            {
       HeaderText = text,
 Text = text,
                UseColumnTextForButtonValue = true,
     Width = 80,
        DefaultCellStyle = { BackColor = bgColor, ForeColor = Color.White, Font = new Font("Segoe UI", 9F, FontStyle.Bold) }
     };
        }

    private void EditStudent(string masv)
{
     SqlParameter[] parameters = { new SqlParameter("@masv", masv) };
   DataTable dt = dbHelper.ExecuteStoredProcedure("sp_GetStudentById", parameters);
        if (dt.Rows.Count == 0) return;

            ShowStudentForm(dt.Rows[0], false);
        }

        private void DeleteStudent(string masv)
{
     if (MessageBox.Show($"Bạn có chắc chắn muốn xóa sinh viên mã {masv}?\n\nLưu ý: Dữ liệu sẽ bị xóa vĩnh viễn và không thể khôi phục!",
       "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
    return;

            SqlParameter[] parameters = { new SqlParameter("@masv", masv) };
            if (dbHelper.ExecuteNonQuery("sp_DeleteStudent", parameters))
  {
    MessageBox.Show("Xóa sinh viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
    LoadStudentData();
          panelChartArea.Invalidate();
       }
    }

        private void BtnAddStudent_Click(object sender, EventArgs e) => AddNewStudent();

        private void AddNewStudent()
        {
      ShowStudentForm(null, true);
    }

  private void ShowStudentForm(DataRow existingData, bool isNew)
        {
     DataTable dtAdvisors = dbHelper.ExecuteStoredProcedure("sp_GetAllAdvisors");
      Form form = CreateBaseForm(isNew ? "Thêm sinh viên mới" : "Chỉnh sửa thông tin sinh viên", 650);
     Panel mainPanel = CreateMainPanel(form);

         var controls = new {
        txtMasv = CreateTextBox(mainPanel, "Mã sinh viên:", 0, existingData?["masv"].ToString(), !isNew),
           txtHoten = CreateTextBox(mainPanel, "Họ và tên:", 45, existingData?["hoten"].ToString()),
      dtpNgaysinh = CreateDatePicker(mainPanel, "Ngày sinh:", 90, existingData != null ? Convert.ToDateTime(existingData["ngaysinh"]) : DateTime.Now),
     txtDienthoai = CreateTextBox(mainPanel, "Số điện thoại:", 135, existingData?["dienthoai"].ToString()),
            txtEmail = CreateTextBox(mainPanel, "Email:", 180, existingData?["email"].ToString()),
          cboGioitinh = CreateComboBox(mainPanel, "Giới tính:", 225, new[] { "Nam", "Nữ" }, existingData?["gioitinh"].ToString() ?? "Nam"),
txtLophoc = CreateTextBox(mainPanel, "Lớp học:", 270, existingData?["lophoc"].ToString()),
 txtChuyennganh = CreateTextBox(mainPanel, "Chuyên ngành:", 315, existingData?["chuyennganh"].ToString()),
    txtBacdaotao = CreateTextBox(mainPanel, "Bậc đào tạo:", 360, existingData?["bacdaotao"].ToString() ?? "Đại học"),
    txtDiemtb = CreateTextBox(mainPanel, "Điểm TB:", 405, existingData?["diemtb"] != DBNull.Value && existingData != null ? existingData["diemtb"].ToString() : ""),
  cboGVHD = CreateAdvisorComboBox(mainPanel, "Giảng viên HD:", 450, dtAdvisors, existingData?["magvhd"]?.ToString())
      };

            CreateButtons(mainPanel, 515, isNew ? "💾 Thêm mới" : "💾 Lưu thay đổi", () =>
            {
       if (string.IsNullOrWhiteSpace(controls.txtMasv.Text) || string.IsNullOrWhiteSpace(controls.txtHoten.Text))
    {
          MessageBox.Show(isNew ? "Vui lòng nhập đầy đủ Mã SV và Họ tên!" : "Vui lòng nhập họ tên!", 
   "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      return;
    }

         string magvhd = controls.cboGVHD.SelectedItem?.ToString().Split('-')[0].Trim();
       SqlParameter[] parameters = {
  new SqlParameter("@masv", controls.txtMasv.Text.Trim()),
      new SqlParameter("@hoten", controls.txtHoten.Text.Trim()),
         new SqlParameter("@ngaysinh", controls.dtpNgaysinh.Value),
             new SqlParameter("@gioitinh", controls.cboGioitinh.SelectedItem?.ToString() ?? "Nam"),
     new SqlParameter("@dienthoai", controls.txtDienthoai.Text.Trim()),
       new SqlParameter("@email", controls.txtEmail.Text.Trim()),
             new SqlParameter("@lophoc", controls.txtLophoc.Text.Trim()),
     new SqlParameter("@chuyennganh", controls.txtChuyennganh.Text.Trim()),
         new SqlParameter("@bacdaotao", controls.txtBacdaotao.Text.Trim()),
          new SqlParameter("@diemtb", string.IsNullOrWhiteSpace(controls.txtDiemtb.Text) ? (object)DBNull.Value : Convert.ToDouble(controls.txtDiemtb.Text)),
  new SqlParameter("@magvhd", magvhd ?? (object)DBNull.Value)
          };

              string procedure = isNew ? "sp_InsertStudent" : "sp_UpdateStudent";
            if (dbHelper.ExecuteNonQuery(procedure, parameters))
          {
     MessageBox.Show(isNew ? "Thêm sinh viên mới thành công!" : "Cập nhật thông tin sinh viên thành công!", 
   "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
  form.Close();
    LoadStudentData();
            panelChartArea.Invalidate();
         }
  }, form);

    form.ShowDialog();
     }

        // Helper methods for creating form controls
        private Form CreateBaseForm(string title, int height = 500)
  {
            return new Form
         {
    Text = title,
       Size = new Size(550, height),
       StartPosition = FormStartPosition.CenterParent,
    FormBorderStyle = FormBorderStyle.FixedDialog,
        MaximizeBox = false,
        MinimizeBox = false,
    BackColor = Color.White
            };
        }

    private Panel CreateMainPanel(Form form)
     {
        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(30, 20, 30, 20), AutoScroll = true };
 form.Controls.Add(panel);
   return panel;
}

        private TextBox CreateTextBox(Panel parent, string label, int yPos, string value = "", bool readOnly = false)
        {
        parent.Controls.Add(new Label
  {
         Text = label,
    Left = 0,
          Top = yPos,
          Width = 130,
       Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(55, 65, 81)
            });

 var txt = new TextBox
      {
      Left = 140,
    Top = yPos,
       Width = 320,
                Font = new Font("Segoe UI", 10F),
    Text = value ?? "",
            ReadOnly = readOnly,
                BackColor = readOnly ? Color.FromArgb(243, 244, 246) : Color.White
    };
            parent.Controls.Add(txt);
            return txt;
        }

        private DateTimePicker CreateDatePicker(Panel parent, string label, int yPos, DateTime value)
        {
          parent.Controls.Add(new Label
            {
        Text = label,
      Left = 0,
 Top = yPos,
    Width = 130,
     Font = new Font("Segoe UI", 10F, FontStyle.Bold),
       ForeColor = Color.FromArgb(55, 65, 81)
            });

     var dtp = new DateTimePicker
 {
     Left = 140,
      Top = yPos,
     Width = 320,
                Font = new Font("Segoe UI", 10F),
      Format = DateTimePickerFormat.Short,
        Value = value
            };
    parent.Controls.Add(dtp);
        return dtp;
        }

        private ComboBox CreateComboBox(Panel parent, string label, int yPos, string[] items, string selected)
        {
        parent.Controls.Add(new Label
            {
           Text = label,
 Left = 0,
          Top = yPos,
      Width = 130,
           Font = new Font("Segoe UI", 10F, FontStyle.Bold),
    ForeColor = Color.FromArgb(55, 65, 81)
            });

    var cbo = new ComboBox
{
        Left = 140,
          Top = yPos,
        Width = 320,
                DropDownStyle = ComboBoxStyle.DropDownList,
      Font = new Font("Segoe UI", 10F)
     };
     cbo.Items.AddRange(items);
            cbo.SelectedItem = selected;
  parent.Controls.Add(cbo);
   return cbo;
        }

 private ComboBox CreateAdvisorComboBox(Panel parent, string label, int yPos, DataTable advisors, string selectedMagv)
     {
            parent.Controls.Add(new Label
            {
        Text = label,
     Left = 0,
    Top = yPos,
    Width = 130,
       Font = new Font("Segoe UI", 10F, FontStyle.Bold),
          ForeColor = Color.FromArgb(55, 65, 81)
            });

            var cbo = new ComboBox
 {
     Left = 140,
           Top = yPos,
Width = 320,
         DropDownStyle = ComboBoxStyle.DropDownList,
       Font = new Font("Segoe UI", 10F)
         };

         foreach (DataRow row in advisors.Rows)
          cbo.Items.Add($"{row["magv"]} - {row["hotengv"]}");

            if (!string.IsNullOrEmpty(selectedMagv))
    {
  for (int i = 0; i < cbo.Items.Count; i++)
           {
             if (cbo.Items[i].ToString().StartsWith(selectedMagv))
         {
 cbo.SelectedIndex = i;
  break;
    }
        }
  }
            else if (cbo.Items.Count > 0)
cbo.SelectedIndex = 0;

            parent.Controls.Add(cbo);
return cbo;
        }

   private void CreateButtons(Panel parent, int yPos, string saveText, Action saveAction, Form form)
        {
            var buttonPanel = new Panel { Left = 0, Top = yPos, Width = 460, Height = 45 };
         parent.Controls.Add(buttonPanel);

            var btnSave = new Button
    {
   Text = saveText,
  Width = 150,
Height = 40,
      Left = 75,
           BackColor = Color.FromArgb(34, 197, 94),
   ForeColor = Color.White,
          Font = new Font("Segoe UI", 10F, FontStyle.Bold),
FlatStyle = FlatStyle.Flat,
           Cursor = Cursors.Hand
       };
            btnSave.FlatAppearance.BorderSize = 0;
         btnSave.Click += (s, e) => saveAction();

         var btnCancel = new Button
         {
        Text = "❌ Hủy bỏ",
       Width = 150,
        Height = 40,
Left = 235,
      BackColor = Color.FromArgb(107, 114, 128),
     ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
 FlatStyle = FlatStyle.Flat,
      Cursor = Cursors.Hand
            };
        btnCancel.FlatAppearance.BorderSize = 0;
      btnCancel.Click += (s, e) => form.Close();

            buttonPanel.Controls.AddRange(new Control[] { btnSave, btnCancel });
        }

 private void btnHome_Click(object sender, EventArgs e)
        {
   new MainForm().Show();
            this.Hide();
        }

        private void btnTeacherInfo_Click(object sender, EventArgs e)
      {
  new TeacherForm().Show();
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

        DataTable dt = dbHelper.ExecuteStoredProcedure("sp_GetStudentGPAStats");
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

            string[] gpaLabels = { "< 2.0\nYếu", "2.0-2.5\nTrung bình", "2.5-3.2\nKhá", "3.2-3.6\nGiỏi", "3.6-4.0\nXuất sắc" };
    Color[] colorsStart = {
    Color.FromArgb(239, 68, 68), Color.FromArgb(249, 115, 22),
       Color.FromArgb(234, 179, 8), Color.FromArgb(34, 197, 94), Color.FromArgb(16, 185, 129)
            };
     Color[] colorsEnd = {
    Color.FromArgb(220, 38, 38), Color.FromArgb(234, 88, 12),
      Color.FromArgb(202, 138, 4), Color.FromArgb(22, 163, 74), Color.FromArgb(5, 150, 105)
        };

            DrawChart(g, bounds, gpaLabels, studentCounts, colorsStart, colorsEnd,
  $"Tổng số sinh viên: {studentCount} người  |  GPA trung bình: {avgGPA:F2}");
        }

  private void DrawChart(Graphics g, Rectangle bounds, string[] labels, int[] counts, 
            Color[] colorsStart, Color[] colorsEnd, string legendText)
        {
            int margin = 60, chartWidth = Math.Max(100, bounds.Width - 2 * margin);
            int chartHeight = Math.Max(100, bounds.Height - 2 * margin - 80);
            int barWidth = Math.Max(10, chartWidth / (labels.Length * 2));
      int maxCount = counts.Max() > 0 ? counts.Max() + 2 : 6;

     using (Font titleFont = new Font("Segoe UI", 10F, FontStyle.Bold))
            using (Brush titleBrush = new SolidBrush(Color.FromArgb(16, 185, 129)))
      using (Font labelFont = new Font("Segoe UI", 9F))
    using (Brush labelBrush = new SolidBrush(Color.FromArgb(107, 114, 128)))
  using (Font countFont = new Font("Segoe UI", 9F, FontStyle.Bold))
            using (Pen gridPen = new Pen(Color.FromArgb(229, 231, 235), 1))
            using (Pen axisPen = new Pen(Color.FromArgb(156, 163, 175), 2))
            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
   {
  g.DrawString("Phân bổ sinh viên theo khoảng GPA", titleFont, titleBrush, margin, 10);

        // Draw grid
      for (int i = 0; i <= 6; i++)
      {
       float y = bounds.Bottom - margin - 60 - (chartHeight * i / 6f);
      if (y >= 30)
         {
      g.DrawString(((int)(maxCount * i / 6f)).ToString(), labelFont, labelBrush, margin - 35, y - 8);
         g.DrawLine(gridPen, margin, y, bounds.Right - margin, y);
          }
       }

           // Draw bars
                for (int i = 0; i < labels.Length; i++)
       {
         int x = margin + (chartWidth / labels.Length) * i + barWidth / 2;
          int bottomY = bounds.Bottom - margin - 60;
              int barHeight = maxCount > 0 ? Math.Max(0, (int)(counts[i] / (float)maxCount * chartHeight)) : 0;

              if (barHeight > 0)
     {
            Rectangle bar = new Rectangle(x, bottomY - barHeight, barWidth, barHeight);
               using (LinearGradientBrush barBrush = new LinearGradientBrush(
     new Point(bar.X, bar.Y), new Point(bar.X, bar.Bottom), colorsStart[i], colorsEnd[i]))
     {
            g.FillRectangle(barBrush, bar);
      }

   SizeF textSize = g.MeasureString(counts[i].ToString(), countFont);
        g.DrawString(counts[i].ToString(), countFont, Brushes.Black,
   x + barWidth / 2 - textSize.Width / 2, bottomY - barHeight - 20);
  }

          string[] labelParts = labels[i].Split('\n');
       g.DrawString(labelParts[0], labelFont, labelBrush, x + barWidth / 2, bottomY + 15, sf);
          using (Font smallFont = new Font("Segoe UI", 8F))
    {
              g.DrawString(labelParts[1], smallFont, labelBrush, x + barWidth / 2, bottomY + 32, sf);
           }
     }

             g.DrawLine(axisPen, margin, bounds.Bottom - margin - 60, bounds.Right - margin, bounds.Bottom - margin - 60);
         using (Font legendFont = new Font("Segoe UI", 8F))
     {
    g.DrawString(legendText, legendFont, labelBrush, margin, bounds.Bottom - 30);
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
