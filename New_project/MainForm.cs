using System.Drawing.Drawing2D;
using System.Data;

namespace New_project
{
    public partial class MainForm : Form
    {
        private readonly DatabaseHelper dbHelper = new DatabaseHelper();

        public MainForm()
        {
          InitializeComponent();
    CustomizeDataGridView();
            InitializeUTF8Text();
        }

        private void InitializeUTF8Text()
    {
          btnHome.Text = "🏠 Trang chủ";
            btnTeacherInfo.Text = "👨‍🏫 Thông tin giảng viên";
            btnStudentInfo.Text = "🎓 Thông tin sinh viên";
       lblChartTitle.Text = "🟢 Chất lượng giảng dạy   ⚪ Điểm số sinh viên";
        }

      private void MainForm_Load(object sender, EventArgs e)
        {
        LoadDashboardData();
    panelChartArea.Invalidate();
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

      private void LoadDashboardData()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("colCategory", "Danh mục");
      dataGridView1.Columns.Add("colTotal", "Tổng số");
          dataGridView1.Columns.Add("colActive", "Đang hoạt động");
            dataGridView1.Columns.Add("colInactive", "Tạm ngưng");
            dataGridView1.Columns.Add("colStatus", "Trạng thái hệ thống");

            foreach (DataGridViewColumn col in dataGridView1.Columns)
                col.Width = col.Index == 4 ? 280 : col.Index % 2 == 0 ? 200 : 180;

dataGridView1.Rows.Clear();

       DataTable dt = dbHelper.ExecuteStoredProcedure("sp_GetDashboardStats");

      string[] icons = { "👨‍🏫 Giảng viên", "🎓 Sinh viên", "📚 Môn học", "🏫 Lớp học" };
    string[] statuses = { "✅ Hoạt động tốt", "✅ Hoạt động tốt", "✅ Đang giảng dạy", "✅ Học kỳ đang diễn ra" };

       for (int i = 0; i < dt.Rows.Count && i < icons.Length; i++)
    {
      int total = Convert.ToInt32(dt.Rows[i]["Total"]);
          dataGridView1.Rows.Add(icons[i], total.ToString(), total.ToString(), "0", statuses[i]);
            }
        }

     private void btnTeacherInfo_Click(object sender, EventArgs e)
        {
 new TeacherForm().Show();
         this.Hide();
        }

        private void btnStudentInfo_Click(object sender, EventArgs e)
        {
            new StudentForm().Show();
          this.Hide();
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
          LoadDashboardData();
    panelChartArea.Invalidate();
  }

        private void panelChartArea_Paint(object sender, PaintEventArgs e)
        {
     DrawBarChart(e.Graphics, panelChartArea.ClientRectangle);
        }

        private void DrawBarChart(Graphics g, Rectangle bounds)
        {
          if (bounds.Width < 100 || bounds.Height < 100) return;

          g.SmoothingMode = SmoothingMode.AntiAlias;

            string[] labels = { "HK1", "HK2", "HK3", "HK4", "HK5", "HK6", "HK7", "HK8", "HK9" };
   double[] greenData = { 8.5, 7.5, 8.2, 9.0, 8.5, 7.0, 9.2, 8.0, 9.5 };
double[] grayData = { 7.0, 7.8, 7.5, 7.2, 7.8, 7.3, 7.5, 7.2, 7.5 };

      int margin = 50;
            int chartWidth = Math.Max(100, bounds.Width - 2 * margin);
        int chartHeight = Math.Max(100, bounds.Height - 2 * margin - 50);
        int barGroupWidth = chartWidth / labels.Length;
int barWidth = Math.Max(10, barGroupWidth / 3);

       using (Font labelFont = new Font("Segoe UI", 9F))
            using (Brush labelBrush = new SolidBrush(Color.FromArgb(107, 114, 128)))
   using (Pen gridPen = new Pen(Color.FromArgb(229, 231, 235), 1))
       using (Pen axisPen = new Pen(Color.FromArgb(156, 163, 175), 2))
      using (StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center })
            {
        // Draw grid lines and Y-axis labels
       for (int i = 0; i <= 4; i++)
       {
         float yValue = i * 2.5f;
        float y = bounds.Bottom - margin - 50 - (chartHeight * i / 4f);
          g.DrawString(yValue.ToString("0.0"), labelFont, labelBrush, margin - 35, y - 8);
             g.DrawLine(gridPen, margin, y, bounds.Right - margin, y);
       }

  // Draw bars
  for (int i = 0; i < labels.Length; i++)
          {
        int x = margin + i * barGroupWidth + barGroupWidth / 2 - barWidth;
   int bottomY = bounds.Bottom - margin - 50;

        DrawBar(g, x, bottomY, barWidth, greenData[i], chartHeight, 
        Color.FromArgb(52, 211, 153), Color.FromArgb(16, 185, 129));
        DrawBar(g, x + barWidth + 5, bottomY, barWidth, grayData[i], chartHeight, 
   Color.FromArgb(209, 213, 219), Color.FromArgb(156, 163, 175));

        g.DrawString(labels[i], labelFont, labelBrush,
     margin + i * barGroupWidth + barGroupWidth / 2, bottomY + 10, sf);
       }

      // Draw X-axis
        g.DrawLine(axisPen, margin, bounds.Bottom - margin - 50,
         bounds.Right - margin, bounds.Bottom - margin - 50);
 }
        }

private void DrawBar(Graphics g, int x, int bottomY, int barWidth, double value, 
 int chartHeight, Color colorTop, Color colorBottom)
        {
            int barHeight = Math.Max(0, (int)(value / 10.0 * chartHeight));
            if (barHeight <= 0) return;

 Rectangle bar = new Rectangle(x, bottomY - barHeight, barWidth, barHeight);
         using (LinearGradientBrush brush = new LinearGradientBrush(
                new Point(bar.X, bar.Y), new Point(bar.X, bar.Bottom), colorTop, colorBottom))
       {
g.FillRectangle(brush, bar);
         }
}

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
     Application.Exit();
        }
    }
}
