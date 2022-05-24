using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;




public class GameManager{
    private static GameManager _instance;
    

    public King whiteKing = new King(0);
    public King blackKing = new King(1);
    public King ownKing;
    public Queen whiteQueen = new Queen(0);
    public Queen blackQueen = new Queen(1);
    public Bishop whiteBishop = new Bishop(0);
    public Bishop blackBishop = new Bishop(1);
    public Rook whiteRook = new Rook(0);
    public Rook blackRook = new Rook(1);
    public Knight whiteKnight = new Knight(0);
    public Knight blackKnight = new Knight(1);


    public Pawn[] whitePawns =  new Pawn[8];
    public Pawn[] blackPawns =  new Pawn[8];

    public Deck whiteDeck, blackDeck;
    public List<Piece> whiteHand, blackHand;
    public Piece[] Board = new Piece[64];
    public enum GameState {WHITEPAWNS, BLACKPAWNS, WHITEHAND, BLACKHAND, WHITEMOVE, BLACKMOVE}; //n sei se vai ser assim ainda
    private GameObject WhiteHandTiles, BlackHandTiles;
    public GameState gameState { get; private set; }

    public static GameManager GetInstance()
    {
        if(_instance == null)
        {
            _instance = new GameManager();
        }
        return _instance;
    }

    public void SetInitialPieces(){
        Board[60] = whiteKing;
        Board[4] = blackKing;
    }

    public bool BlockedPath(Piece p, int target){
        bool pathIsBlocked = false;
        int path;
        int hTile = Math.Max(p.position, target);
        int lTile = Math.Min(p.position, target);

        path = hTile % lTile;
        if(path % 9 == 0){
            path = 9;
        }
        else if(path % 8 == 0)
        {
            path =8;
        }
        else if(path % 7 == 0){
            path = 7;
        }
        else{
            path = 1;
        }
        for(int i = 0; i < 8; i++){
            if(Board[(i+lTile)*path].GetType() == p.GetType()){
                pathIsBlocked = true;
            }

        }

        return(pathIsBlocked);
    }

    public void MovePiece(Piece p, int i){
        if(p.id == 0){
            King ownKing = whiteKing;
        }
        else{
            King ownKing = blackKing;
        }
        bool canMove = false;
        bool canCapture = false;
        bool isKinginCheck = false;

        int moveOffset = i - p.position;

        foreach(int move in p.legalMoves){
            if(move == moveOffset){
                //Check if King of same color is in check after move
                if(!isKinginCheck){
                    canMove = true;
                }
            }
        }
        foreach(int move in p.captureMoves){
            if(move == moveOffset){
                if(Board[i].GetType() == p.GetType()){
                    //Check if King of same color is in check after move
                    if(!isKinginCheck){
                        canCapture = true;
                    }
                }
            }
        }

        if(canMove == true){
            p.position = i;
        }

        if(canCapture == true){
            p.position = i;
            Board[i].Capture();
        }

    }

    public void ChangeState(GameState nextState)
    {
        if(nextState == GameState.WHITEPAWNS){
            PawnHand(0);
        }
        gameState = nextState;
    }

    private GameManager()
    {
        gameState = GameState.WHITEPAWNS;
        WhiteHandTiles = GameObject.Find("WhiteHand");
        BlackHandTiles = GameObject.Find("BlackHand");
        whiteDeck = new Deck(0);
        blackDeck = new Deck(1);
    }

    public void PawnHand(int id){
        if(id == 0){
            whiteHand.Add(whiteDeck.getPawn());
        }
        else{
            blackHand.Add(blackDeck.getPawn());
        }
    }

    public void GetPiece(int id){
        if(id == 0){
            whiteHand.Add(whiteDeck.GetPiece());
        }
        else{
            blackHand.Add(blackDeck.GetPiece());
        }
    }

    public void clickedTile(int id){
        if(Board[id] != null){

        }
    }
}
