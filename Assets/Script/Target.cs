using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public static Target instance = null;
    public delegate void voidMessage();
    public event voidMessage OnPlayerCollision;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        if (instance == this)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OnPlayerCollision();
        } 
    }
}
