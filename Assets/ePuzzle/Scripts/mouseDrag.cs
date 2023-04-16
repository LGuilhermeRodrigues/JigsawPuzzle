using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
public class mouseDrag : MonoBehaviour
{
    public static int topOrder = 1;
    private Vector3 positionOnDragStartObject;
    private Vector3 positionOnDragStartCursor;
    private Vector2 initialPosition;
    private Vector2 endPosition;
    private Vector2 TopLeftCorner = new Vector2(-8.8f, 4.8f);
    private Vector2 BottomRightCorner = new Vector2(-2.3f, -2f);
    private bool initialMoving = false;
    private float deltaTimeSum = 0;
    private bool stayFixed = true;
    
    private void Start()
    {
        //initialPosition = transform.position;
        TopLeftCorner = GameObject.Find("TopLeftCorner").transform.localPosition;
        Debug.Log($"TopLeftCorner: {TopLeftCorner}");
        BottomRightCorner = GameObject.Find("BottomRightCorner").transform.localPosition;
        Debug.Log($"BottomRightCorner: {BottomRightCorner}");
        StartCoroutine(SpreadPiece());
    }

    private IEnumerator SpreadPiece()
    {
        yield return new WaitForSeconds(3);
        //transform.position = new Vector2(TopLeftCorner.x,
        //    TopLeftCorner.y);
        //transform.position = new Vector2(BottomRightCorner.x,
        //    BottomRightCorner.y);
        var randomX = UnityEngine.Random.Range(TopLeftCorner.x, BottomRightCorner.x);
        var randomY = UnityEngine.Random.Range(BottomRightCorner.y, TopLeftCorner.y);
        //transform.position = new Vector2(randomX, randomY);
        endPosition = new Vector2(randomX, randomY);
        initialMoving = true;
    }

    private void Update()
    {
        if (initialMoving)
        {
            if (deltaTimeSum > 5)
            {
                stayFixed = false;
                transform.localPosition = endPosition;
                initialMoving = false;
            }
            deltaTimeSum += Time.deltaTime;
            transform.localPosition = 
                Vector3.MoveTowards(transform.localPosition, endPosition,
                    60 * Time.deltaTime);
        }
        
        if (stayFixed)
        {
            return;
        }
        /*
        if (isCloseToInitialPosition())
        {
            transform.localPosition = initialPosition;
        }
        */
        var distance = Vector2.Distance(
            Camera.main.WorldToScreenPoint(transform.position), initialPosition);
        if (gameObject.name.Equals("Piece 0 0"))
        {
            Debug.Log($"distance: {distance} position: {transform.position} transformPoint: {transform.TransformPoint(transform.localPosition)} initialPosition: {initialPosition} localPosition = {transform.localPosition}");
        }
        
    }

    public void OnMouseDrag(){
        Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y,transform.position.z-Camera.main.transform.position.z);
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(screenPosition);
        var mouseMovePath = mousePosition - positionOnDragStartCursor;
        transform.position = positionOnDragStartObject + mouseMovePath;
    }

    private void OnMouseDown()
    {
        positionOnDragStartCursor = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y,transform.position.z-Camera.main.transform.position.z));
        positionOnDragStartObject = transform.position;
        // Change the piece to the top of the stack in canvas
        transform.SetAsLastSibling();
    }

    private void OnMouseUp()
    {
        //Debug.Log("OnMouseUp");
        //var respawn = GetComponent<Respawn>();
        if (isCloseToInitialPosition())
        {
            //respawn.ReturnToInitialPosition();
        }
    }
    
    bool isCloseToInitialPosition()
    {
        //var respawn = GetComponent<Respawn>();
        var distance = Vector2.Distance(transform.TransformPoint(transform.localPosition), initialPosition);
        if (gameObject.name.Equals("Piece 0 0"))
        {
            //Debug.Log(distance);
        }
        //return distance < 2f;
        return false;
    }

    public void SetInitialPosition()
    {
        initialPosition = Camera.main.WorldToScreenPoint(transform.position);
        if (gameObject.name.Equals("Piece 0 0"))
        {
            Debug.Log($"initialPosition: {initialPosition}");
        }
        // set endPosition to be random in the range of the corners
        /*endPosition = new Vector2(
            UnityEngine.Random.Range(TopLeftCorner.x, BottomRightCorner.x),
            UnityEngine.Random.Range(BottomRightCorner.y, TopLeftCorner.y));*/
    }
}
