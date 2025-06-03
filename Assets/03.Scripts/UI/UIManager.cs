using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public struct SpaceToggleEvent
{

}

public struct BoostUIEvent
{
    public bool isUse;
    public int boostCount;

    public float maxtimer;
    public float timer;
}



public class UIManager : MonoBehaviour
{
    [Header("Space")]
    [SerializeField] Image spaceToggle;
    [SerializeField] Color spaceOnColor;
    [SerializeField] Color spaceOffColor;
    [SerializeField] bool isToggle = false;

    [Header("AttackType")]
    [SerializeField] Image attackTypeImage;
    [SerializeField] Color blowColor;
    [SerializeField] Color slashOffColor;

    [Header("Boost")]
    [SerializeField] Image[] boostGages;

    
    void OnEnable()
    {
        EventBus.Subscribe<SpaceToggleEvent>(SpaceToggle);
        EventBus.Subscribe<BoostUIEvent>(UpdateBoostGage);
        EventBus.Subscribe<EAttakcType>(ChangeAttackType);
    }

    // Update is called once per frame
    void OnDisable()
    {
        EventBus.Unsubscribe<SpaceToggleEvent>(SpaceToggle);
        EventBus.Unsubscribe<BoostUIEvent>(UpdateBoostGage);
        EventBus.Unsubscribe<EAttakcType>(ChangeAttackType);
    }

    private void Start()
    {
        spaceToggle.color = spaceOffColor;
    }


    public void UpdateBoostGage(BoostUIEvent e)
    {
        for (int i = 0; i < boostGages.Length; i++)
        {
            boostGages[i].fillAmount = 0f;
        }

        for (int i = 0; i < e.boostCount; i++)
        {
            boostGages[i].fillAmount = 1f;
        }

        if(e.boostCount < boostGages.Length)
            boostGages[e.boostCount].fillAmount = e.timer / e.maxtimer;
    }

    public void ChangeAttackType(EAttakcType e)
    {
        attackTypeImage.color = e == EAttakcType.Blow ? blowColor : slashOffColor;
    }


    Coroutine SpaceCoroitine;
    public void SpaceToggle(SpaceToggleEvent e)
    {
        if(isToggle)
        {
            //StopCoroutine(SpaceCoroitine);
            spaceToggle.color = spaceOffColor;
            spaceToggle.gameObject.SetActive(true);
            isToggle = false;
            return;
        }
        spaceToggle.color = spaceOnColor;
        isToggle = true;
        //SpaceCoroitine = StartCoroutine(SpaceToggleCoroitine());
    }
    IEnumerator SpaceToggleCoroitine()
    {
        while (true)
        {
            spaceToggle.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.15f);
            spaceToggle.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.15f);
        }
    }
}
