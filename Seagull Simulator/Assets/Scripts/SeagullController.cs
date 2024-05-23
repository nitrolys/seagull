using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SeagullController : MonoBehaviour
{
    public GameObject seagull;
    private Animator animator;
    private bool inSky;
    private float health;
    private float wantedLevel;
    private float speed;
    private float movementX;
    private float movementY;
    private float movementZ;
    private Vector3 direction;
    // Start is called before the first frame update
    void Start() {
        animator = seagull.GetComponent<Animator>();
        inSky = true;
        health = 100f;
        wantedLevel = 0f;
        speed = 2.5f;
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
            movementZ = -2;
        }
        else {
            movementZ = 2;
        }
        transform.Translate(0, movementZ, 0, null);
        inSky = !inSky;
    }

    // private void OnTriggerEnter(Collider other) {
    //     if (other.gameObject == player && hasFries) {

    //     }
    // }
}
