using Game.Enums;
using Game.Properties;
using System.Drawing; // Agrega este using
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Game.Models
{
    public class PowerUp
    {
        public PowerUpType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool CanCombineWithOthers { get; set; }
        public string ShortName { get; set; }
        public Bitmap Image { get; set; }

        public PowerUp(PowerUpType type)
        {
            Type = type;

            switch (type)
            {
                case PowerUpType.EliminateOption:
                    Name = "Eliminar Opción";
                    ShortName = "Eliminar";
                    Description = "Elimina una respuesta incorrecta de las 4 opciones";
                    CanCombineWithOthers = true;
                    Image = new Bitmap(Resources.eliminateOption, new Size(35, 35));
                    break;
                case PowerUpType.ExtraTime:
                    Name = "+10 Segundos";
                    ShortName = "+10s";
                    Description = "Añade 10 segundos adicionales para responder";
                    CanCombineWithOthers = true;
                    Image = new Bitmap(Resources.timeAdd, new Size(35, 35)); 
                    break;
                case PowerUpType.Retry:
                    Name = "Cambiar pregunta";
                    ShortName = "Cambio";
                    Description = "Cambia la pregunta si no sabes la respuesta";
                    CanCombineWithOthers = false;
                    Image = new Bitmap(Resources.retry, new Size(35, 35));
                    break;
                case PowerUpType.DoublePoints:
                    Name = "Doble Puntos";
                    ShortName = "x2 Pts";
                    Description = "Duplica los puntos de esta pregunta";
                    CanCombineWithOthers = true;
                    Image = new Bitmap(Resources.doublePoint, new Size(35, 35));
                    break;
            }
        }
    }
}
