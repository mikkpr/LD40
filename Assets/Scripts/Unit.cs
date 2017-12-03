using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    public int startHealth = 3;
    public int health = 0;
    public float interval = 1.0f;
    public float offset = 0.0f;
    public bool sequence = false;
    public List<KeyCode> keyCodes = null;
    public UnitGroup group = null;
    public float scrollSpeed = 0.2f; // TODO get from world
    public Vector3 scrollDirection = new Vector3(-1.0f, 0.0f, 0.0f);
    public Vector3 inGroupTargetPosition = Vector3.zero;
    public float maxSpeed = 1.0f;
    public float globalYToZOffset = 2.0f; // TODO get this from world
    public float yToZ = -0.5f;
    public float bannerTextUpdateDelta = 0.5f;

    private UnitGroup candidateGroup = null;
    private bool followInGroupTarget = false;
    private TextMesh bannerText = null;
    private float lastBannerTextUpdate = float.NegativeInfinity;
    private Animator bannerManAnimator = null;

    void Awake() {
        keyCodes = new List<KeyCode>();
        health = startHealth;
        {
            Component[] components = GetComponentsInChildren(typeof(TextMesh));
            foreach (Component c in components) {
                if (c.gameObject.tag == "BannerText") {
                    bannerText = (TextMesh)c;
                    break;
                }
            }
        }
        {
            Component[] components = GetComponentsInChildren(typeof(Animator));
            foreach (Component c in components) {
                if (c.gameObject.tag == "BannerMan") {
                    bannerManAnimator = (Animator)c;
                    break;
                }
            }
        }
    }

    void Update () {
        float t = Time.time;
        Vector3 newPosition = Vector3.zero;

        // Change position
        if (group == null) {
            newPosition = transform.position + scrollDirection * Time.deltaTime;
        } else {
            if (followInGroupTarget) {
                newPosition = Vector3.Lerp(transform.position, inGroupTargetPosition, Time.deltaTime * maxSpeed);
            }
        }

        // Set depth according to Y position
        newPosition.z = (globalYToZOffset - newPosition.y) * yToZ;

        // TODO also adjust scale depending on depth

        transform.position = newPosition;

        // Update banner, if exists
        if (lastBannerTextUpdate < t) {
            if (bannerText != null && keyCodes != null) {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (KeyCode kc in keyCodes) {
                    sb.Append(kc);
                }
                bannerText.text = sb.ToString();
            }

            lastBannerTextUpdate = t + bannerTextUpdateDelta;
        }
    }

    public void OnBeatStart() {
        // Time window for unit key presses started
        if (bannerManAnimator != null) {
            bannerManAnimator.SetBool("Alert", true);
        }
    }

    public void OnBeatEnd(bool success) {
        if (health <= 0) {
            // This unit is already dismissed
            return;
        }

        // Time window for unit key presses ended
        if (success) {
            // This unit is successful
            if (group == null) {
                // Not in the army yet
                if (candidateGroup != null) {
                    // Join the army
                    candidateGroup.AddUnit(this);
                    group = candidateGroup;
                }
            } else {
                // We are in the army
                // Keep up the good work
            }
        } else {
            // This unit failed
            if (group != null) {
                if (health > 0) {
                    // Reduce this unit's health
                    health -= 1;
                    if (health == 0) {
                        // This unit is dismissed from the army
                        group.RemoveUnit(this);
                        group = null;
                        inGroupTargetPosition = Vector3.zero;
                        followInGroupTarget = false;
                    }
                }
            }
        }

        if (bannerManAnimator != null) {
            bannerManAnimator.SetBool("Alert", false);
        }
    }

    public void OnBeatHit() {
        // Input was pressed in time window
    }

    public void OnBeatDouble() {
        // Double input was given in time window
    }

    public void OnOutOfBeat() {
        // Keys were pressed out of time window
    }

    public void SetInGroupTarget(Vector3 target) {
        inGroupTargetPosition = target;
        followInGroupTarget = true;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (candidateGroup != null) {
            return;
        }
        if (other.gameObject.tag == "Army") {
            candidateGroup = other.gameObject.GetComponent<UnitGroup>();
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (candidateGroup == null) {
            return;
        }
        if (other.gameObject.tag == "Army") {
            candidateGroup = null;
        }
    }
}
