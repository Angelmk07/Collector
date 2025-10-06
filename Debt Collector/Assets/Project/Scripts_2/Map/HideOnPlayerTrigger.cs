using UnityEngine;

public class HideOnPlayerTrigger : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    [SerializeField] private LayerMask Player;

    private int playerLayer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(" 22");
        if (((1 << other.gameObject.layer) & Player) != 0)
        {
            Debug.Log(" 444");
            targetObject.SetActive(false);
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & Player) != 0)
        {
            targetObject.SetActive(true);
        }
    }
}
