using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerCam : MonoBehaviour
{
    CinemachineVirtualCamera cam;
    [SerializeField]
    private Vector2 _mouseSensitivity = new Vector2(1, 1);

    private float _rotationY;
    private float _rotationX;

    bool isInvertX;
    bool isInvertY;

    [SerializeField]
    private Transform _target;

    [SerializeField]
    private float _distanceFromTarget = 3.0f;

    private Vector3 _currentRotation;
    private Vector3 _smoothVelocity = Vector3.zero;

    [SerializeField]
    private float _smoothTime = 0.2f;

    [SerializeField]
    private Vector2 _rotationXMinMax = new Vector2(-40, 40);

 
    void LateUpdate()
    {
        
        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity.x;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity.y;

        if (isInvertX)
        {
            mouseX *= -1;
        }
        if (isInvertY)
        {
            mouseY *= -1;
        }
        
        _rotationY += mouseX;
        _rotationX += mouseY;
        

        // Apply clamping for x rotation 
        _rotationX = Mathf.Clamp(_rotationX, _rotationXMinMax.x, _rotationXMinMax.y);

        Vector3 nextRotation = new Vector3(_rotationX, _rotationY);

        // Apply damping between rotation changes
        _currentRotation = Vector3.SmoothDamp(_currentRotation, nextRotation, ref _smoothVelocity, _smoothTime);
        transform.localEulerAngles = _currentRotation;

        // Substract forward vector of the GameObject to point its forward vector to the target
        transform.position = _target.position - transform.forward * _distanceFromTarget;
    }

    public void ChangeVelocityX(float _speed)
    {
        _mouseSensitivity.x = _speed;
    }
    public void ChangeVelocityY(float _speed)
    {
        _mouseSensitivity.y = _speed;
    }
    public void ChangeInvertX(bool _invert)
    {
        isInvertX = _invert;
    }
    public void ChangeInvertY(bool _invert)
    {
        isInvertY = _invert;
    }
}
