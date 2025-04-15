using UnityEngine;
using UnityEngine.Playables;

public enum EPlayerState
{ 
    None = 0,

    Exhaustion, //Ż��
    Stiffness, //����
    Stun, //����
    Fainting, //����
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
