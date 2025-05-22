using UnityEngine;


public enum EffectType
{
    Blow,
    Slash,
    Ult,

    Stun,
    Invinsible,

}



public class EffectManager
{
    private static EffectManager instance;
    public static EffectManager Instance
    {
        get 
        { 
            if(instance == null)
                instance = new EffectManager();

            return instance; 
        } 
    }



    public void PlayEffect(EffectData data, Transform parent)
    {
        GameObject fx = GameObject.Instantiate(data.effectPrefab, parent);

        fx.transform.localPosition = data.localPositionOffset;
        fx.transform.localEulerAngles = data.localRotation;
        fx.transform.localScale = data.localScale;

        GameObject.Destroy(fx, data.duration);
    }
}
