using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetachChildren : MonoBehaviour
{
    /* Detaches ALL children that is not a spriterenderer type 
     * from this object using recursion.
     * Updates all destinations to their current position as well.
     */
    private int spriteLayer;

    private void Awake()
    {
        spriteLayer = LayerMask.NameToLayer("Sprite");
    }

    public void detachAllChildren()
    {
        List<Transform> children = new List<Transform>();
        getChildren(transform, children);
        updateDestAndParent(children);
    }

    private void getChildren(Transform parent, List<Transform> children)
    {
        foreach (Transform child in parent)
        {
            children.Add(child);
            getChildren(child, children);
        }
    }

    private void updateDestAndParent(List<Transform> children)
    {
        foreach (Transform child in children)
        {
            if (child.gameObject.layer != spriteLayer)
            {
                child.SetParent(null);
                child.GetComponent<PlayerManager>().updateDestinationToCurrPosition();
            }
        }
    }
}
