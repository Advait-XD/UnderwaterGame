using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    public AudioSource footstepsSound; // Reference to the footstep sound
    public AudioSource swimmingSound; // Reference to the swimming sound
    public KeyCode sprintKey = KeyCode.LeftShift; // Key to trigger sprinting
    public float stepDelay = 0.5f; // Time delay between footsteps
    private float nextStepTime; // Time when the next step can be played
    private bool isSwimming = false; // Track if the player is swimming

    void Update()
    {
        // Check if the player is moving
        if (IsMoving())
        {
            // Play footstep sound if it is not already playing, it's time for the next step, and the player is not swimming
            if (!footstepsSound.isPlaying && Time.time >= nextStepTime && !isSwimming)
            {
                // Increase pitch for sprinting
                if (Input.GetKey(sprintKey))
                {
                    footstepsSound.pitch = 1.5f; // Increase pitch for sprinting
                }
                else
                {
                    footstepsSound.pitch = 1.0f; // Normal pitch for walking
                }

                footstepsSound.Play();
                nextStepTime = Time.time + stepDelay; // Set the next step time
            }
            else if (isSwimming)
            {
                // Play swimming sound if the player is swimming and it is not already playing
                if (!swimmingSound.isPlaying)
                {
                    swimmingSound.Play();
                }
            }
        }
        else
        {
            // Stop footstep sound if not moving
            if (footstepsSound.isPlaying)
            {
                footstepsSound.Stop();
            }

            // Stop swimming sound if not swimming
            if (isSwimming && swimmingSound.isPlaying)
            {
                swimmingSound.Stop();
            }
        }
    }

    // Helper method to check if the player is moving
    private bool IsMoving()
    {
        // Return true if any movement keys are pressed
        return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
    }

    // Detect terrain changes
    private void OnTriggerEnter(Collider other)
    {
        // Check the tag of the collider the player enters
        if (other.CompareTag("Water"))
        {
            isSwimming = true; // Set swimming state to true
            footstepsSound.Stop(); // Stop the footstep sound if swimming starts

            // Start swimming sound if not already playing
            if (!swimmingSound.isPlaying)
            {
                swimmingSound.Play();
            }
        }
        else if (other.CompareTag("Sand"))
        {
            isSwimming = false; // Set swimming state to false
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Reset swimming state when exiting the water
        if (other.CompareTag("Water"))
        {
            isSwimming = false; // Set swimming state to false
            swimmingSound.Stop(); // Stop swimming sound when exiting water
        }
    }
}
