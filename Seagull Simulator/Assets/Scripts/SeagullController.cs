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
    private float movementX;
    private float movementY;
    private float movementZ;
    private Vector3 direction;

    public bool getInSky()
    {
        return inSky;
    }

    void Start() {
        animator = gameObject.GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody>();
        inSky = false;
        health = 100f;
        wantedLevel = 0f;
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
        health -= Time.deltaTime * (float) 0.5;
        wantedLevel -= Time.deltaTime * (float) 0.5;

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
        }
        else {
            StartCoroutine(takeFlight());
        }
        inSky = !inSky;
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
}
