using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vc
{
    /*
        Script to active components when the vehicle tutorial closes
     */
    public class ActiveOnTutorialClose : MonoBehaviour,IToggle
    {
        [SerializeField] MonoBehaviour[] gameObjects;

        private void Start()
        {
            SetObjectsState(false);
        }

        public void Toggle()
        {
            SetObjectsState(true);
        }

        void SetObjectsState(bool state)
        {
            foreach (var go in gameObjects)
            {
                go.enabled = state;
            }
        }

    }
}