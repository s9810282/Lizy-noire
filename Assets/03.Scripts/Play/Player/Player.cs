using UnityEngine;
using UnityEngine.Playables;

public enum EPlayerState
{ 
    None = 0,

    Exhaustion, 
    Idle, 
    Move, 
    KnockBack,
    Boost,

    Dead
}



public enum EAttakcType
{
    None,
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
