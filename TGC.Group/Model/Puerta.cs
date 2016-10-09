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
    public class Puerta
    {
        public enum Estado{CERRADA, ABIERTA, CERRANDO, ABRIENDO, BLOQUEADA};

        public Estado estado = Estado.CERRADA;

        public Action funcion = null;

        public TgcMesh mesh;

        public void actualizarEstado(TgcCamera Camara, float ElapsedTime, Enemigo[] enemigos)
        {
            switch (this.estado)
            {

                case (Puerta.Estado.ABIERTA):
                    if (TgcCollisionUtils.sqDistPointAABB(Camara.Position, this.mesh.BoundingBox) > 100000f && !colisionConEnemigos(enemigos))
                        this.estado = Puerta.Estado.CERRANDO;
                    break;

                case (Puerta.Estado.ABRIENDO):
                    if (this.mesh.Position.Y < 195)
                        this.mesh.move(new Vector3(0, 80f * ElapsedTime, 0));
                    else
                    {
                        this.estado = Puerta.Estado.ABIERTA;
                        //if (funcion != null)
                            //funcion();
                    }
                    break;

                case (Puerta.Estado.CERRANDO):
                    if (this.mesh.Position.Y > 0)
                        this.mesh.move(new Vector3(0, -80f * ElapsedTime, 0));
                    else
                        this.estado = Puerta.Estado.CERRADA;
                    break;

            }
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
