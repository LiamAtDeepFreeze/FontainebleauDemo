using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Serialization;

public class DepthOfFieldController : MonoBehaviour
{
    [SerializeField] private PostProcessProfile _profile;
    [SerializeField] private bool overrideAperature = true;
    [SerializeField] private float aperature = 5.4f;
    [SerializeField] private float maxRange = 200f;
    [SerializeField] private float transitionSpeed = 1f;
    
    private Ray _raycast;
    private RaycastHit _raycastHit;
    private bool _isHit;
    private float _hitDistance;

    private DepthOfField _depthOfField;

    private void Start()
    {
        _profile.TryGetSettings(out _depthOfField);

        if (_depthOfField != null)
        {
            if (overrideAperature)
            {
                _depthOfField.aperture.overrideState = true;
                _depthOfField.aperture.value = aperature;
            }
        }
    }

    private void Update()
    {
        _raycast = new Ray(transform.position, transform.forward * maxRange);
        _isHit = false;
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
        
        SetFocus();
    }

    private void SetFocus()
    {
        if (_depthOfField == null)
        {
            return;
        }
        
        _depthOfField.focusDistance.value = Mathf.Lerp(_depthOfField.focusDistance.value, _hitDistance, Time.deltaTime * transitionSpeed);
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
