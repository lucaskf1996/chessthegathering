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
    public List<Piece> whiteHand = new List<Piece>(0);
    public List<Piece> blackHand = new List<Piece>(0);
    public int whiteHandSize = 0;
    public int blackHandSize = 0;
    public int moveCount = 0;
    public Piece clickedPiece;

    public Piece[] Board = new Piece[64];
    public enum GameState {WHITEPAWNS, BLACKPAWNS, WHITEHAND, BLACKHAND, WHITEMOVE, BLACKMOVE}; //n sei se vai ser assim ainda
    private GameObject WhiteHandTiles, BlackHandTiles;
    public GameState gameState { get; private set; }
    public int selectedTile = -1;
    private int initialPawns = 0;
    private int selectedPiece;
    private bool clickedHand = false;
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
        King ownKing;
        if(colorId == 0){
            ownKing = whiteKing;
        }
        else{
            ownKing = blackKing;
        }
        for(int i = 0; i < 64; i++){
            if(this.Board[i] == ownKing){
                ownKingPosition = i;
            }
        }
        return ownKingPosition;
    }

    public bool SelfCheck(int kingPosition){
        bool kingInCheck = false;
        bool legalCap = false;
        bool blockedPath;
        for(int i = 0; i < 64; i++){
            if(this.Board[i]== null){
                continue;
            }
            if(this.Board[i].id != this.Board[kingPosition].id){
                legalCap = this.LegalCapture(i, kingPosition);
                blockedPath = this.BlockedPath(i, kingPosition);
                if(legalCap && !blockedPath){
                    kingInCheck = true;
                    Debug.Log("CHECK");
                }
            }
        }
        return kingInCheck;
    }

    public bool BlockedPath(int pPosition, int target){
        Piece p = this.Board[pPosition];
        if(this.Board[target]!= null){
            if(p.id == this.Board[target].id){
                return true;
            }
        }
        bool pathIsBlocked = false;
        int path;
        int hTile = Math.Max(pPosition, target);
        int lTile = Math.Min(pPosition, target);
        int bIndex;
        path = hTile - lTile;

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
            bIndex = lTile+ i * path ;
            if(bIndex == pPosition || bIndex > hTile || bIndex == target){
                continue;
            }
            if(bIndex > 0 && bIndex < 63 ){
                if(this.Board[lTile + i*path] != null){
                    pathIsBlocked = true;
                }
            }
        }
        return(pathIsBlocked);
    }

    public bool LegalMovement(int pPosition, int i){
        bool canMove = false;
        int moveOffset = i - pPosition;
        Piece p = this.Board[pPosition];
        foreach(int move in p.legalMoves){
            if(move == moveOffset){
                canMove = true;
            }
        }
        return canMove;
    }

    public bool LegalCapture(int pPosition, int i){
        int moveOffset = i - pPosition;
        Piece p = this.Board[pPosition];
        bool canCapture = false;
        foreach(int move in p.captureMoves){
            if(move == moveOffset){
                if(Board[i]!=null){
                    if(Board[i].id != p.id){
                        canCapture = true;
                        }
                }
            }
        }
        return canCapture;
    }
    public bool MovePiece(int pPosition, int i){
        Piece p = this.Board[pPosition];
        bool canMove = this.LegalMovement(pPosition, i);
        bool canCapture = this.LegalCapture(pPosition, i);
        Debug.Log(canMove);
        Debug.Log(canCapture);
        bool selfChecked = false;
        bool isBlocked = false;
        int ownKingPosition;
        int tempPosition;


        if(canCapture){
            canMove = false;
        }
        if(canMove == true){
            if(p.GetType() != whiteKnight.GetType()){
                isBlocked = this.BlockedPath(pPosition, i);
            }
            if(isBlocked == true){
                return false;
            }
            // Move to opponent piece should not be allowed
            if(this.Board[i] != null){
                if (this.Board[i].id != p.id){
                    return false;
                }
            }
            this.Board[pPosition] = null;
            tempPosition = pPosition;
            pPosition = i;
            this.Board[i] = p;
            ownKingPosition = this.getOwnKingPosition(p.id);
            selfChecked = this.SelfCheck(ownKingPosition);
            if(selfChecked){
                this.Board[tempPosition] = p;
                this.Board[i] = null;
                Debug.Log("SELF CHECKED");
                return false;
            }
            return true;
        }
        Debug.Log(canCapture);
        if(canCapture == true){
            if(p.GetType() != whiteKnight.GetType()){
                isBlocked = this.BlockedPath(pPosition, i);
            }
            if(isBlocked == true){
                return false;
            }

            //Arrumar isso aqui
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

        if(nextState == GameState.WHITEPAWNS)
            if (this.moveCount < 2){
                this.PawnHand(0);
            }
            else{
                this.RandomPiece(0);
            }
        else if(nextState == GameState.BLACKPAWNS)
            if (this.moveCount < 2){
                this.PawnHand(1);
            }
            else{
                this.RandomPiece(1);
            }
        gameState = nextState;
    }

    private GameManager()
    {
        WhiteHandTiles = GameObject.Find("WhiteHand");
        BlackHandTiles = GameObject.Find("BlackHand");
        for(int i = 0; i<8; i++){
            whitePawns[i] = new Pawn(0);
            blackPawns[i] = new Pawn(1);
        }
        whiteDeck = new Deck(whitePawns, whiteBishop, whiteQueen, whiteRook, whiteKnight);
        blackDeck = new Deck(blackPawns, blackBishop, blackQueen, blackRook, blackKnight);
        // ChangeState(gameState);
    }

    public void FillDefaultBoard(){
            // this.Board[0] = this.blackRook;
            // this.Board[7] = this.blackRook;
            // this.Board[1] = this.blackKnight;
            // this.Board[6] = this.blackKnight;
            // this.Board[2] = this.blackBishop;
            // this.Board[5] = this.blackBishop;
            // this.Board[3] = this.blackQueen;
            // this.Board[56] = this.whiteRook;
            // this.Board[63] = this.whiteRook;
            // this.Board[57] = this.whiteKnight;
            // this.Board[62] = this.whiteKnight;
            // this.Board[58] = this.whiteBishop;
            // this.Board[61] = this.whiteBishop;
            // this.Board[59] = this.whiteQueen;
            // for(int i = 8, j = 0; i<16; i++, j++){
            //     this.Board[i] = this.blackPawns[j];
            // }
            // for(int i = 48, j = 0; i<56; i++, j++){
            //     this.Board[i] = this.whitePawns[j];
            // }
    }



    public void PawnHand(int id){
        if(id == 0){
            whiteHand.Add(whiteDeck.getPawn(0));
            whiteHandSize++;
        }
        else{
            blackHand.Add(blackDeck.getPawn(1));
            blackHandSize++;
        }
    }

    public void RandomPiece(int id){
        if(id == 0){
            if(whiteHandSize < 8){
                whiteHand.Add(whiteDeck.GetPiece(0));
                whiteHandSize++;
            }
            else{
                whiteDeck.GetPiece();
            }
        }
        else{
            if(blackHandSize < 8){
                whiteHand.Add(whiteDeck.GetPieblackce(1));
                blackHandSize++;
            }
            else{
                blackDeck.GetPiece();
            }
        }
    }


    public bool placePiece(Piece p, int position){
        if(Board[position] == null){
            Board[position] = p;
            return true;
        }
        else return false;
    }

    public void clickedTile(int tileID, int color){
        if(this.gameState == GameState.WHITEPAWNS && color == 1){
            this.clickedHand = false;
            return;
        } 
        
        else if(this.gameState == GameState.BLACKPAWNS && color == 0){
            this.clickedHand = false;
            return;
        }
        if(color == 1){
            if(blackHand[tileID] == null){
                return;
            }
        }
        else{
            if(whiteHand[tileID] == null){
                return;
            } 
        }
        this.selectedPiece = tileID;
        this.clickedHand = true;
        if(color == 0){
            this.ChangeState(GameState.WHITEMOVE);
        }
        else{
            this.ChangeState(GameState.BLACKMOVE);
        }
        
    }


    public void clickedTile(int id){
        // Debug.Log(String.Format("Clicked tile {0} with state {1}", id, this.gameState.ToString()));
        switch (this.gameState) {
            case (GameState.WHITEPAWNS):
                if(this.Board[id] == null) return;
                if (this.Board[id].id == 0){
                    this.selectedTile = id;
                    this.ChangeState(GameState.WHITEMOVE);
                }
                break;
            case (GameState.WHITEMOVE):
                if(clickedHand){
                    this.clickedHand = false;
                    bool placed = this.placePiece(whiteHand[selectedPiece], id);
                    if(placed) {
                        this.ChangeState( GameState.BLACKPAWNS);
                        this.selectedTile = -1;
                        whiteHand[selectedPiece] = null;
                    }
                    else{
                        this.gameState = GameState.WHITEPAWNS;
                        this.selectedTile = -1;
                    }
                    break;
                }
                bool moved = this.MovePiece(this.selectedTile, id);
                if(moved){
                    this.ChangeState( GameState.BLACKPAWNS);
                    this.selectedTile = -1;
                }
                else{
                    this.gameState = GameState.WHITEPAWNS;
                    this.selectedTile = -1;
                }
                break;
            case (GameState.BLACKPAWNS):
                if(this.Board[id] == null) return;
                if (this.Board[id].id == 1){
                    this.selectedTile = id;
                    this.ChangeState(GameState.BLACKMOVE);
                }
                break;
            case (GameState.BLACKMOVE):
                if(clickedHand){
                    this.clickedHand = false;
                    bool placed = this.placePiece(blackHand[selectedPiece], id);
                    if(placed) {
                        this.ChangeState(GameState.WHITEPAWNS);
                        this.selectedTile = -1;
                        blackHand[selectedPiece] = null;
                    }
                    else{
                        this.gameState = GameState.BLACKPAWNS;
                        this.selectedTile = -1;
                    }
                    break;
                }
                
                moved = this.MovePiece(this.selectedTile, id);
                if(moved){
                    this.ChangeState(GameState.WHITEPAWNS);
                    this.selectedTile = -1;
                    this.moveCount ++;
                }
                else{
                    this.gameState = GameState.BLACKPAWNS;
                    this.selectedTile = -1;
                }
                break;
            default:
                break;
        }
    }
}
