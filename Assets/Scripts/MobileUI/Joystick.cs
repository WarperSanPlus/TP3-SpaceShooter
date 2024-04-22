using UnityEngine;
using UnityEngine.UI;
using Movements;
using UnityEngine.EventSystems;
namespace MobileUI
{
    public class Joystick : MonoBehaviour
    {
        //https://pressstart.vip/tutorials/2018/06/22/39/mobile-joystick-in-unity.html
        [SerializeField] Image outerCircle;
        [SerializeField] Image innerCircle;
        [HideInInspector] 
        public PlayerMovement player;
        const int RANGE_SNEAK = 300;

        private void Start()
        {
            outerCircle.enabled = false;
            innerCircle.enabled = false;
        }
        private void Update()
        {
            if (Input.touchCount == 0)
                return;

            var touch = Input.GetTouch(0);

            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return;

            if (touch.phase == TouchPhase.Began)
            {
                // Fait apparaitre le joystick a la position appuyée
                outerCircle.transform.position = Input.mousePosition;
                innerCircle.transform.position = Input.mousePosition;
                innerCircle.enabled = true;
                outerCircle.enabled = true;
            }

            if (touch.phase != TouchPhase.Ended)
            {
                // Bouge le inner circle dans la direction actuelle
                float maxBound = outerCircle.rectTransform.sizeDelta.x * 0.5f;
                Vector2 direction = touch.position - (Vector2) outerCircle.transform.position;
                innerCircle.transform.position = outerCircle.transform.position + Vector3.ClampMagnitude(direction, maxBound);

                // Actualise le joueur
                player.SetDirection(direction.normalized);
                player.SetSneak(direction.sqrMagnitude <= RANGE_SNEAK);
            } 
            else 
            {
                // Desactive le joystick
                innerCircle.enabled = false;
                outerCircle.enabled = false;
                player.SetDirection(Vector2.zero);
            }
        }
    }
}