//using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VrGamesDev;
using VrGamesDev.BHEL;
using VrGamesDev.DDuA;


// <summary>
// This class controls the mechanics of the example DDuA/03 Spawner
// Custom the duration and number of random spawns before they dissapears
// </summary>

///#IGNORE
public class Example_Bullet : MonoBehaviour
{
    //[SerializeField]
 //   private Collider m_Collider = null;

    [SerializeField]
    private Rigidbody m_Rigidbody = null;

    /// <summary>
    /// The Sound effect to play, use a SpawnPrefab sound to have a fire and forget SFx
    /// </summary>
    [Tooltip("The Sound effect to play, use a SpawnPrefab sound to have a fire and forget SFx")]
    [SerializeField] private GameObject m_SFx = null;

    /// <summary>
    /// The Sound effect to play, use a SpawnPrefab sound to have a fire and forget SFx
    /// </summary>
    [Tooltip("The Sound effect to play, use a SpawnPrefab sound to have a fire and forget SFx")]
    [SerializeField] private GameObject m_Hit = null;


    [SerializeField]
    private float m_Force = 10.0f;

    [SerializeField]
    private float m_Destroy = 3.5f;



    private void OnCollisionEnter(Collision collision)
    {
        // if there is a sound
        if (this.m_Hit != null)
        {
            // ... play it
            Object.Instantiate(this.m_Hit, this.transform);
        }
    }

    public void Play()
    {
        this.Stop();

        this.m_Rigidbody.useGravity = true;
        this.m_Rigidbody.AddForce(this.transform.forward * (this.m_Force * Random.Range(0.55f, 1.25f)), ForceMode.Impulse);

        // if there is a sound
        if (this.m_SFx != null)
        {
            // ... play it
            Object.Instantiate(this.m_SFx, this.transform);
        }

        StartCoroutine(this.AutoDestroy());
    }

    private IEnumerator AutoDestroy()
    {
        if (this.m_Destroy > 0)
        {
            yield return new WaitForSeconds(this.m_Destroy);

            this.gameObject.SetActive(false);
        }

        yield return null;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        this.Stop();
    }

    public void Stop()
    {
        this.m_Rigidbody.useGravity = false;
        this.m_Rigidbody.velocity = Vector3.zero;
        this.m_Rigidbody.angularVelocity = Vector3.zero;
    }
}