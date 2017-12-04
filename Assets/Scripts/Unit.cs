using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Unit : MonoBehaviour
{

    public int startHealth = 3;
    public int health = 0;
    public float interval = 1.0f;
    public float offset = 0.0f;
    public bool sequence = false;
    public List<KeyCode> keyCodes = null;
    public UnitGroup group = null;
    public Vector3 scrollDirection = new Vector3(-1.0f, 0.0f, 0.0f);
    public Vector3 inGroupTargetPosition = Vector3.zero;
    public float maxSpeed = 1.0f;
    public float globalYToZOffset = 2.0f; // TODO get this from world
    public float yToZ = -0.5f;
    public float bannerTextUpdateDelta = 0.5f;

    private UnitGroup candidateGroup = null;
    private bool followInGroupTarget = false;
    private TextMeshPro bannerText = null;
    private float lastBannerTextUpdate = float.NegativeInfinity;
    private Animator bannerManAnimator = null;
    private List<Animator> soldierAnimators = null;

    void Awake()
    {
        keyCodes = new List<KeyCode>();
        health = startHealth;
        {
            Component[] components = GetComponentsInChildren(typeof(TextMeshPro));
            foreach (Component c in components)
            {
                if (c.gameObject.tag == "BannerText")
                {
                    bannerText = (TextMeshPro)c;
                    break;
                }
            }
        }
        {
            soldierAnimators = new List<Animator>();
            Component[] components = GetComponentsInChildren(typeof(Animator));
            foreach (Component c in components)
            {

                if (c.gameObject.tag == "BannerMan")
                {
                    bannerManAnimator = (Animator)c;
                }
                else
                {
                    soldierAnimators.Add((Animator)c);
                }
            }
        }
    }

    void Update()
    {

        float t = Time.time;
        Vector3 newPosition = Vector3.zero;

        // Change position
        if (group == null)
        {
            newPosition = transform.position + scrollDirection * Time.deltaTime;
        }
        else
        {
            if (followInGroupTarget)
            {
                newPosition = Vector3.Lerp(transform.position, inGroupTargetPosition, Time.deltaTime * maxSpeed);
            }
        }

        // Set depth according to Y position
        newPosition.z = (globalYToZOffset - newPosition.y) * yToZ;

        // TODO also adjust scale depending on depth

        transform.position = newPosition;

        // Update banner, if exists
        if (lastBannerTextUpdate < t)
        {
            if (bannerText != null && keyCodes != null)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (KeyCode kc in keyCodes)
                {
                    switch (kc)
                    {
                        case KeyCode.Space:
                            sb.Append("Spc");
                            break;
                        case KeyCode.Alpha0:
                            sb.Append("0");
                            break;
                        case KeyCode.Alpha1:
                            sb.Append("1");
                            break;
                        case KeyCode.Alpha2:
                            sb.Append("2");
                            break;
                        case KeyCode.Alpha3:
                            sb.Append("3");
                            break;
                        case KeyCode.Alpha4:
                            sb.Append("4");
                            break;
                        case KeyCode.Alpha5:
                            sb.Append("5");
                            break;
                        case KeyCode.Alpha6:
                            sb.Append("6");
                            break;
                        case KeyCode.Alpha7:
                            sb.Append("7");
                            break;
                        case KeyCode.Alpha8:
                            sb.Append("8");
                            break;
                        case KeyCode.Alpha9:
                            sb.Append("9");
                            break;
                        default:
                            sb.Append(kc);
                            break;
                    }
                }
                bannerText.text = sb.ToString();
            }

            lastBannerTextUpdate = t + bannerTextUpdateDelta;
        }
    }

    public void OnBeatWindup()
    {
        // Time window for unit key presses started
        if (bannerManAnimator != null)
        {
            bannerManAnimator.SetTrigger("Alert");
        }

        if (soldierAnimators != null)
        {
            foreach (Animator a in soldierAnimators)
            {
                a.SetTrigger("Alert");
            }
        }
    }

    public void OnBeatStart()
    {
    }

    public void OnBeatEnd(bool success)
    {
        if (health <= 0)
        {
            // This unit is already dismissed
            return;
        }

        // Time window for unit key presses ended
        if (success)
        {
            // This unit is successful
            if (group == null)
            {
                // Not in the army yet
                if (candidateGroup != null)
                {
                    // Join the army
                    candidateGroup.AddUnit(this);
                    group = candidateGroup;
                }
            }
            else
            {
                // We are in the army
                // Keep up the good work
            }
        }
        else
        {
            // This unit failed
            if (group != null)
            {
                if (health > 0)
                {
                    // Reduce this unit's health
                    health -= 1;
                    if (health == 0)
                    {
                        // This unit is dismissed from the army
                        group.RemoveUnit(this);
                        group = null;
                        inGroupTargetPosition = Vector3.zero;
                        followInGroupTarget = false;
                    }

                    // Play failure animation
                    if (soldierAnimators != null)
                    {
                        foreach (Animator a in soldierAnimators)
                        {
                            a.SetTrigger("Stumble");
                        }
                    }
                }
            }
        }
    }

    public void OnBeatHit()
    {
        // Input was pressed in time window
    }

    public void OnBeatDouble()
    {
        // Double input was given in time window
    }

    public void OnOutOfBeat()
    {
        // Keys were pressed out of time window
    }

    public void SetInGroupTarget(Vector3 target)
    {
        inGroupTargetPosition = target;
        followInGroupTarget = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (candidateGroup != null)
        {
            return;
        }

        if (other.gameObject.tag == "Army")
        {
            candidateGroup = other.gameObject.GetComponent<UnitGroup>();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (candidateGroup == null)
        {
            return;
        }

        if (other.gameObject.tag == "Army")
        {
            candidateGroup = null;
        }
    }
}
