using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIToggleConfiguration : MonoBehaviour
{
    [SerializeField] GameObject carConfiguration;
    [SerializeField] Button ConfigButton;

    private void OnEnable()
    {
        ConfigButton.onClick.AddListener(ToggleConfiguration);
    }

    private void OnDisable()
    {
        ConfigButton.onClick.RemoveListener(ToggleConfiguration);
    }

    private void ToggleConfiguration()
    {
        carConfiguration.SetActive(!carConfiguration.activeSelf);
    }
}
