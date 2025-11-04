namespace New_project
{
    partial class TeacherForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TeacherForm));
            panelTopBar = new Panel();
            pictureBox1 = new PictureBox();
            lblUserName = new Label();
            picUserAvatar = new PictureBox();
            picNotification = new PictureBox();
            txtSearch = new TextBox();
            panelSideMenu = new Panel();
            btnStudentInfo = new Button();
            btnTeacherInfo = new Button();
            btnHome = new Button();
            panelMain = new Panel();
            panelChart = new Panel();
            panelChartArea = new Panel();
            lblChartTitle = new Label();
            btnAddTeacher = new Button();
            dataGridView1 = new DataGridView();
            panelTopBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picUserAvatar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picNotification).BeginInit();
            panelSideMenu.SuspendLayout();
            panelMain.SuspendLayout();
            panelChart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // panelTopBar
            // 
            panelTopBar.BackColor = Color.White;
            panelTopBar.Controls.Add(pictureBox1);
            panelTopBar.Controls.Add(lblUserName);
            panelTopBar.Controls.Add(picUserAvatar);
            panelTopBar.Controls.Add(picNotification);
            panelTopBar.Controls.Add(txtSearch);
            panelTopBar.Dock = DockStyle.Top;
            panelTopBar.Location = new Point(0, 0);
            panelTopBar.Name = "panelTopBar";
            panelTopBar.Size = new Size(1400, 70);
            panelTopBar.TabIndex = 0;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(44, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(65, 65);
            pictureBox1.TabIndex = 5;
            pictureBox1.TabStop = false;
            // 
            // lblUserName
            // 
            lblUserName.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblUserName.AutoSize = true;
            lblUserName.Font = new Font("Segoe UI", 10F);
            lblUserName.Location = new Point(1228, 25);
            lblUserName.Name = "lblUserName";
            lblUserName.Size = new Size(123, 23);
            lblUserName.TabIndex = 4;
            lblUserName.Text = "Amanda Smith";
            // 
            // picUserAvatar
            // 
            picUserAvatar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            picUserAvatar.BackColor = Color.LightGray;
            picUserAvatar.Location = new Point(1180, 18);
            picUserAvatar.Name = "picUserAvatar";
            picUserAvatar.Size = new Size(40, 40);
            picUserAvatar.SizeMode = PictureBoxSizeMode.StretchImage;
            picUserAvatar.TabIndex = 3;
            picUserAvatar.TabStop = false;
            // 
            // picNotification
            // 
            picNotification.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            picNotification.BackColor = Color.WhiteSmoke;
            picNotification.Location = new Point(1130, 23);
            picNotification.Name = "picNotification";
            picNotification.Size = new Size(30, 30);
            picNotification.SizeMode = PictureBoxSizeMode.CenterImage;
            picNotification.TabIndex = 2;
            picNotification.TabStop = false;
            // 
            // txtSearch
            // 
            txtSearch.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSearch.BackColor = Color.WhiteSmoke;
            txtSearch.BorderStyle = BorderStyle.None;
            txtSearch.Font = new Font("Segoe UI", 11F);
            txtSearch.ForeColor = Color.Gray;
            txtSearch.Location = new Point(450, 23);
            txtSearch.Name = "txtSearch";
            txtSearch.PlaceholderText = " Search...";
            txtSearch.Size = new Size(600, 25);
            txtSearch.TabIndex = 1;
            // 
            // panelSideMenu
            // 
            panelSideMenu.BackColor = Color.FromArgb(249, 250, 251);
            panelSideMenu.Controls.Add(btnStudentInfo);
            panelSideMenu.Controls.Add(btnTeacherInfo);
            panelSideMenu.Controls.Add(btnHome);
            panelSideMenu.Dock = DockStyle.Left;
            panelSideMenu.Location = new Point(0, 70);
            panelSideMenu.Name = "panelSideMenu";
            panelSideMenu.Padding = new Padding(20);
            panelSideMenu.Size = new Size(260, 730);
            panelSideMenu.TabIndex = 1;
            // 
            // btnStudentInfo
            // 
            btnStudentInfo.Cursor = Cursors.Hand;
            btnStudentInfo.Dock = DockStyle.Top;
            btnStudentInfo.FlatAppearance.BorderSize = 0;
            btnStudentInfo.FlatStyle = FlatStyle.Flat;
            btnStudentInfo.Font = new Font("Segoe UI", 10F);
            btnStudentInfo.ForeColor = Color.FromArgb(107, 114, 128);
            btnStudentInfo.ImageAlign = ContentAlignment.MiddleLeft;
            btnStudentInfo.Location = new Point(20, 140);
            btnStudentInfo.Name = "btnStudentInfo";
            btnStudentInfo.Padding = new Padding(10, 0, 0, 0);
            btnStudentInfo.Size = new Size(220, 60);
            btnStudentInfo.TabIndex = 2;
            btnStudentInfo.Text = "Thông tin sinh viên";
            btnStudentInfo.TextAlign = ContentAlignment.MiddleLeft;
            btnStudentInfo.UseVisualStyleBackColor = true;
            btnStudentInfo.Click += btnStudentInfo_Click;
            // 
            // btnTeacherInfo
            // 
            btnTeacherInfo.BackColor = Color.FromArgb(59, 130, 246);
            btnTeacherInfo.Cursor = Cursors.Hand;
            btnTeacherInfo.Dock = DockStyle.Top;
            btnTeacherInfo.FlatAppearance.BorderSize = 0;
            btnTeacherInfo.FlatStyle = FlatStyle.Flat;
            btnTeacherInfo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnTeacherInfo.ForeColor = Color.White;
            btnTeacherInfo.ImageAlign = ContentAlignment.MiddleLeft;
            btnTeacherInfo.Location = new Point(20, 80);
            btnTeacherInfo.Name = "btnTeacherInfo";
            btnTeacherInfo.Padding = new Padding(10, 0, 0, 0);
            btnTeacherInfo.Size = new Size(220, 60);
            btnTeacherInfo.TabIndex = 1;
            btnTeacherInfo.Text = "Thông tin giảng viên";
            btnTeacherInfo.TextAlign = ContentAlignment.MiddleLeft;
            btnTeacherInfo.UseVisualStyleBackColor = false;
            // 
            // btnHome
            // 
            btnHome.Cursor = Cursors.Hand;
            btnHome.Dock = DockStyle.Top;
            btnHome.FlatAppearance.BorderSize = 0;
            btnHome.FlatStyle = FlatStyle.Flat;
            btnHome.Font = new Font("Segoe UI", 10F);
            btnHome.ForeColor = Color.FromArgb(107, 114, 128);
            btnHome.ImageAlign = ContentAlignment.MiddleLeft;
            btnHome.Location = new Point(20, 20);
            btnHome.Name = "btnHome";
            btnHome.Padding = new Padding(10, 0, 0, 0);
            btnHome.Size = new Size(220, 60);
            btnHome.TabIndex = 0;
            btnHome.Text = "Trang chủ";
            btnHome.TextAlign = ContentAlignment.MiddleLeft;
            btnHome.UseVisualStyleBackColor = true;
            btnHome.Click += btnHome_Click;
            // 
            // panelMain
            // 
            panelMain.AutoScroll = true;
            panelMain.BackColor = Color.White;
            panelMain.Controls.Add(panelChart);
            panelMain.Controls.Add(btnAddTeacher);
            panelMain.Controls.Add(dataGridView1);
            panelMain.Dock = DockStyle.Fill;
            panelMain.Location = new Point(260, 70);
            panelMain.Name = "panelMain";
            panelMain.Padding = new Padding(30, 20, 30, 20);
            panelMain.Size = new Size(1140, 730);
            panelMain.TabIndex = 2;
            // 
            // panelChart
            // 
            panelChart.Controls.Add(panelChartArea);
            panelChart.Controls.Add(lblChartTitle);
            panelChart.Dock = DockStyle.Fill;
            panelChart.Location = new Point(30, 315);
            panelChart.Name = "panelChart";
            panelChart.Padding = new Padding(0, 20, 0, 0);
            panelChart.Size = new Size(1080, 395);
            panelChart.TabIndex = 2;
            // 
            // panelChartArea
            // 
            panelChartArea.BackColor = Color.White;
            panelChartArea.Dock = DockStyle.Fill;
            panelChartArea.Location = new Point(0, 65);
            panelChartArea.Name = "panelChartArea";
            panelChartArea.Size = new Size(1080, 330);
            panelChartArea.TabIndex = 2;
            panelChartArea.Paint += panelChartArea_Paint;
            // 
            // lblChartTitle
            // 
            lblChartTitle.Dock = DockStyle.Top;
            lblChartTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblChartTitle.ForeColor = Color.FromArgb(107, 114, 128);
            lblChartTitle.Location = new Point(0, 20);
            lblChartTitle.Name = "lblChartTitle";
            lblChartTitle.Padding = new Padding(0, 10, 0, 10);
            lblChartTitle.Size = new Size(1080, 45);
            lblChartTitle.TabIndex = 1;
            lblChartTitle.Text = "Phân bổ đánh giá giảng viên theo cấp độ";
            // 
            // btnAddTeacher
            // 
            btnAddTeacher.BackColor = Color.FromArgb(34, 197, 94);
            btnAddTeacher.Cursor = Cursors.Hand;
            btnAddTeacher.Dock = DockStyle.Top;
            btnAddTeacher.FlatAppearance.BorderSize = 0;
            btnAddTeacher.FlatStyle = FlatStyle.Flat;
            btnAddTeacher.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnAddTeacher.ForeColor = Color.White;
            btnAddTeacher.Location = new Point(30, 270);
            btnAddTeacher.Name = "btnAddTeacher";
            btnAddTeacher.Size = new Size(1080, 45);
            btnAddTeacher.TabIndex = 1;
            btnAddTeacher.Text = "Thêm giảng viên mới";
            btnAddTeacher.UseVisualStyleBackColor = false;
            btnAddTeacher.Click += BtnAddTeacher_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridView1.ColumnHeadersHeight = 40;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView1.Dock = DockStyle.Top;
            dataGridView1.GridColor = Color.FromArgb(229, 231, 235);
            dataGridView1.Location = new Point(30, 20);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.RowTemplate.Height = 50;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new Size(1080, 250);
            dataGridView1.TabIndex = 0;
            // 
            // TeacherForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1400, 800);
            Controls.Add(panelMain);
            Controls.Add(panelSideMenu);
            Controls.Add(panelTopBar);
            MinimumSize = new Size(1200, 700);
            Name = "TeacherForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Thông tin giảng viên - Admin";
            Load += TeacherForm_Load;
            panelTopBar.ResumeLayout(false);
            panelTopBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)picUserAvatar).EndInit();
            ((System.ComponentModel.ISupportInitialize)picNotification).EndInit();
            panelSideMenu.ResumeLayout(false);
            panelMain.ResumeLayout(false);
            panelChart.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelTopBar;
        private Panel panelSideMenu;
        private Panel panelMain;
        private TextBox txtSearch;
        private PictureBox picNotification;
        private PictureBox picUserAvatar;
        private Label lblUserName;
        private Button btnHome;
        private Button btnTeacherInfo;
        private Button btnStudentInfo;
        private DataGridView dataGridView1;
        private Button btnAddTeacher;
        private Panel panelChart;
        private Panel panelChartArea;
        private Label lblChartTitle;
        private PictureBox pictureBox1;
        private ContextMenuStrip contextMenu;
    }
}