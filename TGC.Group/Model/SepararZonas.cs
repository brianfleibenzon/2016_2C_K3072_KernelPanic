using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class SepararZonas
    {
        public static List<TgcMesh> zona1;
        public static List<TgcMesh> zona2;
        public static List<TgcMesh> zona3;
        public static List<TgcMesh> zona4;
        public static List<TgcMesh> zona5;
        public static List<TgcMesh> zona6;
        public static List<TgcMesh> zona7;
        public static List<TgcMesh> zona8;
        public static List<TgcMesh> zona9;

        public static List<TgcMesh> comunes;

        public static void separar(TgcScene scene)
        {
            zona1 = new List<TgcMesh>();
            zona2 = new List<TgcMesh>();
            zona3 = new List<TgcMesh>();
            zona4 = new List<TgcMesh>();
            zona5 = new List<TgcMesh>();
            zona6 = new List<TgcMesh>();
            zona7 = new List<TgcMesh>();
            zona8 = new List<TgcMesh>();
            zona9 = new List<TgcMesh>();
            comunes = new List<TgcMesh>();


            zona1.AddRange(obtenerHabitacion(scene, "Room-1"));
            zona1.AddRange(obtenerHabitacion(scene, "Room-3"));
            zona1.AddRange(obtenerHabitacion(scene, "Room-4"));
            zona1.AddRange(obtenerHabitacion(scene, "Room-10"));
            zona1.Add(scene.getMeshByName("Vela"));
            scene.getMeshByName("Esqueleto2").AlphaBlendEnable = true;
            zona1.Add(scene.getMeshByName("Esqueleto2"));
            zona1.Add(scene.getMeshByName("Box1"));
            zona1.Add(scene.getMeshByName("Box3"));
            zona1.Add(scene.getMeshByName("Box4"));
            zona1.Add(scene.getMeshByName("Box5"));
            scene.getMeshByName("Esqueleto20").AlphaBlendEnable = true;
            zona1.Add(scene.getMeshByName("Esqueleto20"));
            zona1.Add(scene.getMeshByName("BarrilPolvora10"));
            zona1.Add(scene.getMeshByName("BarrilPolvora12"));
            zona1.Add(scene.getMeshByName("BarrilPolvora13"));
            scene.getMeshByName("Calabera10").AlphaBlendEnable = true;
            zona1.Add(scene.getMeshByName("Calabera10"));
            scene.getMeshByName("Calabera11").AlphaBlendEnable = true;
            zona1.Add(scene.getMeshByName("Calabera11"));
            

            zona2.AddRange(obtenerHabitacion(scene, "Room-7"));
            zona2.Add(scene.getMeshByName("Interruptor3"));            

            zona3.AddRange(obtenerHabitacion(scene, "Room-5"));
            zona3.AddRange(obtenerHabitacion(scene, "Room-8"));
            zona3.AddRange(obtenerHabitacion(scene, "Room-11"));
            zona3.Add(scene.getMeshByName("Contenedor3"));
            zona3.Add(scene.getMeshByName("LuzEstatica5"));
            scene.getMeshByName("Calabera52").AlphaBlendEnable = true;
            zona3.Add(scene.getMeshByName("Calabera52"));
            zona3.Add(scene.getMeshByName("PilarEgipcio50"));
            zona3.Add(scene.getMeshByName("Contenedor51"));
            scene.getMeshByName("Esqueleto51").AlphaBlendEnable = true;
            zona3.Add(scene.getMeshByName("Esqueleto51"));
            scene.getMeshByName("Esqueleto50").AlphaBlendEnable = true;
            zona3.Add(scene.getMeshByName("Esqueleto50"));
            zona3.Add(scene.getMeshByName("Contenedor50"));


            zona4.AddRange(obtenerHabitacion(scene, "Room-12"));
            zona4.AddRange(obtenerHabitacion(scene, "Room-13"));
            zona4.AddRange(obtenerHabitacion(scene, "Room-14"));
            zona4.AddRange(obtenerHabitacion(scene, "Room-15"));
            zona4.Add(scene.getMeshByName("Contenedor2"));
            zona4.Add(scene.getMeshByName("LuzEstatica4"));

            zona5.AddRange(obtenerHabitacion(scene, "Room-35"));
            zona5.Add(scene.getMeshByName("Interruptor1"));
            zona5.Add(scene.getMeshByName("Farol"));
            zona5.Add(scene.getMeshByName("BarrilPolvora1"));
            zona5.Add(scene.getMeshByName("BarrilPolvora2"));
            scene.getMeshByName("Esqueleto1").AlphaBlendEnable = true;
            zona5.Add(scene.getMeshByName("Esqueleto1"));
            zona5.Add(scene.getMeshByName("Contenedor1"));
            zona5.Add(scene.getMeshByName("LuzEstatica3"));
            zona5.Add(scene.getMeshByName("Guillotina10"));
            zona5.Add(scene.getMeshByName("Guillotina11"));
            zona5.Add(scene.getMeshByName("MesaTortura10"));

            zona6.AddRange(obtenerHabitacion(scene, "Room-31"));
            scene.getMeshByName("Esqueleto3").AlphaBlendEnable = true;
            zona6.Add(scene.getMeshByName("Esqueleto3"));
            zona6.Add(scene.getMeshByName("Guillotina"));
            zona6.Add(scene.getMeshByName("SogaEnrollada"));
            zona6.Add(scene.getMeshByName("LuzEstatica1"));

            zona7.AddRange(obtenerHabitacion(scene, "Room-32"));
            zona7.Add(scene.getMeshByName("Linterna"));
            scene.getMeshByName("Calabera").AlphaBlendEnable = true;
            zona7.Add(scene.getMeshByName("Calabera"));
            zona7.Add(scene.getMeshByName("PilarEgipcio"));

            zona8.AddRange(obtenerHabitacion(scene, "Room-33"));
            zona8.Add(scene.getMeshByName("Interruptor2"));
            zona8.Add(scene.getMeshByName("Mesa10"));
            scene.getMeshByName("Esqueleto30").AlphaBlendEnable = true;
            zona8.Add(scene.getMeshByName("Esqueleto30"));
            zona8.Add(scene.getMeshByName("DispenserAgua"));

            zona9.AddRange(obtenerHabitacion(scene, "Room-34"));
            zona9.Add(scene.getMeshByName("MesaTortura"));
            scene.getMeshByName("Esqueleto4").AlphaBlendEnable = true;
            zona9.Add(scene.getMeshByName("Esqueleto4"));
            zona9.Add(scene.getMeshByName("LuzEstatica2"));

            comunes.AddRange(obtenerHabitacion(scene, "Room-37"));
            comunes.AddRange(obtenerHabitacion(scene, "Room-38"));
            comunes.AddRange(obtenerHabitacion(scene, "Room-39"));
            comunes.AddRange(obtenerHabitacion(scene, "Room-42"));
            comunes.AddRange(obtenerHabitacion(scene, "Room-44"));
            comunes.AddRange(obtenerHabitacion(scene, "Room-47"));
            comunes.AddRange(obtenerHabitacion(scene, "Room-50"));
            comunes.AddRange(obtenerHabitacion(scene, "Room-51"));
            comunes.AddRange(obtenerPuertas(scene));
        }

        private static List<TgcMesh> obtenerHabitacion(TgcScene scene, String nombre)
        {
            List<TgcMesh> aux = new List<TgcMesh>();

            aux.AddRange(obtenerRangoDePared(scene, nombre + "-Roof-"));
            aux.AddRange(obtenerRangoDePared(scene, nombre + "-Floor-"));
            aux.AddRange(obtenerRangoDePared(scene, nombre + "-West-"));
            aux.AddRange(obtenerRangoDePared(scene, nombre + "-North-"));
            aux.AddRange(obtenerRangoDePared(scene, nombre + "-South-"));
            aux.AddRange(obtenerRangoDePared(scene, nombre + "-East-"));

            return aux;

        }

        private static List<TgcMesh> obtenerRangoDePared(TgcScene scene, String nombre)
        {
            List<TgcMesh> aux = new List<TgcMesh>();
            int i = 0;

            TgcMesh encontrado = scene.getMeshByName(nombre + i.ToString());
            while (encontrado != null)
            {
                aux.Add(encontrado);
                i++;
                encontrado = scene.getMeshByName(nombre + i.ToString());
            }

            return aux;

        }

        private static List<TgcMesh> obtenerPuertas(TgcScene scene)
        {
            List<TgcMesh> aux = new List<TgcMesh>();
            for (int i = 0; i < 8; i++)
            {
                aux.Add(scene.getMeshByName("Puerta" + (i + 1)));
            }
            return aux;

        }
    }

}
