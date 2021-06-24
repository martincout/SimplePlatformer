
using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// Still need to be implemented
/// </summary>
public class UIMenuBehaviour : MonoBehaviour
{
    public EventSystem _eventSystem;
    //Pause
    public void SetupBehaviour()
    {
        UpdateUIMenuState(false);
    }

    public void UpdateUIMenuState(bool newState)
    {
        switch (newState)
        {
            case true:
                Behaviour();
                //pause
                break;

            case false:
                break;
        }
    }

    public void Behaviour() {
        //NOTHING
    }
    
}