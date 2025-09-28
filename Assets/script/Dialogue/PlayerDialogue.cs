using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using NUnit.Framework.Constraints;

public class PlayerDialogue : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Rigidbody rb;
    private GameEntity entity;
    float triggerRadius = 10.0f;
    private DialogueUIController dialogueUI;
    public Sprite potrait;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        rb = gameObject.GetComponent<Rigidbody>();

        entity = gameObject.GetComponent<GameEntity>();
        dialogueUI = GameObject.Find("Dialogue").GetComponent<DialogueUIController>();


    }
    private HashSet<GameObject> alreadyEntered = new();

    GameObject[] proximityEntity;
    // Update is called once per frame
    void Update()
    {
        float radius = 2f;
        float radiusSqr = radius * radius;
        Vector3 origin = transform.position;
        GameObject[] potentialTargets = GameObject.FindGameObjectsWithTag("Entity");
        HashSet<GameObject> newInside = new();
        // The collider shits itself so I have to make a makeshift proximity detector...
        foreach (GameObject target in potentialTargets)
        {
            float distSqr = (target.transform.position - origin).sqrMagnitude;
            if (distSqr > radiusSqr || alreadyEntered.Contains(target))
            {
                continue;
            }

            alreadyEntered.Add(target);

            alreadyEntered.Add(target);
            OnProximityEnter(target);

        }

        foreach (GameObject target in alreadyEntered)
        {
            float exitRadius = (radius + 0.5f) * (radius + 0.5f);
            float distSqr = (target.transform.position - origin).sqrMagnitude;
            if (distSqr <= radiusSqr)
            {
                newInside.Add(target);
            }
            else
            {
                OnProximityExit(target);
                // Debug.Log("Exiting set: " + target.name);
            }
        }
        alreadyEntered = newInside;
        //Debug.Log("meow");
    }

    void OnProximityEnter(GameObject gameObject)
    {
        DialogueGraph graph = gameObject.GetComponent<DialogueGraph>();
        if (graph != null)
        {
            dialogueUI.Trigger(graph, gameObject.name, potrait, graph.potrait);
        }
    }
    void OnProximityExit(GameObject gameObject)
    {
        DialogueGraph graph = gameObject.GetComponent<DialogueGraph>();
        if (graph != null)
        {
            Debug.Log("mrrp mrrp meowwww");
            dialogueUI.End();
        }
    }
}
