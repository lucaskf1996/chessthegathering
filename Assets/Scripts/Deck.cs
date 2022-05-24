using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    public List<Piece> deck;
    // Start is called before the first frame update
    public Deck(Pawn[] pawn, Bishop bishop, Queen queen, Rook rook, Knight knight)
    {      
        deck = new List<Piece>();
        for(int i = 0; i<pawn.Length; i++){
            deck.Add(pawn[i]);
        }
        deck.Add(queen);
        deck.Add(rook);
        deck.Add(knight);
        deck.Add(bishop);
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

    public Piece getPawn(int id){
        Piece temp = new Pawn(id);
        return temp;
    }
}
