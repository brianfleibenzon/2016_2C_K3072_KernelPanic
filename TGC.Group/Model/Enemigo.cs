using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Camara;
using TGC.Core.Collision;
using TGC.Core.Geometry;
using TGC.Core.SceneLoader;
using TGC.Core.SkeletalAnimation;

namespace TGC.Group.Model
{
    public class Enemigo
    {
        public TgcSkeletalMesh mesh;

        protected string mediaDir = Environment.CurrentDirectory + "\\" + Game.Default.MediaDirectory;        
        protected string[] animationList;        

        private float MovementSpeed = 150f;

        protected Estado estado;

        public Vector3 posicionInicial;

        public bool persecutor=false;

        GameModel gameModel;

        public enum Estado
        {
            Parado = 0, // Hay que pasarle 2 viewpoints: posicion y direccion donde mira.
            Persiguiendo = 1,
            Retornando = 2
        }

        public Enemigo(GameModel gameModel, Vector3 posicionInicial)
        {
            this.gameModel = gameModel;

            //Paths para archivo XML de la malla
            string pathMesh = mediaDir + "SkeletalAnimations\\Robot\\Robot-TgcSkeletalMesh.xml";

            //Path para carpeta de texturas de la malla
            string mediaPath = mediaDir + "SkeletalAnimations\\Robot\\";

            //Lista de animaciones disponibles
            animationList = new string[]{
                "Parado",
                "Caminando",
                "Correr",
                "PasoDerecho",
                "PasoIzquierdo",
                "Empujar",
                "Patear",
                "Pegar",
                "Arrojar",
            };

            //Crear rutas con cada animacion
            string[] animationsPath = new string[animationList.Length];

            for (int i = 0; i < animationList.Length; i++)
            {
                animationsPath[i] = mediaPath + animationList[i] + "-TgcSkeletalAnim.xml";
            }

            TgcSkeletalLoader loader = new TgcSkeletalLoader();
            mesh = loader.loadMeshAndAnimationsFromFile(pathMesh, mediaPath, animationsPath);
            mesh.AutoTransformEnable = true;

            //Crear esqueleto a modo Debug
            mesh.buildSkletonMesh();

            //Elegir animacion Caminando
            // mesh.BoundingBox.move(new Vector3(15,0,-170));
            //mesh.BoundingBox.scaleTranslate(mesh.BoundingBox.Position, new Vector3(4f,0.8f,4f)); // este sera el rango de vision

            setEstado(Estado.Parado);

            mesh.Position = posicionInicial;

            this.posicionInicial = posicionInicial;

        }

        public void setEstado(Estado estado)
        {
            this.estado = estado;
            String selectedAnim = animationList[0];
            switch (estado)
            {
                case Estado.Parado:
                    selectedAnim = animationList[0];
                    break;
                case Estado.Persiguiendo:
                    selectedAnim = animationList[1];
                    break;
                case Estado.Retornando:
                    selectedAnim = animationList[1];
                    break;
            }
            mesh.playAnimation(selectedAnim, true);
        }

        public void activar()
        {
            persecutor = false;
            this.setEstado(Estado.Persiguiendo);
        }

        public void desactivar()
        {
            this.setEstado(Estado.Parado);
        }

        public void retornar()
        {
            if (this.estado == Estado.Persiguiendo)
                persecutor = true;
            if (this.estado != Estado.Parado)
                this.setEstado(Estado.Retornando);
        }

        public void actualizarEstado(TgcCamera Camara, float ElapsedTime, TgcScene scene)
        {
            if (estado != Estado.Parado)
            {
                Vector3 posicionAnterior = mesh.Position;

                Vector3 vector;

                if (estado == Estado.Persiguiendo)
                    vector = Camara.Position - mesh.Position;
                else
                    vector = posicionInicial - mesh.Position;
                vector.Normalize();
                vector.Y = 0;

                Vector3 intento = vector;

                mesh.Position += intento * MovementSpeed * ElapsedTime;
                

                if (verificarColision(Camara, scene))
                {
                    mesh.Position = posicionAnterior;
                    intento.X = valorUnitario(vector.X);
                    intento.Z = 0;
                    mesh.Position += intento * MovementSpeed * ElapsedTime;                  


                    if (verificarColision(Camara, scene))
                    {
                        this.mesh.Position = posicionAnterior;
                        intento.X = 0;
                        intento.Z = valorUnitario(vector.Z);
                        this.mesh.Position += intento * MovementSpeed * ElapsedTime;

                        /*   if (valorUnitario(vector.Z)<0.02f && verificarColision(Camara, scene))
                           {
                               this.mesh.Position = posicionAnterior;
                               intento.X = 0;
                               intento.Z = 1;
                               this.mesh.Position += intento * MovementSpeed * ElapsedTime;
                               if(valorUnitario(vector.X) < 0.02f && verificarColision(Camara, scene))
                               {
                                   this.mesh.Position = posicionAnterior;
                                   intento.X = 1;
                                   intento.Z = 0;
                                   this.mesh.Position += intento * MovementSpeed * ElapsedTime; */
                            if (verificarColision(Camara, scene))                                
                                    mesh.Position = posicionAnterior;
                           // }
                       // }
                        
                        
                    }
                }
 
                mesh.rotateY((float)Math.Atan2(intento.X, intento.Z) - mesh.Rotation.Y - Geometry.DegreeToRadian(180f));

            }

            if ((estado == Estado.Retornando) && TgcCollisionUtils.sqDistPointAABB(posicionInicial, mesh.BoundingBox) < 2f)
                desactivar();
        }

        private float valorUnitario(float numero)
        {
            if (numero >= 0f && numero <= 0.6f)
            {
                return 1;
            }
            else if(numero <= -0f && numero >= -0.6f)
            {
                return -1;
            }
            else
            {
                return numero;
            }            
            
        }


        private Boolean verificarColision(TgcCamera Camara, TgcScene scene)
        {
            if (((Camara.TgcFpsCamera)Camara).colisiones)
            {


                if (TgcCollisionUtils.classifyBoxBox(this.mesh.BoundingBox, ((Camara.TgcFpsCamera)Camara).camaraBox) == TgcCollisionUtils.BoxBoxResult.Atravesando)
                {
                    gameModel.perder();
                }


                foreach (var mesh in scene.Meshes)
                {
                    if (TgcCollisionUtils.classifyBoxBox(this.mesh.BoundingBox, mesh.BoundingBox) == TgcCollisionUtils.BoxBoxResult.Atravesando)
                    {
                        return true;
                    }                    

                }

            }
            return false;
        }

        public virtual void render(float ElapsedTime)
        {
            mesh.animateAndRender(ElapsedTime);
        }
    }
}
