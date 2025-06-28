using Sirenix.Utilities;
using System;
using UnityEngine;

namespace vc
{
    public class EngineSound : MonoBehaviour
    {
        [SerializeField] FloatVariable engineRPM;
        [SerializeField] EngineSoundFile[] engineSounds;

        // normalization of volume as multiple files are playing at the same time
        float[] volumes;
        float totalvolume = default; 

        public void Awake()
        {
            volumes = new float[engineSounds.Length];
        }
        
        public void Update()
        {
            SetPitchLevels();
            CaclulateSoundLevels();
            NormalizeAudio();
        }
        private void SetPitchLevels()
        {
            engineSounds.ForEach(sound => { sound.SetPitch(engineRPM.Value);});
        }

        private void CaclulateSoundLevels()
        {
            totalvolume = 0f;
            for (int i = 0; i < engineSounds.Length; i++)
            {
                totalvolume += (volumes[i] = engineSounds[i].CalculateNewVolume(engineRPM.Value));
            }
        }

        float nomarlizeAudioLevel(float vol) => MathHelper.SafeDivide(vol,totalvolume);
        private void NormalizeAudio()
        {
            for (int i = 0; i < engineSounds.Length; i++)
            {
                engineSounds[i].SetVolume(nomarlizeAudioLevel(volumes[i]));
            }
        }
            
        
    }

    [Serializable]
    public class EngineSoundFile
    {        
        public AudioSource audio;
        public float enableRPM;
        public float maxVolumeRPM;
        public float disableRPM;
        public float pitchPerfectRPM;

        public void SetVolume(float volume)
        {
            audio.mute = (audio.volume = volume) == 0; // must the sound if the volume is to low 
        }
        
        public void SetPitch(float rpm) => audio.pitch = rpm / pitchPerfectRPM;
        bool rpmOutOfRange(float rpm) => (rpm < enableRPM) || (rpm > disableRPM);
        float mapVolumeIncrease(float rpm) => Mathf.InverseLerp(enableRPM, maxVolumeRPM, rpm);
        float mapVolumeDecrease(float rpm) => Mathf.InverseLerp(disableRPM, maxVolumeRPM, rpm);
        public float CalculateNewVolume(float rpm)
        {
            if (rpmOutOfRange(rpm)) return 0f;

            return (rpm < maxVolumeRPM)? mapVolumeIncrease (rpm): mapVolumeDecrease(rpm);                
        }

    }
}