using Game.Enums;
using Game.Properties;

namespace Game.Models
{
    public class Penalty
    {
        public PenaltyType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Bitmap Image { get; set; }

        public Penalty(PenaltyType type)
        {
            Type = type;

            switch (type)
            {
                case PenaltyType.ReducedTime:
                    Name = "Menos Tiempo";
                    Description = "Reduce 5 segundos del temporizador";
                    Image = new Bitmap(Resources.TimeOut, new Size(35, 35));
                    break;
                case PenaltyType.ShuffleOptions:
                    Name = "Orden Confuso";
                    Description = "Cambia el orden de las respuestas cada 7 segundos";
                    Image = new Bitmap(Resources.shufflePenality, new Size(35, 35));
                    break;
                case PenaltyType.GhostAnswer:
                    Name = "Respuesta fantasma";
                    Description = "Cambia lijeramente el color de una de las respuestas para confundirte";
                    Image = new Bitmap(Resources.ghostPenality, new Size(35, 35));
                    break;
                case PenaltyType.BlockPowerUp:
                    Name = "Bloqueo";
                    Description = "No puedes usar power-ups para esta pregunta";
                    Image = new Bitmap(Resources.XError, new Size(35, 35));
                    break;
            }
        }
    }
}
