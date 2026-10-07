using UnityEngine;

public class UpdraftZone : MonoBehaviour
{
    public float updraftSpeed = 15f;

    private void OnTriggerStay(Collider other)
    {
        IUpdraftable player = other.GetComponent<IUpdraftable>();

        if (player != null)
        {
            player.ApplyUpdraft(updraftSpeed);
        }
    }
}