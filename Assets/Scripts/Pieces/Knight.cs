using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class Knight : Piece
{
    public int id {get; set; }
    public int spriteId {get; set; }
    public Piece.STATE state {get; set; }
    public List<int> legalMoves {get;}
    public List<int> captureMoves {get;}
    public int position {get; set; }
    public Knight(int id){
        this.id = id;
        this.spriteId = (this.id == 0) ? 9 : 3;
        this.state = Piece.STATE.DECK;
        this.legalMoves = new List<int>();
        this.legalMoves.Add(-17);
        this.legalMoves.Add(-15);
        this.legalMoves.Add(-10);
        this.legalMoves.Add(-6);
        this.legalMoves.Add(6);
        this.legalMoves.Add(10);
        this.legalMoves.Add(15);
        this.legalMoves.Add(17);
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
        if((Math.Abs(colOffset) == 1) && (Math.Abs(lineOffset) == 2)){ // Sideways L
            return true;
        }
        if((Math.Abs(colOffset) == 2) && (Math.Abs(lineOffset) == 1)){ // Vertical L
            return true;
        }
        return false;

    }

    public bool blockedPath(Piece[] Board, int pPosition, int tPosition){
        return false; // Can't block this
    }

    public bool canCapture(Piece[] Board, int pPosition, int tPosition){
        return(this.canMove(Board, pPosition, tPosition));
    }
    public bool legalMovement(Piece[] Board, int pPosition, int tPosition){
        return((this.canMove(Board, pPosition, tPosition) || this.canCapture(Board, pPosition, tPosition)) && !this.blockedPath(Board, pPosition, tPosition));
    }
}