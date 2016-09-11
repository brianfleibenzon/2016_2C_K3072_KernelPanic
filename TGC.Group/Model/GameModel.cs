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
        

        public TgcScene scene;

        private TgcPickingRay pickingRay;

        private Puerta[] puertas = new Puerta[8];

        private Interruptor[] interruptores = new Interruptor[3];

        private Iluminacion[] iluminaciones = new Iluminacion[3];

        private Vector3 collisionPoint;

        private float mostrarBloqueado = 0;

        TgcMesh bloqueado;

        private Iluminacion iluminacionEnMano;


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

            Camara = new TgcFpsCamera(this, new Vector3(128f, 90f, 51f) , Input);

            pickingRay = new TgcPickingRay(Input);

            InicializarPuertas();
            InicializarInterruptores();
            InicializarIluminaciones();

            bloqueado = loader.loadSceneFromFile(MediaDir + "Bloqueado\\locked-TgcScene.xml").Meshes[0];
            bloqueado.Scale = new Vector3(0.004f, 0.004f, 0.004f);
            bloqueado.Position = new Vector3(0.65f, -0.38f, 1f);
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

        void InicializarIluminaciones()
        {
            iluminaciones[0] = new Iluminacion();  
            iluminaciones[0].mesh = scene.getMeshByName("Vela");
            iluminaciones[0].posicionarEnMano = () =>
            {
                iluminacionEnMano.mesh.Scale = new Vector3(0.008f, 0.008f, 0.008f);
                iluminacionEnMano.mesh.Position = -iluminacionEnMano.mesh.BoundingBox.Position;
                iluminacionEnMano.mesh.Position += new Vector3(-0.8f, -0.38f, 1f);
            };

            iluminaciones[1] = new Iluminacion();
            iluminaciones[1].mesh = scene.getMeshByName("Linterna");
            iluminaciones[1].posicionarEnMano = () =>
            {
                
                iluminacionEnMano.mesh.Scale = new Vector3(0.005f, 0.005f, 0.005f);
                iluminacionEnMano.mesh.Position = -iluminacionEnMano.mesh.BoundingBox.Position;
                iluminacionEnMano.mesh.Position += new Vector3(-0.8f, -0.38f, 1f);                
            };

            iluminaciones[2] = new Iluminacion();
            iluminaciones[2].mesh = scene.getMeshByName("Farol");
            iluminaciones[2].posicionarEnMano = () =>
            {
                iluminacionEnMano.mesh.Scale = new Vector3(0.005f, 0.005f, 0.005f);
                iluminacionEnMano.mesh.Position = -iluminacionEnMano.mesh.BoundingBox.Position;
                iluminacionEnMano.mesh.Position += new Vector3(-0.8f, -0.38f, 1f);
            };

        }

        void ActualizarEstadoPuertas()
        {
            foreach (var puerta in puertas)
            {
                puerta.actualizarEstado(Camara, ElapsedTime);
                
            }
        }

        public bool VerificarSiMeshEsIluminacion(TgcMesh mesh)
        {
            foreach(var ilum in iluminaciones)
            {
                if(ilum.mesh == mesh)
                {
                    mesh.Enabled = false;
                    iluminacionEnMano = ilum;
                    iluminacionEnMano.posicionarEnMano();                    
                    return true;
                }
            }
            return false;
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

        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aquí todo el código referido al renderizado.
        ///     Borrar todo lo que no haga falta.
        /// </summary>
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
                    "Colisiones activadas (C para desactivar)", 0, 30,
                    Color.OrangeRed);
            else
                DrawText.drawText(
                   "Colisiones desactivadas (C para activar)", 0, 30,
                   Color.OrangeRed);
            

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

            if (iluminacionEnMano!=null)
            {


                var matrizView = D3DDevice.Instance.Device.Transform.View;
                D3DDevice.Instance.Device.Transform.View = Matrix.Identity;
                iluminacionEnMano.mesh.Enabled = true;
                iluminacionEnMano.mesh.render();
                iluminacionEnMano.mesh.Enabled = false;
                D3DDevice.Instance.Device.Transform.View = matrizView;

            }

            scene.renderAll();

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