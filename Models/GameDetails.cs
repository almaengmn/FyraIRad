namespace FyraIRad.Models
{
    public class GameDetails
    {
        public int? GameId { get; set; }

        public int? playerRedId { get; set; }

        public int? playerYellowId { get; set; }

        public char CurrentTurn { get; set; }

        public string? Status { get; set; }

        public char? Winner { get; set; }


    }
}
