using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Serialization;

public class DepthOfFieldController : MonoBehaviour
{
    [Serializable]
    public enum RaycastMode
    {
        Raycast = 0,
        SphereCast = 1
    }
    
    [SerializeField] private PostProcessProfile _profile;
    [SerializeField] private VolumeProfile _volumeProfile;
    [SerializeField] private bool overrideAperature = true;
    [SerializeField] private float aperature = 5.4f;
    [SerializeField] private RaycastMode mode;
    [SerializeField] private float sphereCastRadius = 2f;
    [SerializeField] private float maxRange = 200f;
    [SerializeField] private float transitionSpeed = 1f;
    
    private Ray _raycast;
    private RaycastHit _raycastHit;
    private bool _isHit;
    private float _hitDistance;

    private DepthOfField _depthOfField;
    private UnityEngine.Rendering.HighDefinition.DepthOfField _volumeDepthOfField;

    private void Start()
    {
        if (_profile != null)
        {
            _profile.TryGetSettings(out _depthOfField);
        }

        if (_depthOfField != null)
        {
            if (overrideAperature)
            {
                _depthOfField.aperture.overrideState = true;
                _depthOfField.aperture.value = aperature;
            }
        }

        if (_volumeProfile != null)
        {
            _volumeProfile.TryGet(out _volumeDepthOfField);
        }
    }

    private void Update()
    {
        _isHit = false;
        _raycast = new Ray(transform.position, transform.forward * maxRange);
        
        switch (mode)
        {
            case RaycastMode.Raycast:
                if (Physics.Raycast(_raycast, out _raycastHit, maxRange))
                {
                    _isHit = true;
                    _hitDistance = Vector3.Distance(transform.position, _raycastHit.point);
                }
                else
                {
                    if (_hitDistance < maxRange)
                    {
                        _hitDistance++;
                    }
                }
                break;
            case RaycastMode.SphereCast:
                if (Physics.SphereCast(_raycast, sphereCastRadius, out _raycastHit))
                {
                    _isHit = true;
                    _hitDistance = Vector3.Distance(transform.position, _raycastHit.point);
                }
                else
                {
                    if (_hitDistance < maxRange)
                    {
                        _hitDistance++;
                    }
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        
        SetFocus();
    }

    private void SetFocus()
    {
        if (_depthOfField != null)
        {
            _depthOfField.focusDistance.value = Mathf.Lerp(_depthOfField.focusDistance.value, _hitDistance, Time.deltaTime * transitionSpeed);
        }

        if (_volumeDepthOfField != null)
        {
            _volumeDepthOfField.focusDistance.value = Mathf.Lerp(_volumeDepthOfField.focusDistance.value, _hitDistance, Time.deltaTime * transitionSpeed);
        }
    }

    private void OnDrawGizmos()
    {
        if (_isHit)
        {
            Gizmos.DrawSphere(_raycastHit.point, 0.1f);
            Debug.DrawRay(transform.position, transform.forward * _hitDistance);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.forward * maxRange);
        }
    }
}
