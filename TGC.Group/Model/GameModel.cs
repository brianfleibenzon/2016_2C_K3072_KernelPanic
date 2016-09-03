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
        

        private TgcScene scene;

        private TgcPickingRay pickingRay;

        private Puerta[] puertas = new Puerta[8];
        private Puerta puertaAbierta;

        private Vector3 collisionPoint;

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

            Camara = new TgcFpsCamera(new Vector3(128f, 66f, 51f) , Input);

            pickingRay = new TgcPickingRay(Input);

            InicializarPuertas();            
  
        }

        void InicializarPuertas()
        {
            for(int i=0; i<8; i++)
            {
                puertas[i] = new Puerta();
                puertas[i].mesh = scene.getMeshByName("Puerta"+(i+1));
            }
        }

        void ActualizarEstadoPuertas()
        {
            foreach (var puerta in puertas)
            {
                switch (puerta.estado)
                {

                    case (Puerta.Estado.ABIERTA):
                        if (TgcCollisionUtils.sqDistPointAABB(Camara.Position, puerta.mesh.BoundingBox) > 100000f)
                            puerta.estado = Puerta.Estado.CERRANDO;
                        break;

                    case (Puerta.Estado.ABRIENDO):
                        if (puerta.mesh.Position.Y < 195)
                            puerta.mesh.move(new Vector3(0, 80f * ElapsedTime, 0));
                        else
                            puerta.estado = Puerta.Estado.ABIERTA;
                        break;

                    case (Puerta.Estado.CERRANDO):
                        if (puerta.mesh.Position.Y > 0)
                            puerta.mesh.move(new Vector3(0, -80f * ElapsedTime, 0));
                        else
                            puerta.estado = Puerta.Estado.CERRADA;
                        break;

                }
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

                    if (puerta.estado == Puerta.Estado.CERRADA && TgcCollisionUtils.intersectRayAABB(pickingRay.Ray, aabb, out collisionPoint))
                    {
                        if (TgcCollisionUtils.sqDistPointAABB(Camara.Position, puerta.mesh.BoundingBox) < 15000f)
                        {
                            puerta.estado = Puerta.Estado.ABRIENDO;
                            if (puertaAbierta != null)
                            {
                                puertaAbierta.estado = Puerta.Estado.CERRANDO;
                            }
                            puertaAbierta = puerta;
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
            DrawText.drawText("Con la tecla F se dibuja el bounding box.", 0, 20, Color.OrangeRed);
            DrawText.drawText(
                "Con clic izquierdo subimos la camara [Actual]: " + TgcParserUtils.printVector3(Camara.Position) + " - LookAt: " + TgcParserUtils.printVector3(Camara.LookAt), 0, 30,
                Color.OrangeRed);

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