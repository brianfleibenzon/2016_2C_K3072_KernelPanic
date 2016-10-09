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

        private Tgc3dSound sonidoLinterna;

        public TgcScene scene;

        private TgcPickingRay pickingRay;

        private Puerta[] puertas = new Puerta[8];

        private Interruptor[] interruptores = new Interruptor[3];

        private Iluminacion[] iluminaciones = new Iluminacion[3];

        public Enemigo[] enemigos = new Enemigo[2];

        private Vector3 collisionPoint;

        private float mostrarBloqueado = 0;

        TgcMesh bloqueado;

        private Iluminacion iluminacionEnMano;

        private Microsoft.DirectX.Direct3D.Effect effect;

        private bool luzActivada = true;

        public List<TgcMesh> meshesARenderizar = new List<TgcMesh>();

        //VARIABLES DE BATERIA

        Size resolucionPantalla = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
        float contador = 0;


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

            effect = TgcShaders.loadEffect(ShadersDir + "MultiDiffuseLights.fx");

            Camara = new TgcFpsCamera(this, new Vector3(128f, 90f, 51f), Input);

            pickingRay = new TgcPickingRay(Input);            

            InicializarEnemigos();
            InicializarPuertas();
            InicializarInterruptores();
            InicializarIluminaciones();

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
            meshesARenderizar.AddRange(SepararZonas.zona1);
            meshesARenderizar.AddRange(SepararZonas.comunes);
        }

        void InicializarPuertas()
        {
            for (int i = 0; i < 8; i++)
            {
                puertas[i] = new Puerta();
                puertas[i].mesh = scene.getMeshByName("Puerta" + (i + 1));
            }

            puertas[0].funcion = () => { this.meshesARenderizar.Clear(); this.meshesARenderizar.AddRange(SepararZonas.zona6); this.meshesARenderizar.AddRange(SepararZonas.zona1); this.meshesARenderizar.AddRange(SepararZonas.comunes); };

            puertas[1].funcion = () => { this.meshesARenderizar.Clear(); this.meshesARenderizar.AddRange(SepararZonas.zona7); this.meshesARenderizar.AddRange(SepararZonas.zona1); this.meshesARenderizar.AddRange(SepararZonas.comunes); };

            puertas[2].estado = Puerta.Estado.BLOQUEADA;
            puertas[2].funcion = () => { this.meshesARenderizar.Clear(); this.meshesARenderizar.AddRange(SepararZonas.zona8); this.meshesARenderizar.AddRange(SepararZonas.zona1); this.meshesARenderizar.AddRange(SepararZonas.comunes); };
        
            puertas[3].estado = Puerta.Estado.BLOQUEADA;
            puertas[3].funcion = () => { this.meshesARenderizar.Clear(); this.meshesARenderizar.AddRange(SepararZonas.zona9); this.meshesARenderizar.AddRange(SepararZonas.zona1); this.meshesARenderizar.AddRange(SepararZonas.comunes); };

            puertas[4].estado = Puerta.Estado.BLOQUEADA;
            puertas[4].funcion = () => { enemigos[0].activar(); this.meshesARenderizar.Clear(); this.meshesARenderizar.AddRange(SepararZonas.zona2); this.meshesARenderizar.AddRange(SepararZonas.zona1); this.meshesARenderizar.AddRange(SepararZonas.comunes); };

            puertas[5].funcion = () => { this.meshesARenderizar.Clear(); this.meshesARenderizar.AddRange(SepararZonas.zona1); this.meshesARenderizar.AddRange(SepararZonas.zona3); this.meshesARenderizar.AddRange(SepararZonas.comunes); };

            puertas[6].funcion = () => { if (interruptores[0].estado == Interruptor.Estado.ACTIVADO) enemigos[1].retornar(); else enemigos[1].activar(); this.meshesARenderizar.Clear(); this.meshesARenderizar.AddRange(SepararZonas.zona3); this.meshesARenderizar.AddRange(SepararZonas.zona4); this.meshesARenderizar.AddRange(SepararZonas.comunes); };

            puertas[7].funcion = () => { if (interruptores[0].estado == Interruptor.Estado.DESACTIVADO) enemigos[1].retornar(); else enemigos[1].activar(); this.meshesARenderizar.Clear(); this.meshesARenderizar.AddRange(SepararZonas.zona4); this.meshesARenderizar.AddRange(SepararZonas.zona5); this.meshesARenderizar.AddRange(SepararZonas.comunes); };

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
                68.0f, 0.25f, 38.0f, 0.5f, 135f, true, false);
            iluminaciones[0].posicionarEnMano = () =>
            {
                iluminacionEnMano.mesh.Scale = new Vector3(0.008f, 0.008f, 0.008f);
                iluminacionEnMano.mesh.Position = -iluminacionEnMano.mesh.BoundingBox.Position;
                iluminacionEnMano.mesh.Position += new Vector3(x, -0.38f, 1f);
            };
            iluminaciones[1] = new Iluminacion(Color.Gray, "Linterna", scene, new Vector3(30f, 10f, 40f),
                108f, 0.25f, 38f, 0.5f, 240f, false, true);
            iluminaciones[1].posicionarEnMano = () =>
            {

                iluminacionEnMano.mesh.Scale = new Vector3(0.005f, 0.005f, 0.005f);
                iluminacionEnMano.mesh.Position = -iluminacionEnMano.mesh.BoundingBox.Position;
                iluminacionEnMano.mesh.Position += new Vector3(x, -0.38f, 1f);
            };
            iluminaciones[2] = new Iluminacion(Color.YellowGreen, "Farol", scene, new Vector3(0f, 25f, 0f), 
                90f, 0.15f, 38f, 0.5f, 190f, false, false);
            iluminaciones[2].posicionarEnMano = () =>
            {
                iluminacionEnMano.mesh.Scale = new Vector3(0.005f, 0.005f, 0.005f);
                iluminacionEnMano.mesh.Position = -iluminacionEnMano.mesh.BoundingBox.Position;
                iluminacionEnMano.mesh.Position += new Vector3(x, -0.38f, 1f);
            };

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
                                    if(puerta.funcion != null)
                                        puerta.funcion();
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


        //Intervalo: Cantidad de bateria que se pierde por intervalo
        //Porciento: Cantidad de bateria que se pierde por intervalo
        void reducirBateria()
        {

            if (iluminacionEnMano!=null && luzActivada)
            {
                contador += ElapsedTime;

                if (contador > iluminacionEnMano.duracion)
                {
                    iluminacionEnMano = null;
                    contador = 0;                 
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

            escucharTeclas();

            reducirBateria();

            ActualizarEstadoPuertas();

            ActualizarEstadoEnemigos();

            ActualizarEstadoLuces();


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


            var lightColors = new ColorValue[iluminaciones.Length];
            var pointLightPositions = new Vector4[iluminaciones.Length];
            var pointLightIntensity = new float[iluminaciones.Length];
            var pointLightAttenuation = new float[iluminaciones.Length];


            for (var i = 0; i < iluminaciones.Length; i++)
            {

                lightColors[i] = ColorValue.FromColor(iluminaciones[i].lightColors);


               if (iluminacionEnMano == iluminaciones[i])
               {                   
                    
                    if (luzActivada)
                    {
                        pointLightPositions[i] = TgcParserUtils.vector3ToVector4(Camara.Position);
                        pointLightIntensity[i] = iluminaciones[i].pointLightIntensityAgarrada;
                        pointLightAttenuation[i] = iluminaciones[i].pointLightAttenuationAgarrada;
                    }

                    iluminaciones[i].mesh.Effect = TgcShaders.Instance.TgcMeshShader;
                    iluminaciones[i].mesh.Technique = TgcShaders.Instance.getTgcMeshTechnique(TgcMesh.MeshRenderType.DIFFUSE_MAP);


                }
                else if(iluminaciones[i].mesh.Enabled == false)
                {

                    pointLightPositions[i] = TgcParserUtils.vector3ToVector4(iluminaciones[i].pointLightPosition);

                    pointLightIntensity[i] = (float)0;

                    pointLightAttenuation[i] = (float)0;
                }
                else
                {
                    
                    pointLightPositions[i] = TgcParserUtils.vector3ToVector4(iluminaciones[i].pointLightPosition);

                    pointLightIntensity[i] = iluminaciones[i].pointLightIntensity;
                    pointLightAttenuation[i] = iluminaciones[i].pointLightAttenuation;
                }

            }


            //Renderizar meshes
            foreach (var mesh in meshesARenderizar)
            {               

                if (iluminacionEnMano == null || mesh != iluminacionEnMano.mesh)
                {
                    mesh.Effect = effect;
                    mesh.Technique = "MultiDiffuseLightsTechnique";

                    mesh.UpdateMeshTransform();

                    //Cargar variables de shader
                    mesh.Effect.SetValue("lightColor", lightColors);
                    mesh.Effect.SetValue("lightPosition", pointLightPositions);
                    mesh.Effect.SetValue("lightIntensity", pointLightIntensity);
                    mesh.Effect.SetValue("lightAttenuation", pointLightAttenuation);
                    mesh.Effect.SetValue("materialEmissiveColor",
                        ColorValue.FromColor((Color.Black)));
                    mesh.Effect.SetValue("materialDiffuseColor",
                        ColorValue.FromColor(Color.White));
                }
                //Renderizar modelo
            }

            //--------Nuebla---------//
            fog.Enabled = true;
            fog.StartDistance = 50f;
            fog.EndDistance = 1000f;
            fog.Density = 0.0015f;
            fog.Color = Color.Black;

            if (fog.Enabled)
            {
                fog.updateValues();
            }

           

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

            if (iluminacionEnMano != null)
                DrawText.drawText(
                   "BATERIA: " + getBateria() + "%", resolucionPantalla.Width - 175, 30, Color.OrangeRed);

            if (luzActivada && iluminacionEnMano!=null && iluminacionEnMano.puedeApagarse)
                DrawText.drawText(
          "Precionar F pare apagar", 0, 70, Color.OrangeRed);
            else if (!luzActivada && iluminacionEnMano != null && iluminacionEnMano.puedeApagarse)
                DrawText.drawText(
          "Precionar F pare encender", 0, 70, Color.OrangeRed);

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
                var matrizView = D3DDevice.Instance.Device.Transform.View;
                D3DDevice.Instance.Device.Transform.View = Matrix.Identity;
                iluminacionEnMano.mesh.Enabled = true;
                iluminacionEnMano.mesh.render();
                iluminacionEnMano.mesh.Enabled = false;
                D3DDevice.Instance.Device.Transform.View = matrizView;

            }


            foreach (var enemigo in enemigos)
            {

                enemigo.render(ElapsedTime);

            }

            foreach (var mesh in meshesARenderizar)
            {
                mesh.render();
            }
            //scene.renderAll();

            //Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
            PostRender();

        }

        private int getBateria()
        {
            return 100 - (int)Math.Ceiling((contador / iluminacionEnMano.duracion) * 100);
        }

        private void escucharTeclas()
        {
            if (Input.keyPressed(Key.F) && iluminacionEnMano!=null && iluminacionEnMano.puedeApagarse)
            {
                luzActivada = !luzActivada;
            }
            if (Input.keyDown(Key.W) || Input.keyDown(Key.S) || Input.keyDown(Key.A) || Input.keyDown(Key.D))
            {

                sonidoPisadas.play(false);
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
            bloqueado.dispose();
            scene.disposeAll();
        }
    }
}