using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vc
{
    public interface IHUDController
    {
        public void ShowHUD();
        public void HideHUD();
        public void ToggleHUD();
    }
    public class VehcileHUDController : MonoBehaviour, IHUDController
    {
        [SerializeField] GameObject HUDPrefab;
        
        GameObject HUD;

        public void Awake()
        {
            
        }

        public void ShowHUD()
        {
    
        }

        public void HideHUD()
        {
    
        }
        public void ToggleHUD()
        {

        }
    }
}