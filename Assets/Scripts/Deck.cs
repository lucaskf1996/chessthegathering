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
        for(int i = 0; i<deck.Count; i++){
            print(deck);
        }
    }

    public List<Piece> getCards(){
        List<Piece> temp;
        if(deck.Count == 0){
            temp = new List<Piece>{};
        }
        else if(deck.Count == 1){
            temp = new List<Piece>{deck[0]};
            deck.RemoveAt(0);
        }
        else if(deck.Count == 2){
            temp = new List<Piece>{deck[0],deck[1]};
            deck.RemoveAt(0);
            deck.RemoveAt(1);
        }
        else{
            temp = new List<Piece>{deck[0],deck[1],deck[2]};
            deck.RemoveAt(0);
            deck.RemoveAt(1);
            deck.RemoveAt(2);
        }
        return temp;
    }

    private void returnedCards(List<Piece> returned){
        for(int i = 0; i<returned.Count; i++){
            deck.Add(returned[i]);
        }
        int n = deck.Count;
        for (int i = 0; i < n; i++) {
            Piece temp = deck[i];
            int randomIndex = Random.Range(i, n);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    public List<Piece> getPawns(){
        List<Piece> temp = new List<Piece>{new Pawn(this.id),new Pawn(this.id),new Pawn(this.id)};
        return temp;
    }
}
