using System;

namespace Lab1
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.loadButton = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.thresholdSlider = new System.Windows.Forms.TrackBar();
            this.OKbutton = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.button7 = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.фильтрыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.точечныеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.матричныеToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.комбинированныеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.цветокоррекцияToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.матморфологияToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.thresholdSlider)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Location = new System.Drawing.Point(16, 24);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1035, 449);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.DragDrop += new System.Windows.Forms.DragEventHandler(this.pictureBox1_DragDrop);
            this.pictureBox1.DragEnter += new System.Windows.Forms.DragEventHandler(this.pictureBox1_DragEnter);
            this.pictureBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseClick);
            // 
            // loadButton
            // 
            this.loadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.loadButton.Location = new System.Drawing.Point(16, 496);
            this.loadButton.Margin = new System.Windows.Forms.Padding(4);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(100, 43);
            this.loadButton.TabIndex = 2;
            this.loadButton.Text = "Загрузить";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.LoadImageButton_Click);
            // 
            // button5
            // 
            this.button5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button5.Location = new System.Drawing.Point(122, 496);
            this.button5.Margin = new System.Windows.Forms.Padding(4);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(100, 43);
            this.button5.TabIndex = 7;
            this.button5.Text = "Сохранить";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // thresholdSlider
            // 
            this.thresholdSlider.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.thresholdSlider.Location = new System.Drawing.Point(712, 496);
            this.thresholdSlider.Maximum = 256;
            this.thresholdSlider.Name = "thresholdSlider";
            this.thresholdSlider.Size = new System.Drawing.Size(269, 56);
            this.thresholdSlider.TabIndex = 12;
            this.thresholdSlider.TickFrequency = 128;
            this.thresholdSlider.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.thresholdSlider.Value = 128;
            this.thresholdSlider.ValueChanged += new System.EventHandler(this.SliderChange);
            // 
            // OKbutton
            // 
            this.OKbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OKbutton.Location = new System.Drawing.Point(988, 496);
            this.OKbutton.Margin = new System.Windows.Forms.Padding(4);
            this.OKbutton.Name = "OKbutton";
            this.OKbutton.Size = new System.Drawing.Size(63, 43);
            this.OKbutton.TabIndex = 14;
            this.OKbutton.Text = "OK";
            this.OKbutton.UseVisualStyleBackColor = true;
            this.OKbutton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(0, 0);
            this.progressBar1.Maximum = 1000;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(1067, 7);
            this.progressBar1.Step = 1;
            this.progressBar1.TabIndex = 15;
            // 
            // textBox3
            // 
            this.textBox3.AllowDrop = true;
            this.textBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox3.Location = new System.Drawing.Point(679, 507);
            this.textBox3.Margin = new System.Windows.Forms.Padding(4);
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(36, 22);
            this.textBox3.TabIndex = 16;
            this.textBox3.Text = "125";
            this.textBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // button7
            // 
            this.button7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button7.Location = new System.Drawing.Point(230, 496);
            this.button7.Margin = new System.Windows.Forms.Padding(4);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(108, 43);
            this.button7.TabIndex = 17;
            this.button7.Text = "Восстановить";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.RestoreButton_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.фильтрыToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(467, 496);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(198, 44);
            this.menuStrip1.TabIndex = 18;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // фильтрыToolStripMenuItem
            // 
            this.фильтрыToolStripMenuItem.AutoSize = false;
            this.фильтрыToolStripMenuItem.BackColor = System.Drawing.SystemColors.ControlLight;
            this.фильтрыToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.точечныеToolStripMenuItem,
            this.матричныеToolStripMenuItem1,
            this.комбинированныеToolStripMenuItem,
            this.цветокоррекцияToolStripMenuItem,
            this.матморфологияToolStripMenuItem});
            this.фильтрыToolStripMenuItem.Name = "фильтрыToolStripMenuItem";
            this.фильтрыToolStripMenuItem.Size = new System.Drawing.Size(190, 40);
            this.фильтрыToolStripMenuItem.Text = "Фильтры";
            // 
            // точечныеToolStripMenuItem
            // 
            this.точечныеToolStripMenuItem.Name = "точечныеToolStripMenuItem";
            this.точечныеToolStripMenuItem.Size = new System.Drawing.Size(228, 26);
            this.точечныеToolStripMenuItem.Text = "Точечные";
            this.точечныеToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ToolStripMenuItem_DropDownItemClicked);
            // 
            // матричныеToolStripMenuItem1
            // 
            this.матричныеToolStripMenuItem1.Name = "матричныеToolStripMenuItem1";
            this.матричныеToolStripMenuItem1.Size = new System.Drawing.Size(228, 26);
            this.матричныеToolStripMenuItem1.Text = "Матричные";
            this.матричныеToolStripMenuItem1.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ToolStripMenuItem_DropDownItemClicked);
            // 
            // комбинированныеToolStripMenuItem
            // 
            this.комбинированныеToolStripMenuItem.Name = "комбинированныеToolStripMenuItem";
            this.комбинированныеToolStripMenuItem.Size = new System.Drawing.Size(228, 26);
            this.комбинированныеToolStripMenuItem.Text = "Комбинированные";
            this.комбинированныеToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ToolStripMenuItem_DropDownItemClicked);
            // 
            // цветокоррекцияToolStripMenuItem
            // 
            this.цветокоррекцияToolStripMenuItem.Name = "цветокоррекцияToolStripMenuItem";
            this.цветокоррекцияToolStripMenuItem.Size = new System.Drawing.Size(228, 26);
            this.цветокоррекцияToolStripMenuItem.Text = "Цветокоррекция";
            this.цветокоррекцияToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ToolStripMenuItem_DropDownItemClicked);
            // 
            // матморфологияToolStripMenuItem
            // 
            this.матморфологияToolStripMenuItem.Name = "матморфологияToolStripMenuItem";
            this.матморфологияToolStripMenuItem.Size = new System.Drawing.Size(228, 26);
            this.матморфологияToolStripMenuItem.Text = "Матморфология";
            this.матморфологияToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ToolStripMenuItem_DropDownItemClicked);
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1067, 554);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.OKbutton);
            this.Controls.Add(this.thresholdSlider);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.loadButton);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.menuStrip1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(1060, 400);
            this.Name = "Form1";
            this.ShowIcon = false;
            this.Text = "Nazvanie";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.thresholdSlider)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.TrackBar thresholdSlider;
        private System.Windows.Forms.Button OKbutton;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem фильтрыToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem точечныеToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem матричныеToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem комбинированныеToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem цветокоррекцияToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem матморфологияToolStripMenuItem;
    }
}

