using System.Collections.Generic;
using UnityEngine;

public class TransparencyToPlayer : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float checkRadius = .5f;
    [SerializeField] private float playerHeight = 1f;

    private readonly MeshRenderer[] hitMeshes = new MeshRenderer[100];
    private readonly RaycastHit[] rayHits = new RaycastHit[100];
    private static readonly int PlayerBlockCoords = Shader.PropertyToID("_PlayerPos");
    private Camera camera;

    private void FixedUpdate()
    {
        SetBlockPosOnMaterials();
    }

    private void SetBlockPosOnMaterials()
    {
        foreach (MeshRenderer mr in hitMeshes)
        {
            if (mr == null) continue;
            List<Material> materials = new List<Material>();
            mr.GetMaterials(materials);
            foreach (Material m in materials)
            {
                m.SetVector(PlayerBlockCoords, Vector3.positiveInfinity);
            }
        }

        Vector3 playerPos = player.position;
        playerPos.y += playerHeight;
        Vector3 screenPos = camera.WorldToScreenPoint(playerPos);
        screenPos.x /= camera.pixelWidth;
        screenPos.y /= camera.pixelHeight;
        Ray ray = new Ray(transform.position, playerPos - transform.position);
        float distance = Vector3.Distance(transform.position, playerPos);
        int hits = Physics.SphereCastNonAlloc(ray, checkRadius, rayHits, distance);
        for (int i = 0; i < hits; i++)
        {
            if (!rayHits[i].transform.TryGetComponent(out MeshRenderer meshRenderer)) continue;
            List<Material> materials = new List<Material>();
            meshRenderer.GetMaterials(materials);
            foreach (Material m in materials)
            {
                m.SetVector(PlayerBlockCoords, screenPos);
                hitMeshes[i] = meshRenderer;
            }
        }
    }

    private void Awake()
    {
        camera = Camera.main;
    }
}