using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ledgeDetector : MonoBehaviour
{
    public GameObject grabPosition;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PlayerLedgeGrabber")
        {
            GameObject.Find("Player").GetComponent<Player>().isGrabingEdge = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "PlayerLedgeGrabber")
        {
            GameObject.Find("Player").GetComponent<Player>().StopGrabingEdge();
            GameObject.Find("Player").GetComponent<Player>().isGrabingEdge = false;
        }
    }
}
