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
                    Description = "Reduce 3 segundos del temporizador";
                    break;
                case PenaltyType.ShuffleOptions:
                    Name = "Orden Confuso";
                    Description = "Cambia el orden de las respuestas";
                    break;
                case PenaltyType.HarderQuestion:
                    Name = "Dificultad Sorpresa";
                    Description = "Pregunta más difícil";
                    break;
                case PenaltyType.BlockPowerUp:
                    Name = "Bloqueo";
                    Description = "No puedes usar power-ups";
                    break;
            }
        }
    }
}
