namespace Common.Main
{
	partial class Form1
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.textBoxUrlMex = new System.Windows.Forms.TextBox();
			this.textBoxUrl = new System.Windows.Forms.TextBox();
			this.regetButton = new System.Windows.Forms.Button();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.SaveButton = new System.Windows.Forms.Button();
			this.labelLog = new System.Windows.Forms.Label();
			this.logTextBox = new System.Windows.Forms.TextBox();
			this.labelResponse = new System.Windows.Forms.Label();
			this.listView2 = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.labelRequest = new System.Windows.Forms.Label();
			this.listView1 = new System.Windows.Forms.ListView();
			this.col0 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.Cols1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.Cols2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ClearButton = new System.Windows.Forms.Button();
			this.test2Button = new System.Windows.Forms.Button();
			this.openButton = new System.Windows.Forms.Button();
			this.labelOuput = new System.Windows.Forms.Label();
			this.outputTextBox = new System.Windows.Forms.TextBox();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.runOptionsTextBox = new System.Windows.Forms.TextBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label5 = new System.Windows.Forms.Label();
			this.clientContractComboBox = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.clientBindingComboBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.clientAddressTextBox = new System.Windows.Forms.TextBox();
			this.setDefaultClientButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.clientNameComboBox = new System.Windows.Forms.ComboBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.includeTimeTracingCheckBox = new System.Windows.Forms.CheckBox();
			this.dbFolderTextBox = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.folderTextBox = new System.Windows.Forms.TextBox();
			this.includeWSDLCheckBox = new System.Windows.Forms.CheckBox();
			this.logFolderButton = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.customerReportBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.coilsBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.tabControl1.SuspendLayout();
			this.tabPage3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.customerReportBindingSource)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.coilsBindingSource)).BeginInit();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(1473, 896);
			this.tabControl1.TabIndex = 20;
			// 
			// tabPage3
			// 
			this.tabPage3.BackColor = System.Drawing.Color.SlateGray;
			this.tabPage3.Controls.Add(this.textBoxUrlMex);
			this.tabPage3.Controls.Add(this.textBoxUrl);
			this.tabPage3.Controls.Add(this.regetButton);
			this.tabPage3.Controls.Add(this.dataGridView1);
			this.tabPage3.Location = new System.Drawing.Point(4, 25);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage3.Size = new System.Drawing.Size(1465, 867);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "TEST";
			// 
			// textBoxUrlMex
			// 
			this.textBoxUrlMex.Location = new System.Drawing.Point(24, 71);
			this.textBoxUrlMex.Multiline = true;
			this.textBoxUrlMex.Name = "textBoxUrlMex";
			this.textBoxUrlMex.Size = new System.Drawing.Size(1412, 116);
			this.textBoxUrlMex.TabIndex = 19;
			this.textBoxUrlMex.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
			// 
			// textBoxUrl
			// 
			this.textBoxUrl.Location = new System.Drawing.Point(24, 30);
			this.textBoxUrl.Name = "textBoxUrl";
			this.textBoxUrl.Size = new System.Drawing.Size(1412, 22);
			this.textBoxUrl.TabIndex = 18;
			// 
			// regetButton
			// 
			this.regetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.regetButton.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
			this.regetButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.regetButton.Location = new System.Drawing.Point(584, 193);
			this.regetButton.Margin = new System.Windows.Forms.Padding(4);
			this.regetButton.Name = "regetButton";
			this.regetButton.Size = new System.Drawing.Size(119, 46);
			this.regetButton.TabIndex = 17;
			this.regetButton.Text = "REGET";
			this.regetButton.UseVisualStyleBackColor = false;
			this.regetButton.Visible = false;
			this.regetButton.Click += new System.EventHandler(this.regetButton_Click);
			// 
			// dataGridView1
			// 
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Location = new System.Drawing.Point(24, 193);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.RowTemplate.Height = 24;
			this.dataGridView1.Size = new System.Drawing.Size(543, 406);
			this.dataGridView1.TabIndex = 0;
			this.dataGridView1.Visible = false;
			// 
			// tabPage1
			// 
			this.tabPage1.BackgroundImage = global::Count4U.WindowsHost.Properties.Resources.Home;
			this.tabPage1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.tabPage1.Controls.Add(this.SaveButton);
			this.tabPage1.Controls.Add(this.labelLog);
			this.tabPage1.Controls.Add(this.logTextBox);
			this.tabPage1.Controls.Add(this.labelResponse);
			this.tabPage1.Controls.Add(this.listView2);
			this.tabPage1.Controls.Add(this.labelRequest);
			this.tabPage1.Controls.Add(this.listView1);
			this.tabPage1.Controls.Add(this.ClearButton);
			this.tabPage1.Controls.Add(this.test2Button);
			this.tabPage1.Controls.Add(this.openButton);
			this.tabPage1.Controls.Add(this.labelOuput);
			this.tabPage1.Controls.Add(this.outputTextBox);
			this.tabPage1.ForeColor = System.Drawing.Color.Black;
			this.tabPage1.Location = new System.Drawing.Point(4, 25);
			this.tabPage1.Margin = new System.Windows.Forms.Padding(4);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(4);
			this.tabPage1.Size = new System.Drawing.Size(1465, 867);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "MONITOR";
			this.tabPage1.UseVisualStyleBackColor = true;
			this.tabPage1.Click += new System.EventHandler(this.tabPage1_Click);
			// 
			// SaveButton
			// 
			this.SaveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.SaveButton.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
			this.SaveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.SaveButton.Location = new System.Drawing.Point(1333, 201);
			this.SaveButton.Margin = new System.Windows.Forms.Padding(4);
			this.SaveButton.Name = "SaveButton";
			this.SaveButton.Size = new System.Drawing.Size(119, 46);
			this.SaveButton.TabIndex = 25;
			this.SaveButton.Text = "SAVE";
			this.SaveButton.UseVisualStyleBackColor = false;
			this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
			// 
			// labelLog
			// 
			this.labelLog.AutoSize = true;
			this.labelLog.BackColor = System.Drawing.Color.Transparent;
			this.labelLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.labelLog.ForeColor = System.Drawing.Color.Black;
			this.labelLog.Location = new System.Drawing.Point(72, 135);
			this.labelLog.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labelLog.Name = "labelLog";
			this.labelLog.Size = new System.Drawing.Size(38, 17);
			this.labelLog.TabIndex = 24;
			this.labelLog.Text = "LOG";
			this.labelLog.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelLog.Click += new System.EventHandler(this.labelLog_Click);
			// 
			// logTextBox
			// 
			this.logTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.logTextBox.Location = new System.Drawing.Point(124, 135);
			this.logTextBox.Margin = new System.Windows.Forms.Padding(4);
			this.logTextBox.Multiline = true;
			this.logTextBox.Name = "logTextBox";
			this.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.logTextBox.Size = new System.Drawing.Size(1187, 714);
			this.logTextBox.TabIndex = 23;
			// 
			// labelResponse
			// 
			this.labelResponse.AutoSize = true;
			this.labelResponse.BackColor = System.Drawing.Color.Transparent;
			this.labelResponse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.labelResponse.Location = new System.Drawing.Point(25, 356);
			this.labelResponse.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labelResponse.Name = "labelResponse";
			this.labelResponse.Size = new System.Drawing.Size(84, 17);
			this.labelResponse.TabIndex = 22;
			this.labelResponse.Text = "RESPONSE";
			this.labelResponse.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelResponse.Visible = false;
			// 
			// listView2
			// 
			this.listView2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listView2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
			this.listView2.FullRowSelect = true;
			this.listView2.GridLines = true;
			this.listView2.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listView2.Location = new System.Drawing.Point(124, 356);
			this.listView2.Margin = new System.Windows.Forms.Padding(4);
			this.listView2.Name = "listView2";
			this.listView2.Size = new System.Drawing.Size(1187, 263);
			this.listView2.TabIndex = 21;
			this.listView2.UseCompatibleStateImageBehavior = false;
			this.listView2.View = System.Windows.Forms.View.Details;
			this.listView2.Visible = false;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Operation";
			this.columnHeader1.Width = 121;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Name";
			this.columnHeader2.Width = 120;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Value";
			this.columnHeader3.Width = 650;
			// 
			// labelRequest
			// 
			this.labelRequest.AutoSize = true;
			this.labelRequest.BackColor = System.Drawing.Color.Transparent;
			this.labelRequest.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.labelRequest.Location = new System.Drawing.Point(35, 135);
			this.labelRequest.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labelRequest.Name = "labelRequest";
			this.labelRequest.Size = new System.Drawing.Size(75, 17);
			this.labelRequest.TabIndex = 20;
			this.labelRequest.Text = "REQUEST";
			this.labelRequest.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelRequest.Visible = false;
			// 
			// listView1
			// 
			this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.col0,
            this.Cols1,
            this.Cols2});
			this.listView1.FullRowSelect = true;
			this.listView1.GridLines = true;
			this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listView1.Location = new System.Drawing.Point(124, 135);
			this.listView1.Margin = new System.Windows.Forms.Padding(4);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(1187, 203);
			this.listView1.TabIndex = 19;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			this.listView1.Visible = false;
			// 
			// col0
			// 
			this.col0.Text = "Operation";
			this.col0.Width = 120;
			// 
			// Cols1
			// 
			this.Cols1.Text = "Name";
			this.Cols1.Width = 120;
			// 
			// Cols2
			// 
			this.Cols2.Text = "Value";
			this.Cols2.Width = 650;
			// 
			// ClearButton
			// 
			this.ClearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ClearButton.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
			this.ClearButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ClearButton.Location = new System.Drawing.Point(1333, 135);
			this.ClearButton.Margin = new System.Windows.Forms.Padding(4);
			this.ClearButton.Name = "ClearButton";
			this.ClearButton.Size = new System.Drawing.Size(119, 46);
			this.ClearButton.TabIndex = 18;
			this.ClearButton.Text = "CLEAR";
			this.ClearButton.UseVisualStyleBackColor = false;
			this.ClearButton.Click += new System.EventHandler(this.ClearButton_Click);
			// 
			// test2Button
			// 
			this.test2Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.test2Button.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
			this.test2Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.test2Button.Location = new System.Drawing.Point(1333, 73);
			this.test2Button.Margin = new System.Windows.Forms.Padding(4);
			this.test2Button.Name = "test2Button";
			this.test2Button.Size = new System.Drawing.Size(119, 46);
			this.test2Button.TabIndex = 17;
			this.test2Button.Text = "STOP";
			this.test2Button.UseVisualStyleBackColor = false;
			this.test2Button.Click += new System.EventHandler(this.test2Button_Click);
			// 
			// openButton
			// 
			this.openButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.openButton.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
			this.openButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.openButton.Location = new System.Drawing.Point(1333, 15);
			this.openButton.Margin = new System.Windows.Forms.Padding(4);
			this.openButton.Name = "openButton";
			this.openButton.Size = new System.Drawing.Size(119, 46);
			this.openButton.TabIndex = 16;
			this.openButton.Text = "START";
			this.openButton.UseVisualStyleBackColor = false;
			this.openButton.Click += new System.EventHandler(this.openButton_Click);
			// 
			// labelOuput
			// 
			this.labelOuput.AutoSize = true;
			this.labelOuput.BackColor = System.Drawing.Color.Transparent;
			this.labelOuput.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.labelOuput.ForeColor = System.Drawing.Color.Black;
			this.labelOuput.Location = new System.Drawing.Point(47, 15);
			this.labelOuput.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labelOuput.Name = "labelOuput";
			this.labelOuput.Size = new System.Drawing.Size(63, 17);
			this.labelOuput.TabIndex = 15;
			this.labelOuput.Text = "RESULT";
			this.labelOuput.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// outputTextBox
			// 
			this.outputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.outputTextBox.Location = new System.Drawing.Point(124, 15);
			this.outputTextBox.Margin = new System.Windows.Forms.Padding(4);
			this.outputTextBox.Multiline = true;
			this.outputTextBox.Name = "outputTextBox";
			this.outputTextBox.Size = new System.Drawing.Size(1187, 102);
			this.outputTextBox.TabIndex = 14;
			// 
			// tabPage2
			// 
			this.tabPage2.BackgroundImage = global::Count4U.WindowsHost.Properties.Resources.Inventor;
			this.tabPage2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.tabPage2.Controls.Add(this.groupBox3);
			this.tabPage2.Controls.Add(this.groupBox2);
			this.tabPage2.Controls.Add(this.groupBox1);
			this.tabPage2.Location = new System.Drawing.Point(4, 25);
			this.tabPage2.Margin = new System.Windows.Forms.Padding(4);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(4);
			this.tabPage2.Size = new System.Drawing.Size(1465, 867);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "OPTIONS";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.runOptionsTextBox);
			this.groupBox3.Location = new System.Drawing.Point(29, 379);
			this.groupBox3.Margin = new System.Windows.Forms.Padding(4);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Padding = new System.Windows.Forms.Padding(4);
			this.groupBox3.Size = new System.Drawing.Size(979, 161);
			this.groupBox3.TabIndex = 34;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "RUN LIKE";
			this.groupBox3.Visible = false;
			// 
			// runOptionsTextBox
			// 
			this.runOptionsTextBox.Location = new System.Drawing.Point(93, 30);
			this.runOptionsTextBox.Margin = new System.Windows.Forms.Padding(4);
			this.runOptionsTextBox.Multiline = true;
			this.runOptionsTextBox.Name = "runOptionsTextBox";
			this.runOptionsTextBox.ReadOnly = true;
			this.runOptionsTextBox.Size = new System.Drawing.Size(807, 111);
			this.runOptionsTextBox.TabIndex = 0;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.clientContractComboBox);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.clientBindingComboBox);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.clientAddressTextBox);
			this.groupBox2.Controls.Add(this.setDefaultClientButton);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.clientNameComboBox);
			this.groupBox2.Location = new System.Drawing.Point(24, 174);
			this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
			this.groupBox2.Size = new System.Drawing.Size(984, 191);
			this.groupBox2.TabIndex = 33;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "REMOTE SERVER";
			this.groupBox2.Visible = false;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.BackColor = System.Drawing.Color.Transparent;
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label5.Location = new System.Drawing.Point(1, 151);
			this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(84, 17);
			this.label5.TabIndex = 36;
			this.label5.Text = "CONTRACT";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// clientContractComboBox
			// 
			this.clientContractComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.clientContractComboBox.Location = new System.Drawing.Point(99, 149);
			this.clientContractComboBox.Margin = new System.Windows.Forms.Padding(4);
			this.clientContractComboBox.Name = "clientContractComboBox";
			this.clientContractComboBox.ReadOnly = true;
			this.clientContractComboBox.Size = new System.Drawing.Size(807, 22);
			this.clientContractComboBox.TabIndex = 35;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.BackColor = System.Drawing.Color.Transparent;
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label4.Location = new System.Drawing.Point(20, 112);
			this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(64, 17);
			this.label4.TabIndex = 34;
			this.label4.Text = "BINDING";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// clientBindingComboBox
			// 
			this.clientBindingComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.clientBindingComboBox.Location = new System.Drawing.Point(99, 110);
			this.clientBindingComboBox.Margin = new System.Windows.Forms.Padding(4);
			this.clientBindingComboBox.Name = "clientBindingComboBox";
			this.clientBindingComboBox.ReadOnly = true;
			this.clientBindingComboBox.Size = new System.Drawing.Size(807, 22);
			this.clientBindingComboBox.TabIndex = 33;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.BackColor = System.Drawing.Color.Transparent;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label3.Location = new System.Drawing.Point(11, 73);
			this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(74, 17);
			this.label3.TabIndex = 32;
			this.label3.Text = "ADDRESS";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// clientAddressTextBox
			// 
			this.clientAddressTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.clientAddressTextBox.Location = new System.Drawing.Point(99, 70);
			this.clientAddressTextBox.Margin = new System.Windows.Forms.Padding(4);
			this.clientAddressTextBox.Name = "clientAddressTextBox";
			this.clientAddressTextBox.ReadOnly = true;
			this.clientAddressTextBox.Size = new System.Drawing.Size(807, 22);
			this.clientAddressTextBox.TabIndex = 31;
			// 
			// setDefaultClientButton
			// 
			this.setDefaultClientButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.setDefaultClientButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.setDefaultClientButton.Image = ((System.Drawing.Image)(resources.GetObject("setDefaultClientButton.Image")));
			this.setDefaultClientButton.Location = new System.Drawing.Point(915, 31);
			this.setDefaultClientButton.Margin = new System.Windows.Forms.Padding(4);
			this.setDefaultClientButton.Name = "setDefaultClientButton";
			this.setDefaultClientButton.Size = new System.Drawing.Size(48, 25);
			this.setDefaultClientButton.TabIndex = 30;
			this.setDefaultClientButton.UseVisualStyleBackColor = true;
			this.setDefaultClientButton.Visible = false;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label1.Location = new System.Drawing.Point(39, 33);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(47, 17);
			this.label1.TabIndex = 29;
			this.label1.Text = "NAME";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// clientNameComboBox
			// 
			this.clientNameComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.clientNameComboBox.FormattingEnabled = true;
			this.clientNameComboBox.Location = new System.Drawing.Point(99, 30);
			this.clientNameComboBox.Margin = new System.Windows.Forms.Padding(4);
			this.clientNameComboBox.Name = "clientNameComboBox";
			this.clientNameComboBox.Size = new System.Drawing.Size(807, 24);
			this.clientNameComboBox.TabIndex = 0;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.includeTimeTracingCheckBox);
			this.groupBox1.Controls.Add(this.dbFolderTextBox);
			this.groupBox1.Controls.Add(this.button1);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.folderTextBox);
			this.groupBox1.Controls.Add(this.includeWSDLCheckBox);
			this.groupBox1.Controls.Add(this.logFolderButton);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Location = new System.Drawing.Point(24, 18);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
			this.groupBox1.Size = new System.Drawing.Size(984, 142);
			this.groupBox1.TabIndex = 32;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "LOG";
			// 
			// includeTimeTracingCheckBox
			// 
			this.includeTimeTracingCheckBox.AutoSize = true;
			this.includeTimeTracingCheckBox.Checked = true;
			this.includeTimeTracingCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.includeTimeTracingCheckBox.Location = new System.Drawing.Point(295, 68);
			this.includeTimeTracingCheckBox.Margin = new System.Windows.Forms.Padding(4);
			this.includeTimeTracingCheckBox.Name = "includeTimeTracingCheckBox";
			this.includeTimeTracingCheckBox.Size = new System.Drawing.Size(162, 21);
			this.includeTimeTracingCheckBox.TabIndex = 35;
			this.includeTimeTracingCheckBox.Text = "Include Time Tracing";
			this.includeTimeTracingCheckBox.UseVisualStyleBackColor = true;
			// 
			// dbFolderTextBox
			// 
			this.dbFolderTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dbFolderTextBox.Location = new System.Drawing.Point(97, 98);
			this.dbFolderTextBox.Margin = new System.Windows.Forms.Padding(4);
			this.dbFolderTextBox.Name = "dbFolderTextBox";
			this.dbFolderTextBox.ReadOnly = true;
			this.dbFolderTextBox.Size = new System.Drawing.Size(807, 22);
			this.dbFolderTextBox.TabIndex = 34;
			this.dbFolderTextBox.TextChanged += new System.EventHandler(this.dbFolderTextBox_TextChanged);
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button1.Image = global::Count4U.WindowsHost.Properties.Resources.folder;
			this.button1.Location = new System.Drawing.Point(913, 98);
			this.button1.Margin = new System.Windows.Forms.Padding(4);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(48, 25);
			this.button1.TabIndex = 33;
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.BackColor = System.Drawing.Color.Transparent;
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label6.Location = new System.Drawing.Point(61, 102);
			this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(27, 17);
			this.label6.TabIndex = 32;
			this.label6.Text = "DB";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.label6.Click += new System.EventHandler(this.label6_Click);
			// 
			// folderTextBox
			// 
			this.folderTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.folderTextBox.Location = new System.Drawing.Point(99, 30);
			this.folderTextBox.Margin = new System.Windows.Forms.Padding(4);
			this.folderTextBox.Name = "folderTextBox";
			this.folderTextBox.ReadOnly = true;
			this.folderTextBox.Size = new System.Drawing.Size(807, 22);
			this.folderTextBox.TabIndex = 30;
			// 
			// includeWSDLCheckBox
			// 
			this.includeWSDLCheckBox.AutoSize = true;
			this.includeWSDLCheckBox.Location = new System.Drawing.Point(99, 68);
			this.includeWSDLCheckBox.Margin = new System.Windows.Forms.Padding(4);
			this.includeWSDLCheckBox.Name = "includeWSDLCheckBox";
			this.includeWSDLCheckBox.Size = new System.Drawing.Size(136, 21);
			this.includeWSDLCheckBox.TabIndex = 31;
			this.includeWSDLCheckBox.Text = "Include Message";
			this.includeWSDLCheckBox.UseVisualStyleBackColor = true;
			// 
			// logFolderButton
			// 
			this.logFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.logFolderButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.logFolderButton.Image = global::Count4U.WindowsHost.Properties.Resources.folder;
			this.logFolderButton.Location = new System.Drawing.Point(915, 30);
			this.logFolderButton.Margin = new System.Windows.Forms.Padding(4);
			this.logFolderButton.Name = "logFolderButton";
			this.logFolderButton.Size = new System.Drawing.Size(48, 25);
			this.logFolderButton.TabIndex = 29;
			this.logFolderButton.UseVisualStyleBackColor = true;
			this.logFolderButton.Click += new System.EventHandler(this.logFolderButton_Click_1);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.BackColor = System.Drawing.Color.Transparent;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label2.Location = new System.Drawing.Point(24, 33);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(64, 17);
			this.label2.TabIndex = 28;
			this.label2.Text = "FOLDER";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// customerReportBindingSource
			// 
			this.customerReportBindingSource.DataMember = "CustomerReport";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ClientSize = new System.Drawing.Size(1473, 896);
			this.Controls.Add(this.tabControl1);
			this.DoubleBuffered = true;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "MONITOR \\ REMOTE SERVER COUNT4U";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.tabControl1.ResumeLayout(false);
			this.tabPage3.ResumeLayout(false);
			this.tabPage3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this.tabPage2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.customerReportBindingSource)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.coilsBindingSource)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridViewTextBoxColumn iDDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn coilCodeDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn coilNumberDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn operatorNameDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn jobNameDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn autoStartDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn autoStopDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn lengthDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn panelNumberDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn pieceDescriptionDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn operationDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn totalLengthDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn totalScrapDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn ticksTimeSpanDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn domainCodeDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn percentScrapDataGridViewTextBoxColumn;
		private System.Windows.Forms.BindingSource coilsBindingSource;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Label labelOuput;
		public System.Windows.Forms.TextBox outputTextBox;
		private System.Windows.Forms.Label labelLog;
		public System.Windows.Forms.TextBox logTextBox;
		private System.Windows.Forms.Label labelResponse;
		public System.Windows.Forms.ListView listView2;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.Label labelRequest;
		public System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader col0;
		private System.Windows.Forms.ColumnHeader Cols1;
		private System.Windows.Forms.ColumnHeader Cols2;
		private System.Windows.Forms.Button ClearButton;
		private System.Windows.Forms.Button test2Button;
		private System.Windows.Forms.Button openButton;
		public System.Windows.Forms.TextBox folderTextBox;
		private System.Windows.Forms.Button logFolderButton;
		private System.Windows.Forms.Label label2;
		public System.Windows.Forms.CheckBox includeWSDLCheckBox;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button setDefaultClientButton;
		private System.Windows.Forms.Label label1;
		public System.Windows.Forms.ComboBox clientNameComboBox;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label3;
		public System.Windows.Forms.TextBox clientAddressTextBox;
		private System.Windows.Forms.Label label5;
		public System.Windows.Forms.TextBox clientContractComboBox;
		private System.Windows.Forms.Label label4;
		public System.Windows.Forms.TextBox clientBindingComboBox;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.TextBox runOptionsTextBox;
		private System.Windows.Forms.Button SaveButton;
		public System.Windows.Forms.TextBox dbFolderTextBox;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label6;
		public System.Windows.Forms.CheckBox includeTimeTracingCheckBox;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.BindingSource customerReportBindingSource;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
		private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn descriptionDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn reportCodeDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn customerCodeDataGridViewTextBoxColumn;
		private System.Windows.Forms.Button regetButton;
		private System.Windows.Forms.TextBox textBoxUrlMex;
		private System.Windows.Forms.TextBox textBoxUrl;
	}
}