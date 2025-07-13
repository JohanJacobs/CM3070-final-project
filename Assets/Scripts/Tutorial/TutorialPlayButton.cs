using System;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPlayButtonClicked : MonoBehaviour
{
    public static event EventHandler onPlayButtonClicked;
    Button playButton;
    private void Awake()
    {
        playButton = GetComponent<Button>();
        playButton.onClick.AddListener(() => { onPlayButtonClicked?.Invoke(this, EventArgs.Empty); });
    }
}
