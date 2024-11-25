using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameOrb : MonoBehaviour
{
    [SerializeField] private SO_Gun GunData;

    public Vector3 targ;

    private Camera cam;

    private Vector3 direction;

    private Rigidbody rb;

    public float NumOfBounces = 5;

    public float WaitTimeBeforeTurn = 1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        cam = Camera.main;

        direction = cam.transform.forward;

        Vector3 newTarget = transform.forward;
        newTarget.y = transform.position.y;
        transform.rotation = Quaternion.LookRotation(direction);
        StartCoroutine(ZigZag());
    }

    private void Update()
    {
        //rb.AddForce( transform.forward * (GunData.ProjectileSpeed * Time.deltaTime) );
        transform.rotation = Quaternion.LookRotation(direction);
        rb.velocity = transform.forward * GunData.ProjectileSpeed;
    }
    private void OnCollisionEnter(Collision collision)
    {

    }

    private IEnumerator ZigZag()
    {
        direction = (Quaternion.AngleAxis(45, transform.up) * transform.forward);//45 degrees first so it zig zags in front of player instead of diagonally
        transform.rotation = Quaternion.LookRotation(direction);
        yield return new WaitForSeconds(WaitTimeBeforeTurn / 2);
        direction = (Quaternion.AngleAxis(-90, transform.up) * transform.forward);
        transform.rotation = Quaternion.LookRotation(direction);

        for (int i = 0; i < NumOfBounces; i++)
        {
            yield return new WaitForSeconds(WaitTimeBeforeTurn);
            direction = (Quaternion.AngleAxis(90, transform.up) * transform.forward);
            transform.rotation = Quaternion.LookRotation(direction);
            yield return new WaitForSeconds(WaitTimeBeforeTurn);
            direction = (Quaternion.AngleAxis(-90, transform.up) * transform.forward);
            transform.rotation = Quaternion.LookRotation(direction);
        }

        Destroy(gameObject);
    }
}
