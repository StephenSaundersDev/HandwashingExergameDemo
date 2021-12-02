using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OculusSampleFramework;
using UnityEngine.Events;


public class CustomButtonListener : MonoBehaviour
{
    public UnityEvent proximityEvent;
    public UnityEvent contactEvent;
    public UnityEvent actionEvent;
    public UnityEvent defaultEvent;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<ButtonController>().InteractableStateChanged.AddListener(InitiateEvent);
    }

    void InitiateEvent(InteractableStateArgs state)
    {
        if (state.NewInteractableState == InteractableState.ProximityState)
            proximityEvent.Invoke();
        else if (state.NewInteractableState == InteractableState.ContactState)
            contactEvent.Invoke();
        else if (state.NewInteractableState == InteractableState.ActionState)
            actionEvent.Invoke(); //OnActionEvent(); 
        else
            defaultEvent.Invoke();
    }

    void OnActionEvent()
    {
        gameObject.transform.position = new Vector3(0,0,0);
    }
}
