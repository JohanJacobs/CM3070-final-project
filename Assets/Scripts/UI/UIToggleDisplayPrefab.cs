using UnityEngine;
using UnityEngine.Events;

public class UIToggleDisplayPrefab : MonoBehaviour, IToggle
{
    [SerializeField] GameObject prefab;
    GameObject ui;
    [SerializeField] bool isVisibleOnStart;
    /*
     * Event to trigger when this objects visibility state change
     *  TRUE: object is visible and enabled
     *  FALSE: Object is not visible and disabled
     */

    public UnityEvent<bool> OnStateChange;
    
    private void Awake()
    {
        ui = CreateUI(prefab);
        ui.transform.SetAsFirstSibling();
        SetVisible(ui, isVisibleOnStart);
    }

    GameObject CreateUI(GameObject uiPrefab)
    {
        ui = Instantiate(uiPrefab, transform.parent);        
        return ui;
    }

    GameObject GetOrCreateUI()
    {
        if (ui == null)
            ui = CreateUI(prefab);

        return ui;
    }
    
    private void SetVisible(GameObject go, bool state) => go.SetActive(state);


    #region IToggle
    public void Toggle()
    {
        var ui = GetOrCreateUI();
        SetVisible(ui, !ui.activeSelf);
    }
    #endregion IToggle

}
