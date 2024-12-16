using UnityEngine;

public class Shape : MonoBehaviour
{
    public bool isCorrectShape;

    private void OnMouseDown()
    {
        FindObjectOfType<GameManager>().OnShapeClicked(isCorrectShape);
        Destroy(gameObject);
    }
}