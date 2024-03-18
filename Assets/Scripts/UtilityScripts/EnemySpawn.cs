using Interfaces;
using UnityEngine;

/// <summary>
/// Temporary script that set ups entities
/// </summary>
public class EnemySpawn : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        IResetable[] resetables = this.GetComponents<IResetable>();

        foreach (IResetable item in resetables)
            item.OnReset();

        this.enabled = false;
    }
}
