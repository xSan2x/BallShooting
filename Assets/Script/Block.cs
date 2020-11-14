using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public enum State
    {
        Wait,
        Infected
    }
    public State state = State.Wait;
    private float size = 1f;
    // Update is called once per frame
    void Update()
    {
        if (state == State.Infected)
        {
            size += Time.deltaTime*3;
            if (size > 1.5f)
            {
                size += Time.deltaTime * 5;
            }
            transform.localScale = new Vector3(size * 0.4f, size * 0.8f, size * 0.4f);
            if (size > 2.5f)
            {
                Destroy(gameObject);
            }
        }
    }
    public void ChangeToInfected()
    {
        GetComponent<MeshRenderer>().material.color = new Color32(253, 91, 0, 255); //Change color of infected block
        state = State.Infected;
        GetComponent<Collider>().isTrigger = true;
        Way.instance.RemoveBlock(gameObject);
    }
}
