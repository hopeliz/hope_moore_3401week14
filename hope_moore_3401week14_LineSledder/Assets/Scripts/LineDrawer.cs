using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineDrawer : MonoBehaviour
{
    private LineRenderer _currentLine;
    private GameObject _currentSledder;
    private Vector3 _previousMousePosition;
    private float _mouseOffset;


    public GameObject linePrefab;
    public GameObject bouncyLinePrefab;
    public GameObject sledderPrefab;
    public Text buttonText;
    public float mininumSegmentLength = 0.5F;
    public bool isBouncy = false;



    void Start()
    {
        _currentSledder = Instantiate(sledderPrefab);
    }
    
    void Update()
    {
        if (_currentSledder != null)
        {
            if (_currentSledder.transform.position.y < -6)
            {
                Destroy(_currentSledder);
                _currentSledder = null;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            _previousMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // Spawn line prefab on left click down
            // Assign the current line

            if (!isBouncy)
            {
                _currentLine = Instantiate(linePrefab).GetComponent<LineRenderer>();
            }
            else
            {
                _currentLine = Instantiate(bouncyLinePrefab).GetComponent<LineRenderer>();
            }
        }

        if (_currentLine != null)
        {
            // Make sure mouse has moved enough
            // Get world position of the mouse when clicked
            Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentMousePosition.z = 0;

            if (Vector3.Distance(_previousMousePosition, currentMousePosition) > mininumSegmentLength)
            {
                // Add a new point to the line
                _currentLine.positionCount += 1;

                // Set the last position (the one just added) to the mouse position
                _currentLine.SetPosition(_currentLine.positionCount - 1, currentMousePosition);

                // Add line points to array for collider
                Vector2[] edgePoints = new Vector2[_currentLine.positionCount];

                for (int i = 0; i < edgePoints.Length; i++)
                {
                    edgePoints[i] = _currentLine.GetPosition(i);
                }

                // Set collider points
                _currentLine.gameObject.GetComponent<EdgeCollider2D>().points = edgePoints;

                // Update the previous mouse position
                _previousMousePosition = currentMousePosition;
            }
        }

        // Stop drawing upon release
        if (Input.GetMouseButtonUp(0))
        {
            // Destroy line if fewer than two positions
            if (_currentLine.positionCount < 2)
            {
                Destroy(_currentLine.gameObject);
            }

            _currentLine = null;
        }

        if (Input.GetMouseButtonDown(1))
        {
            // Since camera starts at (0,0)
            _mouseOffset = Camera.main.transform.position.x + Input.mousePosition.x;
        }

        if (Input.GetMouseButton(1))
        {
            Camera.main.transform.position = new Vector3(-(Input.mousePosition.x - _mouseOffset) / 50, 0, Camera.main.transform.position.z);
        }
    }

    public void SwitchToNormalLine()
    {
        isBouncy = false;
    }

    public void ToggleLineType()
    {
        if (isBouncy)
        {
            isBouncy = false;
            buttonText.text = "Normal";
        }
        else
        {
            isBouncy = true;
            buttonText.text = "Bouncy";
        }
    }

    public void LaunchSledder()
    {
        _currentSledder.GetComponent<Rigidbody2D>().simulated = true;
    }

    public void LoadNewSledder()
    {
        if (_currentSledder == null)
        {
            _currentSledder = Instantiate(sledderPrefab);
        }
    }
}
