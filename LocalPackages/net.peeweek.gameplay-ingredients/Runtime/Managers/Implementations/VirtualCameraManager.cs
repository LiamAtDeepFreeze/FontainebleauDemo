using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace GameplayIngredients
{
    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(CinemachineBrain))]
    [ManagerDefaultPrefab("VirtualCameraManager")]
    public class VirtualCameraManager : Manager
    {
        public Camera Camera { get; private set; }
        public CinemachineBrain Brain { get; private set; }

        private void Awake()
        {
            Camera = GetComponent<Camera>();
            Brain = GetComponent<CinemachineBrain>();
        }

        private void Start()
        {
            Invoke(nameof(SetPosition), 1f);
        }

        private void SetPosition()
        {
            gameObject.transform.position = new Vector3(-5.86f, 0.68f, 4.6f);
            gameObject.transform.eulerAngles = new Vector3(-6.152f, -4.022f, 0f);
        }

        private void Update()
        {
            gameObject.transform.position = new Vector3(-5.86f, 1.68f, 4.6f);
            gameObject.transform.eulerAngles = new Vector3(-6.152f, -4.022f, 0f);
        }
    }
}
