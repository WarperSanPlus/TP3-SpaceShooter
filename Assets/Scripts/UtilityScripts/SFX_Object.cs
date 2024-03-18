using Interfaces;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFX_Object : MonoBehaviour, IResetable
{
    public const string NAMESPACE = "SFX";

    private AudioSource source;
    private float timer = 0f;

    #region MonoBehaviour

    /// <inheritdoc/>
    private void FixedUpdate()
    {
        this.timer -= Time.fixedDeltaTime;

        if (this.timer > 0f)
            return;

        // Disable object
        this.gameObject.SetActive(false);
    }

    #endregion

    #region IResetable

    /// <inheritdoc/>
    public void OnReset()
    {
        this.gameObject.SetActive(true);

        if (this.source == null)
            this.source = this.GetComponent<AudioSource>();

        this.source.Play();
        this.timer = this.source.clip.length;
    }

    #endregion
}