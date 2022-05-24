using System.Collections.Generic;

public class King : Piece
{
    public int id {get; set; }
    public int spriteId {get; set; }
    public Piece.STATE state {get; set; }
    public List<int> legalMoves {get;}
    public List<int> captureMoves {get;}
    public int position {get; set; }
    public King(int id){
        this.id = id;
        this.spriteId = (this.id == 0) ? 6 : 0;
        this.state = Piece.STATE.DECK;
        this.legalMoves = new List<int>();
        this.legalMoves.Add(-9);
        this.legalMoves.Add(-8);
        this.legalMoves.Add(-7);
        this.legalMoves.Add(-1);
        this.legalMoves.Add(1);
        this.legalMoves.Add(7);
        this.legalMoves.Add(8);
        this.legalMoves.Add(9);
        this.captureMoves = new List<int>(this.legalMoves);
        this.position = -1; // Not in play yet        
    }

    public void Capture(){
        this.state = Piece.STATE.CAPTURED;
        this.position = -1;
    }
}
