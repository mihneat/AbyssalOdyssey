using System;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Scripts
{
    [RequireComponent(typeof(LineRenderer))]
    public class HarpoonRopeRenderer : MonoBehaviour
    {
        [SerializeField] private Vector3 startOffset;
        [SerializeField] private float harpoonOffset;

        [SerializeField] private Transform harpoon;

        private LineRenderer lineRenderer;
        private readonly Vector3[] positions = new Vector3[2];

        private Transform transformRef;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();

            transformRef = transform;
            positions[0] = transformRef.position + transformRef.right * startOffset.x + transformRef.up * startOffset.y + transformRef.forward * startOffset.z;
            positions[1] = harpoon.position + harpoon.forward * harpoonOffset;
            
            lineRenderer.SetPositions(positions);
        }

        private void Update()
        {
            positions[0] = transformRef.position + transformRef.right * startOffset.x + transformRef.up * startOffset.y + transformRef.forward * startOffset.z;
            positions[1] = harpoon.position + harpoon.forward * harpoonOffset;
            
            lineRenderer.SetPositions(positions);
        }

        private void OnValidate()
        {
            if (harpoon == null)
                return;

            var transform1 = transform;
            positions[0] = transform1.position + transform1.right * startOffset.x + transform1.up * startOffset.y + transform1.forward * startOffset.z;
            positions[1] = harpoon.position + harpoon.forward * harpoonOffset;
            
            GetComponent<LineRenderer>().SetPositions(positions);
        }
    }
}
