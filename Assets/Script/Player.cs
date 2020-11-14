using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance = null;
    Target target;
    GameManager gm;
    public float playerSize = 10f;
    public Vector3 direction;
    public enum State
    {
        Wait,
        Aim,
        Shoot,
        JumpToTarget
    }
    public State state = State.JumpToTarget; // Current state of player
    public delegate void positionMessage(Vector3 rayPosition);
    public delegate void voidMessage();
    public event positionMessage OnAimStarted; // Event starts when player touch the screen
    public event positionMessage OnShootingStarted; // Event starts when player stops touching the screen
    public event positionMessage OnChangeAimingPoint; // Event starts when player moves finger after touch
    public delegate void changingSize(float currentSize);
    public event changingSize OnChangingSize; // Event starts when player moves finger after touch
    public event voidMessage OnGameOver; // Event starts when player has no more size
    public float touchTime = 0f; //Counts single touch time
    //public bool IsJumped = false;

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
    private void Start()
    {
        target = Target.instance;
        target.OnPlayerCollision += WinState;
        gm = GameManager.instance;
    }

    private void Update()
    {
        if (Input.touchCount > 0 && state != State.Shoot && state != State.JumpToTarget && gm.timeToRestart<=0) // Screen touched
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null)
                {
                    Vector3 newPoint = hit.point;
                    newPoint.y = transform.position.y;
                    direction = newPoint - transform.position;
                }
            } else
            {
                direction = Vector3.zero;
            }
            //direction.y = transform.position.y;
            switch (Input.GetTouch(0).phase)
            {
                case TouchPhase.Began:
                    touchTime = 0f; //Touching just began
                    if (GameObject.Find("Shot(Clone)") == null) //If fields has not shots
                    {
                        state = State.Aim; //Change player state to aim
                        OnAimStarted(direction.normalized); //Call aim event
                    }
                    break;
                case TouchPhase.Stationary:
                    touchTime += Time.deltaTime;
                    break;
                case TouchPhase.Moved:
                    touchTime += Time.deltaTime;
                    OnChangeAimingPoint(direction.normalized); //Call aiming point change event
                    break;
                case TouchPhase.Ended:
                    if(state != State.Wait)
                    {
                        state = State.Shoot;
                        OnShootingStarted(direction.normalized); //Shooting started
                    }
                    break;
                default:
                    Debug.Log("Touch canceled by device!");
                    break;
            }
        }
        if (state == State.Aim) ChangePlayerSize(Time.deltaTime); //Holding finger means player lost his size
        if(state == State.Wait) if(GetComponent<Rigidbody>().velocity != Vector3.zero)
                GetComponent<Rigidbody>().velocity = Vector3.zero; // Restart player velocity
    }
    private void OnCollisionEnter(Collision col)
    {
        if (state == State.JumpToTarget && col.gameObject.CompareTag("Plane"))
        {
            Vector3 newTarget = Target.instance.transform.position;
            newTarget.y = 50f;
            transform.LookAt(newTarget);
            GetComponent<Rigidbody>().AddForce(transform.forward * 100);
        } else
        {
            if (GetComponent<Rigidbody>().velocity != Vector3.zero) 
                GetComponent<Rigidbody>().velocity = Vector3.zero; // Restart player velocity
        }
    }
    void WinState()
    {
        state = State.Wait;
    }
    public void JumpToTarget()
    {
        switch (state)
        {
            case State.Aim:
                state = State.Shoot;
                OnShootingStarted(direction.normalized); //Shooting started
                break;
            default:
                break;
        }
        Vector3 newTarget = Target.instance.transform.position;
        newTarget.y = 50f;
        transform.LookAt(newTarget);
        GetComponent<Rigidbody>().mass = 0.5f;
        GetComponent<Rigidbody>().AddForce(transform.forward * 40);
        state = State.JumpToTarget;
    }
    private void ChangePlayerSize(float value)
    {
        if (playerSize < 0)
        {
            OnGameOver();
            state = State.Wait;
        }
        playerSize -= value;
        transform.localScale = new Vector3(playerSize / 10, playerSize / 10, playerSize / 10);
        OnChangingSize(playerSize);
    }
}
