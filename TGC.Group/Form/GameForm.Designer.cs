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
            this.btnControles = new System.Windows.Forms.Button();
            this.lblResultado = new System.Windows.Forms.Label();
            this.botonSalir = new System.Windows.Forms.Button();
            this.botonJugar = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnVolver = new System.Windows.Forms.Button();
            this.panel3D.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.botonX)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel3D
            // 
            this.panel3D.BackColor = System.Drawing.Color.Transparent;
            this.panel3D.Controls.Add(this.botonX);
            this.panel3D.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3D.Location = new System.Drawing.Point(0, 0);
            this.panel3D.Margin = new System.Windows.Forms.Padding(4);
            this.panel3D.Name = "panel3D";
            this.panel3D.Size = new System.Drawing.Size(1045, 690);
            this.panel3D.TabIndex = 0;
            this.panel3D.Visible = false;
            // 
            // botonX
            // 
            this.botonX.BackColor = System.Drawing.Color.Transparent;
            this.botonX.BackgroundImage = global::TGC.Group.Properties.Resources.botonXx;
            this.botonX.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.botonX.Cursor = System.Windows.Forms.Cursors.Hand;
            this.botonX.Location = new System.Drawing.Point(1000, 12);
            this.botonX.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.botonX.Name = "botonX";
            this.botonX.Size = new System.Drawing.Size(33, 39);
            this.botonX.TabIndex = 25;
            this.botonX.TabStop = false;
            this.botonX.Visible = false;
            this.botonX.Click += new System.EventHandler(this.botonX_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.btnControles);
            this.panel1.Controls.Add(this.lblResultado);
            this.panel1.Controls.Add(this.botonSalir);
            this.panel1.Controls.Add(this.botonJugar);
            this.panel1.Location = new System.Drawing.Point(197, 54);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(699, 546);
            this.panel1.TabIndex = 22;
            // 
            // btnControles
            // 
            this.btnControles.Font = new System.Drawing.Font("Chiller", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnControles.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnControles.Location = new System.Drawing.Point(201, 367);
            this.btnControles.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnControles.Name = "btnControles";
            this.btnControles.Size = new System.Drawing.Size(315, 66);
            this.btnControles.TabIndex = 1;
            this.btnControles.Text = "Controles";
            this.btnControles.UseVisualStyleBackColor = true;
            this.btnControles.Click += new System.EventHandler(this.btnControles_Click);
            // 
            // lblResultado
            // 
            this.lblResultado.Font = new System.Drawing.Font("Chiller", 72F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResultado.ForeColor = System.Drawing.Color.Red;
            this.lblResultado.Location = new System.Drawing.Point(4, 15);
            this.lblResultado.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblResultado.Name = "lblResultado";
            this.lblResultado.Size = new System.Drawing.Size(691, 140);
            this.lblResultado.TabIndex = 23;
            this.lblResultado.Text = "Kernel Panic";
            this.lblResultado.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // botonSalir
            // 
            this.botonSalir.Font = new System.Drawing.Font("Chiller", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.botonSalir.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.botonSalir.Location = new System.Drawing.Point(201, 438);
            this.botonSalir.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.botonSalir.Name = "botonSalir";
            this.botonSalir.Size = new System.Drawing.Size(315, 66);
            this.botonSalir.TabIndex = 2;
            this.botonSalir.Text = "Salir";
            this.botonSalir.UseVisualStyleBackColor = true;
            this.botonSalir.Click += new System.EventHandler(this.botonSalir_Click);
            // 
            // botonJugar
            // 
            this.botonJugar.Font = new System.Drawing.Font("Chiller", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.botonJugar.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.botonJugar.Location = new System.Drawing.Point(201, 295);
            this.botonJugar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.botonJugar.Name = "botonJugar";
            this.botonJugar.Size = new System.Drawing.Size(315, 66);
            this.botonJugar.TabIndex = 0;
            this.botonJugar.Text = "Jugar";
            this.botonJugar.UseVisualStyleBackColor = true;
            this.botonJugar.Click += new System.EventHandler(this.botonJugar_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.btnVolver);
            this.panel2.Location = new System.Drawing.Point(197, 54);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(699, 546);
            this.panel2.TabIndex = 24;
            this.panel2.Visible = false;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Chiller", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(163, 15);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(480, 400);
            this.label2.TabIndex = 24;
            this.label2.Text = "- Mover la cámara\r\n- Mover hacia adelante\r\n- Mover hacia la izquierda\r\n- Mover ha" +
    "cia atras\r\n- Mover hacia la derecha\r\n- Encender / Apagar iluminación\r\n- Esconder" +
    "se\r\n- Pausa";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Chiller", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(40, 15);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 400);
            this.label1.TabIndex = 23;
            this.label1.Text = "Mouse\r\nW\r\nA\r\nS\r\nD\r\nF\r\nCtrl\r\nEsc";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // btnVolver
            // 
            this.btnVolver.Font = new System.Drawing.Font("Chiller", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVolver.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnVolver.Location = new System.Drawing.Point(201, 438);
            this.btnVolver.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnVolver.Name = "btnVolver";
            this.btnVolver.Size = new System.Drawing.Size(315, 66);
            this.btnVolver.TabIndex = 0;
            this.btnVolver.Text = "Volver";
            this.btnVolver.UseVisualStyleBackColor = true;
            this.btnVolver.Click += new System.EventHandler(this.btnVolver_Click);
            // 
            // GameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TGC.Group.Properties.Resources.fondo1;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1045, 690);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3D);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "GameForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GameForm_FormClosing);
            this.Load += new System.EventHandler(this.GameForm_Load);
            this.panel3D.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.botonX)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel3D;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button botonSalir;
        private System.Windows.Forms.Button botonJugar;
        private System.Windows.Forms.PictureBox botonX;
        private System.Windows.Forms.Label lblResultado;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnVolver;
        private System.Windows.Forms.Button btnControles;
    }
}

