using UnityEngine;

namespace Singletons
{
    internal class SceneScalingManager : Singleton<SceneScalingManager>
    {
        private Vector2 screenSize;
        private Vector2 scaleMultiplier;
        private Vector2 minPosition;
        private Vector2 maxPosition;

        public static void SetCamera(Camera cam)
        {
            Vector2 origin = cam.gameObject.transform.position;

            // https://youtu.be/ailbszpt_AI?si=aBLqcL0_CV5yxHin
            Vector2 screenSize = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

            Vector2 defaultScale = new(48.02f, 30.00f); // aka 16:10 aspect
            Instance.scaleMultiplier = screenSize / defaultScale;

            Instance.minPosition = origin - screenSize;
            Instance.maxPosition = origin + screenSize;

            Instance.screenSize = screenSize;
        }

        public static Vector2 GetMin(Vector2 offset) => Instance.minPosition + offset;
        public static Vector2 GetMax(Vector2 offset) => Instance.maxPosition - offset;
        public static Vector2 Multiplier => Instance.scaleMultiplier;

        #region Singleton

        /// <inheritdoc/>
        protected override void OnAwake() => SetCamera(Camera.main);

        #endregion
    }
}
