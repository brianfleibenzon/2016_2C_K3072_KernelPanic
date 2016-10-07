using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Core.Utils;
using TGC.Core.Camara;
using TGC.Group.Camara;
using TGC.Core.Collision;

using TGC.Core.Shaders;
using TGC.Core.Fog;
using TGC.Core.Sound;

using System;
using System.Globalization;








namespace TGC.Group.Model
{
    /// <summary>
    ///     Ejemplo para implementar el TP.
    ///     Inicialmente puede ser renombrado o copiado para hacer más ejemplos chicos, en el caso de copiar para que se
    ///     ejecute el nuevo ejemplo deben cambiar el modelo que instancia GameForm <see cref="Form.GameForm.InitGraphics()" />
    ///     line 97.
    /// </summary>
    public class GameModel : TgcExample
    {
        /// <summary>
        ///     Constructor del juego.
        /// </summary>
        /// <param name="mediaDir">Ruta donde esta la carpeta con los assets</param>
        /// <param name="shadersDir">Ruta donde esta la carpeta con los shaders</param>
        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        private Effect effect;
        private TgcFog fog;

        private string currentFile;

        private TgcMp3Player sonidoEntorno;

        private TgcMp3Player sonidoPisadas;

        private TgcScene scene;

        private TgcPickingRay pickingRay;

        private Puerta[] puertas = new Puerta[8];

        private Interruptor[] interruptores = new Interruptor[3];

        private Vector3 collisionPoint;

        private float mostrarBloqueado = 0;
        TgcMesh bloqueado;


        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aquí todo el código de inicialización: cargar modelos, texturas, estructuras de optimización, todo
        ///     procesamiento que podemos pre calcular para nuestro juego.
        ///     Borrar el codigo ejemplo no utilizado.
        /// </summary>
        public override void Init()
        {
            //Device de DirectX para crear primitivas.
            var d3dDevice = D3DDevice.Instance.Device;

            var loader = new TgcSceneLoader();
            scene = loader.loadSceneFromFile(MediaDir + "Escenario\\Escenario-TgcScene.xml");

            Camara = new TgcFpsCamera(scene, new Vector3(128f, 66f, 51f) , Input);

            pickingRay = new TgcPickingRay(Input);

            InicializarPuertas();
            InicializarInterruptores();

            bloqueado = loader.loadSceneFromFile(MediaDir + "Bloqueado\\locked-TgcScene.xml").Meshes[0];
            bloqueado.Scale = new Vector3(0.004f, 0.004f, 0.004f);
            bloqueado.Position = new Vector3(0.65f, -0.38f, 1f);


            fog = new TgcFog();

            sonidoPisadas = new TgcMp3Player();
            sonidoEntorno = new TgcMp3Player();

        }

        void InicializarPuertas()
        {
            for(int i=0; i<8; i++)
            {
                puertas[i] = new Puerta();
                puertas[i].mesh = scene.getMeshByName("Puerta"+(i+1));
            }

            puertas[2].estado = Puerta.Estado.BLOQUEADA;
            puertas[3].estado = Puerta.Estado.BLOQUEADA;
        }

        void InicializarInterruptores()
        {
            for (int i = 0; i < 3; i++)
            {
                interruptores[i] = new Interruptor();
                interruptores[i].mesh = scene.getMeshByName("Interruptor" + (i + 1));
            }

            interruptores[0].funcion = () => { puertas[2].estado = Puerta.Estado.CERRADA; puertas[3].estado = Puerta.Estado.CERRADA; };
            interruptores[1].funcion = () => { puertas[4].estado = Puerta.Estado.CERRADA; };
            
        }

        void ActualizarEstadoPuertas()
        {
            foreach (var puerta in puertas)
            {
                puerta.actualizarEstado(Camara, ElapsedTime);
                
            }
        }

