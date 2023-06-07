using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField] private Material onMaterial;
    [SerializeField] private Material offMaterial;
    [SerializeField] private Renderer indicatorRenderer;
    [SerializeField] private Transform foodPrefab;
    [SerializeField] private Transform spawnPointTransform;
    [SerializeField] private Transform environmentTransform;

    private bool hasBeenPressed = false;

    public void Reset() 
    {
        hasBeenPressed = false;
        indicatorRenderer.material = onMaterial;
    }

    public void SetSpawnPoint(Vector3 nextPos)
    {
        spawnPointTransform.localPosition = nextPos;
    }

    public bool TryPressButton(out Transform foodTransform)
    {
        if (hasBeenPressed) {
            foodTransform = null;
            return false;
        }

        hasBeenPressed = true;
        indicatorRenderer.material = offMaterial;

        foodTransform = Instantiate(foodPrefab, spawnPointTransform.position, Quaternion.identity, environmentTransform);

        return true;
    }
}
