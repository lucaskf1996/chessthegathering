using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public List<int> whiteDeck, blackDeck;
    // Start is called before the first frame update
    void Start()
    {
        // 1 = pawn, 2 = knight, 3 = bishop, 4 = rook, 5 = queen
        for(int i = 0; i<15; i++){
            whiteDeck.Add(1);
            blackDeck.Add(1);
        }
        for(int i = 0; i<4; i++){
            whiteDeck.Add(2);
            blackDeck.Add(2);
            whiteDeck.Add(3);
            blackDeck.Add(3);
            whiteDeck.Add(4);
            blackDeck.Add(4);
        }
        whiteDeck.Add(5);
        blackDeck.Add(5);
        whiteDeck.Add(5);
        blackDeck.Add(5);

        int n = whiteDeck.Count;
        for (int i = 0; i < n; i++) {
            int temp = whiteDeck[i];
            int randomIndex = Random.Range(i, n);
            whiteDeck[i] = whiteDeck[randomIndex];
            whiteDeck[randomIndex] = temp;
        }
        for (int i = 0; i < n; i++) {
            int temp = blackDeck[i];
            int randomIndex = Random.Range(i, n);
            blackDeck[i] = blackDeck[randomIndex];
            blackDeck[randomIndex] = temp;
        }

        for(int i = 0; i<n; i++){
            print(whiteDeck);
        }
        for(int i = 0; i<n; i++){
            print(blackDeck);
        }
    }

    public void getCards(){
        print("test");
    }
}
