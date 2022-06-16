using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SettingsCameraPlayer : MonoBehaviour
{
    CinemachineFreeLook freeLookCamera;
    private void Awake()
    {
        freeLookCamera = GetComponent<CinemachineFreeLook>();
    }

    //Velocity
    public void ChangeVelocityX(float speed)
    {
        freeLookCamera.m_XAxis.m_MaxSpeed = speed;
    }
    public void ChangeVelocityY(float speed)
    {
        freeLookCamera.m_YAxis.m_MaxSpeed = speed;
    }

    //Inverted
    public void SetInvertedX(bool _value)
    {
        freeLookCamera.m_XAxis.m_InvertInput = _value;
    }
    public void SetInvertedY(bool _value)
    {
        freeLookCamera.m_YAxis.m_InvertInput = _value;
    }

    #region Getters
    public float GetVelocityX()
    {
        return freeLookCamera.m_XAxis.m_MaxSpeed;
    }
    public float GetVelocityY()
    {
        return freeLookCamera.m_YAxis.m_MaxSpeed;
    }
    public bool IsInvertedX()
    {
        return freeLookCamera.m_XAxis.m_InvertInput;
    }
    public bool IsInvertedY()
    {
        return freeLookCamera.m_YAxis.m_InvertInput;
    }
    #endregion
}
