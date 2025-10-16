namespace FyraIRad.Models
{
    public class MoveDetails
    {
        public int? MoveId { get; set; }
        public int? GameId { get; set; }
        public int? PlayerId { get; set; }
        public int ColumnIndex { get; set; }
        public int RowIndex { get; set; }
        public char DiscColor { get; set; }
        public DateTime MoveTime { get; set; }
    }
}