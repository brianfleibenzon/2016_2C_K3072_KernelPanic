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
        public static List<TgcMesh> zona1 = new List<TgcMesh>();
        public static List<TgcMesh> zona2 = new List<TgcMesh>();
        public static List<TgcMesh> zona3 = new List<TgcMesh>();
        public static List<TgcMesh> zona4 = new List<TgcMesh>();
        public static List<TgcMesh> zona5 = new List<TgcMesh>();
        public static List<TgcMesh> zona6 = new List<TgcMesh>();
        public static List<TgcMesh> zona7 = new List<TgcMesh>();
        public static List<TgcMesh> zona8 = new List<TgcMesh>();
        public static List<TgcMesh> zona9 = new List<TgcMesh>();

        public static List<TgcMesh> comunes = new List<TgcMesh>();

        public static void separar(TgcScene scene)
        {

            zona1.AddRange(obtenerHabitacion(scene, "Room-1"));
            zona1.AddRange(obtenerHabitacion(scene, "Room-3"));
            zona1.AddRange(obtenerHabitacion(scene, "Room-4"));
            zona1.AddRange(obtenerHabitacion(scene, "Room-10"));
            zona1.Add(scene.getMeshByName("Vela"));
            zona1.Add(scene.getMeshByName("Esqueleto2"));

            zona2.AddRange(obtenerHabitacion(scene, "Room-7"));
            zona2.Add(scene.getMeshByName("Interruptor3"));

            zona3.AddRange(obtenerHabitacion(scene, "Room-5"));
            zona3.AddRange(obtenerHabitacion(scene, "Room-8"));
            zona3.AddRange(obtenerHabitacion(scene, "Room-11"));

            zona4.AddRange(obtenerHabitacion(scene, "Room-12"));
            zona4.AddRange(obtenerHabitacion(scene, "Room-13"));
            zona4.AddRange(obtenerHabitacion(scene, "Room-14"));
            zona4.AddRange(obtenerHabitacion(scene, "Room-15"));

            zona5.AddRange(obtenerHabitacion(scene, "Room-35"));
            zona5.Add(scene.getMeshByName("Interruptor1"));
            zona5.Add(scene.getMeshByName("Farol"));
            zona5.Add(scene.getMeshByName("BarrilPolvora1"));
            zona5.Add(scene.getMeshByName("BarrilPolvora2"));
            zona5.Add(scene.getMeshByName("Esqueleto1"));

            zona6.AddRange(obtenerHabitacion(scene, "Room-31"));
            zona6.Add(scene.getMeshByName("Esqueleto3"));
            zona6.Add(scene.getMeshByName("Guillotina"));
            zona6.Add(scene.getMeshByName("SogaEnrollada"));

            zona7.AddRange(obtenerHabitacion(scene, "Room-32"));
            zona7.Add(scene.getMeshByName("Linterna"));

            zona8.AddRange(obtenerHabitacion(scene, "Room-33"));
            zona8.Add(scene.getMeshByName("Interruptor2"));

            zona9.AddRange(obtenerHabitacion(scene, "Room-34"));
            zona9.Add(scene.getMeshByName("MesaTortura"));

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
