using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMecanism : MonoBehaviour
{
    public GameObject waypointsParent;
    public GameObject particle;
    public Transform pivot;
    private List<Transform> waypoints;
    private List<GameObject> doors;
    private int current = 0;
    private bool finished;
    private bool doorsOpen;
    public float timeBtwPlay = 0.1f;
    private float elapsedBtwPlay;
    private bool on;

    public void Play(List<GameObject> _doors)
    {
        on = true;
        doors = _doors;
    }

    private void Start()
    {
        waypoints = new List<Transform>();
        foreach (Transform child in waypointsParent.transform)
        {
            waypoints.Add(child.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (on)
        {
            if (!finished)
            {
                if (elapsedBtwPlay > 0)
                {
                    elapsedBtwPlay -= Time.deltaTime;
                }
                else
                {
                    GameObject instance = Instantiate(particle, pivot.position, Quaternion.identity);
                    Destroy(instance, 1f);
                    elapsedBtwPlay = timeBtwPlay;
                }
            }


            if (current > waypoints.Count - 1)
            {
                finished = true;
            }
            if (!finished)
            {
                pivot.position = Vector2.MoveTowards(pivot.position, waypoints[current].position, 0.1f);
                if (Vector2.Distance(pivot.position, waypoints[current].position) < 0.1f)
                {
                    current++;
                }
            }
            if (finished && !doorsOpen)
            {
                if (doors != null)
                {
                    foreach (GameObject door in doors)
                    {
                        door.GetComponent<CellDoor>().Interact();
                    }
                    doorsOpen = true;
                }
            }
        }

    }



}
