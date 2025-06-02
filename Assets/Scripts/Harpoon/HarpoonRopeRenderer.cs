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
        
        [SerializeField] private Transform hookPoint;
        [SerializeField] private Transform cableStartPoint;

        private LineRenderer lineRenderer;
        private readonly Vector3[] positions = new Vector3[2];

        private void Awake()
        {
            UpdateLinePositions();
        }

        private void Update()
        {
            UpdateLinePositions();
        }

        private void OnValidate()
        {
            UpdateLinePositions();
        }

        private void UpdateLinePositions()
        {
            positions[0] = cableStartPoint.position;
            positions[1] = hookPoint.position;
            
            if (lineRenderer == null)
                lineRenderer = GetComponent<LineRenderer>();
                    
            lineRenderer.SetPositions(positions);
        }
    }
}
