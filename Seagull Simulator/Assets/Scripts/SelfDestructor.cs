using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestructor : MonoBehaviour
{
    public float lifeTime;
    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(SelfDestruct());
    }

    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
