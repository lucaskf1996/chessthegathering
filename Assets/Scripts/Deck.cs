using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public List<Piece> deck;
    public int id;
    // Start is called before the first frame update
    public Deck(int id)
    {
        this.id = id;
        // 1 = pawn, 2 = knight, 3 = bishop, 4 = rook, 5 = queen
        for(int i = 0; i<15; i++){
            deck.Add(new Pawn(this.id));
        }
        for(int i = 0; i<4; i++){
            deck.Add(new Knight(this.id));
            deck.Add(new Bishop(this.id));
            deck.Add(new Rook(this.id));
        }
        deck.Add(new Queen(this.id));
        deck.Add(new Queen(this.id));
        int n = deck.Count;
        for (int i = 0; i < n; i++) {
            Piece temp = deck[i];
            int randomIndex = Random.Range(i, n);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    public Piece GetPiece(){
        Piece piece = deck[0];
        deck.RemoveAt(0);
        return piece;
    }

    public Piece getPawn(){
        Piece temp = new Pawn(this.id);
        return temp;
    }
}
