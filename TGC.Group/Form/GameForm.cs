using System;
using System.Threading;
using System.Windows.Forms;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Input;
using TGC.Core.Shaders;
using TGC.Core.Sound;
using TGC.Core.Textures;
using TGC.Group.Model;
using System.Drawing;



namespace TGC.Group.Form
{
    /// <summary>
    ///     GameForm es el formulario de entrada, el mismo invocara a nuestro modelo  que extiende TgcExample, e inicia el
    ///     render loop.
    /// </summary>
    public partial class GameForm : System.Windows.Forms.Form
    {
        /// <summary>
        ///     Constructor de la ventana.
        /// </summary>
        public GameForm()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Ejemplo del juego a correr
        /// </summary>
        private TgcExample Modelo { get; set; }

        /// <summary>
        ///     Obtener o parar el estado del RenderLoop.
        /// </summary>
        private bool ApplicationRunning { get; set; }

        /// <summary>
        ///     Permite manejar el sonido.
        /// </summary>
        private TgcDirectSound DirectSound { get; set; }

        /// <summary>
        ///     Permite manejar los inputs de la computadora.
        /// </summary>
        private TgcD3dInput Input { get; set; }

        private bool hayQueReiniciar = false;

        private void GameForm_Load(object sender, EventArgs e)
        {
            //Centra los componentes, adaptandose al tamaño del monitor//
            Size resolucionPantalla = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;


            //Centrar Panel
            Int32 anchoDePanel = (this.Width - panel1.Width) / 2;
            Int32 largoDePanel = (this.Height - panel1.Height) / 2;
            panel1.Location = new Point(anchoDePanel, largoDePanel);
            panel2.Location = new Point(anchoDePanel, largoDePanel);

            //Cerrar
            Int32 anchoDeX = (this.Width - botonX.Width) - 10;
            botonX.Location = new Point(anchoDeX, botonX.Location.Y);


            this.AcceptButton = botonJugar;
        }

        private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ApplicationRunning)
            {
                ShutDown();
            }
        }

        /// <summary>
        ///     Inicio todos los objetos necesarios para cargar el ejemplo y directx.
        /// </summary>
        public void InitGraphics()
        {
            //Se inicio la aplicación
            ApplicationRunning = true;

            //Inicio Device
            D3DDevice.Instance.InitializeD3DDevice(panel3D);

            //Inicio inputs
            Input = new TgcD3dInput();
            Input.Initialize(this, panel3D);

            //Inicio sonido
            DirectSound = new TgcDirectSound();
            DirectSound.InitializeD3DDevice(panel3D);

            //Directorio actual de ejecución
            var currentDirectory = Environment.CurrentDirectory + "\\";

            //Cargar shaders del framework
            TgcShaders.Instance.loadCommonShaders(currentDirectory + Game.Default.ShadersDirectory);

            //Juego a ejecutar, si quisiéramos tener diferentes modelos aquí podemos cambiar la instancia e invocar a otra clase.
            Modelo = new GameModel(currentDirectory + Game.Default.MediaDirectory,
                currentDirectory + Game.Default.ShadersDirectory, this);

            //Cargar juego.
            ExecuteModel();
        }

        /// <summary>
        ///     Comienzo el loop del juego.
        /// </summary>
        public void InitRenderLoop()
        {
            while (ApplicationRunning)
            {
                //Renderizo si es que hay un ejemplo activo.
                if (Modelo != null)
                {
                    //Solo renderizamos si la aplicacion tiene foco, para no consumir recursos innecesarios.
                    if (ApplicationActive())
                    {
                        Modelo.Update();
                        Modelo.Render();
                    }
                    else
                    {
                        //Si no tenemos el foco, dormir cada tanto para no consumir gran cantidad de CPU.
                        Thread.Sleep(100);
                    }
                }
                // Process application messages.
                Application.DoEvents();
            }
        }

        /// <summary>
        ///     Indica si la aplicacion esta activa.
        ///     Busca si la ventana principal tiene foco o si alguna de sus hijas tiene.
        /// </summary>
        public bool ApplicationActive()
        {
            if (ContainsFocus)
            {
                return true;
            }

            foreach (var form in OwnedForms)
            {
                if (form.ContainsFocus)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Arranca a ejecutar un ejemplo.
        ///     Para el ejemplo anterior, si hay alguno.
        /// </summary>
        public void ExecuteModel()
        {
            //Ejecutar Init
            try
            {
                Modelo.ResetDefaultConfig();
                Modelo.DirectSound = DirectSound;
                Modelo.Input = Input;
                Modelo.Init();
                panel3D.Focus();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error en Init() del juego", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        ///     Deja de ejecutar el ejemplo actual
        /// </summary>
        public void StopCurrentExample()
        {
            if (Modelo != null)
            {
                Modelo.Dispose();
                Modelo = null;
            }
        }

        /// <summary>
        ///     Finalizar aplicacion
        /// </summary>
        public void ShutDown()
        {
            ApplicationRunning = false;

            StopCurrentExample();

            //Liberar Device al finalizar la aplicacion
            D3DDevice.Instance.Dispose();
            TexturesPool.Instance.clearAll();
        }

        public void ganar()
        {
            lblResultado.ForeColor = Color.Green;
            lblResultado.Text = "GANASTE";
            ApplicationRunning = false;
            hayQueReiniciar = true;
            panel1.Visible = true;
            botonX.Visible = false;
            panel3D.Visible = false;
        }
        public void perder()
        {
            lblResultado.ForeColor = Color.Red;
            lblResultado.Text = "PERDISTE";
            ApplicationRunning = false;
            hayQueReiniciar = true;
            panel1.Visible = true;
            botonX.Visible = false;
            panel3D.Visible = false;
        }

        private void botonJugar_Click(object sender, EventArgs e)
        {
            botonJugar.Text = "Jugar";
            lblResultado.Text = "";
            botonX.Visible = true;
            panel3D.Visible = true;
            panel1.Visible = false;

            if (Modelo == null)
            {      
                //Iniciar graficos.
                InitGraphics();

                //Titulo de la ventana principal.
                //Text = Modelo.Name + " - " + Modelo.Description;                
            }
            else
            {
                if (hayQueReiniciar)
                {
                    this.ShutDown();
                    InitGraphics();
                    hayQueReiniciar = false;
                }
                else
                {
                    ApplicationRunning = true;
                }
                
            }

            //Inicio el ciclo de Render.
            InitRenderLoop();

            //Focus panel3D.
            panel3D.Focus();
        }

        private void botonSalir_Click(object sender, EventArgs e)
        {
            if (ApplicationRunning == false && Modelo!=null && !hayQueReiniciar)
            {
                this.ShutDown();
                botonJugar.Text = "Jugar";
                botonSalir.Text = "Salir";
                botonJugar.Focus();
            }
            else
            {
                this.Close();
            }
            
        }

        private void botonX_Click(object sender, EventArgs e)
        {
            pausar();
        }

        public void pausar()
        {
            botonJugar.Text = "Reanudar";
            botonSalir.Text = "Rendirse";
            panel3D.Visible = false;
            ApplicationRunning = false;
            panel3D.Visible = false;
            botonX.Visible = false;
            panel1.Visible = true;
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
            panel1.Visible = true;
        }

        private void btnControles_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel2.Visible = true;
        }

    }
}