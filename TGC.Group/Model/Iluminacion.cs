using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
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

        public Action posicionarEnMano = null;

        float ultimaVariacion = 0;
        float pointLightIntensityOriginal;
        float pointLightIntensityAgarradaOriginal;
        public bool variarLuzEnabled = false;

        public bool puedeApagarse = false;
        public float duracion;

        public bool usarFog;

        public Iluminacion(Color unColor, string nombre, TgcScene scene, Vector3 posicionLuz,
            float intensidadAgarrada, float atenuacionAgarrada, float intensidad, float atenuacion,
            float duracion, bool variarLuz, bool puedeApagarse, bool usarFog)
        {
            this.lightColors = unColor;
            this.mesh = scene.getMeshByName(nombre);
            this.pointLightPosition = this.mesh.BoundingBox.Position + posicionLuz;
            this.pointLightIntensityAgarrada = intensidadAgarrada;
            this.pointLightAttenuationAgarrada = atenuacionAgarrada;
            this.pointLightIntensity = intensidad;
            this.pointLightAttenuation = atenuacion;
            this.variarLuzEnabled = variarLuz;
            this.duracion = duracion;
            this.puedeApagarse = puedeApagarse;
            this.usarFog = usarFog;
        }

        public void variarLuz(float ElpasedTime)
        {
            if (variarLuzEnabled){
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
