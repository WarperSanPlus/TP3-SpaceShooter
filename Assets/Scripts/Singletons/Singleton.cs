using UnityEngine;

namespace Singletons
{
    /// <summary>
    /// Class that allows any class to be accessed from anywhere
    /// </summary>
    /// <typeparam name="T">Type of the class</typeparam>
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        /// <summary>
        /// Unique instance of <typeparamref name="T"/>
        /// </summary>
        public static T Instance { get; private set; }

        #region MonoBehaviour

        /// <inheritdoc/>
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning($"Another instance of {this.GetType().Name} has been found.");
                Destroy(this.gameObject);
                return;
            }

            Instance = this.gameObject.GetComponent<T>();

            if (!this.DestroyOnLoad)
                DontDestroyOnLoad(this.gameObject);

            this.OnAwake();
        }

        #endregion

        #region Virtual

        /// <summary>
        /// Defines if the singleton should be destroy when loading a new scene
        /// </summary>
        protected virtual bool DestroyOnLoad { get; } = false;

        /// <summary>
        /// Called when <see cref="Awake"/> is called
        /// </summary>
        protected virtual void OnAwake()
        { }

        #endregion Virtual
    }
}