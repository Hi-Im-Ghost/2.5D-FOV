using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody _rb;
    private Camera _camera;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float rotationSpeed = 10f;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _camera = Camera.main;
        InputManager.PlayerMovementEvent += MoveCharacter;
        InputManager.PlayerRotationEvent += HandleRotation;
    }

    private void OnDestroy()
    {
        InputManager.PlayerMovementEvent -= MoveCharacter;
        InputManager.PlayerRotationEvent -= HandleRotation;
    }

    private void MoveCharacter(Vector3 direction)
    {
        _rb.MovePosition(_rb.position + (direction * moveSpeed) * Time.fixedDeltaTime);       
    }

    private void HandleRotation(float rotationValue)
    {
        Vector3 rotation = Vector3.up * rotationValue * rotationSpeed * Time.fixedDeltaTime;
        _rb.MoveRotation(_rb.rotation * Quaternion.Euler(rotation));
    }
    private void HandleMouseRotation()
    {
        
        Vector3 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = new Vector3(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y, _camera.transform.position.y);
        transform.LookAt(direction); 
    }


}
