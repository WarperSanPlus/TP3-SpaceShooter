using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    private Quaternion initialRotation;

    [SerializeField]
    private bool useOpposite = false;

    private void FixedUpdate()
    {
        // Get z-angle
        var zAngle = Quaternion.LookRotation(Vector3.forward, this.transform.position - this.target.position).eulerAngles.z;

        if (this.useOpposite)
            zAngle += 180;

        // Lerp
        this.transform.rotation = Quaternion.Euler(0, 0, zAngle);
    }
}
