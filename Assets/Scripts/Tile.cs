using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private int id;
    private GameObject controller;
    private int[] infoList = new int[2];
    private void OnMouseDown() {
         this.controller.SendMessage("TileClicked", this.infoList);
    }

    public void setId(int id){
        this.id = id;
        this.infoList[0] = this.id;
        this.infoList[1] = -1;
    }

    public void setController (GameObject controller){
        this.controller = controller;
    }

}
