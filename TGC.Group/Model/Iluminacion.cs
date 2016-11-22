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
        float pointLightAttenuationOriginal;
        float pointLightAttenuationAgarradaOriginal;
        public bool variarLuzEnabled = false;

        public bool puedeApagarse = false;
        public float duracion;

        public bool usarFog;

        public bool esEstatica;
        public Vector3 lookAt;

        Vector3 punto1;
        Vector3 punto2;
        Vector3 punto3;
        Vector3 punto4;


        public Iluminacion(Color unColor, string nombre, TgcScene scene, Vector3 posicionLuz, Vector3 lookAt,
            float intensidadAgarrada, float atenuacionAgarrada, float intensidad, float atenuacion,
            float duracion, bool variarLuz, bool puedeApagarse, bool usarFog, bool esEstatica)
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
            this.esEstatica = esEstatica;
            this.lookAt = lookAt;
        }

        public bool estaCerca(Vector3 posicion)
        {
            if (posicion.X > punto1.X && posicion.Z > punto1.Z &&
                posicion.X > punto2.X && posicion.Z < punto2.Z &&
                posicion.X < punto3.X && posicion.Z > punto3.Z &&
                posicion.X < punto4.X && posicion.Z < punto4.Z)
                return true;
            return false;
        }

        public void definirPuntos(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            punto1 = p1;
            punto2 = p2;
            punto3 = p3;
            punto4 = p4;
        }

        public void variarLuz(float ElpasedTime)
        {
            if (variarLuzEnabled){
                ultimaVariacion += ElpasedTime;
                if (ultimaVariacion > 0.1f)
                {
                    if (this.pointLightAttenuationOriginal == 0)
                    {
                        this.pointLightAttenuationOriginal = this.pointLightAttenuation;
                        this.pointLightAttenuationAgarradaOriginal = this.pointLightAttenuationAgarrada;
                    }
                    if (this.pointLightAttenuationOriginal != this.pointLightAttenuation)
                    {
                        this.pointLightAttenuation = this.pointLightAttenuationOriginal;
                        this.pointLightAttenuationAgarrada = this.pointLightAttenuationAgarradaOriginal;
                    }
                    else
                    {
                        Random rnd = new Random();
                        float random = rnd.Next(-10, 10);
                        random /= 100f;
                        this.pointLightAttenuation += random;
                        this.pointLightAttenuationAgarrada += random;
                    }

                    ultimaVariacion = 0;
                }
            }
        }

    }
}
