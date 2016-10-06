using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Camara;
using TGC.Core.Collision;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class Enemigo
    {
        public TgcMesh mesh;

        private Boolean activo = false;

        private float MovementSpeed = 150f;

        public void activar()
        {
            this.activo = true;
            this.mesh.Position += new Vector3(0, 10f, 0);
        }

        public void desactivar()
        {
            this.mesh.Position = new Vector3(0, 0, 0);
            this.activo = false;
        }

        public void actualizarEstado(TgcCamera Camara, float ElapsedTime, TgcScene scene)
        {
            if (activo)
            {
                Vector3 posicionAnterior = this.mesh.Position;
                Vector3 vector = new Vector3(Camara.Position.X - this.mesh.BoundingBox.Position.X, 0, Camara.Position.Z - this.mesh.BoundingBox.Position.Z);
                vector.Normalize();
                this.mesh.Position += vector * MovementSpeed * ElapsedTime;
                if (verificarColision(Camara, scene))
                {
                    this.mesh.Position = posicionAnterior;
                    this.mesh.Position += new Vector3(vector.X, 0, 0) * MovementSpeed * 2f * ElapsedTime;
                    if (verificarColision(Camara, scene))
                    {
                        this.mesh.Position = posicionAnterior;
                        this.mesh.Position += new Vector3(0, 0, vector.Z) * MovementSpeed * 2f * ElapsedTime;
                        if (verificarColision(Camara, scene))
                        {
                            this.mesh.Position = posicionAnterior;
                        }
                    }
                }                

            }
        }

        private Boolean verificarColision(TgcCamera Camara, TgcScene scene)
        {
            if (((Camara.TgcFpsCamera)Camara).colisiones)
            {
                if (TgcCollisionUtils.classifyBoxBox(this.mesh.BoundingBox, ((Camara.TgcFpsCamera)Camara).camaraBox) == TgcCollisionUtils.BoxBoxResult.Atravesando)
                {
                    scene.disposeAll();
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
    }
}
