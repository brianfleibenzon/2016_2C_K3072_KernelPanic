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
            this.botonX = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblResultado = new System.Windows.Forms.Label();
            this.botonSalir = new System.Windows.Forms.Button();
            this.botonJugar = new System.Windows.Forms.Button();
            this.panel3D.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.botonX)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel3D
            // 
            this.panel3D.BackColor = System.Drawing.Color.Transparent;
            this.panel3D.Controls.Add(this.botonX);
            this.panel3D.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3D.Location = new System.Drawing.Point(0, 0);
            this.panel3D.Name = "panel3D";
            this.panel3D.Size = new System.Drawing.Size(784, 561);
            this.panel3D.TabIndex = 0;
            this.panel3D.Paint += new System.Windows.Forms.PaintEventHandler(this.panel3D_Paint);
            // 
            // botonX
            // 
            this.botonX.BackColor = System.Drawing.Color.Transparent;
            this.botonX.BackgroundImage = global::TGC.Group.Properties.Resources.botonXx;
            this.botonX.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.botonX.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonX.Location = new System.Drawing.Point(750, 10);
            this.botonX.Margin = new System.Windows.Forms.Padding(2);
            this.botonX.Name = "botonX";
            this.botonX.Size = new System.Drawing.Size(25, 32);
            this.botonX.TabIndex = 25;
            this.botonX.TabStop = false;
            this.botonX.Visible = false;
            this.botonX.Click += new System.EventHandler(this.botonX_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.lblResultado);
            this.panel1.Controls.Add(this.botonSalir);
            this.panel1.Controls.Add(this.botonJugar);
            this.panel1.Location = new System.Drawing.Point(148, 44);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(524, 444);
            this.panel1.TabIndex = 22;
            // 
            // lblResultado
            // 
            this.lblResultado.Font = new System.Drawing.Font("Microsoft Sans Serif", 72F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResultado.ForeColor = System.Drawing.Color.Red;
            this.lblResultado.Location = new System.Drawing.Point(28, 12);
            this.lblResultado.Name = "lblResultado";
            this.lblResultado.Size = new System.Drawing.Size(469, 114);
            this.lblResultado.TabIndex = 23;
            this.lblResultado.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // botonSalir
            // 
            this.botonSalir.Font = new System.Drawing.Font("Microsoft JhengHei UI Light", 26.25F);
            this.botonSalir.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.botonSalir.Location = new System.Drawing.Point(151, 342);
            this.botonSalir.Margin = new System.Windows.Forms.Padding(2);
            this.botonSalir.Name = "botonSalir";
            this.botonSalir.Size = new System.Drawing.Size(236, 73);
            this.botonSalir.TabIndex = 22;
            this.botonSalir.Text = "SALIR";
            this.botonSalir.UseVisualStyleBackColor = true;
            this.botonSalir.Click += new System.EventHandler(this.botonSalir_Click);
            // 
            // botonJugar
            // 
            this.botonJugar.Font = new System.Drawing.Font("Microsoft JhengHei UI Light", 26.25F);
            this.botonJugar.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.botonJugar.Location = new System.Drawing.Point(151, 240);
            this.botonJugar.Margin = new System.Windows.Forms.Padding(2);
            this.botonJugar.Name = "botonJugar";
            this.botonJugar.Size = new System.Drawing.Size(236, 73);
            this.botonJugar.TabIndex = 21;
            this.botonJugar.Text = "JUGAR";
            this.botonJugar.UseVisualStyleBackColor = true;
            this.botonJugar.Click += new System.EventHandler(this.botonJugar_Click);
            // 
            // GameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TGC.Group.Properties.Resources.fondo;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3D);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "GameForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GameForm_FormClosing);
            this.Load += new System.EventHandler(this.GameForm_Load);
            this.panel3D.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.botonX)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel3D;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button botonSalir;
        private System.Windows.Forms.Button botonJugar;
        private System.Windows.Forms.PictureBox botonX;
        private System.Windows.Forms.Label lblResultado;
    }
}

