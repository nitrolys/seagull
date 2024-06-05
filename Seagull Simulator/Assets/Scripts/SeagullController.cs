using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SeagullController : MonoBehaviour
{
    public float speed;
    public Rigidbody rb;
    public Material mat;

    private Animator animator;
    private bool inSky;
    private float health;
    private int firesEaten;

    private float flightCDMax;
    private float flightCD;

    private float movementX;
    private float movementY;
    private Vector3 direction;
    private bool gameEnded;

    private int numGotFries;

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

    public int getFriesEaten()
    {
        return firesEaten;
    }

    void Start() {
        animator = gameObject.GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody>();
        inSky = false;
        health = 100f;
        firesEaten = 0;
        flightCDMax = 5;
        flightCD = flightCDMax;
        numGotFries = 0;
        gameEnded = false;
    }

    // Update is called once per frame
    void Update() {
        changeAnimation();
        changePosition();
        changeStat();
    }

    void changeAnimation() {
        if (! gameEnded)
        {
            if (inSky)
            {
                animator.Play("Fly");
            }
            else if (movementX != 0 || movementY != 0)
            {
                animator.Play("Walk");
            }
            else
            {
                animator.Play("Idle_A");
            }
        }
    }

    void changePosition() {
        transform.position += speed * new Vector3(movementX, 0, movementY) * Time.deltaTime;
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);
    }

    void changeStat() {
        IncrementHealth(-Time.deltaTime * 2);

        if (health <= 0)
        {
            // Implement Game Over logic
            StartCoroutine(Death());
        }

        if (numGotFries >= 10)
        {
            StartCoroutine(Win());
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
        rb.isKinematic = true;
        while (transform.position.y < 2)
        {
            transform.position += new Vector3(0, 10, 0) * Time.deltaTime;
            yield return null;
        }
        rb.isKinematic = false;
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

    public void EatFries()
    {
        firesEaten += 1;
    }

    public void IncrementFries()
    {
        numGotFries += 1;
    }

    public void TakeHit(float damage, Vector3 attackerPos)
    {
        Vector3 knockBack = 10 * Vector3.Normalize(transform.position - attackerPos);
        rb.velocity = knockBack;
        IncrementHealth(-damage);
        StartCoroutine(flashRed());
    }

    private IEnumerator flashRed()
    {
        mat.SetColor("_tint", new Color(1, 0, 0, 1));
        yield return new WaitForSeconds(0.1f);
        mat.SetColor("_tint", new Color(1, 1, 1, 1));
    }

    private IEnumerator Death()
    {
        gameEnded = true;
        animator.speed = 0.2f;
        animator.Play("Death");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("GameOver");
    }

    private IEnumerator Win()
    {
        gameEnded = true;
        animator.Play("Spin");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("WinScene");
    }
}
