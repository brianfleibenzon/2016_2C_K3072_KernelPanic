using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    public class Puerta
    {
        public enum Estado{CERRADA, ABIERTA, CERRANDO, ABRIENDO, BLOQUEADA};

        public Estado estado = Estado.CERRADA;

        public TgcMesh mesh;

    }
}
