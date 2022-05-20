using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    private static GameManager _instance;
    public enum GameState {WHITEPAWNS, BLACKPAWNS, WHITEHAND, BLACKHAND, WHITEMOVE, BLACKMOVE};
    private GameObject WhiteHand, BlackHand;
    public GameState gameState { get; private set; }

    public static GameManager GetInstance()
    {
        if(_instance == null)
        {
            _instance = new GameManager();
        }
        return _instance;
    }

    public void ChangeState(GameState nextState)
    {
        gameState = nextState;
    }

    private GameManager()
    {
        gameState = GameState.WHITEPAWNS;
        WhiteHand = GameObject.Find("WhiteHand");
        BlackHand = GameObject.Find("BlackHand");
    }

    public void PawnHand(int id){
        
    }

    public void RandomHand(int id){

    }
}
