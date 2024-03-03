using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraScaler : MonoBehaviour
{

    public float referenceHeight;
    public float referenceWidth;
    public float referenceSize;

    public SpriteRenderer board;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Screen Height : " + Screen.height);
        Debug.Log("Screen Height : " + Screen.width);
        GetComponent<Camera>().orthographicSize = board.bounds.size.x * Screen.height / Screen.width * 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
