using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTile : MonoBehaviour
{
    private int id;
    private GameObject controller;
    private int color;
    public GameObject halo;
    private int[] infoList = new int[2];
    private void OnMouseDown() {
        this.controller.SendMessage("TileClicked", this.infoList);
    }

    public void setId(int id){
        this.id = id;
    }

    public void setColor(int color){
        this.color = color;
    }

    public void setHalo(bool t){
        this.halo.SetActive(t);
    }

    public void createInfoList(){
        this.infoList[0] = id;
        this.infoList[1] = color;
    }

    public void setController (GameObject controller){
        this.controller = controller;
    }
}
