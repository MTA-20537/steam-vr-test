using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Hand : MonoBehaviour
{
    public SteamVR_Action_Boolean m_GrabAction = null;

    private SteamVR_Behaviour_Pose m_Pose = null;
    private FixedJoint m_Joint = null;

    private Interactable m_CurrentInteractable = null;
    public List<Interactable> m_ContactInteractable = new List<Interactable>();


    private void Awake()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
        m_Joint = GetComponent<FixedJoint>();
    }

    // Update is called once per frame
    private void Update()
    {
        if(m_GrabAction.GetStateDown(m_Pose.inputSource))
        {
            print(m_Pose.inputSource + "Trigger down");
            pickup();
        }
        if (m_GrabAction.GetStateUp(m_Pose.inputSource))
        {
            print(m_Pose.inputSource + "Trigger up");
            drop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Interactable"))
        {
            return;
        }
        m_ContactInteractable.Add(other.gameObject.GetComponent<Interactable>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Interactable"))
        {
            return;
        }
        m_ContactInteractable.Remove(other.gameObject.GetComponent<Interactable>());
    }

    public void pickup()
    {
        m_CurrentInteractable = GetNearestInteractable();

        if (!m_CurrentInteractable)
            return;

        if (m_CurrentInteractable.m_activeHand)
        {
            m_CurrentInteractable.m_activeHand.drop();
        }
        m_CurrentInteractable.transform.position = transform.position;

        Rigidbody targetbody = m_CurrentInteractable.GetComponent<Rigidbody>();
        m_Joint.connectedBody = targetbody;

        m_CurrentInteractable.m_activeHand = this;
    }

    public void drop()
    {
        if (!m_CurrentInteractable)
            return;

        Rigidbody targetbody = m_CurrentInteractable.GetComponent<Rigidbody>();
        targetbody.velocity = m_Pose.GetVelocity();
        targetbody.angularVelocity = m_Pose.GetAngularVelocity();

        m_Joint.connectedBody = null;

        m_CurrentInteractable.m_activeHand = null;
        m_CurrentInteractable = null;
    }

    private Interactable GetNearestInteractable()
    {
        Interactable nearest = null;
        float minDistance = float.MaxValue;
        float distance = 0.0f;

        foreach(Interactable interactable in m_ContactInteractable)
        {
            distance = (interactable.transform.position - transform.position).sqrMagnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = interactable;
            }
        }
        return null;
    }
}
