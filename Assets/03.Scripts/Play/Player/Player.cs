using UnityEngine;

public enum EPlayerState
{ 
    None = 0,

    Exhaustion, //Ż��
    Stiffness, //����
    Stun, //����
    Fainting, //����
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
