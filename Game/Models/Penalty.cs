using Game.Enums;

namespace Game.Models
{
    public class Penalty
    {
        public PenaltyType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Penalty(PenaltyType type)
        {
            Type = type;

            switch (type)
            {
                case PenaltyType.ReducedTime:
                    Name = "Menos Tiempo";
                    Description = "Reduce 5 segundos del temporizador";
                    break;
                case PenaltyType.ShuffleOptions:
                    Name = "Orden Confuso";
                    Description = "Cambia el orden de las respuestas cada 5 segundos";
                    break;
                case PenaltyType.GhostAnswer:
                    Name = "Respuesta fantasma";
                    Description = "Cambia lijeramente el color de una de las respuestas para confundirte";
                    break;
                case PenaltyType.BlockPowerUp:
                    Name = "Bloqueo";
                    Description = "No puedes usar power-ups";
                    break;
            }
        }
    }
}
