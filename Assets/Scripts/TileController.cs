using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    private GameObject board;
    private GameObject whiteHand;
    private GameObject blackHand;
    public Sprite boardSprite;
    public Sprite handSprite;
    public Sprite[] pieces; // Black -> White; King, Queen, Bishop, Knight, Rook, Pawn
    private List<GameObject> tiles;
    private List<GameObject> whiteHandTiles;
    private List<GameObject> blackHandTiles;
    private SpriteRenderer sprBoard;
    private SpriteRenderer sprWhiteHand;
    private SpriteRenderer sprBlackHand;
    public GameObject tilePrefab;
    public GameObject handTilePrefab;
    private int whiteId = 0;
    private int blackId = 1;
    private Ray ray;
    private RaycastHit rht;
    private GameManager gm;
    public bool _debugging;

    void Start()
    {   
        gm = GameManager.GetInstance();
        gm.SetInitialPieces();
        this.board = new GameObject("Board");
        this.whiteHand = new GameObject("whiteHand");
        this.blackHand = new GameObject("blackHand");
        this.board.transform.localScale = new Vector3(0.5f, 0.5f, 0.0f);
        this.whiteHand.transform.localScale = new Vector3(2f, 2f, 0.0f);
        this.blackHand.transform.localScale = new Vector3(2f, 2f, 0.0f);
        this.sprBoard = this.board.AddComponent<SpriteRenderer>();
        this.sprBoard.sprite = this.boardSprite;
        this.sprWhiteHand = this.whiteHand.AddComponent<SpriteRenderer>();
        this.sprWhiteHand.sprite = this.handSprite;
        this.sprBlackHand = this.blackHand.AddComponent<SpriteRenderer>();
        this.sprBlackHand.sprite = this.handSprite;

        // Get dimensions
        float width, height;
        width = this.sprBoard.bounds.size.x;
        height = this.sprBoard.bounds.size.y;
        Debug.Log("Width: " + width);
        Debug.Log("Height: " + height);
        float half_width = width/2f;
        float half_height = height/2f;
        float xoff, yoff, whiteOffX, blackOffX;
        xoff = -half_width+width/16f;
        yoff = -half_height;
        
        whiteOffX = -half_width-(width/16f)*5;
        blackOffX = -half_width-(width/16f)*3;

        this.whiteHand.transform.position = new Vector3(-half_width-(width/16f)*4, 0f, 0.0f);
        this.blackHand.transform.position = new Vector3(half_width+(width/16f)*4, 0f, 0.0f);

        int tileCount = 0;
        tiles = new List<GameObject>();
        for(float j = 9f; j>0f; j-=height/8f){
            for(float i = 0f; i<9f; i+=width/8f){
                GameObject p = Instantiate(tilePrefab, new Vector3(i+xoff, j+yoff, 0), Quaternion.identity);
                p.transform.parent = this.board.transform;
                tiles.Add(p);
                p.SendMessage("setId", tileCount);
                p.SendMessage("setController", this.gameObject);
                tileCount ++;
            }
        }
        int tileCountWhite = 0;
        whiteHandTiles = new List<GameObject>();
        blackHandTiles = new List<GameObject>();
        for(float j = 6.5f; j>2.5f; j-=height/8f){
            for(float i = 0f; i<2*width/8f; i+=width/8f){
                GameObject w = Instantiate(handTilePrefab, new Vector3(i+whiteOffX, j+yoff, 0), Quaternion.identity);
                w.transform.parent = this.whiteHand.transform;
                whiteHandTiles.Add(w);
                w.SendMessage("setId", tileCountWhite);
                w.SendMessage("setColor", this.whiteId);
                w.SendMessage("createInfoList");
                w.SendMessage("setController", this.gameObject);

                GameObject b = Instantiate(handTilePrefab, new Vector3(i-blackOffX, j+yoff, 0), Quaternion.identity);
                b.transform.parent = this.blackHand.transform;
                blackHandTiles.Add(b);
                b.SendMessage("setId", tileCountWhite);
                b.SendMessage("setColor", this.blackId);
                b.SendMessage("createInfoList");
                b.SendMessage("setController", this.gameObject);

                tileCountWhite ++;
            }
        }
        gm.ChangeState(GameManager.GameState.WHITEPAWNS);
        this.fillHands();
        if(this._debugging){
            gm.FillDefaultBoard();
        }
        this.fillBoard();
    }

    void TileClicked(int tileID){
        // Debug.Log(id);
        // this.setSprite(id, Random.Range(0,12));
        this.gm.clickedTile(tileID);
        this.fillBoard();
        this.fillHands();
    }

    void TileClicked(int[] infoList){
        // Debug.Log(id);
        // this.setSprite(id, Random.Range(0,12));
        int tileID = infoList[0];
        int color = infoList[1];
        this.gm.clickedTile(tileID, color);
        this.fillBoard();
        this.fillHands();
    }

    void setSprite(int tileID, int spriteID){
        // Debug.Log(string.Format("Setting tile: {0} to: {1}", tileID, spriteID));
        this.tiles[tileID].GetComponent<SpriteRenderer>().sprite = this.pieces[spriteID];
    }

    void setSpriteHand(int tileID, int spriteID, int hand){
        // print(hand);
        if(hand == 0){
            this.whiteHandTiles[tileID].GetComponent<SpriteRenderer>().sprite = this.pieces[spriteID];
        }
        else{
            this.blackHandTiles[tileID].GetComponent<SpriteRenderer>().sprite = this.pieces[spriteID];                
        }
    }

    void clearSprite(int tileId){
        this.tiles[tileId].GetComponent<SpriteRenderer>().sprite = null;
    }

    void clearSpriteHand(int tileId, int hand){
        if(hand == 0){
            this.whiteHandTiles[tileId].GetComponent<SpriteRenderer>().sprite = null;
        }
        else{
            Debug.Log("Entrei Else");
            this.blackHandTiles[tileId].GetComponent<SpriteRenderer>().sprite = null;
        }
    }

    void fillBoard(){
        for(int i = 0; i < 64; i++){
            removeGlow(i);
            if(gm.Board[i] != null){
                if (i == gm.selectedTile) setGlow(i);  
                setSprite(i, gm.Board[i].spriteId);
            } else clearSprite(i);
        }
    }

    void setGlow(int i){
        this.tiles[i].SendMessage("setHalo", true);
    }

    void removeGlow(int i){
        this.tiles[i].SendMessage("setHalo", false);
    }

    void setGlowHand(int i, int id){
        if(id == 1){
            this.blackHandTiles[i].SendMessage("setHalo", true);
        }
        else{
            this.whiteHandTiles[i].SendMessage("setHalo", true);
        }
    }

    void removeGlowHand(int i){
        this.whiteHandTiles[i].SendMessage("setHalo", false);
        this.blackHandTiles[i].SendMessage("setHalo", false);
    }
    
    void fillHands(){
        for(int i = 0; i < 8; i++){
            removeGlowHand(i);
            // Debug.Log("white hand:  " + gm.whiteHand.Count);
            if(i<=gm.whiteHand.Count-1){
                if (i == gm.selectedPiece) setGlowHand(i,0); 
                setSpriteHand(i, gm.whiteHand[i].spriteId, this.whiteId);
            } else clearSpriteHand(i, this.whiteId);
            if(i <= gm.blackHand.Count-1){
                if (i == gm.selectedPiece) setGlowHand(i,1); 
                setSpriteHand(i, gm.blackHand[i].spriteId, this.blackId);
            } else clearSpriteHand(i, this.blackId);
        }
    }
}
