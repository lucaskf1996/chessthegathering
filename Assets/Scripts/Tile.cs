using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private int id;
    private GameObject controller;
    private void OnMouseDown() {
         this.controller.SendMessage("TileClicked", this.id);
    }

    public void setId(int id){
        this.id = id;
    }

    public void setController (GameObject controller){
        this.controller = controller;
    }

}
