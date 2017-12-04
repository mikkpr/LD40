using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

/// <summary>
/// Parallax scrolling script that should be assigned to a layer
/// </summary>
public class ScrollingScript : MonoBehaviour
{
    /// <summary>
    /// Scrolling speed
    /// </summary>
    public Vector2 speed = new Vector2(10, 10);

    /// <summary>
    /// Moving direction
    /// </summary>
    public Vector2 direction = new Vector2(-1, 0);

    /// <summary>
    /// Movement should be applied to camera
    /// </summary>
    public bool isLinkedToCamera = false;

    /// <summary>
    /// 1 - Background is infinite
    /// </summary>
    public bool isLooping = false;

    /// <summary>
    /// 2 - List of children with a renderer.
    /// </summary>
    private List<SpriteRenderer> backgroundPart;

    public Boss boss;

    // 3 - Get all the children
    void Start()
    {
        // For infinite background only
        if (isLooping)
        {
            // Get all the children of the layer with a renderer
            backgroundPart = new List<SpriteRenderer>();

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                SpriteRenderer r = child.GetComponent<SpriteRenderer>();

                // Add only the visible children
                if (r != null)
                {
                    backgroundPart.Add(r);
                }
            }

            // Sort by position.
            // Note: Get the children from left to right.
            // We would need to add a few conditions to handle
            // all the possible scrolling directions.
            backgroundPart = backgroundPart.OrderBy(t => t.transform.position.x).ToList();
        }
    }

    public static EventHandler<EventArgs> ProgressUpdated;

    void Update()
    {
        if (Time.time >= Constants.Level1Duration)
        {
            if (boss != null)
            {
                RhythmEngine.GetTagged().SetBoss(boss);
            }
            else
            {
                print("Boss not attached to this GameObject, ignoring");
            }

            return;
        }

        if (ProgressUpdated != null)
        {
            float passed = SoundManager.instance.GetCurrentTime();
            float progress = passed / Constants.Level1Duration * 100;
            ProgressUpdated(progress, EventArgs.Empty);
        }

        // Movement
        Vector3 movement = new Vector3(speed.x * direction.x, speed.y * direction.y, 0);

        movement *= Time.deltaTime;
        transform.Translate(movement);

        // Move the camera
        if (isLinkedToCamera)
        {
            Camera.main.transform.Translate(movement);
        }

        // 4 - Loop
        if (isLooping)
        {
            // Get the first object.
            // The list is ordered from left (x position) to right.
            SpriteRenderer firstChild = backgroundPart.FirstOrDefault();

            if (firstChild != null)
            {
                // Check if the child is already (partly) before the camera.
                // We test the position first because the IsVisibleFrom
                // method is a bit heavier to execute.
                if (firstChild.transform.position.x < Camera.main.transform.position.x)
                {
                    // If the child is already on the left of the camera,
                    // we test if it's completely outside and needs to be
                    // recycled.
                    if (firstChild.IsVisibleFrom(Camera.main) == false)
                    {
                        // Get the last child position.
                        SpriteRenderer lastChild = backgroundPart.LastOrDefault();

                        Vector3 lastPosition = lastChild.transform.position;
                        Vector3 lastSize = (lastChild.bounds.max - lastChild.bounds.min);

                        // Set the position of the recyled one to be AFTER
                        // the last child.
                        // Note: Only work for horizontal scrolling currently.
                        firstChild.transform.position = new Vector3(lastPosition.x + lastSize.x, firstChild.transform.position.y, firstChild.transform.position.z);

                        // Set the recycled child to the last position
                        // of the backgroundPart list.
                        backgroundPart.Remove(firstChild);
                        backgroundPart.Add(firstChild);
                    }
                }
            }
        }
    }
}

public static class RendererExtensions
{
    public static bool IsVisibleFrom(this Renderer renderer, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }
}

public class DistanceEventArgs : EventArgs
{
    public float X { get; set; }

    public float Distance { get; set; }
}