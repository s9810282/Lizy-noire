using UnityEngine;

public enum EPlayerState
{ 
    None = 0,

    Exhaustion, //탈진
    Stiffness, //경직
    Stun, //스턴
    Fainting, //기절
    Battle,

}



public class Player : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement;



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerMovement.Handle();
    }
}
