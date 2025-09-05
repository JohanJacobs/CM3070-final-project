using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;

    private static float currentFX = -20f;
    private static float prevFX = -14f;


    private static AudioManager _instance;
    // Singleton Pattern for the audio manager that should be globally accessible
    public static AudioManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<AudioManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(AudioManager).Name);
                    _instance = singletonObject.AddComponent<AudioManager>();
                    DontDestroyOnLoad(_instance.gameObject);                    
                }
            }

            return _instance;
        }
    }


    private void Start()
    {
        Instance.InitializeAudioDefaults();
    }

    string masterVolumeParameter = "MasterVolume";
    public void SetSFXVolume(float volume)
    {
        prevFX = currentFX;
        currentFX = volume;
        mixer.SetFloat(masterVolumeParameter, currentFX);
    }

    public float GetSFXVolume()
    {
        float db; 
        mixer.GetFloat(masterVolumeParameter,out db);
        return db;
    }

    private void InitializeAudioDefaults()
    {
        SetSFXVolume(currentFX);
    }
}
