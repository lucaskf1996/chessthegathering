using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayStateScript : MonoBehaviour
{
    private GameManager gm;
    public Text txt;
    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.GetInstance();
    }

    // Update is called once per frame
    void Update()
    {
        txt.text = gm.gameState.ToString();
    }
}
