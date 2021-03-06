using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;




public class GameManager{
    private static GameManager _instance;
    private AudioSource PieceMove, FailMove;
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
    public List<Piece> whiteHand;
    public List<Piece> blackHand;
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
        bool legalMovement = false;
        Piece p;
        for(int i = 0; i < 64; i++){
            if(this.Board[i] == null || i==kingPosition){ // Skip empty positions and king (no suicide)
                continue;
            }
            if(this.Board[i].id != this.Board[kingPosition].id){
                p = this.Board[i];
                legalMovement = p.legalMovement(this.Board, i, kingPosition);
                if(legalMovement){
                    kingInCheck = true;
                    Debug.Log("SELF CHECK");
                    return true;
                }
            }
        }
        return false;
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
        bool isLegal = p.legalMovement(this.Board, pPosition, i);
        if (!isLegal) return false;
        // Only legal moves are treated below, now to filter checks

        // Temporarily go to target position 
        Piece temp;
        if (this.Board[i] != null){
            temp = this.Board[i];
        } else {
            temp = null;
        }
        this.Board[i] = p;
        this.Board[pPosition] = null;

        // See if result board is in check
        bool leadsToCheck = this.SelfCheck(this.getOwnKingPosition(p.id));

        // If leadsToCheck undo move and return false;
        if (leadsToCheck) {
            this.Board[pPosition] = p;
            this.Board[i] = temp;
            return false;
        }

        // Else, make move permanent
        if (temp != null) temp.Capture();
        
        if (p is King) updateDistance(i, p);

        return true;
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

        if(gameState == GameState.BLACKMOVE && nextState == GameState.WHITEPAWNS){
            PieceMove.Play();
        }
        else if(gameState == GameState.WHITEMOVE && nextState == GameState.BLACKPAWNS){
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

        whiteHand = new List<Piece>(0);
        blackHand = new List<Piece>(0);

        WhiteHandTiles = GameObject.Find("WhiteHand");
        BlackHandTiles = GameObject.Find("BlackHand");
        for(int i = 0; i<6; i++){
            whitePawns[i] = new Pawn(0);
            blackPawns[i] = new Pawn(1);
        }
        whiteDeck = new Deck(whitePawns, whiteBishop, whiteQueen, whiteRook, whiteKnight);
        blackDeck = new Deck(blackPawns, blackBishop, blackQueen, blackRook, blackKnight);
        // ChangeState(gameState);
        PieceMove = GameObject.Find("Main Camera").GetComponents<AudioSource>()[0];
        FailMove = GameObject.Find("Main Camera").GetComponents<AudioSource>()[1];
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
        if(this.canPlace(p, position)){
            Board[position] = p;
            return true;
        }
        return false;
    }

    public bool canPlace(Piece p, int destination){
        // Return true if is valid placement
        return 
        (this.Board[destination] == null)
        &&
        (checkPlacingDistance(p, destination));
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
        // White turn; Clicked on black hand
        if(this.gameState == GameState.WHITEPAWNS && color == 1){
            this.clickedHand = false;
            this.selectedTile = -1;
            this.selectedPiece = -1;
            return;
        } 
        
        // Black turn; Clicked on white hand
        if(this.gameState == GameState.BLACKPAWNS && color == 0){
            this.clickedHand = false;
            this.selectedTile = -1;
            this.selectedPiece = -1;
            return;
        }

        // Cant move to white hand
        if(this.gameState == GameState.WHITEMOVE){
            this.clickedHand = false;
            this.selectedTile = -1;
            this.selectedPiece = -1;
            this.gameState = GameState.WHITEPAWNS;
            return;
        }

        // Cant move to black hand
        if(this.gameState == GameState.BLACKMOVE){
            this.clickedHand = false;
            this.selectedTile = -1;
            this.selectedPiece = -1;
            this.gameState = GameState.BLACKPAWNS;
            return;
        }



        // Debug.Log("BlackCount: " + blackHand.Count);
        // Debug.Log("WhiteCount: " + whiteHand.Count);
        // Debug.Log("Id: " + tileID);
        if(color == 1){
            if(blackHand.Count <= tileID){
                return;
            }
        }
        else{
            if(whiteHand.Count <= tileID){
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
                    if (placed) {
                        this.ChangeState( GameState.BLACKPAWNS);
                        this.Board[id] = whiteHand[selectedPiece];
                        whiteHand.RemoveAt(selectedPiece);
                        this.moveCount ++;
                        placed = false;
                        this.gameStarted = true;
                    } else {
                        this.FailMove.Play();
                        this.gameState = GameState.WHITEPAWNS;
                    }
                    this.selectedTile = -1;
                    this.selectedPiece = -1;
                    break;
                }
                bool moved = this.MovePiece(this.selectedTile, id);
                if (moved) {
                    this.ChangeState( GameState.BLACKPAWNS);
                    this.gameStarted = true; // Only does anything the first time;
                } else {
                    this.FailMove.Play();
                    this.gameState = GameState.WHITEPAWNS;
                }
                this.selectedTile = -1;
                this.selectedPiece = -1;
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
                    if(placed) {
                        this.ChangeState(GameState.WHITEPAWNS);
                        this.Board[id] = blackHand[selectedPiece];
                        blackHand.RemoveAt(selectedPiece);
                        this.moveCount ++;
                    }
                    else{
                        this.FailMove.Play();
                        this.gameState = GameState.BLACKPAWNS;
                    }
                    this.selectedTile = -1;
                    this.selectedPiece = -1;
                    break;
                }
                
                moved = this.MovePiece(this.selectedTile, id);
                if(moved){
                    this.ChangeState(GameState.WHITEPAWNS);
                    this.moveCount ++;
                }
                else {
                    this.FailMove.Play();
                    this.gameState = GameState.BLACKPAWNS;
                }
                this.selectedTile = -1;
                this.selectedPiece = -1;
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