using Game.Enums;

namespace Game.Models
{
    public class PowerUp
    {
        public PowerUpType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool CanCombineWithOthers { get; set; }

        public PowerUp(PowerUpType type)
        {
            Type = type;

            switch (type)
            {
                case PowerUpType.EliminateOption:
                    Name = "Eliminar Opción";
                    Description = "Elimina una respuesta incorrecta";
                    CanCombineWithOthers = true;
                    break;
                case PowerUpType.ExtraTime:
                    Name = "+5 Segundos";
                    Description = "Añade 5 segundos al temporizador";
                    CanCombineWithOthers = true;
                    break;
                case PowerUpType.Retry:
                    Name = "Reintento";
                    Description = "Permite volver a responder";
                    CanCombineWithOthers = false;
                    break;
                case PowerUpType.DoublePoints:
                    Name = "Doble Puntos";
                    Description = "Duplica los puntos de esta pregunta";
                    CanCombineWithOthers = true;
                    break;
            }
        }
    }
}
