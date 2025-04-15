using UnityEngine;
using UnityEngine.Playables;

public enum EPlayerState
{ 
    None = 0,

    Exhaustion, //탈진
    Stiffness, //경직
    Stun, //스턴
    Fainting, //기절
    Battle,

}

public enum AttakcType
{
    Blow,
    Slash,
    Ult,
}


public class Player : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] AttakcType attakcType;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerMovement.Handle();
    }
}
