using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovementControl : MonoBehaviour
{

    public GameObject movementStriker, movementBar;
    public GameObject realStriker;
    public float extent;

    public Text overlapAlert;
    // Start is called before the first frame update
    void Start()
    {
        realStriker.GetComponent<Striker>().OnReset += OnStrikerReset;
    }

    bool movementStarted = false;

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D,Vector3.zero);
            if(hit.transform == movementBar.transform)
            {
                movementStarted = true;
                overlapAlert.gameObject.SetActive(false);
                realStriker.GetComponent<CircleCollider2D>().isTrigger = true;
                UpdateStrikerPosition(mousePos);
            }
        }

        if(movementStarted && Input.GetMouseButton(0))
        {
            UpdateStrikerPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }

        if (Input.GetMouseButtonUp(0))
        {
            movementStarted = false;
            CheckOverlaps();
        }

#elif UNITY_ANDROID

#endif
    }

    void UpdateStrikerPosition(Vector3 inputPos)
    {
        if (movementStarted)
        {
            float posX = inputPos.x;
            if(posX > 0 && posX > extent)
            {
                posX = extent;
            }else if(posX < -extent)
            {
                posX = -extent;
            }

            Vector3 movementStrikerPos = movementStriker.transform.position;
            movementStrikerPos.x = posX;
            movementStriker.transform.position = movementStrikerPos;

            Vector3 realStrikerPos = realStriker.transform.position;
            realStrikerPos.x = posX;
            realStriker.transform.position = realStrikerPos;
        }
    }

    void OnStrikerReset()
    {
        Vector3 movementStrikerPos = movementStriker.transform.position;
        movementStrikerPos.x = realStriker.transform.position.x;
        movementStriker.transform.position = movementStrikerPos;
        StartCoroutine(CheckOverlapInNextFrame());
    }

    IEnumerator CheckOverlapInNextFrame()
    {
        yield return new WaitForEndOfFrame();
        CheckOverlaps();
    }

    void CheckOverlaps()
    {
        Collider2D[] overlappingColliders = new Collider2D[19];
        if (realStriker.GetComponent<Rigidbody2D>().OverlapCollider((new ContactFilter2D()).NoFilter(), overlappingColliders) > 0)
        {
            realStriker.GetComponent<Striker>().isDisabled = true;
            overlapAlert.gameObject.SetActive(true);
            return;
        }
        realStriker.GetComponent<CircleCollider2D>().isTrigger = false;
        realStriker.GetComponent<Striker>().isDisabled = false;
        overlapAlert.gameObject.SetActive(false);
    }
}
