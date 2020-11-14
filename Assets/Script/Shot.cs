using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour
{
    Player player;
    public enum State
    {
        Charging,
        Fly,
        Explodes
    }

    public State state = State.Charging;
    public float shotSize;
    public float explodeShotSize = 1000f;
    public float startPlayerSize;
    void Start()
    {
        player = Player.instance;
        startPlayerSize = player.playerSize;
        player.OnChangingSize += ChangeSize;
        player.OnChangeAimingPoint += ChangeDirection;
        player.OnShootingStarted += StartShoot;
    }
    private void OnDestroy()
    {
        player.OnChangingSize -= ChangeSize;
        player.OnChangeAimingPoint -= ChangeDirection;
        player.OnShootingStarted -= StartShoot;
    }


    void Update()
    {
        if (state == State.Explodes)
        {
            shotSize += Time.deltaTime * 50;
            transform.localScale = new Vector3(shotSize / 10, shotSize / 10, shotSize / 10);
            if (shotSize > explodeShotSize)
            {
                Destroy(gameObject);
            }
        }
        if(transform.position.x>5 || transform.position.x < -5 || transform.position.z > 5 || transform.position.z < -5)
        {
            if (player.state != Player.State.JumpToTarget)
            {
                player.state = Player.State.Wait; //Player can shoot again
            }
            Destroy(gameObject);
        }

    }
    private void OnCollisionEnter(Collision col)
    {
        if (state == State.Fly && col.gameObject.CompareTag("Block") && col.gameObject.GetComponent<Block>().state == Block.State.Wait )
        {
            state = State.Explodes;
            GetComponent<Collider>().isTrigger = true;
            explodeShotSize = shotSize * 12;
            GetComponent<Rigidbody>().velocity = Vector3.zero; //Stop the shot at collision position
            if(player.state != Player.State.JumpToTarget)
            {
                player.state = Player.State.Wait; //Player can shoot again
            }
            if (shotSize>1f)
                GetComponent<SphereCollider>().radius = 0.25f;
            else
                GetComponent<SphereCollider>().radius = 0.0f;
        }
        if (col.gameObject.CompareTag("Block"))
        {
            col.gameObject.GetComponent<Block>().ChangeToInfected();
        }
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Block"))
        {
            col.gameObject.GetComponent<Block>().ChangeToInfected();
        }
    }
    public void ChangeSize(float currentSize) //When player changes his size, way change it too
    {
        shotSize = Mathf.Abs(currentSize - startPlayerSize) + 0.75f;
        transform.localScale = new Vector3(shotSize / 10, shotSize / 10, shotSize / 10);
    }
    public void ChangeDirection(Vector3 direction)
    {
        transform.position = player.transform.position + direction;
    }
    public void StartShoot(Vector3 direction)
    {
        transform.position = player.transform.position + direction;
        state = State.Fly;
        GetComponent<Rigidbody>().AddForce(direction * 50f);
    }
}
