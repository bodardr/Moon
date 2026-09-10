using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class ReactorParticleSpawner : MonoBehaviour
{
    [FormerlySerializedAs("cellDensity")]
    [Tooltip("To be read as the amount of cells per unit squared")]
    public float particleDensity;
    public Vector2Int dimensions;

    public int initialCandidateSamples;
    [FormerlySerializedAs("cellsPerFrame")]
    public int particlesPerFrame;

    [SerializeField] private Transform particleParent;
    [SerializeField] private GameObject particlePrefab;

    [Header("Walls")]
    [SerializeField] private float wallThickness;
    [SerializeField] private Transform[] walls;

    private Vector2[] particlePositions;
    private List<GameObject> spawnedParticles = new();


    public void CreateWalls()
    {
        var wallSizeOffset = new Vector2Int(1, 1);
        var wallDimensions = dimensions + wallSizeOffset;

        for (var i = 0; i < walls.Length; i++)
        {
            var wall = walls[i];

            var size = new Vector2(wallThickness, i % 2 == 0 ? wallDimensions.x : wallDimensions.y);
            var pos = new Vector2((i > walls.Length / 2 - 1 ? size.y : -size.y) * 0.5f, 0);

            if (i % 2 == 0)
            {
                (size.x, size.y) = (size.y, size.x);
                (pos.x, pos.y) = (pos.y, pos.x);
            }

            wall.localScale = size;
            wall.localPosition = pos + (Vector2)dimensions * 0.5f;
        }
    }

    public async Awaitable SpawnParticles()
    {
        //Get particle positions.
        var totalCellCount = (int)(particleDensity * dimensions.x * dimensions.y);
        particlePositions = new Vector2[totalCellCount];

        var candidateSampleCount = initialCandidateSamples;
        var attemptsPerFrame = particlesPerFrame;
        for (int i = 0; i < totalCellCount; i++)
        {
            if (--attemptsPerFrame <= 0)
            {
                await Awaitable.NextFrameAsync();
                attemptsPerFrame = particlesPerFrame;
            }

            particlePositions[i] = GenerateNewCell(particlePositions, i, candidateSampleCount);
        }
        
        //Spawn particles
        foreach (var particlePosition in particlePositions)
        {
            var particle = Instantiate(particlePrefab, particlePosition, Quaternion.identity, particleParent);
            spawnedParticles.Add(particle);
        }
    }

    private Vector2 GenerateNewCell(Vector2[] cells, int cellCount, int candidateCount)
    {
        var bestCell = Vector2.zero;
        var bestDistSqr = float.MaxValue;
        for (int i = 0; i < candidateCount; i++)
        {
            var pos = new Vector2(Random.value * dimensions.x, Random.value * dimensions.y);

            var minDistance = float.MaxValue;
            for (int j = 0; j < cellCount; j++)
            {
                var cell = cells[j];
                var distSqr = (pos - cell).sqrMagnitude;
                if (distSqr < minDistance)
                    minDistance = distSqr;
            }

            if (i == 0 || minDistance > bestDistSqr)
            {
                bestCell = pos;
                bestDistSqr = minDistance;
            }
        }

        return bestCell;
    }

    private void OnDrawGizmos()
    {
        if (particlePositions == null)
            return;
        
        Gizmos.color = Color.cyan;
        foreach (var cell in particlePositions)
            Gizmos.DrawWireSphere(cell, 0.2f);
    }

}
