using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen instance;
    public GameObject LoadingScreenCanvas;

    private List<Transform> childrenToKeep = new List<Transform>();


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }
    public void DeleteOrDeactivate()
    {
        childrenToKeep.Clear(); // Clear the list before populating it again

        // Save child objects of this object
        foreach (Transform child in transform)
        {
            childrenToKeep.Add(child);
        }

        // Get all objects in the scene
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            // Loop through the objects in the scene
            foreach (GameObject obj in allObjects)
            {
                if (obj != mainCamera.gameObject && !IsThisObject(obj) && !IsChildOfThisObject(obj))
                {
                    Destroy(obj);
                }
            }
        }
    }
    bool IsThisObject(GameObject obj)
    {
        return obj == gameObject;
    }
    bool IsChildOfThisObject(GameObject obj)
    {
        Transform parent = obj.transform.parent;

        // Check if the object's parent is the object where this script is attached
        while (parent != null)
        {
            if (parent.gameObject == gameObject)
            {
                return true;
            }
            parent = parent.parent;
        }
        return false;
    }


    void Awake()
    {
        // Ensure there is only one instance of the GameManager script in the scene.
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            // Destroy this instance if there is already another one in the scene.
            Destroy(gameObject);
            return;
        }

        // Keep this GameObject alive throughout the entire game.
        DontDestroyOnLoad(gameObject);
    }
}
