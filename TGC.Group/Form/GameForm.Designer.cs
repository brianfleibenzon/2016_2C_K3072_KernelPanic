namespace TGC.Group.Form
{
    partial class GameForm
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
            this.panel3D = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.botonSalir = new System.Windows.Forms.Button();
            this.botonJugar = new System.Windows.Forms.Button();
            this.botonx = new System.Windows.Forms.PictureBox();
            this.botonXx = new System.Windows.Forms.PictureBox();
            this.botonx2 = new System.Windows.Forms.PictureBox();
            this.panel3D.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.botonx)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.botonXx)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.botonx2)).BeginInit();
            this.SuspendLayout();
            // 
            // panel3D
            // 
            this.panel3D.BackgroundImage = global::TGC.Group.Properties.Resources.fondo;
            this.panel3D.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel3D.Controls.Add(this.botonx2);
            this.panel3D.Controls.Add(this.panel1);
            this.panel3D.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3D.Location = new System.Drawing.Point(0, 0);
            this.panel3D.Margin = new System.Windows.Forms.Padding(4);
            this.panel3D.Name = "panel3D";
            this.panel3D.Size = new System.Drawing.Size(1045, 690);
            this.panel3D.TabIndex = 0;
            this.panel3D.Paint += new System.Windows.Forms.PaintEventHandler(this.panel3D_Paint);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.botonSalir);
            this.panel1.Controls.Add(this.botonJugar);
            this.panel1.Location = new System.Drawing.Point(198, 54);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(698, 546);
            this.panel1.TabIndex = 22;
            // 
            // botonSalir
            // 
            this.botonSalir.Font = new System.Drawing.Font("Microsoft JhengHei UI Light", 26.25F);
            this.botonSalir.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.botonSalir.Location = new System.Drawing.Point(201, 421);
            this.botonSalir.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.botonSalir.Name = "botonSalir";
            this.botonSalir.Size = new System.Drawing.Size(315, 90);
            this.botonSalir.TabIndex = 22;
            this.botonSalir.Text = "SALIR";
            this.botonSalir.UseVisualStyleBackColor = true;
            this.botonSalir.Click += new System.EventHandler(this.button1_Click);
            // 
            // botonJugar
            // 
            this.botonJugar.Font = new System.Drawing.Font("Microsoft JhengHei UI Light", 26.25F);
            this.botonJugar.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.botonJugar.Location = new System.Drawing.Point(201, 295);
            this.botonJugar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.botonJugar.Name = "botonJugar";
            this.botonJugar.Size = new System.Drawing.Size(315, 90);
            this.botonJugar.TabIndex = 21;
            this.botonJugar.Text = "JUGAR";
            this.botonJugar.UseVisualStyleBackColor = true;
            this.botonJugar.Click += new System.EventHandler(this.botonJugar_Click);
            // 
            // botonx
            // 
            this.botonx.BackColor = System.Drawing.Color.Black;
            this.botonx.BackgroundImage = global::TGC.Group.Properties.Resources.botonXx;
            this.botonx.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.botonx.Location = new System.Drawing.Point(974, 106);
            this.botonx.Name = "botonx";
            this.botonx.Size = new System.Drawing.Size(48, 61);
            this.botonx.TabIndex = 25;
            this.botonx.TabStop = false;
            this.botonx.Click += new System.EventHandler(this.botonX_Click);
            // 
            // botonXx
            // 
            this.botonXx.BackColor = System.Drawing.Color.Black;
            this.botonXx.BackgroundImage = global::TGC.Group.Properties.Resources.botonXx;
            this.botonXx.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.botonXx.Location = new System.Drawing.Point(974, 106);
            this.botonXx.Name = "botonXx";
            this.botonXx.Size = new System.Drawing.Size(48, 61);
            this.botonXx.TabIndex = 25;
            this.botonXx.TabStop = false;
            this.botonXx.Click += new System.EventHandler(this.botonX_Click);
            // 
            // botonx2
            // 
            this.botonx2.BackColor = System.Drawing.Color.Black;
            this.botonx2.BackgroundImage = global::TGC.Group.Properties.Resources.botonXx;
            this.botonx2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.botonx2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonx2.Location = new System.Drawing.Point(1000, 12);
            this.botonx2.Name = "botonx2";
            this.botonx2.Size = new System.Drawing.Size(33, 39);
            this.botonx2.TabIndex = 25;
            this.botonx2.TabStop = false;
            this.botonx2.Click += new System.EventHandler(this.botonX_Click);
            // 
            // GameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1045, 690);
            this.Controls.Add(this.panel3D);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "GameForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GameForm_FormClosing);
            this.Load += new System.EventHandler(this.GameForm_Load);
            this.panel3D.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.botonx)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.botonXx)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.botonx2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel3D;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button botonSalir;
        private System.Windows.Forms.Button botonJugar;
        private System.Windows.Forms.PictureBox botonX;
        private System.Windows.Forms.PictureBox botonx2;
        private System.Windows.Forms.PictureBox botonx;
        private System.Windows.Forms.PictureBox botonXx;
    }
}

