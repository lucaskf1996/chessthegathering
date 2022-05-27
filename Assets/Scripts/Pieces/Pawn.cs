using System.Collections.Generic;

public class Pawn : Piece
{
    public int id {get; set; }
    public int spriteId {get; set; }
    public Piece.STATE state {get; set; }
    public List<int> legalMoves {get;}
    public List<int> captureMoves {get;}
    public int position {get; set; }
    
    public Pawn(int id){
        this.id = id;
        this.state = Piece.STATE.DECK;
        this.spriteId = (this.id == 0) ? 11 : 5;
        this.legalMoves = new List<int>();
        this.captureMoves = new List<int>();
        // If it's a black pawn
        if (this.id == 1) {
            this.legalMoves.Add(8); // Go down
            this.captureMoves.Add(7); // Diag left
            this.captureMoves.Add(9); // Diag right
        } else { // White pawn
            this.legalMoves.Add(-8); // Go up
            this.captureMoves.Add(-7); // Diag left
            this.captureMoves.Add(-9); // Diag right
        }
        this.position = -1; // Not in play yet        
    }

    public void Capture(){
        this.state = Piece.STATE.CAPTURED;
        this.position = -1;
    }
}
