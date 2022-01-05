using UnityEngine;

[CreateAssetMenu(fileName = "Drop", menuName = "ScriptableObjects/Drop", order = 1)]
public class Drop : ScriptableObject
{
    public GameObject item;
    public float dropChance;
    public int amount;
}
