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
    public int playerTurn = 0;
    public Piece clickedPiece;

    public Piece[] Board = new Piece[64];
    public enum GameState {WHITEPAWNS, BLACKPAWNS, WHITEHAND, BLACKHAND, WHITEMOVE, BLACKMOVE}; //n sei se vai ser assim ainda
    private GameObject WhiteHandTiles, BlackHandTiles;
    public GameState gameState { get; private set; }
    private int selectedTile;
    public static GameManager GetInstance()
    {
        if(_instance == null)
        {
            _instance = new GameManager();
        }
        return _instance;
    }

    public void SetInitialPieces(){
        this.Board[60] = whiteKing;
        this.Board[4] = blackKing;
    }

    public int getOwnKingPosition(int colorId){
        int ownKingPosition = 0;
        if(colorId == 0){
            King ownKing = whiteKing;
        }
        else{
            King ownKing = blackKing;
        }
        for(int i = 0; i < 64; i++){
            if(this.Board[i] == ownKing){
                ownKingPosition = i;
            }
        }
        return ownKingPosition;
    }

    public bool isInCheck(int kingPosition, Piece[] boardCopy){
        bool kingInCheck = false;
        int moveOffset;
        bool blockedPath;

        // for(int i = 0; i < 64; i++){
        //     if(boardCopy[i]!= null){
        //         if(boardCopy[i].id != boardCopy[kingPosition].id){
        //             moveOffset = kingPosition - i;
        //             foreach(int move in boardCopy[i].captureMoves){
        //                 blockedPath = this.BlockedPath(i, kingPosition, boardCopy);
        //                 if(move == moveOffset){
        //                     if(blockedPath == false){
        //                         kingInCheck = true;
        //                     }
        //                 }
        //             }

        //         }
        //     }
        // }
        return kingInCheck;
    }

    public bool BlockedPath(int pPosition, int target, Piece[] boardCopy){
        Piece p = this.Board[pPosition];
        bool pathIsBlocked = false;
        int path;
        int hTile = Math.Max(pPosition, target);
        int lTile = Math.Min(pPosition, target);
        int bIndex;
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
        Debug.Log("Path = " + path);
        Debug.Log("lTile = " + lTile);
        for(int i = 0; i < 8; i++){
            bIndex = lTile+ i * path ;
            if(bIndex == pPosition || bIndex > hTile){
                continue;
            }
            if(bIndex > 0 && bIndex < 63 ){
                if(boardCopy[lTile + i*path] != null){
                    pathIsBlocked = true;
                }
            }
        }
        Debug.Log("PathBlocked = " + pathIsBlocked);
        return(pathIsBlocked);
    }

    public bool MovePiece(int pPosition, int i, Piece[] boardCopy){
        Debug.Log("pPosition = " + pPosition + " desiredPosition = " + i); 
        Piece p = this.Board[pPosition];
        int ownKingPosition = this.getOwnKingPosition(p.id);
        bool canMove = false;
        bool canCapture = false;
        bool isKinginCheck = false;
        bool isBlocked = false;

        int moveOffset = i - pPosition;
        // Debug.Log("moveOffset = " + moveOffset); 
        // Debug.Log("PieceID = " + p.id);
        foreach(int move in p.legalMoves){
            // Debug.Log("Move = " + move);
            if(move == moveOffset){
                // Debug.Log("Entered legalMove" + isKinginCheck);
                //Check if King of same color is in check after move
                // boardCopy[i] = p;
                // boardCopy[pPosition] = null;
                isKinginCheck = this.isInCheck(ownKingPosition, boardCopy);
                if(!isKinginCheck){
                    canMove = true;
                }
            }
        }
        foreach(int move in p.captureMoves){
            if(move == moveOffset){
                if(Board[i]!=null){
                    //Check if King of same color is in check after move
                    // boardCopy[i] = p;
                    // boardCopy[pPosition] = null;
                    isKinginCheck = this.isInCheck(ownKingPosition, boardCopy);
                    if(!isKinginCheck){
                        canCapture = true;
                        canMove = false;
                    }
                }
            }
        }

        // Debug.Log("King in check? " + isKinginCheck);
        Debug.Log("CAN MOVE = " + canMove);
        if(canMove == true){
            if(p.GetType() != whiteKnight.GetType()){
                isBlocked = this.BlockedPath(pPosition, i, boardCopy);
            }
            if(isBlocked == true){
                return false;
            }
            this.Board[pPosition] = null;
            pPosition = i;
            this.Board[i] = p;
            Debug.Log("Moved");
            return true;
        }

        if(canCapture == true){
            this.Board[i].Capture();
            this.Board[pPosition] = null;
            pPosition = i;
            this.Board[i] = p;
            return true;
        }
        else{
            Debug.Log("Cant Move");
            return false;
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
        for(int i = 0; i<8; i++){
            whitePawns[i] = new Pawn(0);
            blackPawns[i] = new Pawn(1);
        }
        whiteDeck = new Deck(whitePawns, whiteBishop, whiteQueen, whiteRook, whiteKnight);
        blackDeck = new Deck(blackPawns, blackBishop, blackQueen, blackRook, blackKnight);

    }

    public void FillDefaultBoard(){
            this.Board[0] = this.blackRook;
            this.Board[7] = this.blackRook;
            this.Board[1] = this.blackKnight;
            this.Board[6] = this.blackKnight;
            this.Board[2] = this.blackBishop;
            this.Board[5] = this.blackBishop;
            this.Board[3] = this.blackQueen;
            this.Board[56] = this.whiteRook;
            this.Board[63] = this.whiteRook;
            this.Board[57] = this.whiteKnight;
            this.Board[62] = this.whiteKnight;
            this.Board[58] = this.whiteBishop;
            this.Board[61] = this.whiteBishop;
            this.Board[59] = this.whiteQueen;
            for(int i = 8, j = 0; i<16; i++, j++){
                this.Board[i] = this.blackPawns[j];
            }
            for(int i = 48, j = 0; i<56; i++, j++){
                this.Board[i] = this.whitePawns[j];
            }
    }

    public void PawnHand(int id){
        if(id == 0){
            whiteHand.Add(whiteDeck.getPawn(0));
        }
        else{
            blackHand.Add(blackDeck.getPawn(1));
        }
    }

    public void RandomPiece(int id){
        if(id == 0){
            whiteHand.Add(whiteDeck.GetPiece());
        }
        else{
            blackHand.Add(blackDeck.GetPiece());
        }
    }

    public void clickedTile(int id){
        // Debug.Log(String.Format("Clicked tile {0} with state {1}", id, this.gameState.ToString()));
        switch (this.gameState) {
            case (GameState.WHITEPAWNS):
                if(this.Board[id] == null) return;
                if (this.Board[id].id == 0){
                    this.selectedTile = id;
                    this.gameState = GameState.WHITEMOVE;
                }
                break;
            case (GameState.WHITEMOVE):
                bool moved = this.MovePiece(this.selectedTile, id, this.Board);
                if(moved){
                    this.gameState = GameState.BLACKPAWNS;
                }
                else{
                    this.gameState = GameState.WHITEPAWNS;
                }
                break;
            case (GameState.BLACKPAWNS):
                if(this.Board[id] == null) return;
                if (this.Board[id].id == 1){
                    this.selectedTile = id;
                    this.gameState = GameState.BLACKMOVE;
                }
                break;
            case (GameState.BLACKMOVE):
                moved = this.MovePiece(this.selectedTile, id, this.Board);
                if(moved){
                    this.gameState = GameState.WHITEPAWNS;
                }
                else{
                    this.gameState = GameState.BLACKPAWNS;
                }
                break;
            default:
                break;
        }
    }

    // public void PlaceCards(int id){
        
    // }
}
