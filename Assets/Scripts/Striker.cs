using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Striker : MonoBehaviour
{

    public float maxLineLength, minLineLength, thrust;
    public Transform strikerSpawnPoint;
    public ResetEvent OnReset;

    [HideInInspector]
    public bool isDisabled = false;

    LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    bool movementStarted = false;
    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0) && !isDisabled)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector3.zero);
            if (hit.transform == transform)
            {
                movementStarted = true;
                DrawLine(mousePos);
            }
        }

        if (movementStarted && Input.GetMouseButton(0))
        {
            DrawLine(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }

        if (Input.GetMouseButtonUp(0) && movementStarted)
        {
            movementStarted = false;
            //Apply Force to Striker
            if(lineDistance > minLineLength)
            {
                Rigidbody2D rigidBody = GetComponent<Rigidbody2D>();
                Vector3 delta = endPoint - transform.position;
                rigidBody.AddForce(delta.normalized * lineDistance * thrust, ForceMode2D.Impulse);
                StartCoroutine(ResetAfterCoinsStopped());
            }
            lineRenderer.enabled = false;
        }
#elif UNITY_ANDROID

#endif
    }

    float lineDistance;
    Vector3 endPoint;
    void DrawLine(Vector3 inputPos)
    {
        if (movementStarted)
        {
            inputPos.z = transform.position.z;          //Don't want to move in z-axis
            Vector3 delta = inputPos - transform.position;

            float distance = delta.magnitude;
            if(distance > maxLineLength)
            {
                distance = maxLineLength;
            }

            Vector3 newPoint = transform.position + (-1.0f * distance * delta.normalized);      //-1 for opposite direction

            lineRenderer.enabled = true;
            lineRenderer.positionCount = 2;
            Vector3[] linePoints = { transform.position, newPoint };
            lineRenderer.SetPositions(linePoints);
            
            lineDistance = distance;
            endPoint = newPoint;
        }
    }

    IEnumerator ResetAfterCoinsStopped()
    {
        Rigidbody2D[] GOS = transform.parent.GetComponentsInChildren<Rigidbody2D>() as Rigidbody2D[];
        
        bool allSleeping = false;

        while (!allSleeping)
        {
            allSleeping = true;

            foreach (Rigidbody2D GO in GOS)
            {
                if (!GO.IsSleeping())
                {
                    allSleeping = false;
                    yield return null;
                    break;
                }
            }

        }
        yield return new WaitForSeconds(0.3f);
        //Do something else
        GetComponent<CircleCollider2D>().isTrigger = true;
        yield return new WaitForEndOfFrame();
        Reset();
    }

    private void Reset()
    {   
        transform.position = strikerSpawnPoint.transform.position;
        OnReset?.Invoke();
    }
}

public delegate void ResetEvent();
