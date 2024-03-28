using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ScrollTexture : MonoBehaviour
{
    [SerializeField]
    private float scrollSpeed = 0.1f; // Vitesse de défilement de la texture

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
        // Appliquer le déplacement de texture à l'objet
        Vector2 déplacement = this.scrollSpeed * Time.time * this.direction.normalized;

        this.material.SetTextureOffset("_MainTex", déplacement);
    }
}