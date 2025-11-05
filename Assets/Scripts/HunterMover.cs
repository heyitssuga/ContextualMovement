using System.Collections.Generic;
using UnityEngine;

public class HunterMover : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    float startingSpeed;
    RaycastHit hitFront, hitLeft, hitRight;
    [SerializeField] float forwardDist, sideDist, downDist;
    bool leftWall, rightWall;
    float Stimer, Itimer;

    int randInt;

    [SerializeField] GameObject downCheck, jumpCheck;

    Rigidbody rbody;

    bool grounded;

    [SerializeField] List<GameObject> targets = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movementSpeed = 5;
        forwardDist = 1.0f;
        sideDist = 2.0f;
        downDist = 1.0f;

        rbody = GetComponent<Rigidbody>();
        grounded = true;
        startingSpeed = movementSpeed;
    }

    // Update is called once per frame
    private void Update()
    {
        if (movementSpeed > startingSpeed)
        {
            if (Stimer > 0)
            {
                Stimer -= Time.deltaTime;
            }
            else
            {
                movementSpeed = startingSpeed;
                Stimer = 5.0f;
            }
        }

        if (grounded)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i] == null)
                {
                    targets.RemoveAt(i);
                }
            }
            // Rotate the mover if an object is detected in front
            if (Physics.BoxCast(transform.position + transform.up, new Vector3(0.5f, 0.9f, 0.5f), transform.forward, out hitFront, Quaternion.identity, forwardDist))
            {
                transform.LookAt(transform.position - hitFront.normal);

                RotateAway();
            }
            else if (targets.Count > 0)
            {
                if (targets[0].CompareTag("Player"))
                {
                    Ray lineOfSight = new Ray(transform.position, transform.forward);
                    RaycastHit playerHit;

                    if (Physics.Raycast(lineOfSight, out playerHit, 9))
                    {
                        if (playerHit.collider.CompareTag("Player") && targets[0] == playerHit.collider)
                        {
                            transform.LookAt(playerHit.transform.position);
                        }
                    }

                    transform.LookAt(targets[0].transform.position);

                    if (Vector3.Distance(transform.position, targets[0].transform.position) < 1f)
                    {
                        targets[0].SetActive(false);
                    }
                }
                else if (!Physics.BoxCast(transform.position + transform.up, new Vector3(0.5f, 0.9f, 0.5f), targets[0].transform.position - transform.position, Quaternion.identity, Vector3.Distance(transform.position, targets[0].transform.position)))
                {
                    transform.LookAt(targets[0].transform.position);
                }

                if (Vector3.Distance(transform.position, targets[0].transform.position) < 0.5f)
                {
                    if (targets[0].CompareTag("Pickup") && targets[0].activeSelf == true)
                    {
                        if (movementSpeed == startingSpeed)
                        {
                            movementSpeed += 2;
                        }
                        Stimer = 5.0f;
                    }

                    targets[0].SetActive(false);
                }

                if (targets[0].activeSelf == false)
                {
                    Destroy(targets[0].gameObject);
                    targets.RemoveAt(0);
                }
            }

            // Rotate the mover if a hole is detected in front
            if (!Physics.BoxCast(downCheck.transform.position, new Vector3(0.5f, 0.9f, 0.5f), -transform.up, out hitFront, Quaternion.identity, forwardDist))
            {
                // Check if there is a floor to jump to
                if (Physics.BoxCast(jumpCheck.transform.position, new Vector3(0.3f, 0.9f, 0.3f), -transform.up, out hitFront, Quaternion.identity, forwardDist))
                {
                    // Check to make sure there is no object in the way of the jump
                    if (!Physics.CheckBox(jumpCheck.transform.position, new Vector3(0.5f, 0.9f, 0.5f)))
                    {
                        rbody.AddRelativeForce(transform.up * 300);
                        grounded = false;
                    }
                    else
                    {
                        RotateAway();
                    }
                }
                else
                {
                    RotateAway();
                }
            }
        }
    }

    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * movementSpeed * Time.fixedDeltaTime);
    }

    void RotateAway()
    {
        leftWall = false;
        rightWall = false;

        if (Physics.BoxCast(transform.position + transform.up, new Vector3(0.5f, 1, 0.5f), -transform.right, out hitLeft, Quaternion.identity, sideDist))
        {
            leftWall = true;
        }

        if (Physics.BoxCast(transform.position + transform.up, new Vector3(0.5f, 1, 0.5f), transform.right, out hitRight, Quaternion.identity, sideDist))
        {
            rightWall = true;
        }

        if (leftWall && !rightWall)
        {
            transform.Rotate(Vector3.up, 90);
        }
        else if (!leftWall && rightWall)
        {
            transform.Rotate(Vector3.up, -90);
        }
        else if (leftWall && rightWall)
        {
            transform.Rotate(Vector3.up, 180);
        }
        else
        {
            randInt = Random.Range(0, 2);
            if (randInt == 0)
            {
                transform.Rotate(Vector3.up, 90);
            }
            else
            {
                transform.Rotate(Vector3.up, -90);
            }

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        grounded = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            targets.Add(other.transform.parent.gameObject);
        }
        else if (other.CompareTag("Pickup"))
        {
            targets.Add(other.transform.parent.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            targets.Remove(other.transform.parent.gameObject);
        }
        else if (other.CompareTag("Pickup"))
        {
            targets.Remove(other.transform.parent.gameObject);
        }
    }
}
