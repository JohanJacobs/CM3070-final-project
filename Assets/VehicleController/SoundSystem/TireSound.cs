using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using vc;
using vc.VehicleComponent;

public class TireSound : MonoBehaviour
{
    [Header("Sound files")]
    [SerializeField] AudioClip skidSound;
    [SerializeField] AudioClip grassSound;
    [SerializeField] AudioClip gravelSound;

    [SerializeField] AudioSource FrontLeftSource;
    [SerializeField] AudioSource FrontRightSource;
    [SerializeField] AudioSource RearLeftSource;
    [SerializeField] AudioSource RearRightSource;

    [Space,SerializeField] VehicleController vehicleController;
    Dictionary<WheelID, WheelAudioData> audioData = new();

    
    Dictionary<WheelID,AudioSource> sources = new();
    private void Awake()
    {
        sources.Add(WheelID.LeftFront, FrontLeftSource);
        sources.Add(WheelID.RightFront, FrontRightSource);
        sources.Add(WheelID.LeftRear, RearLeftSource);
        sources.Add(WheelID.RightRear, RearRightSource);        
    }
    private void Start()
    {
        vehicleController.GetVehicle.wheels.ForEach(w=> {
            audioData.Add(w.Key, new WheelAudioData
            {
                audioSource = sources[w.Key],
                wheelComponent = w.Value,
            });
        });

    }

    private void Update()
    {
        // calculate the slip velocities in the x and y direction 
        audioData.ForEach(w => { UpdateAudio(w.Value); });
    }

    void UpdateAudio(WheelAudioData  wad)
    {
        var v = new Vector2(wad.wheelComponent.longSlipVelocity, wad.wheelComponent.lateralSlipVelocity);
        var vLen = v.magnitude;

        var volume = 0f;
        var slipConstant = 5f;
        if (vLen > slipConstant)
            volume = MathHelper.MapAndClamp(vLen, slipConstant, 20f, 0f, 1.5f);

        if (volume > 0f && wad.wheelComponent.isGrounded)
        {
            wad.audioSource.clip = skidSound;
            wad.audioSource.volume = volume;
            wad.audioSource.mute = volume == 0;
            wad.audioSource.pitch = MathHelper.MapAndClamp(vLen, 0f, 20f, 0.7f, 1.4f);
            if (!wad.audioSource.isPlaying)
            {
                wad.audioSource.Play();
            }
            return;
        }

        wad.audioSource.Stop();
        wad.audioSource.mute = true;
    }
    public class WheelAudioData {
        public AudioSource audioSource;
        public WheelComponent wheelComponent;
    }

}
