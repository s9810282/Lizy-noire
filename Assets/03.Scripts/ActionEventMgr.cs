using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;
public class ActionEventMgr : MonoBehaviour
{
    [SerializeField] private InputActionAsset testActionAsset;

    private InputAction NavigateAction;
    private InputAction AAction;
    private InputAction BAction;
    private InputAction XAction;
    private InputAction YAction;

    private InputAction LSAction;
    private InputAction RSAction;

    private InputAction StartAction;
    private InputAction SelectAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NavigateAction = testActionAsset.FindActionMap("UI").FindAction("Navigate");
        NavigateAction.performed += NavigateAction_performed; ;

        AAction = testActionAsset.FindActionMap("UI").FindAction("A");
        AAction.performed += AAction_performed;
        BAction = testActionAsset.FindActionMap("UI").FindAction("B");
        BAction.performed += BAction_performed;
        XAction = testActionAsset.FindActionMap("UI").FindAction("X");
        XAction.performed += XAction_performed;
        YAction = testActionAsset.FindActionMap("UI").FindAction("Y");
        YAction.performed += YAction_performed;

        LSAction = testActionAsset.FindActionMap("UI").FindAction("LS");
        LSAction.performed += LSAction_performed;
        RSAction = testActionAsset.FindActionMap("UI").FindAction("RS");
        RSAction.performed += RSAction_performed;


        StartAction = testActionAsset.FindActionMap("UI").FindAction("Start");
        StartAction.performed += StartAction_performed;
        SelectAction = testActionAsset.FindActionMap("UI").FindAction("Select");
        SelectAction.performed += SelectAction_performed;

        testActionAsset.Enable();
    }

    private void AAction_performed(InputAction.CallbackContext obj)
    {
        Debug.Log("A_performed! ");
    }
    private void BAction_performed(InputAction.CallbackContext obj)
    {
        Debug.Log("B_performed! ");
    }
    private void XAction_performed(InputAction.CallbackContext obj)
    {
        Debug.Log("X_performed! ");
    }
    private void YAction_performed(InputAction.CallbackContext obj)
    {
        Debug.Log("Y_performed! ");
    }

    private void LSAction_performed(InputAction.CallbackContext obj)
    {
        Debug.Log("LS_performed! ");
    }
    private void RSAction_performed(InputAction.CallbackContext obj)
    {
        Debug.Log("RS_performed! ");
    }
    private void StartAction_performed(InputAction.CallbackContext obj)
    {
        Debug.Log("Start_performed! ");
    }
    private void SelectAction_performed(InputAction.CallbackContext obj)
    {
        Debug.Log("Select_performed! ");
    }


    private void NavigateAction_performed(InputAction.CallbackContext obj)
    {
        var vec = obj.ReadValue<Vector2>();
        Debug.Log("X : " + vec.x + " Y :" + vec.y);
    }
}