
public abstract class UIMenuBehaviour
{
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

    public abstract void Behaviour();
    
}