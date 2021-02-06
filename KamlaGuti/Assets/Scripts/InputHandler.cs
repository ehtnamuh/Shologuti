using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private GameManager _gameManager;

    private void Start() => _gameManager = gameObject.GetComponent<GameManager>();

    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (Camera.main == null) return;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, new Vector3(0, 0, 1), 1000);
        if (hit)
        {
            _gameManager.ProcessHumanInput(hit.collider.gameObject);
        }
        else
            _gameManager.ClearHighlights();
    }
}
