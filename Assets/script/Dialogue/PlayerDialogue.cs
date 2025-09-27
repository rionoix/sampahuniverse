using UnityEngine;

public class PlayerDialogue : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Rigidbody rb;
    private GameEntity entity;
    DialogueUIController dialogueUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        rb = gameObject.GetComponent<Rigidbody>();

        entity = gameObject.GetComponent<GameEntity>();
        dialogueUI = GameObject.Find("Dialogue").GetComponent<DialogueUIController>();

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("meow");
    }

    public (DialogueGraph graph, GameEntity otherEntity) GetDialogue(GameObject objCollision)
    {
        GameEntity other = objCollision.GetComponent<GameEntity>();
        if (other == null)
        {
            Debug.Log("Can't find entity");
            return (null, null);
        }
        DialogueGraph graph = objCollision.GetComponent<DialogueGraph>();
        return (graph, other);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {

        var (dialogueGraph, other) = GetDialogue(collision.gameObject);
        Debug.Log("Collided: " + other + dialogueGraph + collision.gameObject.name);
        if (dialogueGraph != null)
        {
            Debug.Log("Dialogue starting meow!");
            dialogueUI.Trigger(collision.gameObject.GetComponent<GameEntity>(), dialogueGraph, other.stats.entityName);
        }


    }
    void OnTriggerExit2D(Collider2D collision)
    {
        var (dialogueGraph, other) = GetDialogue(collision.gameObject);
        if (dialogueGraph != null)
        {
            dialogueUI.End();
        }
    }
}
