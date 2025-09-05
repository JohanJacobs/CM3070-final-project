using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{

    [SerializeField] private Slider SFXSlider;

    void Start()
    {
        SetupSlider();
    }


    void SetupSlider()
    {
        float db = AudioManager.Instance.GetSFXVolume();
        float sliderValue = Mathf.Pow(10f , (db / 20f));
        SFXSlider.value = sliderValue;
    }

    public void OnSliderChanged()
    {
        float sliderValue = Mathf.Clamp(SFXSlider.value,0f,1f);

        float db = -100f; // completely quiet
        if (sliderValue > 0f) 
            db = Mathf.Log10(sliderValue) * 20f;

        AudioManager.Instance?.SetSFXVolume(db);
    }
}
