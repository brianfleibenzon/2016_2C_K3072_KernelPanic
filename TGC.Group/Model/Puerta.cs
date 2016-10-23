using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Camara;
using TGC.Core.Collision;
using TGC.Core.SceneLoader;
using TGC.Core.Sound;
using TGC.Group.Camara;

namespace TGC.Group.Model
{
    public class Puerta
    {
        public enum Estado{CERRADA, ABIERTA, CERRANDO, ABRIENDO, BLOQUEADA};

        public Estado estado = Estado.CERRADA;

        public Action funcionAbriendo = null;
        public Action funcionAbierta = null;

        public TgcMesh mesh;

        public Tgc3dSound sonido;

        public void actualizarEstado(TgcCamera Camara, float ElapsedTime, Enemigo[] enemigos)
        {
            switch (this.estado)
            {
                
                case (Puerta.Estado.ABIERTA):
                    if (TgcCollisionUtils.sqDistPointAABB(Camara.Position, this.mesh.BoundingBox) > 100000f && !colisionConEnemigos(enemigos)) { 
                        this.estado = Puerta.Estado.CERRANDO;
                        this.sonido.play(true);
                     }
                    break;

                case (Puerta.Estado.ABRIENDO):

                    if (this.mesh.Position.Y < 195)
                    {
                        this.mesh.move(new Vector3(0, 80f * ElapsedTime, 0));
                        if (funcionAbierta != null)
                            funcionAbierta();
                        ((TgcFpsCamera)Camara).moverse = false;
                    }
                    else
                    {
                        this.estado = Puerta.Estado.ABIERTA;


                        this.sonido.stop();
                        ((TgcFpsCamera)Camara).moverse = true;
                    }
                    break;

                case (Puerta.Estado.CERRANDO):
                    if (this.mesh.Position.Y > 0)
                        this.mesh.move(new Vector3(0, -80f * ElapsedTime, 0));
                    else
                    {
                        this.estado = Puerta.Estado.CERRADA;
                        this.sonido.stop();
                    }                        
                    break;

            }
        }

        public void abrir()
        {
            if (funcionAbriendo != null)
                funcionAbriendo();      
            this.estado = Estado.ABRIENDO;
            this.sonido.play();
        }

        private bool colisionConEnemigos(Enemigo[] enemigos)
        {
            foreach(var enemigo in enemigos)
            {
                if (TgcCollisionUtils.sqDistPointAABB(enemigo.mesh.Position, this.mesh.BoundingBox) < 80000f)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
