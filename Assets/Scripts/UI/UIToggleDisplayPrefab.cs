using UnityEngine;
using UnityEngine.Events;

public class UIToggleDisplayPrefab : MonoBehaviour, IToggle
{
    [SerializeField] GameObject prefab;
    GameObject ui;
    [SerializeField] bool isVisibleOnStart;
    [SerializeField] bool currentState;
    Canvas canvas;

    /*
     * Event to trigger when this objects visibility state change
     *  TRUE: object is visible and enabled
     *  FALSE: Object is not visible and disabled
     */

    public UnityEvent<bool> OnStateChange;

    private void Awake()
    {
        ui = CreateUI(prefab);

        currentState = isVisibleOnStart;
        SetVisible(ui, currentState);
    }

    GameObject CreateUI(GameObject uiPrefab)
    {
        ui = Instantiate(uiPrefab, transform);        
        return ui;
    }

    GameObject GetOrCreateUI()
    {
        if (ui == null)
            ui = CreateUI(prefab);

        return ui;
    }
    bool SwitchStateAndReturnUpdated() 
    {
        currentState = !currentState;        
        return currentState;
    }

    private void SetVisible(GameObject go, bool state) => go.SetActive(state);


    #region IToggle
    public void Toggle()
    {
        SetVisible(GetOrCreateUI(), SwitchStateAndReturnUpdated());
    }
    #endregion IToggle

}
