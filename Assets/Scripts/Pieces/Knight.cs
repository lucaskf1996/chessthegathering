using System.Collections.Generic;

public class Knight : Piece
{
    public int id {get; set; }
    public Piece.STATE state {get; set; }
    public List<int> legalMoves {get;}
    public List<int> captureMoves {get;}
    public int position {get; set; }
    public Knight(int id){
        this.id = id;
        this.state = Piece.STATE.DECK;
        this.legalMoves = new List<int>();
        this.legalMoves.Add(-17);
        this.legalMoves.Add(-15);
        this.legalMoves.Add(-10);
        this.legalMoves.Add(-6);
        this.legalMoves.Add(6);
        this.legalMoves.Add(10);
        this.legalMoves.Add(15);
        this.legalMoves.Add(17);
        this.captureMoves = new List<int>(this.legalMoves);
        this.position = -1; // Not in play yet        
    }

    public void Capture(){
        this.state = Piece.STATE.CAPTURED;
        this.position = -1;
    }
}