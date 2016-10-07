using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class Iluminacion
    {
        public TgcMesh mesh;

        public System.Drawing.Color lightColors;
        public float pointLightIntensity;
        public float pointLightAttenuation;
        public Vector3 pointLightPosition;

        public float pointLightIntensityAgarrada;
        public float pointLightAttenuationAgarrada;
        public Vector3 pointLightPositionAgarrada;

        public Action posicionarEnMano = null;

        float ultimaReduccion = 0;
        float ultimaVariacion = 0;
        float pointLightIntensityOriginal;
        float pointLightIntensityAgarradaOriginal;
        public bool variarLuzEnable = false;

        public void variarLuz(float ElpasedTime)
        {
            if (variarLuzEnable){
                ultimaVariacion += ElpasedTime;
                if (ultimaVariacion > 0.1f)
                {
                    if (this.pointLightIntensityOriginal == 0)
                    {
                        this.pointLightIntensityOriginal = this.pointLightIntensity;
                        this.pointLightIntensityAgarradaOriginal = this.pointLightIntensityAgarrada;
                    }
                    if (this.pointLightIntensityOriginal != this.pointLightIntensity)
                    {
                        this.pointLightIntensity = this.pointLightIntensityOriginal;
                        this.pointLightIntensityAgarrada = this.pointLightIntensityAgarradaOriginal;
                    }
                    else
                    {
                        Random rnd = new Random();
                        float random = rnd.Next(1, 15);
                        this.pointLightIntensity -= random;
                        this.pointLightIntensityAgarrada -= random;
                    }

                    ultimaVariacion = 0;
                }
            }
        }

    }
}
