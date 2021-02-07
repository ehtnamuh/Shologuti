using UnityEngine;

public class InputHandler : MonoBehaviour
{
    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (Camera.main == null) return;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, new Vector3(0, 0, 1), 1000);
        if (hit)
        {
            Debug.Log("Kraut");
            GameManager.instance.ProcessHumanInput(hit.collider.gameObject);
        }
        else
            GameManager.instance.ClearHighlights();
    }
}
