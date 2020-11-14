using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Way : MonoBehaviour
{
    Player player = Player.instance;
    public static Way instance = null;
    public List<GameObject> blocks = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance == this)
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        player.OnChangingSize += ChangeSize; //Subscribe to player event
    }

    private void OnDestroy()
    {
        player.OnChangingSize -= ChangeSize; //Unsubscribe from player event
    }

    
    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Block") && col.gameObject.GetComponent<Block>().state == Block.State.Wait)
        {
            RemoveBlock(col.gameObject);
        }
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Block") && col.gameObject.GetComponent<Block>().state == Block.State.Wait)
        {
            blocks.Add(col.gameObject);
        }
    }
    public void RemoveBlock(GameObject gameObjectBlock)
    {
        blocks.Remove(gameObjectBlock);
        if (blocks.Count == 0)
        {
            player.JumpToTarget();
        }
    }
    public void Init(Vector3 first, Vector3 second) //Create the Way between player and target
    {
        float distance = Vector3.Distance(first, second);
        Vector3 vectorBetween = second - first;
        transform.localScale = new Vector3(player.playerSize / 10, transform.localScale.y, distance);
        Vector3 newPosition = first + (vectorBetween / 2f);
        transform.position = newPosition;
        transform.LookAt(second);
        newPosition.y = 0.01f;
        transform.position = newPosition;
    }

    public void ChangeSize(float currentSize) //When player changes his size, way change it too
    {
        transform.localScale = new Vector3(currentSize / 10, transform.localScale.y, transform.localScale.z);
    }
}
