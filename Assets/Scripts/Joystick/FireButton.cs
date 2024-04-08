﻿using Controllers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Joystick
{
    internal class FireButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [HideInInspector]
        public PlayerController playerController;

        public void OnPointerDown(PointerEventData eventData)
        {
            playerController.SetRequestFire(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            playerController.SetRequestFire(false);
        }
    }
}
