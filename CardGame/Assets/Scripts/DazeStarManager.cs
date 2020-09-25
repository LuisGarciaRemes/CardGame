using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DazeStarManager : MonoBehaviour
{
   [SerializeField] private GameObject[] stars;
   [SerializeField] private float[] alpha;
    [SerializeField] private float xRadius;
    [SerializeField] private float yRadius;
    [SerializeField] private float speed;
    [SerializeField] private float rotSpeed;

    private void Start()
    {
        foreach (GameObject star in stars)
        {
            star.transform.rotation = Quaternion.Euler(0.0f,0.0f, Random.Range(0 , 360));
        }
    }

    void Update()
    {
        for(int i = 0; i < stars.Length; i++)
        {
            stars[i].transform.position = new Vector3(transform.position.x + (xRadius * Mathf.Cos(alpha[i])), transform.position.y + (yRadius * Mathf.Sin (alpha[i])),0.0f);
            stars[i].transform.Rotate(new Vector3(0.0f,0.0f,rotSpeed));
            alpha[i] += Time.deltaTime * speed;            
            if(alpha[i] >= 6.3f)
            {
                alpha[i] = 0.0f;
            }
        }
    }
}
