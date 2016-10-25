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

        TgcMesh bateria4;

        TgcMesh bateria3;

        TgcMesh bateria2;

        TgcMesh bateria1;

        private float mostrarBateria = 0;

        private Iluminacion iluminacionEnMano;

        private Microsoft.DirectX.Direct3D.Effect effect;

        private bool luzActivada = true;

        public List<TgcMesh> meshesARenderizar;

        private readonly float far_plane = 2500f;

        private readonly float near_plane = 0.5f;

        private Matrix g_mShadowProj; // Projection matrix for shadow map

        private Surface g_pDSShadow; // Depth-stencil buffer for rendering to shadow map

        private Matrix g_LightView;

        private Texture g_pShadowMap; // Texture to which the shadow map is rendered

        private readonly int SHADOWMAP_SIZE = 1024;

        //VARIABLES DE BATERIA

        Size resolucionPantalla = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
        float contador = 0;

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
            g_mShadowProj = Matrix.PerspectiveFovLH(Geometry.DegreeToRadian(45f), aspectRatio, 50, 5000);
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

            puertas[6].funcionAbriendo = () => { this.meshesARenderizar.Clear(); this.meshesARenderizar.AddRange(SepararZonas.zona3); this.meshesARenderizar.AddRange(SepararZonas.zona4); this.meshesARenderizar.AddRange(SepararZonas.comunes); };
            
            puertas[6].funcionAbierta = () => { if (interruptores[0].estado == Interruptor.Estado.ACTIVADO) enemigos[1].retornar(); else enemigos[1].activar(); };

            puertas[7].funcionAbriendo = () => { enemigos[1].retornar() ; this.meshesARenderizar.Clear(); this.meshesARenderizar.AddRange(SepararZonas.zona4); this.meshesARenderizar.AddRange(SepararZonas.zona5); this.meshesARenderizar.AddRange(SepararZonas.comunes); };
           
            puertas[7].funcionAbierta = () => { puertas[7].sumarUnaPasada(); if(puertas[7].pasadasPorPuerta%2==0)enemigos[1].activar(); };
        }

        void InicializarEnemigos()
        {
            enemigos[0] = new Enemigo(this, new Vector3(318, 2, 1480));

            enemigos[1] = new Enemigo(this, new Vector3(1460, 2, 1460));

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


            iluminaciones[0] = new Iluminacion(Color.DarkOrange, "Vela", scene, new Vector3(0f, 25f, 0f),
                30.0f, 0.35f, 28.0f, 0.5f, 100f, true, false, true);
            iluminaciones[0].posicionarEnMano = () =>
            {
                iluminacionEnMano.mesh.Scale = new Vector3(0.008f, 0.008f, 0.008f);
                iluminacionEnMano.mesh.Position = -iluminacionEnMano.mesh.BoundingBox.Position;
                iluminacionEnMano.mesh.Position += new Vector3(x, -0.38f, 1f);
            };
            iluminaciones[1] = new Iluminacion(Color.Gray, "Linterna", scene, new Vector3(30f, 10f, 40f),
                60f, 0.35f, 38f, 0.5f, 210f, false, true, false);
            iluminaciones[1].posicionarEnMano = () =>
            {

                iluminacionEnMano.mesh.Scale = new Vector3(0.005f, 0.005f, 0.005f);
                iluminacionEnMano.mesh.Position = -iluminacionEnMano.mesh.BoundingBox.Position;
                iluminacionEnMano.mesh.Position += new Vector3(x, -0.38f, 1f);
            };
            iluminaciones[2] = new Iluminacion(Color.Yellow, "Farol", scene, new Vector3(0f, 25f, 0f),
                20f, 0.25f, 18f, 0.7f, 50f, true, false, true);
            iluminaciones[2].posicionarEnMano = () =>
            {
                iluminacionEnMano.mesh.Scale = new Vector3(0.005f, 0.005f, 0.005f);
                iluminacionEnMano.mesh.Position = -iluminacionEnMano.mesh.BoundingBox.Position;
                iluminacionEnMano.mesh.Position += new Vector3(x, -0.38f, 1f);
            };


            // ILUMINACIONES ESTATICAS
            iluminaciones[3] = new Iluminacion(Color.DarkRed, "LuzEstatica1", scene, new Vector3(-40f, 25f, 8f),
                0f, 0f, 40f, 0.5f, 0f, false, false, true);
            iluminaciones[4] = new Iluminacion(Color.DarkRed, "LuzEstatica2", scene, new Vector3(0f, 25f, 0f),
                0f, 0f, 40f, 0.5f, 0f, false, false, true);
            iluminaciones[5] = new Iluminacion(Color.DarkRed, "LuzEstatica3", scene, new Vector3(0f, 25f, 0f),
                0f, 0f, 40f, 0.5f, 0f, false, false, true);
            iluminaciones[6] = new Iluminacion(Color.DarkRed, "LuzEstatica4", scene, new Vector3(0f, 25f, 0f),
                0f, 0f, 40f, 0.5f, 0f, false, false, true);
            iluminaciones[7] = new Iluminacion(Color.DarkRed, "LuzEstatica5", scene, new Vector3(0f, 25f, 0f),
                0f, 0f, 40f, 0.5f, 0f, false, false, true);


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
                            switch (puerta.estado)
                            {
                                case (Puerta.Estado.BLOQUEADA):
                                    mostrarBloqueado = 3f;
                                    break;
                                case (Puerta.Estado.CERRADA):
                                    puerta.abrir();
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
            var matrizView = D3DDevice.Instance.Device.Transform.View;
            if ((carga < x) && (carga >= y))
                D3DDevice.Instance.Device.Transform.View = Matrix.Identity;
            bateria.render();
            D3DDevice.Instance.Device.Transform.View = matrizView;
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

        }

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
            Vector3 posicion = Camara.Position + new Vector3(0.00001f, 0.00001f, 0.00001f);
            Vector3 direccion = Camara.LookAt - posicion;
            direccion.Normalize();



            // Calculo la matriz de view de la luz
            effect.SetValue("g_vLightPos", new Vector4(posicion.X, posicion.Y, posicion.Z, 1));
            effect.SetValue("g_vLightDir", new Vector4(direccion.X, direccion.Y, direccion.Z, 1));
            g_LightView = Matrix.LookAtLH(posicion, direccion + posicion, new Vector3(0, 0, 1));

            // inicializacion standard:
            effect.SetValue("g_mProjLight", g_mShadowProj);
            effect.SetValue("g_mViewLightProj", g_LightView * g_mShadowProj);

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
            effect.SetValue("g_txShadow", g_pShadowMap);
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


                /* enemigo.mesh.Effect.SetValue("lightColor", lightColors);
                 enemigo.mesh.Effect.SetValue("lightPosition", pointLightPositions);
                 enemigo.mesh.Effect.SetValue("lightIntensity", pointLightIntensity);
                 enemigo.mesh.Effect.SetValue("lightAttenuation", pointLightAttenuation);
                 enemigo.mesh.Effect.SetValue("materialEmissiveColor",
                     ColorValue.FromColor((Color.Black)));
                 enemigo.mesh.Effect.SetValue("materialDiffuseColor",
                     ColorValue.FromColor(Color.White));*/
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

            foreach (var mesh in ListaARenderizar)
            {

                if (shadow)
                {
                    mesh.Effect = effect;
                    mesh.Technique = "RenderShadow";
                }
                else
                {
                    mesh.Effect = effect;
                    mesh.Technique = "RenderScene";

                    if (iluminacionEnMano == null || mesh != iluminacionEnMano.mesh)
                    {

                        mesh.UpdateMeshTransform();

                        //Cargar variables de shader
                        mesh.Effect.SetValue("cantidadLuces", j);
                        mesh.Effect.SetValue("lightColor", lightColors);
                        mesh.Effect.SetValue("lightPosition", pointLightPositions);
                        mesh.Effect.SetValue("lightIntensity", pointLightIntensity);
                        mesh.Effect.SetValue("lightAttenuation", pointLightAttenuation);
                        mesh.Effect.SetValue("materialEmissiveColor",
                            ColorValue.FromColor((Color.Black)));
                        mesh.Effect.SetValue("materialDiffuseColor",
                            ColorValue.FromColor(Color.White));
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

            if (Input.keyPressed(Key.LeftControl))
            {
                if (enTacho != null)
                {
                    enTacho.salir((TgcFpsCamera)Camara);
                    enTacho = null;
                    foreach (var enemigo in enemigos)
                        if (enemigo.persecutor)
                            enemigo.setEstado(Enemigo.Estado.Persiguiendo);
                }
                else
                {
                    foreach (var contenedor in contenedores)
                    {
                        if (meshesARenderizar.Contains(contenedor.mesh) && TgcCollisionUtils.sqDistPointAABB(Camara.Position, contenedor.mesh.BoundingBox) < 3000f)
                        {

                            contenedor.esconderse(((TgcFpsCamera)Camara));
                            enTacho = contenedor;

                            foreach (var enemigo in enemigos)
                            {
                                enemigo.retornar();
                            }

                        }
                    }

                }
            }



            if (Input.keyPressed(Key.Escape))
            {

                formulario.pausar();
            }

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
            foreach (var enemigo in enemigos)
            {
                enemigo.mesh.dispose();
            }
            bateria1.dispose();
            bateria2.dispose();
            bateria3.dispose();
            bateria4.dispose();
            bloqueado.dispose();
            effect.Dispose();
            scene.disposeAll();
        }
    }
}