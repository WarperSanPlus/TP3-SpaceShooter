using Controllers;
using Movements;
using UnityEngine;
using UnityEngine.InputSystem;
namespace MobileUI
{
    public class MobileUI : MonoBehaviour
    {
        [SerializeField] PlayerInput playerInput;
        [SerializeField] PlayerMovement player;
        [SerializeField] PlayerController playerController;
        [SerializeField] Joystick joystick;
        [SerializeField] FireButton fireButton;

        #if UNITY_EDITOR

        [SerializeField] bool DisableIfNotMobile = true;

        #endif

        void Start()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
            {
                playerInput.enabled = false;
                fireButton.playerController = playerController;
                joystick.player = player;

            }
            #if UNITY_EDITOR
            else if (this.DisableIfNotMobile)
            {
                    joystick.gameObject.SetActive(false);
                    fireButton.gameObject.SetActive(false);
            }
            #endif
        }
    }
}
