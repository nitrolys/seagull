using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class SeagullController : MonoBehaviour
{
    public float speed;
    public Rigidbody rb;

    private Animator animator;
    private bool inSky;
    private float health;
    private float wantedLevel;

    private float flightCDMax;
    private float flightCD;

    private float movementX;
    private float movementY;
    private Vector3 direction;

    public bool getInSky()
    {
        return inSky;
    }

    public float getHealth()
    {
        return health;
    }

    public float getFlightCD()
    {
        return flightCD;
    }

    public int getWantedLevel()
    {
        return Mathf.FloorToInt(wantedLevel);
    }

    void Start() {
        animator = gameObject.GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody>();
        inSky = false;
        health = 100f;
        wantedLevel = 0f;
        flightCDMax = 5;
        flightCD = flightCDMax;
    }

    // Update is called once per frame
    void Update() {
        changeAnimation();
        changePosition();
        changeStat();
    }

    void changeAnimation() {
        if (inSky) {
            animator.Play("Fly");
        } else if (movementX != 0 || movementY != 0) {
            animator.Play("Walk");
        } else
        {
            animator.Play("Idle_A");
        }
    }

    void changePosition() {
        transform.position += speed * new Vector3(movementX, 0, movementY) * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    void changeStat() {
        IncrementHealth(-Time.deltaTime * 2);

        if (health <= 0)
        {
            Debug.Log("Game Over: Seagull Died!");
            // Implement Game Over logic
        }
    }

    void OnMove(InputValue movementValue) {
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementY = movementVector.y;
        if (movementX != 0 || movementY != 0)
        {
            direction.x = movementX;
            direction.z = movementY;
        }
    }

    void OnFly(InputValue movementValue) {
        if (inSky) {
            StartCoroutine(descend());
            StartCoroutine(flightCDTimer());
            inSky = false;
        }
        else if (flightCD >= flightCDMax) {
            StartCoroutine(takeFlight());
            flightCD = 0;
            inSky = true;
        }
    }

    private IEnumerator flightCDTimer()
    {
        while (flightCD < flightCDMax)
        {
            flightCD += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator takeFlight()
    {
        rb.useGravity = false;
        while (transform.position.y < 2)
        {
            transform.position += new Vector3(0, 10, 0) * Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator descend()
    {
        RaycastHit hit;
        if (Physics.Raycast(origin : transform.position, direction : Vector3.down, hitInfo: out hit))
        {
            float targetY = hit.point.y + 0.2f;
            while (transform.position.y > targetY)
            {
                transform.position -= new Vector3(0, 10, 0) * Time.deltaTime;
                yield return null;
            }

        }
        rb.useGravity = true;
    }

    public void IncrementHealth(float increment)
    {
        health = Mathf.Clamp(health + increment, 0, 100);
    }

    public void IncrementWanted(float increment)
    {
        wantedLevel = Mathf.Clamp(wantedLevel + increment, 0, 5);
    }

    public void TakeHit(float damage, Vector3 attackerPos)
    {
        Vector3 hitForce = 8 * Vector3.Normalize(transform.position - attackerPos);
        rb.AddForce(hitForce, ForceMode.Impulse);
        IncrementHealth(-damage);
    }
}
