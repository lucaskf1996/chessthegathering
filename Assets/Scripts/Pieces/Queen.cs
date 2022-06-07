using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;


public class Queen : Piece
{
    public int id {get; set; }
    public int spriteId {get; set; }
    public Piece.STATE state {get; set; }
    public List<int> legalMoves {get;}
    public List<int> captureMoves {get;}
    public int position {get; set; }
    public Queen(int id){
        this.id = id;
        this.spriteId = (this.id == 0) ? 7 : 1;
        this.state = Piece.STATE.DECK;
        this.legalMoves = new List<int>();
        for (int i = 8; i <= 56; i+=8){
            this.legalMoves.Add(i);
            this.legalMoves.Add(-i);
        }
        for (int i = 1; i < 8; i++){
            this.legalMoves.Add(i);
            this.legalMoves.Add(-i);
        }
        for (int i = 9; i <= 54; i+=9){
            this.legalMoves.Add(i);
            this.legalMoves.Add(-i);
        }
        for (int i = 7; i <= 49; i+=7){
            this.legalMoves.Add(i);
            this.legalMoves.Add(-i);
        }
        
        this.captureMoves = new List<int>(this.legalMoves);
        this.position = -1; // Not in play yet        
    }

    public void Capture(){
        this.state = Piece.STATE.CAPTURED;
        this.position = -1;
    }

    public bool canMove(Piece[] Board, int pPosition, int tPosition){
        int pColumn = pPosition % 8;
        int pLine = pPosition / 8;
        int tColumn = tPosition % 8;
        int tLine = tPosition / 8;
        int colOffset = pColumn - tColumn;
        int lineOffset = pLine - tLine;

        if((colOffset == 0) && (lineOffset == 0)) return false; //Same place
        if(Math.Abs(colOffset) == Math.Abs(lineOffset)){ //Diagonal
            return true;
        }
        if((colOffset == 0) || (lineOffset == 0)){ //Horizontal and Vertical
            return true;
        }

        return false;

    }

    public bool blockedPath(Piece[] Board, int pPosition, int tPosition){
        if(Board[tPosition]!= null){
            if(this.id == Board[tPosition].id){
                return true;
            }
        }
        int path;
        int hTile = Math.Max(pPosition, tPosition);
        int lTile = Math.Min(pPosition, tPosition);
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
            if(bIndex == pPosition || bIndex > hTile || bIndex == tPosition){
                continue;
            }
            if(bIndex > 0 && bIndex < 63 ){
                if(Board[lTile + i*path] != null){
                    Debug.Log("BLOCKED PATH");
                    return true;
                }
            }
        }
        return false;
    }

    public bool canCapture(Piece[] Board, int pPosition, int tPosition){
        return(this.canMove(Board, pPosition, tPosition));

    }
    public bool legalMovement(Piece[] Board, int pPosition, int tPosition){
        return((this.canMove(Board, pPosition, tPosition) || this.canCapture(Board, pPosition, tPosition)) && !this.blockedPath(Board, pPosition, tPosition));

    }
}