        void VerificarColisionConClick()
        {
            if (Input.buttonPressed(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                //Actualizar Ray de colision en base a posicion del mouse
                pickingRay.updateRay();
                //Testear Ray contra el AABB de todos los meshes
                foreach (var puerta in puertas)
                {
                    var aabb = puerta.mesh.BoundingBox;

                    //Ejecutar test, si devuelve true se carga el punto de colision collisionPoint

                    if (TgcCollisionUtils.intersectRayAABB(pickingRay.Ray, aabb, out collisionPoint))
                    {
                        if (TgcCollisionUtils.sqDistPointAABB(Camara.Position, puerta.mesh.BoundingBox) < 15000f)
                        {
                            switch (puerta.estado) {
                                case (Puerta.Estado.BLOQUEADA):
                                    mostrarBloqueado = 3f;
                                    break;
                                case (Puerta.Estado.CERRADA):
                                    puerta.estado = Puerta.Estado.ABRIENDO;
                                    break;
                            }                            
                        }
                        break;
                    }
                }


                foreach (var interruptor in interruptores)
                {
                    var aabb = interruptor.mesh.BoundingBox;

                    //Ejecutar test, si devuelve true se carga el punto de colision collisionPoint

                    if (interruptor.estado == Interruptor.Estado.DESACTIVADO && TgcCollisionUtils.intersectRayAABB(pickingRay.Ray, aabb, out collisionPoint))
                    {
                        if (TgcCollisionUtils.sqDistPointAABB(Camara.Position, interruptor.mesh.BoundingBox) < 15000f)
                        {
                            interruptor.activar(puertas, MediaDir);                            
                        }
                        break;
                    }
                }
            }
        }

        void inicializar_bateria()
        {
            int bateria = 100;
            int minuto = DateTime.Now.Minute;
            int aux = DateTime.Now.Minute;
            while(bateria == 0)
            {
                if(minuto != aux)
                {
                    bateria = bateria - 10;
                    minuto = DateTime.Now.Minute;
                    aux = DateTime.Now.Minute;
                }
                else
                {
                    aux = DateTime.Now.Minute;
                }
            }

        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la lógica de computo del modelo, así como también verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        public override void Update()
        {
            PreUpdate();

            ActualizarEstadoPuertas();

        }

        //VARIABLES DE SONIDO
        String rutaDelRepo = "C:\\Program Files\\TGC\\";


        //VARIABLES DE BATERIA
        int intervalo = 2; //Cantidad de segundos para que se reste el porcenaje
        int porciento = 1; //Cantidad de bateria que se pierde por intervalo
        int bateria = 100;
        Size resolucionPantalla = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
        int segundo = DateTime.Now.Second;
        int aux = DateTime.Now.Second;
        int contador = 0;
        
        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aquí todo el código referido al renderizado.
        ///     Borrar todo lo que no haga falta.
        /// </summary>
        /// 

        public override void Render()
        {
            //Inicio el render de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones según nuestra conveniencia.
            PreRender();

            VerificarColisionConClick();


            //Dibuja un texto por pantalla
            DrawText.drawText(
                "Con clic izquierdo subimos la camara [Actual]: " + TgcParserUtils.printVector3(Camara.Position) + " - LookAt: " + TgcParserUtils.printVector3(Camara.LookAt), 0, 20,
                Color.OrangeRed);


            if (((TgcFpsCamera)Camara).colisiones)

                DrawText.drawText(
                    "Colisiones activadas (C para desactivar)", 0, 50,
                    Color.OrangeRed);
            else
                DrawText.drawText(
                   "Colisiones desactivadas (C para activar)", 0, 50,
                   Color.OrangeRed);

            //--------Nuebla---------//
            //Todavia no la pude hacer funcionar :/
            var fogShader = true;
            fog.Enabled = true;
            fog.StartDistance = 500f;
            fog.EndDistance = 1000f;
            fog.Density = 0.0015f;
            fog.Color = Color.LightGray;

            if (fog.Enabled)
            {
                fog.updateValues();
            }

            //-------Bateria------------//
            if (Input.keyDown(Key.F)) 
            {
                if (contador == intervalo) 
            {
                bateria = bateria - porciento;
                contador = 0;
            }

            if (segundo != aux)
            {
                segundo = DateTime.Now.Second;
                aux = DateTime.Now.Second;
                contador++;
            }
            else
            {
                aux = DateTime.Now.Second;
            }
            }

            DrawText.drawText(
              "BATERIA: " + bateria+"%", resolucionPantalla.Width - 175, 30, Color.OrangeRed);


            //----------Sonidos---------//

            //Pisadas
            sonidoPisadas.FileName = rutaDelRepo+"2016_2C_K3072_KernelPanic\\sonidos\\pasos.mp3";
            //Contro del reproductor por teclado
            var currentState = sonidoPisadas.getStatus();
            if (Input.keyDown(Key.W))
            {
                if (currentState == TgcMp3Player.States.Open)
                {
                    //Reproducir MP3
                    sonidoPisadas.play(true);
                }
            }
            if (Input.keyUp(Key.W)) 
            {

                //Parar y reproducir MP3
                sonidoPisadas.closeFile();
            }

            //Entorno

            sonidoEntorno.FileName = rutaDelRepo + "2016_2C_K3072_KernelPanic\\sonidos\\entorno.mp3";

            //Contro del reproductor por teclado
            if (Input.keyPressed(Key.Y))
            {
                if (currentState == TgcMp3Player.States.Open)
                {
                    //Reproducir MP3
                    sonidoEntorno.play(true);
                }
                if (currentState == TgcMp3Player.States.Stopped)
                {
                    //Parar y reproducir MP3
                    sonidoEntorno.closeFile();
                    sonidoEntorno.play(true);
                }
            }
            else if (Input.keyPressed(Key.U))
            {
                if (currentState == TgcMp3Player.States.Playing)
                {
                    //Pausar el MP3
                    sonidoEntorno.pause();
                }
            }

            //-------------------------//

            if (mostrarBloqueado > 0)
            {


                var matrizView = D3DDevice.Instance.Device.Transform.View;
                D3DDevice.Instance.Device.Transform.View = Matrix.Identity;
                bloqueado.render();
                D3DDevice.Instance.Device.Transform.View = matrizView;
                mostrarBloqueado -= ElapsedTime;

            } else if (mostrarBloqueado < 0) { 
                mostrarBloqueado = 0;
            }
            scene.renderAll();

            //-----------------------------//
            //Efecto niable
            //Cargar valores de niebla


            if (fog.Enabled)
            {
                fog.updateValues();
            }
            //--------------------------//
            //Efecto oscuridad


            //Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
            PostRender();
        }

        /// <summary>
        ///     Se llama cuando termina la ejecución del ejemplo.
        ///     Hacer Dispose() de todos los objetos creados.
        ///     Es muy importante liberar los recursos, sobretodo los gráficos ya que quedan bloqueados en el device de video.
        /// </summary>
        public override void Dispose()
        {
            scene.disposeAll();
        }
    }
}