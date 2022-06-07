using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;




public class GameManager{
    private static GameManager _instance;
    private AudioSource PieceMove;
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
    public int whiteHandSize;
    public int blackHandSize;
    public int moveCount;
    public int distanceTravelledBlack;
    public int distanceTravelledWhite;
    public Piece clickedPiece;
    public Piece[] Board;
    public enum GameState {WHITEPAWNS, BLACKPAWNS, WHITEHAND, BLACKHAND, WHITEMOVE, BLACKMOVE, ENDGAME}; //n sei se vai ser assim ainda
    private GameObject WhiteHandTiles, BlackHandTiles;
    public GameState gameState { get; private set; }
    public int selectedTile = -1;
    private int initialPawns = 0;
    public int selectedPiece = -1;
    private bool clickedHand = false;
    public float whiteTimer, blackTimer;
    public bool gameStarted = false;
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

    public void updateDistance(int position, Piece p){
        if(p.id == 0){
            distanceTravelledWhite = ((8-distanceTravelledWhite)*8>=position) ? distanceTravelledWhite+1 : distanceTravelledWhite;
        }
        else{
            distanceTravelledBlack = (distanceTravelledBlack*8<position) ? distanceTravelledBlack+1 : distanceTravelledBlack;
        }
    }

    public bool MovePiece(int pPosition, int i){
        Piece p = this.Board[pPosition];
        bool canMove = this.LegalMovement(pPosition, i);
        bool canCapture = this.LegalCapture(pPosition, i);
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
            if(p is King){
                this.updateDistance(pPosition, p);
            }
            return true;
        }
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
            return false;
        }

    }
    
    public void ChangeState(GameState nextState)
    {   
        if(nextState == GameState.WHITEPAWNS){
            if (this.moveCount < 2) this.PawnHand(0);
            else this.RandomPiece(0);
        }
        else if(nextState == GameState.BLACKPAWNS){
            if (this.moveCount < 2) this.PawnHand(1);
            else this.RandomPiece(1);
        }

        if(gameState ==GameState.BLACKMOVE && nextState == GameState.WHITEPAWNS){
            PieceMove.Play();
        }
        else if(gameState ==GameState.WHITEMOVE && nextState == GameState.BLACKPAWNS){
            PieceMove.Play();
        }

        gameState = nextState;
    }

    private GameManager()
    {
        SetStartingVariables();
    }

    private void SetStartingVariables(){
        whiteHandSize = 0;
        blackHandSize = 0;
        moveCount = 0;
        distanceTravelledBlack = 1;
        distanceTravelledWhite = 1;
        selectedTile = -1;
        selectedPiece = -1;
        clickedHand = false;
        gameStarted = false;

        WhiteHandTiles = GameObject.Find("WhiteHand");
        BlackHandTiles = GameObject.Find("BlackHand");
        for(int i = 0; i<8; i++){
            whitePawns[i] = new Pawn(0);
            blackPawns[i] = new Pawn(1);
        }
        whiteDeck = new Deck(whitePawns, whiteBishop, whiteQueen, whiteRook, whiteKnight);
        blackDeck = new Deck(blackPawns, blackBishop, blackQueen, blackRook, blackKnight);
        // ChangeState(gameState);
        PieceMove = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        // this.ChangeState(GameState.WHITEPAWNS);
        Board = new Piece[64];
    }

    public void reset(){
        this.SetStartingVariables();
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
            if(whiteHand.Count < 8){
                whiteHand.Add(whiteDeck.GetPiece());
                // whiteHandSize++;
            }
            else{
                whiteDeck.GetPiece();
            }
        }
        else{
            if(blackHand.Count < 8){
                blackHand.Add(blackDeck.GetPiece());
                // blackHandSize++;
            }
            else{
                blackDeck.GetPiece();
            }
        }
    }

    public bool placePiece(Piece p, int position){
        if(Board[position] == null && checkPlacingDistance(p, position)){
            Board[position] = p;
            return true;
        }
        else return false;
    }

    public bool checkPlacingDistance(Piece p, int position){
        if(p is Pawn){
            if(gameState == GameState.WHITEMOVE){
                if(position>=32){
                    return true;
                }
                else{
                    return false;
                }
            }
            else{ 
                if(position<32){
                    return true;
                }
                else{
                    return false;
                }
            }
        }
        else{
            if(gameState == GameState.WHITEMOVE){
                Debug.Log(this.distanceTravelledWhite);
                if(position>=8*(8-this.distanceTravelledWhite) && position>=32){
                    return true;
                }
                else{
                    return false;
                }
            }
            else{ // if(gameState == GameState.BLACKMOVE)
                if(position<8*this.distanceTravelledBlack && position<32){
                    return true;
                }
                else{
                    return false;
                }
            }
        }
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
            if(blackHand.Count < tileID){
                return;
            }
        }
        else{
            if(whiteHand.Count < tileID){
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
                if (this.Board[id] == null) return;
                if (this.Board[id].id == 0) {
                    this.selectedTile = id;
                    this.ChangeState(GameState.WHITEMOVE);
                }
                break;
            case (GameState.WHITEMOVE):
                if (clickedHand) {
                    this.clickedHand = false;
                    int ownKingPosition = this.getOwnKingPosition(0);
                    bool selfChecked = this.SelfCheck(ownKingPosition);
                    bool placed = false;
                    if (selfChecked) {
                        placed = false;
                    } else {
                        placed = this.placePiece(whiteHand[selectedPiece], id);
                    }
                    this.selectedTile = -1;
                    if (placed) {
                        this.ChangeState( GameState.BLACKPAWNS);
                        this.Board[id] = whiteHand[selectedPiece];
                        whiteHand.RemoveAt(selectedPiece);
                        this.moveCount ++;
                        placed = false;
                        this.gameStarted = true;
                    } else {
                        this.gameState = GameState.WHITEPAWNS;
                    }
                    this.selectedPiece = -1;
                    break;
                }
                bool moved = this.MovePiece(this.selectedTile, id);
                this.selectedTile = -1;
                this.selectedPiece = -1;
                if (moved) {
                    this.ChangeState( GameState.BLACKPAWNS);
                    this.gameStarted = true; // Only does anything the first time;
                } else {
                    this.gameState = GameState.WHITEPAWNS;
                }
                break;
            case (GameState.BLACKPAWNS):
                if (this.Board[id] == null) return;
                if (this.Board[id].id == 1) {
                    this.selectedTile = id;
                    this.ChangeState(GameState.BLACKMOVE);
                }
                break;
            case (GameState.BLACKMOVE):
                if(clickedHand){
                    this.clickedHand = false;
                    int ownKingPosition = this.getOwnKingPosition(1);
                    bool selfChecked = this.SelfCheck(ownKingPosition);
                    bool placed = false;
                    if(selfChecked){
                        placed = false;
                    }
                    else{
                        placed = this.placePiece(blackHand[selectedPiece], id);
                    }
                    this.selectedTile = -1;
                    if(placed) {
                        this.ChangeState(GameState.WHITEPAWNS);
                        this.Board[id] = blackHand[selectedPiece];
                        blackHand.RemoveAt(selectedPiece);
                        this.moveCount ++;
                    }
                    else{
                        this.gameState = GameState.BLACKPAWNS;
                    }
                    this.selectedPiece = -1;
                    break;
                }
                
                moved = this.MovePiece(this.selectedTile, id);
                this.selectedTile = -1;
                this.selectedPiece = -1;
                if(moved){
                    this.ChangeState(GameState.WHITEPAWNS);
                    this.moveCount ++;
                }
                else{
                    this.gameState = GameState.BLACKPAWNS;
                }
                break;
            default:
                break;
        }
    }

    public void startTimer(){
        this.whiteTimer = 300.0f;
        this.blackTimer = 300.0f;
    }

    public int getTurn(){
        if(this.gameState == GameState.WHITEPAWNS || this.gameState == GameState.WHITEMOVE) return 0;
        return 1;
    }
}
