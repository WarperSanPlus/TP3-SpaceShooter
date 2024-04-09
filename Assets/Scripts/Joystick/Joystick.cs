using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.LowLevel;
using Movements;
using UnityEngine.InputSystem;

public class Joystick : MonoBehaviour
{
    //https://pressstart.vip/tutorials/2018/06/22/39/mobile-joystick-in-unity.html
    [SerializeField] Image outerCircle;
    [SerializeField] Image innerCircle;
    [HideInInspector] 
    public PlayerMovement player;


    private void Start()
    {
        outerCircle.enabled = false;
        innerCircle.enabled = false;
        
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            outerCircle.transform.position = Input.mousePosition;
            innerCircle.transform.position = Input.mousePosition;
            innerCircle.enabled = true;
            outerCircle.enabled = true;
        }
        else if (Input.GetMouseButton(0))
        {
            float maxBound = outerCircle.rectTransform.sizeDelta.x * 0.5f;
            Vector2 direction = Input.mousePosition - outerCircle.transform.position;
            innerCircle.transform.position = outerCircle.transform.position + Vector3.ClampMagnitude(direction, maxBound);
            player.SetDirection(direction.normalized);
            player.SetSneak(direction.sqrMagnitude <= 300);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            innerCircle.enabled = false;
            outerCircle.enabled = false;
            player.SetDirection(Vector2.zero);
        }
    }
}