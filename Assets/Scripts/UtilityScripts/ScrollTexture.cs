using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ScrollTexture : MonoBehaviour
{
    [SerializeField]
    private float scrollSpeed = 0.1f; // Vitesse de d�filement de la texture

    [SerializeField] private Vector2 direction;
    private Material material;

    private void Start()
    {
        if (!this.TryGetComponent(out Renderer rend))
        {
            this.enabled = false;
            return;
        }

        this.material = rend.material;
    }

    private void FixedUpdate() => this.AdvanceTexture();

    private void AdvanceTexture()
    {
        // Appliquer le deplacement de texture a l'objet
        Vector2 deplacement = this.scrollSpeed * Time.time * this.direction.normalized;

        this.material.SetTextureOffset("_MainTex", deplacement);
    }
}