using Sirenix.OdinInspector;
using System.ComponentModel;
using UnityEngine;

namespace vc
{ 
    public class InputSmoother
    {
        public float Target => target;
        public float Value => currentValue;
        public string Label => label;

        
        float strength;        
        float target;        
        float currentValue;        
        string label;
        
        public InputSmoother(float strength, string label)
        {
            GameObject go = new GameObject($"InputSmoother - {label}");
            var mh = go.AddComponent<MonoHolder>();
            mh.SetSmoother(this);

            this.strength = strength;
            this.target = 0f;
            this.currentValue = 0f;
        }

        public void SetTarget(float target)
        {
            this.target = target;
        }
        public void UpdateSmoothing(float dt)
        {
            if (currentValue == target)
                return;

            var s = (target == 0f) ? strength * 2f : strength;
            currentValue = Mathf.MoveTowards(currentValue, target, dt * s);
        }


        // Game Object to hold the input smoothing component in
        public class MonoHolder : MonoBehaviour
        {
            InputSmoother smoother;
            [SerializeField, Sirenix.OdinInspector.ReadOnly]
            float strength;
            [SerializeField, Sirenix.OdinInspector.ReadOnly]
            float value;
            [SerializeField, Sirenix.OdinInspector.ReadOnly]
            float target;

            public void Update()
            {
                smoother.UpdateSmoothing(Time.deltaTime);
            }

            public void LateUpdate()
            {
                strength = smoother.strength;
                value = smoother.Value;
                target = smoother.Target;
            }

            public void SetSmoother(InputSmoother smoother)
            {
                this.smoother = smoother;
            }
        }

    }
}
