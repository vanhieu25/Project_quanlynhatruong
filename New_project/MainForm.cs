using System.Drawing.Drawing2D;
using System.Data.SqlClient;
using System.Data;
using System.Text;

namespace New_project
{
    public partial class MainForm : Form
    {
        private String connectionString = @"Data Source=LAPTOP-LJP88J8V;Initial Catalog=QLHoctap;Integrated Security=True;TrustServerCertificate=True";
        public MainForm()
        {
            InitializeComponent();
            CustomizeDataGridView();
            InitializeUTF8Text();
        }

        private void InitializeUTF8Text()
        {
            // Set UTF-8 button texts with emojis
            btnHome.Text = "🏠 Trang chủ";
            btnTeacherInfo.Text = "👨‍🏫 Thông tin giảng viên";
            btnStudentInfo.Text = "🎓 Thông tin sinh viên";

            // Set chart title with color circles
            lblChartTitle.Text = "🟢 Chất lượng giảng dạy   ⚪ Điểm số sinh viên";
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadDashboardData();
            panelChartArea.Invalidate();
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

            dataGridView1.Columns[0].Width = 200;
            dataGridView1.Columns[1].Width = 180;
            dataGridView1.Columns[2].Width = 200;
            dataGridView1.Columns[3].Width = 180;
            dataGridView1.Columns[4].Width = 280;

            dataGridView1.Rows.Clear();

            DataTable dtTeachers = ExecuteQuery("SELECT COUNT(*) as total FROM giangvien");
            int totalTeachers = dtTeachers.Rows.Count > 0 ? Convert.ToInt32(dtTeachers.Rows[0]["total"]) : 0;

            DataTable dtStudents = ExecuteQuery("SELECT COUNT(*) as total FROM sinhvien");
            int totalStudents = dtStudents.Rows.Count > 0 ? Convert.ToInt32(dtStudents.Rows[0]["total"]) : 0;

            DataTable dtSubjects = ExecuteQuery("SELECT COUNT(*) as total FROM monhoc");
            int totalSubjects = dtSubjects.Rows.Count > 0 ? Convert.ToInt32(dtSubjects.Rows[0]["total"]) : 0;

            DataTable dtClasses = ExecuteQuery("SELECT COUNT(*) as total FROM lophocphan");
            int totalClasses = dtClasses.Rows.Count > 0 ? Convert.ToInt32(dtClasses.Rows[0]["total"]) : 0;

            dataGridView1.Rows.Add("👨‍🏫 Giảng viên", totalTeachers.ToString(), totalTeachers.ToString(), "0", "✅ Hoạt động tốt");
            dataGridView1.Rows.Add("🎓 Sinh viên", totalStudents.ToString(), totalStudents.ToString(), "0", "✅ Hoạt động tốt");
            dataGridView1.Rows.Add("📚 Môn học", totalSubjects.ToString(), totalSubjects.ToString(), "0", "✅ Đang giảng dạy");
            dataGridView1.Rows.Add("🏫 Lớp học", totalClasses.ToString(), totalClasses.ToString(), "0", "✅ Học kỳ đang diễn ra");
        }

        private void btnTeacherInfo_Click(object sender, EventArgs e)
        {
            TeacherForm teacherForm = new TeacherForm();
            teacherForm.Show();
            this.Hide();
        }

        private void btnStudentInfo_Click(object sender, EventArgs e)
        {
            StudentForm studentForm = new StudentForm();
            studentForm.Show();
            this.Hide();
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            // Already on home page, do nothing or refresh data
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
            {
                for (int i = 0; i <= 4; i++)
                {
                    float yValue = i * 2.5f;
                    float y = bounds.Bottom - margin - 50 - (chartHeight * i / 4f);
                    g.DrawString(yValue.ToString("0.0"), labelFont, labelBrush, margin - 35, y - 8);

                    using (Pen gridPen = new Pen(Color.FromArgb(229, 231, 235), 1))
                    {
                        g.DrawLine(gridPen, margin, y, bounds.Right - margin, y);
                    }
                }

                for (int i = 0; i < labels.Length; i++)
                {
                    int x = margin + i * barGroupWidth + barGroupWidth / 2 - barWidth;
                    int bottomY = bounds.Bottom - margin - 50;

                    int greenHeight = Math.Max(0, (int)(greenData[i] / 10.0 * chartHeight));
                    if (greenHeight > 0)
                    {
                        Rectangle greenBar = new Rectangle(x, bottomY - greenHeight, barWidth, greenHeight);
                        using (LinearGradientBrush greenBrush = new LinearGradientBrush(
                  new Point(greenBar.X, greenBar.Y),
                 new Point(greenBar.X, greenBar.Bottom),
                    Color.FromArgb(52, 211, 153),
                Color.FromArgb(16, 185, 129)))
                        {
                            g.FillRectangle(greenBrush, greenBar);
                        }
                    }

                    int grayHeight = Math.Max(0, (int)(grayData[i] / 10.0 * chartHeight));
                    if (grayHeight > 0)
                    {
                        Rectangle grayBar = new Rectangle(x + barWidth + 5, bottomY - grayHeight, barWidth, grayHeight);
                        using (LinearGradientBrush grayBrush = new LinearGradientBrush(
                      new Point(grayBar.X, grayBar.Y),
                        new Point(grayBar.X, grayBar.Bottom),
                         Color.FromArgb(209, 213, 219),
                      Color.FromArgb(156, 163, 175)))
                        {
                            g.FillRectangle(grayBrush, grayBar);
                        }
                    }

                    using (StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center })
                    {
                        g.DrawString(labels[i], labelFont, labelBrush,
                 margin + i * barGroupWidth + barGroupWidth / 2,
                     bottomY + 10, sf);
                    }
                }

                using (Pen axisPen = new Pen(Color.FromArgb(156, 163, 175), 2))
                {
                    g.DrawLine(axisPen, margin, bounds.Bottom - margin - 50,
               bounds.Right - margin, bounds.Bottom - margin - 50);
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
