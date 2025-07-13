using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace vc
{
    /*
        Script to active components when the vehicle tutorial closes
     */
    public class ActiveOnTutorialClose : MonoBehaviour
    {
        [SerializeField] MonoBehaviour[] gameObjects;
        [SerializeField] Button[] EnableInteractivity;


        private void Start()
        {
            SetInteractivity(false);
            SetObjectsActive(false);
        }

        private void OnEnable()
        {
            TutorialPlayButtonClicked.onPlayButtonClicked += TutorialPlayButtonClicked_onPlayButtonClicked;
        }

        private void OnDisable()
        {
            
        }

        void TutorialPlayButtonClicked_onPlayButtonClicked(object sender, EventArgs e)
        {
            SetInteractivity(true);
            SetObjectsActive(true);
        }


        void SetObjectsActive(bool state)
        {
            foreach (var go in gameObjects) {
                go.enabled = state;
            }
        }
        void SetInteractivity(bool state)
        {
            foreach(Button b in EnableInteractivity)
            {
                b.interactable = state;
            }
        }
    }
}