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
        protected string pathMesh;
        protected string mediaPath;
        protected string[] animationList;
        protected string[] animationsPath;
        protected string selectedAnim;
        protected const float VELOCIDAD_MOVIMIËNTO = 50f;
        protected const float VELOCIDAD_MOVIMIENTO_CORRER = 150f;

        private float MovementSpeed = 150f;

        protected Estado estado;

        public Vector3 posicionInicial;

        public enum Estado
        {
            Parado = 0, // Hay que pasarle 2 viewpoints: posicion y direccion donde mira.
            Persiguiendo = 1,
            Retornando = 2
        }

        public Enemigo(Vector3 posicionInicial)
        {
            //Paths para archivo XML de la malla
            pathMesh = mediaDir + "SkeletalAnimations\\Robot\\Robot-TgcSkeletalMesh.xml";

            //Path para carpeta de texturas de la malla
            mediaPath = mediaDir + "SkeletalAnimations\\Robot\\";

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
            animationsPath = new string[animationList.Length];
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
            this.setEstado(Estado.Persiguiendo);
        }

        public void desactivar()
        {
            this.setEstado(Estado.Parado);
        }

        public void retornar()
        {
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
                mesh.rotateY((float)Math.Atan2(vector.X, vector.Z) - mesh.Rotation.Y - Geometry.DegreeToRadian(180f));

                mesh.Position += vector * MovementSpeed * ElapsedTime;

                if (verificarColision(Camara, scene))
                {
                    mesh.Position = posicionAnterior;
                    mesh.Position += new Vector3(vector.X, 0, 0) * MovementSpeed * 2f * ElapsedTime;
                    mesh.rotateY((float)Math.Atan2(vector.X, 0) - mesh.Rotation.Y - Geometry.DegreeToRadian(180f));
                    if (verificarColision(Camara, scene))
                    {
                        this.mesh.Position = posicionAnterior;
                        this.mesh.Position += new Vector3(0, 0, vector.Z) * MovementSpeed * 2f * ElapsedTime;
                        mesh.rotateY((float)Math.Atan2(0, vector.Z) - mesh.Rotation.Y - Geometry.DegreeToRadian(180f));
                        if (verificarColision(Camara, scene))
                        {
                            mesh.Position = posicionAnterior;
                        }
                    }
                }
                
            }

            if ((estado == Estado.Retornando) && TgcCollisionUtils.sqDistPointAABB(posicionInicial, mesh.BoundingBox) < 2f)
                desactivar();
        }


        private Boolean verificarColision(TgcCamera Camara, TgcScene scene)
        {
            if (((Camara.TgcFpsCamera)Camara).colisiones)
            {


                if (TgcCollisionUtils.classifyBoxBox(this.mesh.BoundingBox, ((Camara.TgcFpsCamera)Camara).camaraBox) == TgcCollisionUtils.BoxBoxResult.Atravesando)
                {
                    System.Windows.Forms.MessageBox.Show("Perdiste");
                    Environment.Exit(0);
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
