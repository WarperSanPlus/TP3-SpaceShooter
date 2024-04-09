using Assets.Scripts.Joystick;
using Controllers;
using Movements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MobileUI : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] PlayerMovement player;
    [SerializeField] PlayerController playerController;
    [SerializeField] Joystick joystick;
    [SerializeField] FireButton fireButton;

    void Start()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
        {
            playerInput.enabled = false;
            fireButton.playerController = playerController;
            joystick.player = player;

        }
        else
        {
#if !UNITY_EDITOR
            joystick.gameObject.SetActive(false);
            fireButton.gameObject.SetActive(false);
#endif
        }
    }
}
