using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Group.Camara;

namespace TGC.Group.Model
{
    public class Contenedor
    {
        public TgcMesh mesh;
        Vector3 posicionAdentro, posicionAfuera;


        public Contenedor(TgcMesh mesh)
        {
            this.mesh = mesh;

        }

        public void definirPosiciones(Vector3 posicionAdentro, Vector3 posicionAfuera)
        {
            this.posicionAdentro = posicionAdentro;
            this.posicionAfuera = posicionAfuera;
        }

        public void esconderse(TgcFpsCamera Camara)
        {
            
            Camara.invertirSentido(posicionAdentro);
            Camara.moverse = false;
        }

        public void salir(TgcFpsCamera Camara)
        {
            Camara.SetCamera(posicionAfuera, new Vector3(0f, 0f, -1f));
            Camara.moverse = true;
        }
    }
}
