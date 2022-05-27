using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    private GameObject board;
    public Sprite boardSprite;
    public Sprite[] pieces; // Black -> White; King, Queen, Bishop, Knight, Rook, Pawn
    private List<GameObject> tiles;
    private SpriteRenderer sprBoard;
    public GameObject tilePrefab;

    private Ray ray;
    private RaycastHit rht;
    private GameManager gm;
    public bool _debugging = true;

    void Start()
    {   
        gm = GameManager.GetInstance();
        gm.SetInitialPieces();
        this.board = new GameObject("Board");
        this.board.transform.localScale = new Vector3(0.5f, 0.5f, 0.0f);
        this.sprBoard = this.board.AddComponent<SpriteRenderer>();
        this.sprBoard.sprite = this.boardSprite;

        // Get dimensions
        float width, height;
        width = this.sprBoard.bounds.size.x;
        height = this.sprBoard.bounds.size.y;
        Debug.Log("Width: " + width);
        Debug.Log("Height: " + height);
        float half_width = width/2f;
        float half_height = height/2f;
        float xoff, yoff;
        xoff = -half_width+width/16f;
        yoff = -half_height;

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
        if(this._debugging){
            gm.FillDefaultBoard();
        }
        this.fillBoard();
    }
    void TileClicked(int id){
        // Debug.Log(id);
        // this.setSprite(id, Random.Range(0,12));
        this.gm.clickedTile(id);
        this.fillBoard();
    }

    void setSprite(int tileID, int spriteID){
        // Debug.Log(string.Format("Setting tile: {0} to: {1}", tileID, spriteID));
        this.tiles[tileID].GetComponent<SpriteRenderer>().sprite = this.pieces[spriteID];
    }

    void clearSprite(int tileId){
        this.tiles[tileId].GetComponent<SpriteRenderer>().sprite = null;
    }

    void fillBoard(){
        for(int i = 0; i < 64; i++){
            if(gm.Board[i] != null){
                setSprite(i, gm.Board[i].spriteId);
            } else clearSprite(i);
        }
    }
}
