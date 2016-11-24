using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
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
using Microsoft.DirectX.DirectInput;
using TGC.Group.Form;
using System.Collections.Generic;
using TGC.Core.UserControls;
using TGC.Core.UserControls.Modifier;



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
        /// 
        private GameForm formulario;


        public GameModel(string mediaDir, string shadersDir, GameForm formulario) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;

            this.formulario = formulario;

        }

        private TgcFog fog;

        private TgcMp3Player sonidoEntorno;

        private Tgc3dSound sonidoPisadas;

        public TgcScene scene;

        private TgcPickingRay pickingRay;

        private Puerta[] puertas = new Puerta[8];

        private Interruptor[] interruptores = new Interruptor[3];

        private Iluminacion[] iluminaciones = new Iluminacion[8];

        public Enemigo[] enemigos = new Enemigo[2];

        private Vector3 collisionPoint;

        private float mostrarBloqueado = 0;

        TgcMesh bloqueado;

        TgcMesh esconderse;

        TgcMesh bateria4;

        TgcMesh bateria3;

        TgcMesh bateria2;

        TgcMesh bateria1;

        private float mostrarPapel = 1;

        TgcMesh papel;

        private Iluminacion iluminacionEnMano;

        private Microsoft.DirectX.Direct3D.Effect effect;

        private Microsoft.DirectX.Direct3D.Effect effectLinterna;

        private bool luzActivada = true;

        public List<TgcMesh> meshesARenderizar;

        private readonly float far_plane = 1500f;

        private readonly float near_plane = 1f;

        private Matrix g_mShadowProj; // Projection matrix for shadow map

        private Surface g_pDSShadow; // Depth-stencil buffer for rendering to shadow map

        private Matrix g_LightView;

        private Texture g_pShadowMap; // Texture to which the shadow map is rendered

        private readonly int SHADOWMAP_SIZE = 1024;

        private float contadorTiempo = 0;
        private int contadorMilisegundos = 0;

        //VARIABLES DE BATERIA

        Size resolucionPantalla = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
        float contador = 0;

        int efectoEnemigo = 0;
        bool estadoEfectoEnemigo = false;
        float contadorEnemigo = 0;

        //ECONDIDAS

        public Contenedor[] contenedores = new Contenedor[3];

        private Contenedor enTacho = null;

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

            effect = TgcShaders.loadEffect(ShadersDir + "LuzYSombra.fx");
            effectLinterna = TgcShaders.loadEffect(ShadersDir + "LinternaYSombra.fx");

            Camara = new TgcFpsCamera(this, new Vector3(128f, 90f, 51f), Input);
            pickingRay = new TgcPickingRay(Input);            

            //Baterias
            bateria1 = loader.loadSceneFromFile(MediaDir + "Bateria\\bateria1-TgcScene.xml").Meshes[0];
            bateria1.Scale = new Vector3(0.002f, 0.002f, 0.002f);
            bateria1.Position = new Vector3(0.50f, 0.30f, 1.15f);
            bateria1.AlphaBlendEnable = true;

            bateria2 = loader.loadSceneFromFile(MediaDir + "Bateria\\bateria2-TgcScene.xml").Meshes[0];
            bateria2.Scale = new Vector3(0.002f, 0.002f, 0.002f);
            bateria2.Position = new Vector3(0.50f, 0.30f, 1.15f);
            bateria2.AlphaBlendEnable = true;

            bateria3 = loader.loadSceneFromFile(MediaDir + "Bateria\\bateria3-TgcScene.xml").Meshes[0];
            bateria3.Scale = new Vector3(0.002f, 0.002f, 0.002f);
            bateria3.Position = new Vector3(0.50f, 0.30f, 1.15f);
            bateria3.AlphaBlendEnable = true;

            bateria4 = loader.loadSceneFromFile(MediaDir + "Bateria\\bateria4-TgcScene.xml").Meshes[0];
            bateria4.Scale = new Vector3(0.002f, 0.002f, 0.002f);
            bateria4.Position = new Vector3(0.50f, 0.30f, 1.15f);
            bateria4.AlphaBlendEnable = true;

            //Papel
            papel = loader.loadSceneFromFile(MediaDir + "Papel\\papel-TgcScene.xml").Meshes[0];
            papel.Scale = new Vector3(0.005f, 0.005f, 0.005f);
            papel.Position = new Vector3(-0.35f, -0.3f, 1f);
            papel.AlphaBlendEnable = true;

            // empieza sombras

            g_pShadowMap = new Texture(D3DDevice.Instance.Device, SHADOWMAP_SIZE, SHADOWMAP_SIZE,
               1, Usage.RenderTarget, Format.R32F,
               Pool.Default);

            g_pDSShadow = D3DDevice.Instance.Device.CreateDepthStencilSurface(SHADOWMAP_SIZE,
                SHADOWMAP_SIZE,
                DepthFormat.D24S8,
                MultiSampleType.None,
                0,
                true);

            var aspectRatio = D3DDevice.Instance.AspectRatio;
            g_mShadowProj = Matrix.PerspectiveFovLH(Geometry.DegreeToRadian(80), aspectRatio, 50, 5000);
            D3DDevice.Instance.Device.Transform.Projection =
                Matrix.PerspectiveFovLH(Geometry.DegreeToRadian(45.0f), aspectRatio, near_plane, far_plane);

            //termina sombras 


            InicializarEnemigos();
            InicializarPuertas();
            InicializarInterruptores();
            InicializarIluminaciones();
            InicializarContenedores();

            bloqueado = loader.loadSceneFromFile(MediaDir + "Bloqueado\\locked-TgcScene.xml").Meshes[0];
            bloqueado.Scale = new Vector3(0.004f, 0.004f, 0.004f);
            bloqueado.Position = new Vector3(-0.05f, 0.20f, 1f);

            esconderse = loader.loadSceneFromFile(MediaDir + "Esconderse\\esconderse-TgcScene.xml").Meshes[0];
            esconderse.Scale = new Vector3(0.004f, 0.004f, 0.004f);
            esconderse.Position = new Vector3(-0.05f, 0.20f, 1f);

            fog = new TgcFog();

            sonidoPisadas = new Tgc3dSound(MediaDir + "Sonidos\\pasos.wav", Camara.Position, DirectSound.DsDevice);
            sonidoPisadas.MinDistance = 30f;

            sonidoEntorno = new TgcMp3Player();
            sonidoEntorno.FileName = MediaDir + "Sonidos\\entorno.mp3";
            sonidoEntorno.play(true);

            SepararZonas.separar(scene);
            meshesARenderizar = new List<TgcMesh>();
            meshesARenderizar.AddRange(SepararZonas.zona1);
            meshesARenderizar.AddRange(SepararZonas.comunes);

            this.AxisLinesEnable = false;
            DrawText.changeFont(new System.Drawing.Font("Chiller", 20));

            //--------Niebla---------//            
            fog.StartDistance = 50f;
            fog.EndDistance = 1000f;
            fog.Density = 0.0015f;
            fog.Color = Color.Black;
            fog.Enabled = true;
            fog.updateValues();
        }

        void InicializarContenedores()
        {
            for (int i = 0; i < 3; i++)
            {
                contenedores[i] = new Contenedor(scene.getMeshByName("Contenedor" + (i + 1)));

            }
            contenedores[0].definirPosiciones(new Vector3(1260f, 80f, 725f), new Vector3(1040f, 90f, 725f));
            contenedores[1].definirPosiciones(new Vector3(1520f, 70f, 1495f), new Vector3(1520f, 90f, 1340f));
            contenedores[2].definirPosiciones(new Vector3(245f, 70f, 1674f), new Vector3(373f, 90f, 1674f));
        }

        void InicializarPuertas()
        {
            for (int i = 0; i < 8; i++)
            {
                puertas[i] = new Puerta();
                puertas[i].mesh = scene.getMeshByName("Puerta" + (i + 1));
                puertas[i].sonido = new Tgc3dSound(MediaDir + "Sonidos\\puerta.wav", puertas[i].mesh.BoundingBox.Position, DirectSound.DsDevice);
                puertas[i].sonido.MinDistance = 1000f;
                puertas[i].sonidoBloqueada = new Tgc3dSound(MediaDir + "Sonidos\\puertaBloqueada.wav", puertas[i].mesh.BoundingBox.Position, DirectSound.DsDevice);
                puertas[i].sonidoBloqueada.MinDistance = 1000f;
            }

            puertas[0].funcionAbriendo = () => { this.meshesARenderizar.Clear(); this.meshesARenderizar.AddRange(SepararZonas.zona6); this.meshesARenderizar.AddRange(SepararZonas.zona1); this.meshesARenderizar.AddRange(SepararZonas.comunes); };

            puertas[1].funcionAbriendo = () => { this.meshesARenderizar.Clear(); this.meshesARenderizar.AddRange(SepararZonas.zona7); this.meshesARenderizar.AddRange(SepararZonas.zona1); this.meshesARenderizar.AddRange(SepararZonas.comunes); };

            puertas[2].estado = Puerta.Estado.BLOQUEADA;
            puertas[2].funcionAbriendo = () => { this.meshesARenderizar.Clear(); this.meshesARenderizar.AddRange(SepararZonas.zona8); this.meshesARenderizar.AddRange(SepararZonas.zona1); this.meshesARenderizar.AddRange(SepararZonas.comunes); };

            puertas[3].estado = Puerta.Estado.BLOQUEADA;
            puertas[3].funcionAbriendo = () => { this.meshesARenderizar.Clear(); this.meshesARenderizar.AddRange(SepararZonas.zona9); this.meshesARenderizar.AddRange(SepararZonas.zona1); this.meshesARenderizar.AddRange(SepararZonas.comunes); };

            puertas[4].estado = Puerta.Estado.BLOQUEADA;
            puertas[4].funcionAbriendo = () => { this.meshesARenderizar.Clear(); this.meshesARenderizar.AddRange(SepararZonas.zona2); this.meshesARenderizar.AddRange(SepararZonas.zona1); this.meshesARenderizar.AddRange(SepararZonas.comunes); };
            puertas[4].funcionAbierta = () => { enemigos[0].activar(); };

            puertas[5].funcionAbriendo = () => { this.meshesARenderizar.Clear(); this.meshesARenderizar.AddRange(SepararZonas.zona1); this.meshesARenderizar.AddRange(SepararZonas.zona3); this.meshesARenderizar.AddRange(SepararZonas.comunes); };

            puertas[6].funcionAbriendo = () => {
                /*enemigos[1].vigilador = true;
                enemigos[1].setEstado(Enemigo.Estado.Vigilando);*/

                

                this.meshesARenderizar.Clear(); this.meshesARenderizar.AddRange(SepararZonas.zona3); this.meshesARenderizar.AddRange(SepararZonas.zona4); this.meshesARenderizar.AddRange(SepararZonas.comunes); };

            puertas[6].funcionAbierta = () =>
            {
                if (interruptores[0].estado == Interruptor.Estado.DESACTIVADO) { 
                    enemigos[1].vigilador = true;
                    enemigos[1].activar();
                }else{
                    enemigos[1].vigilar();
                }
            };


            puertas[7].funcionAbriendo = () => { this.meshesARenderizar.Clear(); this.meshesARenderizar.AddRange(SepararZonas.zona4); this.meshesARenderizar.AddRange(SepararZonas.zona5); this.meshesARenderizar.AddRange(SepararZonas.comunes); };

            puertas[7].funcionAbierta = () =>
            {
                if (interruptores[0].estado == Interruptor.Estado.ACTIVADO)
                    enemigos[1].activar();
                else
                    enemigos[1].vigilar();
            };


        }

        void InicializarEnemigos()
        {
            enemigos[0] = new Enemigo(this, new Vector3(318f, 2f, 1480f));

            enemigos[1] = new Enemigo(this, new Vector3(1425f, 2f, 1450f));
            enemigos[1].puntosARecorrer.Add(new Vector3(1460f, 2f, 540f));
            enemigos[1].puntosARecorrer.Add(new Vector3(540f, 2f, 540f));
            enemigos[1].puntosARecorrer.Add(new Vector3(540f, 2f, 1450f));
            enemigos[1].puntosARecorrer.Add(new Vector3(1425f, 2f, 1450f));          
        }

        void InicializarInterruptores()
        {
            for (int i = 0; i < 3; i++)
            {
                interruptores[i] = new Interruptor();
                interruptores[i].mesh = scene.getMeshByName("Interruptor" + (i + 1));
                interruptores[i].sonido = new Tgc3dSound(MediaDir + "Sonidos\\interruptor.wav", interruptores[i].mesh.BoundingBox.Position, DirectSound.DsDevice);
                interruptores[i].sonido.MinDistance = 1000f;
            }

            interruptores[0].funcion = () => { puertas[2].estado = Puerta.Estado.CERRADA; puertas[3].estado = Puerta.Estado.CERRADA; };
            interruptores[1].funcion = () => { puertas[4].estado = Puerta.Estado.CERRADA; };
            interruptores[2].funcion = () =>
            {
                this.ganar();
            };

        }

        public void ganar()
        {
            formulario.ganar();
        }

        public void perder()
        {
            formulario.perder();
        }

        void InicializarIluminaciones()
        {

            /*
             * 
             * public Iluminacion(Color unColor, string nombre, TgcScene scene, Vector3 posicionLuz,
            float intensidadAgarrada, float atenuacionAgarrada, float intensidad, float atenuacion,
            float duracion, bool atenunar, bool puedeApagarse )
            

            * */

            float x = -0.001f * resolucionPantalla.Width / 2;


            iluminaciones[0] = new Iluminacion(Color.DarkOrange, "Vela", scene, new Vector3(0f, 25f, 0f), new Vector3(0,0,0),
                30.0f, 0.35f, 28.0f, 0.5f, 100f, true, false, true, false);
            iluminaciones[0].posicionarEnMano = () =>
            {
                iluminacionEnMano.mesh.Scale = new Vector3(0.008f, 0.008f, 0.008f);
                iluminacionEnMano.mesh.Position = -iluminacionEnMano.mesh.BoundingBox.Position;
                iluminacionEnMano.mesh.Position += new Vector3(x, -0.38f, 1f);
            };
            iluminaciones[1] = new Iluminacion(Color.White, "Linterna", scene, new Vector3(30f, 10f, 40f), new Vector3(0, 0, 0),
                110f, 0.30f, 38f, 0.5f, 210f, false, true, false, false);
            iluminaciones[1].posicionarEnMano = () =>
            {

                iluminacionEnMano.mesh.Scale = new Vector3(0.005f, 0.005f, 0.005f);
                iluminacionEnMano.mesh.Position = -iluminacionEnMano.mesh.BoundingBox.Position;
                iluminacionEnMano.mesh.Position += new Vector3(x, -0.38f, 1f);
            };
            iluminaciones[2] = new Iluminacion(Color.Yellow, "Farol", scene, new Vector3(0f, 25f, 0f), new Vector3(0, 0, 0),
                20f, 0.25f, 18f, 0.7f, 300f, true, false, true, false);
            iluminaciones[2].posicionarEnMano = () =>
            {
                iluminacionEnMano.mesh.Scale = new Vector3(0.005f, 0.005f, 0.005f);
                iluminacionEnMano.mesh.Position = -iluminacionEnMano.mesh.BoundingBox.Position;
                iluminacionEnMano.mesh.Position += new Vector3(x, -0.38f, 1f);
            };


            // ILUMINACIONES ESTATICAS
            iluminaciones[3] = new Iluminacion(Color.DarkRed, "LuzEstatica1", scene, new Vector3(-40f, 25f, 8f), new Vector3(-1f, -1f, 0),
                0f, 0f, 40f, 0.5f, 0f, false, false, true, true);
            iluminaciones[4] = new Iluminacion(Color.DarkRed, "LuzEstatica2", scene, new Vector3(40f, 25f, -10f), new Vector3(0, -1f, -1f),
                0f, 0f, 40f, 0.5f, 0f, false, false, true, true);
            iluminaciones[5] = new Iluminacion(Color.DarkRed, "LuzEstatica3", scene, new Vector3(0f, 25f, 0f), new Vector3(0, -1f, 0),
                0f, 0f, 40f, 0.5f, 0f, false, false, true, true);
            iluminaciones[6] = new Iluminacion(Color.DarkRed, "LuzEstatica4", scene, new Vector3(0f, 25f, 0f), new Vector3(0, -1f, 0),
                0f, 0f, 40f, 0.5f, 0f, false, false, true, true);
            iluminaciones[7] = new Iluminacion(Color.DarkRed, "LuzEstatica5", scene, new Vector3(0f, 25f, 0f), new Vector3(1f, -1f, 0),
                0f, 0f, 40f, 0.5f, 0f, false, false, true, true);


            iluminaciones[3].definirPuntos(new Vector3(210f, 0f, 2030f), new Vector3(210f, 0f, 2600f), new Vector3(730f, 0f, 2030f), new Vector3(730f, 0f, 2600f));
            iluminaciones[7].definirPuntos(new Vector3(230f, 0f, 1590f), new Vector3(230f, 0f, 1770f), new Vector3(1500f, 0f, 1590f), new Vector3(1500f, 0f, 1770f));
            iluminaciones[5].definirPuntos(new Vector3(670f, 0f, 670f), new Vector3(670f, 0f, 1330f), new Vector3(1330f, 0f, 670f), new Vector3(1330f, 0f, 1330f));
            iluminaciones[6].definirPuntos(new Vector3(450f, 0f, 1370f), new Vector3(450f, 0f, 1550f), new Vector3(1550f, 0f, 1370f), new Vector3(1550f, 0f, 1550f));
            iluminaciones[4].definirPuntos(new Vector3(2030f, 0f, 240f), new Vector3(2030f, 0f, 750f), new Vector3(2580f, 0f, 240f), new Vector3(2580f, 0f, 750f));

        }

        void ActualizarEstadoPuertas()
        {
            foreach (var puerta in puertas)
            {
                puerta.actualizarEstado(Camara, ElapsedTime, enemigos);

            }
        }

        void ActualizarEstadoEnemigos()
        {
            foreach (var enemigo in enemigos)
            {
                enemigo.actualizarEstado(Camara, ElapsedTime, scene);

            }
        }

        void ActualizarEstadoLuces()
        {
            foreach (var iluminacion in iluminaciones)
            {
                iluminacion.variarLuz(ElapsedTime);

            }
        }

        public bool VerificarSiMeshEsIluminacion(TgcMesh mesh)
        {
            foreach (var ilum in iluminaciones)
            {
                if (ilum.mesh == mesh)
                {
                    mesh.Enabled = false;
                    iluminacionEnMano = ilum;
                    contador = 0;
                    luzActivada = true;
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
                            if (puerta.estado == Puerta.Estado.BLOQUEADA)
                            {
                                mostrarBloqueado = 3f;
                            }
                            puerta.abrir();
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

        //Baterias

        //Intervalo: Cantidad de bateria que se pierde por intervalo
        //Porciento: Cantidad de bateria que se pierde por intervalo
        void reducirBateria()
        {

            if (iluminacionEnMano != null && luzActivada)
            {
                contador += ElapsedTime;

                if (contador > iluminacionEnMano.duracion)
                {
                    iluminacionEnMano = null;
                    contador = 0;
                }

            }
        }

        //@DESC: Muestra un determiada imagen de la bateria dependiendo de la cantidad de carga que tenga (entre x e y)
        void renderImagenBateria(int x, int y, TgcMesh bateria, int carga)
        {
            if ((carga < x) && (carga >= y))
            {
                var matrizView = D3DDevice.Instance.Device.Transform.View;

                D3DDevice.Instance.Device.Transform.View = Matrix.Identity;
                bateria.render();
                D3DDevice.Instance.Device.Transform.View = matrizView;
            }
            // mostrarBateria += ElapsedTime;
        }
        //@DESC: Cambia los 4 estados de la bateria dependiendo de la cantidad de carga
        void cambiarImagenBateria(int contador)
        {
            renderImagenBateria(100, 75, bateria1, contador);
            renderImagenBateria(75, 50, bateria2, contador);
            renderImagenBateria(50, 25, bateria3, contador);
            renderImagenBateria(25, 1, bateria4, contador);
        }

        void papelRender()
        {
            var matrizView2 = D3DDevice.Instance.Device.Transform.View;
            D3DDevice.Instance.Device.Transform.View = Matrix.Identity;
            papel.render();
            D3DDevice.Instance.Device.Transform.View = matrizView2;
        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la lógica de computo del modelo, así como también verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        public override void Update()
        {
            PreUpdate();

            escucharTeclas();

            reducirBateria();

            ActualizarEstadoPuertas();

            ActualizarEstadoEnemigos();

            ActualizarEstadoLuces();

            VerificarColisionConClick();

            if (iluminacionEnMano == null || iluminacionEnMano.usarFog)
            {
                fog.Enabled = true;
            }
            else
            {
                fog.Enabled = false;
            }
            fog.updateValues();

            actualizarEfectoEnemigo();

        }

        void actualizarEfectoEnemigo()
        {
            contadorEnemigo += ElapsedTime;

            if (contadorEnemigo >= 1f)
            {
                estadoEfectoEnemigo = !estadoEfectoEnemigo;
                contadorEnemigo = 0;
            }

            efectoEnemigo = 0;
            foreach (var en in enemigos)
            {
                if (en.estabaSiguiendo)
                    if (estadoEfectoEnemigo)
                        efectoEnemigo = 1;
            }

        }

        public int auxMilisegundos = 0;
        public int milisegundoActual;



    /// <summary>
    ///     Se llama cada vez que hay que refrescar la pantalla.
    ///     Escribir aquí todo el código referido al renderizado.
    ///     Borrar todo lo que no haga falta.
    /// </summary>
    public override void Render()
        {
            //Inicio el render de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones según nuestra conveniencia.
            //PreRender();



            ClearTextures();

            D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            D3DDevice.Instance.Device.BeginScene();


            D3DDevice.Instance.Device.EndScene(); // termino el thread anterior

            D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);


            //Genero el shadow map
            RenderShadowMap();

            D3DDevice.Instance.Device.BeginScene();
            // dibujo la escena pp dicha
            D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            RenderScene(false);

            if (!((TgcFpsCamera)Camara).colisiones)

                DrawText.drawText(
                    "God Mode (C para desactivar)", 0, 50,
                    Color.OrangeRed);
            if (mostrarPapel == 1)
            {
                DrawText.drawText(
         "Presionar ESC o I para cerrar", resolucionPantalla.Width / 2 - 125, resolucionPantalla.Height - 60, Color.OrangeRed);
            }


            if (iluminacionEnMano != null)
            {
                DrawText.drawText(
                   "BATERIA: " + getBateria() + "%", resolucionPantalla.Width - 175, 30, Color.OrangeRed);
                cambiarImagenBateria(getBateria());
            }
            if (luzActivada && iluminacionEnMano != null && iluminacionEnMano.puedeApagarse)
                DrawText.drawText(
          "Presionar F para apagar", 0, 70, Color.OrangeRed);
            else if (!luzActivada && iluminacionEnMano != null && iluminacionEnMano.puedeApagarse)
                DrawText.drawText(
          "Presionar F para encender", 0, 70, Color.OrangeRed);


            RenderFPS();

            if (mostrarBloqueado > 0)
            {

                var matrizView = D3DDevice.Instance.Device.Transform.View;
                D3DDevice.Instance.Device.Transform.View = Matrix.Identity;
                bloqueado.render();
                D3DDevice.Instance.Device.Transform.View = matrizView;
                mostrarBloqueado -= ElapsedTime;

            }
            else if (mostrarBloqueado < 0)
            {
                mostrarBloqueado = 0;
            }

            if (contenedorCerca() != null)
            {

                var matrizView = D3DDevice.Instance.Device.Transform.View;
                D3DDevice.Instance.Device.Transform.View = Matrix.Identity;
                esconderse.render();
                D3DDevice.Instance.Device.Transform.View = matrizView;

                if (enTacho != null)
                    DrawText.drawText(
         "Presionar E para salir", 0, 90, Color.OrangeRed);
                else
                    DrawText.drawText(
        "Presionar E para esconderse", 0,90, Color.OrangeRed);
            }

            if (mostrarPapel > 0)
            {
                var matrizView2 = D3DDevice.Instance.Device.Transform.View;
                D3DDevice.Instance.Device.Transform.View = Matrix.Identity;
                papel.render();
                D3DDevice.Instance.Device.Transform.View = matrizView2;
            }            


            if (iluminacionEnMano != null)
            {
                iluminacionEnMano.mesh.Effect = TgcShaders.Instance.TgcMeshShader;
                iluminacionEnMano.mesh.Technique = TgcShaders.Instance.getTgcMeshTechnique(TgcMesh.MeshRenderType.DIFFUSE_MAP);
                var matrizView = D3DDevice.Instance.Device.Transform.View;
                D3DDevice.Instance.Device.Transform.View = Matrix.Identity;
                iluminacionEnMano.mesh.Enabled = true;
                iluminacionEnMano.mesh.render();
                iluminacionEnMano.mesh.Enabled = false;
                D3DDevice.Instance.Device.Transform.View = matrizView;

            }




            D3DDevice.Instance.Device.EndScene();
            D3DDevice.Instance.Device.Present();

        }

        public void RenderShadowMap()
        {
            Iluminacion ilumCerca = iluminacionCerca();
            Vector3 posicion;
            Vector3 direccion;
            if (ilumCerca == null)
            {
                posicion = new Vector3(0, 0, 0);
                direccion = new Vector3(0, 0, 0);
            }
            else
            {

                posicion = ilumCerca.pointLightPosition;
                direccion = ilumCerca.lookAt;
            }
  
            //Vector3 direccion = new Vector3(0f, -1f, 0f);


            Microsoft.DirectX.Direct3D.Effect efecto;
            if (iluminacionEnMano == iluminaciones[1])
                efecto = effectLinterna;
            else
                efecto = effect;


            // Calculo la matriz de view de la luz
            efecto.SetValue("g_vLightPos", new Vector4(posicion.X, posicion.Y, posicion.Z, 1));
            efecto.SetValue("g_vLightDir", new Vector4(direccion.X, direccion.Y, direccion.Z, 1));
            //efecto.SetValue("g_vLightDir", new Vector4(posicion.X, posicion.Y, posicion.Z, 1));
            g_LightView = Matrix.LookAtLH(posicion, direccion + posicion, new Vector3(0, 0, 1));

            // inicializacion standard:
            efecto.SetValue("g_mProjLight", g_mShadowProj);
            efecto.SetValue("g_mViewLightProj", g_LightView * g_mShadowProj);

            // Primero genero el shadow map, para ello dibujo desde el pto de vista de luz
            // a una textura, con el VS y PS que generan un mapa de profundidades.
            var pOldRT = D3DDevice.Instance.Device.GetRenderTarget(0);
            var pShadowSurf = g_pShadowMap.GetSurfaceLevel(0);
            D3DDevice.Instance.Device.SetRenderTarget(0, pShadowSurf);
            var pOldDS = D3DDevice.Instance.Device.DepthStencilSurface;
            D3DDevice.Instance.Device.DepthStencilSurface = g_pDSShadow;
            D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            D3DDevice.Instance.Device.BeginScene();

            // Hago el render de la escena pp dicha
            efecto.SetValue("g_txShadow", g_pShadowMap);
            RenderScene(true);

            // Termino
            D3DDevice.Instance.Device.EndScene();

            //TextureLoader.Save("shadowmap.bmp", ImageFileFormat.Bmp, g_pShadowMap);

            // restuaro el render target y el stencil
            D3DDevice.Instance.Device.DepthStencilSurface = pOldDS;
            D3DDevice.Instance.Device.SetRenderTarget(0, pOldRT);
        }



        public void RenderScene(bool shadow)
        {
            var lightColors = new ColorValue[4];
            var pointLightPositions = new Vector4[4];
            var pointLightIntensity = new float[4];
            var pointLightAttenuation = new float[4];

            int j = 0;

            if (iluminacionEnMano != null)
            {

                if (luzActivada)
                {
                    lightColors[j] = ColorValue.FromColor(iluminacionEnMano.lightColors);
                    pointLightPositions[j] = TgcParserUtils.vector3ToVector4(Camara.Position);
                    pointLightIntensity[j] = iluminacionEnMano.pointLightIntensityAgarrada;
                    pointLightAttenuation[j] = iluminacionEnMano.pointLightAttenuationAgarrada;

                    j++;
                }

                iluminacionEnMano.mesh.Effect = TgcShaders.Instance.TgcMeshShader;
                iluminacionEnMano.mesh.Technique = TgcShaders.Instance.getTgcMeshTechnique(TgcMesh.MeshRenderType.DIFFUSE_MAP);
            }



            for (var i = 0; i < iluminaciones.Length; i++)
            {

                if (meshesARenderizar.Contains(iluminaciones[i].mesh))
                {


                    if (iluminacionEnMano != iluminaciones[i] && iluminaciones[i].mesh.Enabled)
                    {
                        lightColors[j] = ColorValue.FromColor(iluminaciones[i].lightColors);
                        pointLightPositions[j] = TgcParserUtils.vector3ToVector4(iluminaciones[i].pointLightPosition);
                        pointLightIntensity[j] = iluminaciones[i].pointLightIntensity;
                        pointLightAttenuation[j] = iluminaciones[i].pointLightAttenuation;
                        j++;
                    }


                }

            }



            foreach (var enemigo in enemigos)
            {

                //Cargar variables shader de la luz
                enemigo.mesh.Effect.SetValue("lightColor", lightColors);
                enemigo.mesh.Effect.SetValue("lightPosition", pointLightPositions);
                enemigo.mesh.Effect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(Camara.Position));
                enemigo.mesh.Effect.SetValue("lightIntensity", pointLightIntensity);
                enemigo.mesh.Effect.SetValue("lightAttenuation", pointLightAttenuation);

                //Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
                enemigo.mesh.Effect.SetValue("materialEmissiveColor",
                    ColorValue.FromColor((Color.DarkGray)));
                enemigo.mesh.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Color.DarkGray));
                enemigo.mesh.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.DarkGray));
                enemigo.mesh.Effect.SetValue("materialSpecularColor",
                   ColorValue.FromColor(Color.DarkGray));
                enemigo.mesh.Effect.SetValue("materialSpecularExp", 100f);

                enemigo.render(ElapsedTime);
            }

            List<TgcMesh> ListaARenderizar = meshesARenderizar;

            if (((TgcFpsCamera)Camara).colisiones == false)
            {
                ListaARenderizar = new List<TgcMesh>();
                ListaARenderizar.AddRange(SepararZonas.zona1);
                ListaARenderizar.AddRange(SepararZonas.zona2);
                ListaARenderizar.AddRange(SepararZonas.zona3);
                ListaARenderizar.AddRange(SepararZonas.zona4);
                ListaARenderizar.AddRange(SepararZonas.zona5);
                ListaARenderizar.AddRange(SepararZonas.zona6);
                ListaARenderizar.AddRange(SepararZonas.zona7);
                ListaARenderizar.AddRange(SepararZonas.zona8);
                ListaARenderizar.AddRange(SepararZonas.zona9);
                ListaARenderizar.AddRange(SepararZonas.comunes);
            }

            Vector3 lightDir;
            lightDir = Camara.LookAt - Camara.Position;
            lightDir.Normalize();


            milisegundoActual = DateTime.Now.Millisecond;
            if (auxMilisegundos != milisegundoActual)
            {
                contadorMilisegundos++;
                auxMilisegundos = milisegundoActual;
            }

            if (contadorMilisegundos == 30)
            {
                contadorTiempo = contadorTiempo + 0.5f;
                contadorMilisegundos = 0;
            }

            foreach (var mesh in ListaARenderizar)
            {
                if (iluminacionEnMano == iluminaciones[1])
                    mesh.Effect = effectLinterna;
                else
                    mesh.Effect = effect;

                if (shadow)
                {

                    mesh.Technique = "RenderShadow";
                }
                else
                {
                    mesh.Technique = "RenderScene";
                    if (iluminacionEnMano == null || mesh != iluminacionEnMano.mesh)
                    {

                        mesh.UpdateMeshTransform();

                        //Cargar variables de shader                        
                        mesh.Effect.SetValue("efectoEnemigo", efectoEnemigo);

                        mesh.Effect.SetValue("cantidadLuces", j);
                        mesh.Effect.SetValue("lightColor", lightColors);
                        mesh.Effect.SetValue("lightPosition", pointLightPositions);
                        mesh.Effect.SetValue("lightIntensity", pointLightIntensity);
                        mesh.Effect.SetValue("lightAttenuation", pointLightAttenuation);
                        mesh.Effect.SetValue("materialEmissiveColor",
                            ColorValue.FromColor((Color.Black)));
                        mesh.Effect.SetValue("materialDiffuseColor",
                            ColorValue.FromColor(Color.White));
                        mesh.Effect.SetValue("time", contadorTiempo);
                        if (iluminacionEnMano != null)
                            mesh.Effect.SetValue("bateria", getBateria());

                        if (iluminacionEnMano == iluminaciones[1])
                        {

                            mesh.Effect.SetValue("spotLightAngleCos", FastMath.ToRad(0.05f));
                            mesh.Effect.SetValue("spotLightExponent", 30f);
                            mesh.Effect.SetValue("spotLightDir", TgcParserUtils.vector3ToFloat4Array(lightDir));
                        }

                    }
                }

                mesh.render();
            }

        }

        private int getBateria()
        {
            int bateria = 100 - (int)Math.Ceiling((contador / iluminacionEnMano.duracion) * 100);
            if (bateria == 0) perder();
            return bateria;
        }

        public void pausarSonidos()
        {
            foreach (Puerta puerta in puertas)
            {
                puerta.sonidoBloqueada.stop();
                puerta.sonido.stop();
            }
            foreach (Interruptor interruptor in interruptores)
            {
                interruptor.sonido.stop();
            }
            sonidoPisadas.stop();
            sonidoEntorno.pause();
        }

        public void retomarSonidos()
        {
            sonidoEntorno.resume();
        }

        public void reiniciarTimer()
        {
            UpdateClock();
            UpdateClock();
        }

        private void escucharTeclas()
        {
            if (Input.keyPressed(Key.F) && iluminacionEnMano != null && iluminacionEnMano.puedeApagarse)
            {
                luzActivada = !luzActivada;
            }
            if (Input.keyDown(Key.W) || Input.keyDown(Key.S) || Input.keyDown(Key.A) || Input.keyDown(Key.D))
            {

                sonidoPisadas.play(false);
            }

            if (Input.keyPressed(Key.E))
            {
                if (enTacho != null)
                {
                    enTacho.salir((TgcFpsCamera)Camara);
                    enTacho = null;
                    foreach (var enemigo in enemigos)
                        if (enemigo.estabaSiguiendo)
                        enemigo.setEstado(Enemigo.Estado.Persiguiendo);
                }
                else
                {
                    var contenedor = contenedorCerca();
                    if (contenedor != null)
                    {
                        contenedor.esconderse(((TgcFpsCamera)Camara));
                        enTacho = contenedor;

                        foreach (var enemigo in enemigos)
                        {
                            if (enemigo.vigilador)
                                enemigo.vigilar();
                            else
                                enemigo.retornar();
                        }
                    }                    

                }
            }



            if (Input.keyPressed(Key.Escape))
            {
                if (mostrarPapel == 1)
                {
                    mostrarPapel = 0;
                }
                else
                {
                    formulario.pausar();
                }
            }
            // Input.keyDown
            if (Input.keyPressed(Key.I))
            {
                if (mostrarPapel == 0)
                {
                    mostrarPapel = 1;
                }
                else
                {
                    mostrarPapel = 0;
                }
            }

        }

        private Contenedor contenedorCerca() {
            foreach (var contenedor in contenedores)
            {
                if (meshesARenderizar.Contains(contenedor.mesh) && TgcCollisionUtils.sqDistPointAABB(Camara.Position, contenedor.mesh.BoundingBox) < 2000f)
                {
                    return contenedor;

                }
            }
            return null;
        }

        private Iluminacion iluminacionCerca()
        {
            /*
            float masCerca = 0f;
            Iluminacion ilum = null;

            foreach (var iluminacion in iluminaciones)
            {
                float dist = TgcCollisionUtils.sqDistPointAABB(Camara.Position, iluminacion.mesh.BoundingBox);
                if (meshesARenderizar.Contains(iluminacion.mesh) && iluminacion.esEstatica)
                {
                    if (dist < masCerca || ilum == null)
                    {
                        ilum = iluminacion;
                        masCerca = dist;
                    }  

                }
            }
            return ilum;
            */

            foreach (var iluminacion in iluminaciones)
            {
                if (meshesARenderizar.Contains(iluminacion.mesh) && iluminacion.esEstatica && iluminacion.estaCerca(Camara.Position))
                {
                    return iluminacion;

                }
            }
            return null;
        }



        /// <summary>
        ///     Se llama cuando termina la ejecución del ejemplo.
        ///     Hacer Dispose() de todos los objetos creados.
        ///     Es muy importante liberar los recursos, sobretodo los gráficos ya que quedan bloqueados en el device de video.
        /// </summary>
        public override void Dispose()
        {            
            sonidoEntorno.stop();
            sonidoEntorno.closeFile();
            sonidoPisadas.dispose();
            foreach (var enemigo in enemigos)
            {
                enemigo.mesh.dispose();
            }
            foreach (var puerta in puertas)
            {
                puerta.sonido.dispose();
                puerta.sonidoBloqueada.dispose();
            }
            foreach (var interruptor in interruptores)
            {
                interruptor.sonido.dispose();
            }
            bateria1.dispose();
            bateria2.dispose();
            bateria3.dispose();
            bateria4.dispose();
            bloqueado.dispose();
            papel.dispose();
            esconderse.dispose();
            effect.Dispose();
            effectLinterna.Dispose();
            scene.disposeAll();
        }
    }

}