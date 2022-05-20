using System.Collections.Generic;
public interface Piece
{
    // Id to define what kind of piece you are, as well as color
    // 0 = White
    // 1 = Black
    public enum STATE 
    {
        CAPTURED,
        PLAY,
        DECK
    }
    
    public int id {get; set; }
    public STATE state {get; set; }
    public List<int> legalMoves {get;}
    public List<int> captureMoves {get;}
    public int position {get; set; }
    public void Capture();
}
