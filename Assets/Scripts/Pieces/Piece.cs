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
    public int spriteId {get; set; }
    public STATE state {get; set; }
    public void Capture();
    public bool canMove(Piece[] Board, int pPosition, int tPosition);
    public bool canCapture(Piece[] Board, int pPosition, int tPosition); 
    public bool blockedPath(Piece[] Board, int pPosition, int tPosition); 
    public bool legalMovement(Piece[] Board, int pPosition, int tPosition);
}
