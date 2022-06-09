using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    private void Update() {
        if(isLocalPlayer){
            if(Input.GetKeyDown("space")){
                Debug.Log("Space");
            }
        }    
    }
}
