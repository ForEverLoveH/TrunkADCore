namespace TrunkADCore.ADCoreWindow
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            HZH_Controls.Controls.NavigationMenuItem navigationMenuItem1 = new HZH_Controls.Controls.NavigationMenuItem();
            HZH_Controls.Controls.NavigationMenuItem navigationMenuItem2 = new HZH_Controls.Controls.NavigationMenuItem();
            HZH_Controls.Controls.NavigationMenuItem navigationMenuItem3 = new HZH_Controls.Controls.NavigationMenuItem();
            HZH_Controls.Controls.NavigationMenuItem navigationMenuItem4 = new HZH_Controls.Controls.NavigationMenuItem();
            HZH_Controls.Controls.NavigationMenuItem navigationMenuItem5 = new HZH_Controls.Controls.NavigationMenuItem();
            HZH_Controls.Controls.NavigationMenuItem navigationMenuItem6 = new HZH_Controls.Controls.NavigationMenuItem();
            HZH_Controls.Controls.NavigationMenuItem navigationMenuItem7 = new HZH_Controls.Controls.NavigationMenuItem();
            HZH_Controls.Controls.NavigationMenuItem navigationMenuItem8 = new HZH_Controls.Controls.NavigationMenuItem();
            HZH_Controls.Controls.NavigationMenuItem navigationMenuItem9 = new HZH_Controls.Controls.NavigationMenuItem();
            HZH_Controls.Controls.NavigationMenuItem navigationMenuItem10 = new HZH_Controls.Controls.NavigationMenuItem();
            HZH_Controls.Controls.NavigationMenuItem navigationMenuItem11 = new HZH_Controls.Controls.NavigationMenuItem();
            HZH_Controls.Controls.NavigationMenuItem navigationMenuItem12 = new HZH_Controls.Controls.NavigationMenuItem();
            HZH_Controls.Controls.NavigationMenuItem navigationMenuItem13 = new HZH_Controls.Controls.NavigationMenuItem();
            HZH_Controls.Controls.NavigationMenuItem navigationMenuItem14 = new HZH_Controls.Controls.NavigationMenuItem();
            HZH_Controls.Controls.NavigationMenuItem navigationMenuItem15 = new HZH_Controls.Controls.NavigationMenuItem();
            HZH_Controls.Controls.NavigationMenuItem navigationMenuItem16 = new HZH_Controls.Controls.NavigationMenuItem();
            this.ucPanelTitle1 = new HZH_Controls.Controls.UCPanelTitle();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.GroupDataPanel = new Sunny.UI.UITitlePanel();
            this.listView1 = new System.Windows.Forms.ListView();
            this.ucPanelTitle2 = new HZH_Controls.Controls.UCPanelTitle();
            this.MyTreeView = new Sunny.UI.UITreeView();
            this.ucNavigationMenu1 = new HZH_Controls.Controls.UCNavigationMenu();
            this.ucPanelTitle1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.GroupDataPanel.SuspendLayout();
            this.ucPanelTitle2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ucPanelTitle1
            // 
            this.ucPanelTitle1.BackColor = System.Drawing.Color.Transparent;
            this.ucPanelTitle1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.ucPanelTitle1.ConerRadius = 10;
            this.ucPanelTitle1.Controls.Add(this.statusStrip1);
            this.ucPanelTitle1.Controls.Add(this.GroupDataPanel);
            this.ucPanelTitle1.Controls.Add(this.ucPanelTitle2);
            this.ucPanelTitle1.Controls.Add(this.ucNavigationMenu1);
            this.ucPanelTitle1.FillColor = System.Drawing.Color.White;
            this.ucPanelTitle1.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.ucPanelTitle1.IsCanExpand = true;
            this.ucPanelTitle1.IsExpand = false;
            this.ucPanelTitle1.IsRadius = true;
            this.ucPanelTitle1.IsShowRect = true;
            this.ucPanelTitle1.Location = new System.Drawing.Point(0, 14);
            this.ucPanelTitle1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ucPanelTitle1.Name = "ucPanelTitle1";
            this.ucPanelTitle1.Padding = new System.Windows.Forms.Padding(1);
            this.ucPanelTitle1.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.ucPanelTitle1.RectWidth = 1;
            this.ucPanelTitle1.Size = new System.Drawing.Size(1369, 783);
            this.ucPanelTitle1.TabIndex = 0;
            this.ucPanelTitle1.Title = "";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(1, 760);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1367, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.ForeColor = System.Drawing.Color.DimGray;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(101, 17);
            this.toolStripStatusLabel1.Text = "版本号：{0,0,0,0}";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(131, 17);
            this.toolStripStatusLabel2.Text = "toolStripStatusLabel2";
            // 
            // GroupDataPanel
            // 
            this.GroupDataPanel.Controls.Add(this.listView1);
            this.GroupDataPanel.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.GroupDataPanel.Location = new System.Drawing.Point(415, 77);
            this.GroupDataPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.GroupDataPanel.MinimumSize = new System.Drawing.Size(1, 1);
            this.GroupDataPanel.Name = "GroupDataPanel";
            this.GroupDataPanel.Padding = new System.Windows.Forms.Padding(0, 35, 0, 0);
            this.GroupDataPanel.ShowText = false;
            this.GroupDataPanel.Size = new System.Drawing.Size(949, 678);
            this.GroupDataPanel.TabIndex = 3;
            this.GroupDataPanel.Text = "分组信息";
            this.GroupDataPanel.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.GroupDataPanel.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // listView1
            // 
            this.listView1.FullRowSelect = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(3, 39);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(940, 636);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // ucPanelTitle2
            // 
            this.ucPanelTitle2.BackColor = System.Drawing.Color.Transparent;
            this.ucPanelTitle2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.ucPanelTitle2.ConerRadius = 10;
            this.ucPanelTitle2.Controls.Add(this.MyTreeView);
            this.ucPanelTitle2.FillColor = System.Drawing.Color.White;
            this.ucPanelTitle2.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.ucPanelTitle2.IsCanExpand = true;
            this.ucPanelTitle2.IsExpand = false;
            this.ucPanelTitle2.IsRadius = true;
            this.ucPanelTitle2.IsShowRect = true;
            this.ucPanelTitle2.Location = new System.Drawing.Point(0, 77);
            this.ucPanelTitle2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ucPanelTitle2.Name = "ucPanelTitle2";
            this.ucPanelTitle2.Padding = new System.Windows.Forms.Padding(1);
            this.ucPanelTitle2.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.ucPanelTitle2.RectWidth = 1;
            this.ucPanelTitle2.Size = new System.Drawing.Size(407, 678);
            this.ucPanelTitle2.TabIndex = 2;
            this.ucPanelTitle2.Title = "组别选择";
            // 
            // MyTreeView
            // 
            this.MyTreeView.FillColor = System.Drawing.Color.White;
            this.MyTreeView.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.MyTreeView.Location = new System.Drawing.Point(5, 39);
            this.MyTreeView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MyTreeView.MinimumSize = new System.Drawing.Size(1, 1);
            this.MyTreeView.Name = "MyTreeView";
            this.MyTreeView.ShowText = false;
            this.MyTreeView.Size = new System.Drawing.Size(397, 639);
            this.MyTreeView.Style = Sunny.UI.UIStyle.Custom;
            this.MyTreeView.TabIndex = 1;
            this.MyTreeView.Text = "uiTreeView1";
            this.MyTreeView.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.MyTreeView.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.MyTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.MyTreeView_NodeMouseClick);
            // 
            // ucNavigationMenu1
            // 
            this.ucNavigationMenu1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.ucNavigationMenu1.Font = new System.Drawing.Font("微软雅黑", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ucNavigationMenu1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            navigationMenuItem1.AnchorRight = false;
            navigationMenuItem1.DataSource = null;
            navigationMenuItem1.HasSplitLintAtTop = false;
            navigationMenuItem1.Icon = null;
            navigationMenuItem2.AnchorRight = false;
            navigationMenuItem2.DataSource = null;
            navigationMenuItem2.HasSplitLintAtTop = false;
            navigationMenuItem2.Icon = null;
            navigationMenuItem2.Items = null;
            navigationMenuItem2.ItemWidth = 100;
            navigationMenuItem2.ShowTip = false;
            navigationMenuItem2.Text = "项目设置";
            navigationMenuItem2.TipText = null;
            navigationMenuItem1.Items = new HZH_Controls.Controls.NavigationMenuItem[] {
        navigationMenuItem2};
            navigationMenuItem1.ItemWidth = 100;
            navigationMenuItem1.ShowTip = false;
            navigationMenuItem1.Text = "系统管理";
            navigationMenuItem1.TipText = null;
            navigationMenuItem3.AnchorRight = false;
            navigationMenuItem3.DataSource = null;
            navigationMenuItem3.HasSplitLintAtTop = false;
            navigationMenuItem3.Icon = null;
            navigationMenuItem4.AnchorRight = false;
            navigationMenuItem4.DataSource = null;
            navigationMenuItem4.HasSplitLintAtTop = false;
            navigationMenuItem4.Icon = null;
            navigationMenuItem4.Items = null;
            navigationMenuItem4.ItemWidth = 100;
            navigationMenuItem4.ShowTip = false;
            navigationMenuItem4.Text = "初始化数据库";
            navigationMenuItem4.TipText = null;
            navigationMenuItem5.AnchorRight = false;
            navigationMenuItem5.DataSource = null;
            navigationMenuItem5.HasSplitLintAtTop = false;
            navigationMenuItem5.Icon = null;
            navigationMenuItem5.Items = null;
            navigationMenuItem5.ItemWidth = 100;
            navigationMenuItem5.ShowTip = false;
            navigationMenuItem5.Text = "数据库备份";
            navigationMenuItem5.TipText = null;
            navigationMenuItem6.AnchorRight = false;
            navigationMenuItem6.DataSource = null;
            navigationMenuItem6.HasSplitLintAtTop = false;
            navigationMenuItem6.Icon = null;
            navigationMenuItem6.Items = null;
            navigationMenuItem6.ItemWidth = 100;
            navigationMenuItem6.ShowTip = false;
            navigationMenuItem6.Text = "平台设备码";
            navigationMenuItem6.TipText = null;
            navigationMenuItem3.Items = new HZH_Controls.Controls.NavigationMenuItem[] {
        navigationMenuItem4,
        navigationMenuItem5,
        navigationMenuItem6};
            navigationMenuItem3.ItemWidth = 100;
            navigationMenuItem3.ShowTip = false;
            navigationMenuItem3.Text = "数据管理";
            navigationMenuItem3.TipText = null;
            navigationMenuItem7.AnchorRight = true;
            navigationMenuItem7.DataSource = null;
            navigationMenuItem7.HasSplitLintAtTop = false;
            navigationMenuItem7.Icon = null;
            navigationMenuItem7.Items = null;
            navigationMenuItem7.ItemWidth = 100;
            navigationMenuItem7.ShowTip = false;
            navigationMenuItem7.Text = "退出";
            navigationMenuItem7.TipText = null;
            navigationMenuItem8.AnchorRight = true;
            navigationMenuItem8.DataSource = null;
            navigationMenuItem8.HasSplitLintAtTop = false;
            navigationMenuItem8.Icon = null;
            navigationMenuItem9.AnchorRight = false;
            navigationMenuItem9.DataSource = null;
            navigationMenuItem9.HasSplitLintAtTop = false;
            navigationMenuItem9.Icon = null;
            navigationMenuItem9.Items = null;
            navigationMenuItem9.ItemWidth = 100;
            navigationMenuItem9.ShowTip = false;
            navigationMenuItem9.Text = "修改成绩";
            navigationMenuItem9.TipText = null;
            navigationMenuItem10.AnchorRight = false;
            navigationMenuItem10.DataSource = null;
            navigationMenuItem10.HasSplitLintAtTop = false;
            navigationMenuItem10.Icon = null;
            navigationMenuItem10.Items = null;
            navigationMenuItem10.ItemWidth = 100;
            navigationMenuItem10.ShowTip = false;
            navigationMenuItem10.Text = "上传成绩";
            navigationMenuItem10.TipText = null;
            navigationMenuItem11.AnchorRight = false;
            navigationMenuItem11.DataSource = null;
            navigationMenuItem11.HasSplitLintAtTop = false;
            navigationMenuItem11.Icon = null;
            navigationMenuItem11.Items = null;
            navigationMenuItem11.ItemWidth = 100;
            navigationMenuItem11.ShowTip = false;
            navigationMenuItem11.Text = "删除成绩";
            navigationMenuItem11.TipText = null;
            navigationMenuItem12.AnchorRight = false;
            navigationMenuItem12.DataSource = null;
            navigationMenuItem12.HasSplitLintAtTop = false;
            navigationMenuItem12.Icon = null;
            navigationMenuItem12.Items = null;
            navigationMenuItem12.ItemWidth = 100;
            navigationMenuItem12.ShowTip = false;
            navigationMenuItem12.Text = "导出成绩";
            navigationMenuItem12.TipText = null;
            navigationMenuItem8.Items = new HZH_Controls.Controls.NavigationMenuItem[] {
        navigationMenuItem9,
        navigationMenuItem10,
        navigationMenuItem11,
        navigationMenuItem12};
            navigationMenuItem8.ItemWidth = 100;
            navigationMenuItem8.ShowTip = false;
            navigationMenuItem8.Text = "成绩管理";
            navigationMenuItem8.TipText = null;
            navigationMenuItem13.AnchorRight = false;
            navigationMenuItem13.DataSource = null;
            navigationMenuItem13.HasSplitLintAtTop = false;
            navigationMenuItem13.Icon = null;
            navigationMenuItem13.Items = null;
            navigationMenuItem13.ItemWidth = 100;
            navigationMenuItem13.ShowTip = false;
            navigationMenuItem13.Text = "启动测试";
            navigationMenuItem13.TipText = null;
            navigationMenuItem14.AnchorRight = false;
            navigationMenuItem14.DataSource = null;
            navigationMenuItem14.HasSplitLintAtTop = false;
            navigationMenuItem14.Icon = null;
            navigationMenuItem15.AnchorRight = false;
            navigationMenuItem15.DataSource = null;
            navigationMenuItem15.HasSplitLintAtTop = false;
            navigationMenuItem15.Icon = null;
            navigationMenuItem15.Items = null;
            navigationMenuItem15.ItemWidth = 100;
            navigationMenuItem15.ShowTip = false;
            navigationMenuItem15.Text = "导入成绩模板";
            navigationMenuItem15.TipText = null;
            navigationMenuItem16.AnchorRight = false;
            navigationMenuItem16.DataSource = null;
            navigationMenuItem16.HasSplitLintAtTop = false;
            navigationMenuItem16.Icon = null;
            navigationMenuItem16.Items = null;
            navigationMenuItem16.ItemWidth = 100;
            navigationMenuItem16.ShowTip = false;
            navigationMenuItem16.Text = "导入名单模板";
            navigationMenuItem16.TipText = null;
            navigationMenuItem14.Items = new HZH_Controls.Controls.NavigationMenuItem[] {
        navigationMenuItem15,
        navigationMenuItem16};
            navigationMenuItem14.ItemWidth = 100;
            navigationMenuItem14.ShowTip = false;
            navigationMenuItem14.Text = "帮助";
            navigationMenuItem14.TipText = null;
            this.ucNavigationMenu1.Items = new HZH_Controls.Controls.NavigationMenuItem[] {
        navigationMenuItem1,
        navigationMenuItem3,
        navigationMenuItem7,
        navigationMenuItem8,
        navigationMenuItem13,
        navigationMenuItem14};
            this.ucNavigationMenu1.Location = new System.Drawing.Point(0, 35);
            this.ucNavigationMenu1.Name = "ucNavigationMenu1";
            this.ucNavigationMenu1.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.ucNavigationMenu1.Size = new System.Drawing.Size(1369, 43);
            this.ucNavigationMenu1.TabIndex = 1;
            this.ucNavigationMenu1.TipColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(87)))), ((int)(((byte)(34)))));
            this.ucNavigationMenu1.ClickItemed += new System.EventHandler(this.ucNavigationMenu1_ClickItemed);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1370, 799);
            this.Controls.Add(this.ucPanelTitle1);
            this.Name = "MainWindow";
            this.Text = "MainWindow";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainWindow_FormClosed);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.ucPanelTitle1.ResumeLayout(false);
            this.ucPanelTitle1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.GroupDataPanel.ResumeLayout(false);
            this.ucPanelTitle2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private HZH_Controls.Controls.UCPanelTitle ucPanelTitle1;
        private HZH_Controls.Controls.UCNavigationMenu ucNavigationMenu1;
        private Sunny.UI.UITitlePanel GroupDataPanel;
        private HZH_Controls.Controls.UCPanelTitle ucPanelTitle2;
        private Sunny.UI.UITreeView MyTreeView;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ListView listView1;
    }
}