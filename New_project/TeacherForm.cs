using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace New_project
{
    public partial class TeacherForm : Form
    {
     private readonly DatabaseHelper dbHelper = new DatabaseHelper();

        public TeacherForm()
    {
InitializeComponent();
          CustomizeDataGridView();
      SetupContextMenu();
   InitializeUTF8Text();
}

        private void InitializeUTF8Text()
        {
      btnHome.Text = "🏠 Trang chủ";
            btnTeacherInfo.Text = "👨‍🏫 Thông tin giảng viên";
         btnStudentInfo.Text = "👨‍🎓 Thông tin sinh viên";
            btnAddTeacher.Text = "➕ Thêm giảng viên mới";
    lblChartTitle.Text = "Phân bố giảng viên theo mức đánh giá";
        }

        private void SetupContextMenu()
    {
            contextMenu = new ContextMenuStrip();
          var editItem = new ToolStripMenuItem("Chỉnh sửa");
     var deleteItem = new ToolStripMenuItem("Xóa");
            
   editItem.Click += (s, e) => HandleEdit();
            deleteItem.Click += (s, e) => HandleDelete();
        
            contextMenu.Items.AddRange(new ToolStripItem[] {
        new ToolStripMenuItem("Thêm mới", null, (s, e) => AddNewTeacher()),
       new ToolStripSeparator(),
          editItem,
    deleteItem
            });

       dataGridView1.ContextMenuStrip = contextMenu;
       contextMenu.Opening += (s, e) =>
      {
                bool hasSelection = dataGridView1.SelectedRows.Count > 0;
              editItem.Enabled = deleteItem.Enabled = hasSelection;
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
        if (e.RowIndex < 0 || e.ColumnIndex > 1) return;
  string magv = dataGridView1.Rows[e.RowIndex].Cells["colMãGV"].Value?.ToString();
    if (string.IsNullOrEmpty(magv)) return;
          if (e.ColumnIndex == 0) EditTeacher(magv);
    else if (e.ColumnIndex == 1) DeleteTeacher(magv);
        }

  private void HandleEdit()
        {
            if (dataGridView1.SelectedRows.Count == 0) return;
      string magv = dataGridView1.SelectedRows[0].Cells["colMãGV"].Value?.ToString();
  if (!string.IsNullOrEmpty(magv)) EditTeacher(magv);
  }

        private void HandleDelete()
        {
   if (dataGridView1.SelectedRows.Count == 0) return;
    string magv = dataGridView1.SelectedRows[0].Cells["colMãGV"].Value?.ToString();
         if (!string.IsNullOrEmpty(magv)) DeleteTeacher(magv);
 }

    private void CustomizeDataGridView()
{
            dataGridView1.EnableHeadersVisualStyles = false;
      var headerStyle = dataGridView1.ColumnHeadersDefaultCellStyle;
       headerStyle.BackColor = headerStyle.SelectionBackColor = Color.FromArgb(59, 130, 246);
        headerStyle.ForeColor = Color.White;
            headerStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
      headerStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
      headerStyle.Padding = new Padding(10, 0, 0, 0);
            
            var cellStyle = dataGridView1.DefaultCellStyle;
       cellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
     cellStyle.SelectionForeColor = Color.FromArgb(30, 58, 138);
       cellStyle.Font = new Font("Segoe UI", 9.5F);
         cellStyle.Padding = new Padding(10, 5, 10, 5);
  cellStyle.WrapMode = DataGridViewTriState.True;
         dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
        }

      private void LoadTeacherData()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add(CreateButtonColumn("Sửa", Color.FromArgb(59, 130, 246)));
            dataGridView1.Columns.Add(CreateButtonColumn("Xóa", Color.FromArgb(239, 68, 68)));

     string[] headers = { "STT", "Mã GV", "Họ và tên", "Bộ môn", "Giới tính", "Email", "Số điện thoại", "Lớp hướng dẫn", "Ngày sinh" };
            int[] widths = { 60, 100, 200, 180, 80, 220, 120, 150, 100 };
      
          for (int i = 0; i < headers.Length; i++)
            {
              int index = dataGridView1.Columns.Add($"col{headers[i].Replace(" ", "")}", headers[i]);
       dataGridView1.Columns[index].Width = widths[i];
            }

     dataGridView1.Rows.Clear();
            DataTable dt = dbHelper.ExecuteStoredProcedure("sp_GetAllTeachers");
            int stt = 1;
 foreach (DataRow row in dt.Rows)
            {
dataGridView1.Rows.Add(null, null, stt++, row["magv"], row["hotengv"], row["bomon"], 
           row["gioitinh"], row["email"], row["sdt"], row["lophuongdan"],
            Convert.ToDateTime(row["ngaysinh"]).ToString("dd/MM/yyyy"));
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

        private void EditTeacher(string magv)
        {
  SqlParameter[] parameters = { new SqlParameter("@magv", magv) };
            DataTable dt = dbHelper.ExecuteStoredProcedure("sp_GetTeacherById", parameters);
            if (dt.Rows.Count == 0) return;
        ShowTeacherForm(dt.Rows[0], false);
        }

      private void DeleteTeacher(string magv)
        {
         try
 {
      SqlParameter[] checkParams = { new SqlParameter("@magv", magv) };
                DataTable dtConstraints = dbHelper.ExecuteStoredProcedure("sp_CheckTeacherConstraints", checkParams);
              
    int studentCount = Convert.ToInt32(dtConstraints.Rows[0]["StudentCount"]);
         int classCount = Convert.ToInt32(dtConstraints.Rows[0]["ClassCount"]);
       string message = $"Giảng viên mã {magv}";
                
        if (studentCount > 0 || classCount > 0)
                {
            message += " có các ràng buộc sau:\n\n";
  if (studentCount > 0) message += $"- Đang hướng dẫn {studentCount} sinh viên\n";
          if (classCount > 0) message += $"- Đang giảng dạy {classCount} lớp học phần\n";
 message += "\nBạn có muốn:\n- Nhấn YES: Xóa giảng viên và gỡ bỏ tất cả liên kết\n- Nhấn NO: Hủy thao tác xóa";

       if (MessageBox.Show(message, "Cảnh báo ràng buộc dữ liệu", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
  return;

            SqlParameter[] removeParams = { new SqlParameter("@magv", magv) };
 if (!dbHelper.ExecuteNonQuery("sp_RemoveTeacherReferences", removeParams))
               {
                 MessageBox.Show("Không thể cập nhật thông tin liên kết. Vui lòng thử lại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
       return;
              }
     }
                else
                {
       if (MessageBox.Show($"Bạn có chắc chắn muốn xóa giảng viên mã {magv}?\n\nLưu ý: Dữ liệu sẽ bị xóa vĩnh viễn và không thể khôi phục!",
       "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
        return;
 }

 SqlParameter[] deleteParams = { new SqlParameter("@magv", magv) };
   if (dbHelper.ExecuteNonQuery("sp_DeleteTeacher", deleteParams))
       {
              MessageBox.Show("Xóa giảng viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
           LoadTeacherData();
       }
   }
            catch (Exception ex)
      {
        MessageBox.Show($"Lỗi khi xóa giảng viên: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
     }

        private void BtnAddTeacher_Click(object sender, EventArgs e) => AddNewTeacher();
   private void AddNewTeacher() => ShowTeacherForm(null, true);

        private void ShowTeacherForm(DataRow existingData, bool isNew)
        {
     Form form = new Form
            {
       Text = isNew ? "Thêm giảng viên mới" : "Chỉnh sửa thông tin giảng viên",
           Size = new Size(550, 500),
  StartPosition = FormStartPosition.CenterParent,
FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
     MinimizeBox = false,
  BackColor = Color.White
            };
          
        Panel mainPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(30, 20, 30, 20), AutoScroll = true };
form.Controls.Add(mainPanel);

        var controls = new {
      txtMagv = CreateTextBox(mainPanel, "Mã giảng viên:", 0, existingData?["magv"].ToString(), !isNew),
        txtHoten = CreateTextBox(mainPanel, "Họ và tên:", 45, existingData?["hotengv"].ToString()),
      dtpNgaysinh = CreateDatePicker(mainPanel, "Ngày sinh:", 90, existingData != null ? Convert.ToDateTime(existingData["ngaysinh"]) : DateTime.Now),
         txtSdt = CreateTextBox(mainPanel, "Số điện thoại:", 135, existingData?["sdt"].ToString()),
     txtEmail = CreateTextBox(mainPanel, "Email:", 180, existingData?["email"].ToString()),
              cboGioitinh = CreateComboBox(mainPanel, "Giới tính:", 225, new[] { "Nam", "Nữ" }, existingData?["gioitinh"].ToString() ?? "Nam"),
   txtLophd = CreateTextBox(mainPanel, "Lớp hướng dẫn:", 270, existingData?["lophuongdan"].ToString()),
          txtBomon = CreateTextBox(mainPanel, "Bộ môn:", 315, existingData?["bomon"].ToString())
   };

     CreateButtons(mainPanel, 380, isNew ? "💾 Thêm mới" : "💾 Lưu thay đổi", () =>
   {
       if (string.IsNullOrWhiteSpace(controls.txtMagv.Text) || string.IsNullOrWhiteSpace(controls.txtHoten.Text))
   {
  MessageBox.Show(isNew ? "Vui lòng nhập đầy đủ Mã GV và Họ tên!" : "Vui lòng nhập họ tên!", 
   "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      return;
        }

            // Check for duplicate teacher ID when adding new teacher
  if (isNew)
   {
   SqlParameter[] checkParams = { new SqlParameter("@magv", controls.txtMagv.Text.Trim()) };
     DataTable dtCheck = dbHelper.ExecuteStoredProcedure("sp_GetTeacherById", checkParams);
    if (dtCheck.Rows.Count > 0)
 {
 MessageBox.Show($"Mã giảng viên '{controls.txtMagv.Text.Trim()}' đã tồn tại trong hệ thống!\n\nVui lòng sử dụng mã giảng viên khác.", 
        "Mã giảng viên trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    controls.txtMagv.Focus();
  return;
    }
 }

    SqlParameter[] parameters = {
   new SqlParameter("@magv", controls.txtMagv.Text.Trim()),
       new SqlParameter("@hotengv", controls.txtHoten.Text.Trim()),
    new SqlParameter("@ngaysinh", controls.dtpNgaysinh.Value),
    new SqlParameter("@sdt", controls.txtSdt.Text.Trim()),
      new SqlParameter("@email", controls.txtEmail.Text.Trim()),
        new SqlParameter("@gioitinh", controls.cboGioitinh.SelectedItem?.ToString() ?? "Nam"),
      new SqlParameter("@lophuongdan", controls.txtLophd.Text.Trim()),
   new SqlParameter("@bomon", controls.txtBomon.Text.Trim())
       };

 string procedure = isNew ? "sp_InsertTeacher" : "sp_UpdateTeacher";
      if (dbHelper.ExecuteNonQuery(procedure, parameters))
    {
    MessageBox.Show(isNew ? "Thêm giảng viên mới thành công!" : "Cập nhật thông tin giảng viên thành công!", 
          "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
     form.Close();
       LoadTeacherData();
      }
  }, form);

   form.ShowDialog();
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
       Text = "Hủy bỏ",
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

    private void btnStudentInfo_Click(object sender, EventArgs e)
        {
          new StudentForm().Show();
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

       string[] ratingNames = { "1 Sao", "2 Sao", "3 Sao", "4 Sao", "5 Sao" };
      string[] ratingStars = { "⭐", "⭐⭐", "⭐⭐⭐", "⭐⭐⭐⭐", "⭐⭐⭐⭐⭐" };
            
            // Get total teachers count from the stored procedure
         int totalTeachers = 0;
       DataTable dt = dbHelper.ExecuteStoredProcedure("sp_GetDashboardStats");
         if (dt.Rows.Count > 0)
   {
          // Find the row where Category is 'Teachers'
    foreach (DataRow row in dt.Rows)
      {
              if (row["Category"].ToString() == "Teachers")
       {
               totalTeachers = Convert.ToInt32(row["Total"]);
         break;
        }
}
         }
            
 int[] ratingCounts = DistributeEvenly(totalTeachers, 5);

    DrawChart(g, bounds, ratingNames, ratingStars, ratingCounts, 
   Color.FromArgb(52, 211, 153), Color.FromArgb(16, 185, 129),
       $"Tổng số giảng viên: {totalTeachers} người");
        }

        private int[] DistributeEvenly(int total, int parts)
    {
       int[] result = new int[parts];
    int baseCount = total / parts;
          int remainder = total % parts;
            for (int i = 0; i < parts; i++)
      result[i] = baseCount + (i < remainder ? 1 : 0);
  return result;
   }

        private void DrawChart(Graphics g, Rectangle bounds, string[] labels, string[] subLabels, int[] counts, 
      Color colorStart, Color colorEnd, string legendText)
        {
        int margin = 60, chartWidth = Math.Max(100, bounds.Width - 2 * margin);
            int chartHeight = Math.Max(100, bounds.Height - 2 * margin - 80);
    int barWidth = Math.Max(10, chartWidth / (labels.Length * 2));
            int maxCount = Math.Max(6, counts.Max() + 2);

          using (Font titleFont = new Font("Segoe UI", 10F, FontStyle.Bold))
         using (Brush titleBrush = new SolidBrush(Color.FromArgb(59, 130, 246)))
            using (Font labelFont = new Font("Segoe UI", 9F))
     using (Brush labelBrush = new SolidBrush(Color.FromArgb(107, 114, 128)))
    using (Font countFont = new Font("Segoe UI", 9F, FontStyle.Bold))
            using (Pen gridPen = new Pen(Color.FromArgb(229, 231, 235), 1))
     using (Pen axisPen = new Pen(Color.FromArgb(156, 163, 175), 2))
     using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center })
       {
        g.DrawString("Phân bố giảng viên theo mức đánh giá", titleFont, titleBrush, margin, 10);

    for (int i = 0; i <= 6; i++)
     {
      float y = bounds.Bottom - margin - 60 - (chartHeight * i / 6f);
            if (y >= 30)
    {
          g.DrawString(((int)(maxCount * i / 6f)).ToString(), labelFont, labelBrush, margin - 35, y - 8);
   g.DrawLine(gridPen, margin, y, bounds.Right - margin, y);
             }
             }

           for (int i = 0; i < labels.Length; i++)
     {
     int x = margin + (chartWidth / labels.Length) * i + barWidth / 2;
 int bottomY = bounds.Bottom - margin - 60;
         int barHeight = maxCount > 0 ? Math.Max(0, (int)(counts[i] / (float)maxCount * chartHeight)) : 0;

            if (barHeight > 0)
     {
             Rectangle bar = new Rectangle(x, bottomY - barHeight, barWidth, barHeight);
          using (LinearGradientBrush barBrush = new LinearGradientBrush(
      new Point(bar.X, bar.Y), new Point(bar.X, bar.Bottom), colorStart, colorEnd))
         {
          g.FillRectangle(barBrush, bar);
              }

   SizeF textSize = g.MeasureString(counts[i].ToString(), countFont);
          g.DrawString(counts[i].ToString(), countFont, Brushes.Black,
          x + barWidth / 2 - textSize.Width / 2, bottomY - barHeight - 20);
          }

       g.DrawString(labels[i], labelFont, labelBrush, x + barWidth / 2, bottomY + 10, sf);
      using (Font starFont = new Font("Segoe UI", 11F))
          {
       g.DrawString(subLabels[i], starFont, labelBrush, x + barWidth / 2, bottomY + 28, sf);
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
