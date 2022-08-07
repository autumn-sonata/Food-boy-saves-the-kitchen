using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetachChildren : MonoBehaviour
{
    /* Detaches ALL children that is not a SpriteRenderer type 
     * from this object using recursion.
     * Updates all destinations to their current position as well.
     */

    private int spriteLayer;

    #region Unity specific functions
    private void Awake()
    {
        spriteLayer = LayerMask.NameToLayer("Sprite");
    }

    #endregion

    #region parent and child interactions
    public void detachAllChildren()
    {
        /* Detaches all the children from this game object.
         * 
         * Parameters
         * ----------
         * 
         * 
         * Return
         * ------
         * 
         */

        List<Transform> children = new List<Transform>();
        getChildren(transform, children);
        updateDestAndParent(children);
    }

    private void getChildren(Transform parent, List<Transform> children)
    {
        /* Helper recursive function for getting all the children and 
         * flattening all entries into a list.
         * 
         * Parameters
         * ----------
         * 1) parent: root gameObject of all children.
         * 2) children: array to which all children will be added to 
         *   for flattening.
         * 
         * Return
         * ------
         * 
         */

        foreach (Transform child in parent)
        {
            children.Add(child);
            getChildren(child, children);
        }
    }

    private void updateDestAndParent(List<Transform> children)
    {
        /* Update the children's destination to their current position
         * if the root is no longer pushing them.
         * 
         * Parameters
         * ----------
         * 1) children: list of all the children of the root gameObject.
         * 
         * Return
         * ------
         * 
         */

        foreach (Transform child in children)
        {
            if (child.gameObject.layer != spriteLayer)
            {
                child.SetParent(null);
                child.GetComponent<PlayerManager>().updateDestinationToCurrPosition();
            }
        }
    }

    #endregion
}
