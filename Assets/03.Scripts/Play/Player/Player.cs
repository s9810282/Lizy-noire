using UnityEngine;
using UnityEngine.Playables;

public enum EPlayerState
{ 
    None = 0,

    Exhaustion, //탈진
    Stiffness, //경직
    Idle, //스턴
    Move, //기절
    KnockBack,
    Boost,
}



public enum EAttakcType
{
    Blow,
    Slash,
    Ult,
}


public class Player : MonoBehaviour
{
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }
}
