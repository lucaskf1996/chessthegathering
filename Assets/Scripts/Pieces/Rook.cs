using System.Collections.Generic;

public class Rook : Piece
{
    public int id {get; set; }
    public int spriteId {get; set; }
    public Piece.STATE state {get; set; }
    public List<int> legalMoves {get;}
    public List<int> captureMoves {get;}
    public int position {get; set; }
    public Rook(int id){
        this.id = id;
        this.spriteId = (this.id == 0) ? 10 : 4;
        this.state = Piece.STATE.DECK;
        this.legalMoves = new List<int>();
        for (int i = 8; i <= 56; i+=8){
            this.legalMoves.Add(i);
            this.legalMoves.Add(-i);
        }
        for (int i = 1; i < 8; i++){
            this.legalMoves.Add(i);
            this.legalMoves.Add(-i);
        }
        
        this.captureMoves = new List<int>(this.legalMoves);
        this.position = -1; // Not in play yet        
    }

    public void Capture(){
        this.state = Piece.STATE.CAPTURED;
        this.position = -1;
    }
}
