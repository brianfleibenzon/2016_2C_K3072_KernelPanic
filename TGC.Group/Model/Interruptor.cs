using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;

namespace TGC.Group.Model
{
    class Interruptor
    {
        public enum Estado { ACTIVADO, DESACTIVADO };

        public Estado estado = Estado.DESACTIVADO;

        public TgcMesh mesh;

        public Action funcion = null;

        public int puertaADesbloquear = -1;

        public void activar(Puerta[] puertas, String MediaDir)
        {
            this.estado = Interruptor.Estado.ACTIVADO;
            var diffuseMaps = new TgcTexture[1];
            diffuseMaps[0] = TgcTexture.createTexture(MediaDir + "Escenario\\Textures\\than_button2.jpg");
            this.mesh.changeDiffuseMaps(diffuseMaps);
            
            if (funcion!=null)
                funcion();
        }
    }
}
