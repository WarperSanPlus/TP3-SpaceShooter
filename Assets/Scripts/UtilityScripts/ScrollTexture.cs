using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ScrollTexture : MonoBehaviour
{
    [SerializeField]
    private float scrollSpeed = 0.1f; // Vitesse de d�filement de la texture

    [SerializeField] private Vector2 direction;
    private Renderer rend;

    private void Start() => this.rend = this.GetComponent<Renderer>();

    private void FixedUpdate() => this.AdvanceTexture();

    private void AdvanceTexture()
    {
        // Appliquer le d�placement de texture � l'objet
        Vector2 d�placement = this.scrollSpeed * Time.time * this.direction.normalized;

        this.rend.material.SetTextureOffset("_MainTex", d�placement);
    }
}