using UnityEngine;
using System.Collections.Generic;

namespace VivaParintins.Runner
{
    public enum PhaseTheme
    {
        Porto = 0,
        Mercado = 1,
        Tribo = 2,
        Curral = 3,
        Praca = 4,
        Bumbodromo = 5
    }

    public class ProceduralLevelGenerator : MonoBehaviour
    {
        public static ProceduralLevelGenerator Instance { get; private set; }

        [Header("Player Reference")]
        public Transform player;

        [Header("Platform Chunks")]
        [Tooltip("Prefabs de plataforma: chão plano, escada, gap pequeno, gap grande, plataforma flutuante")]
        public GameObject[] platformChunks;
        public float chunkLength = 15f;

        [Header("Obstacles by Phase")]
        [Tooltip("Caixas no porto")]
        public GameObject[] obstaclesPorto;
        [Tooltip("Barracas no mercado")]
        public GameObject[] obstaclesMercado;
        [Tooltip("Tochas na tribo")]
        public GameObject[] obstaclesTribo;
        [Tooltip("Grades no curral")]
        public GameObject[] obstaclesCurral;
        [Tooltip("Estátuas na praça")]
        public GameObject[] obstaclesPraca;
        [Tooltip("Fogos no bumbódromo")]
        public GameObject[] obstaclesBumbodromo;

        [Header("Collectible Patterns")]
        [Tooltip("Prefabs de padrão: arco, linha, zig-zag")]
        public GameObject[] collectiblePatterns;

        [Header("Level Settings")]
        public float levelLength = 300f;
        public float spawnLookAhead = 45f;
        public float destroyDistance = 30f;

        [Header("Landmarks")]
        public GameObject[] landmarkPrefabs;

        [Header("Phase Theme")]
        public PhaseTheme phaseTheme;

        private float nextSpawnX;
        private float totalSpawned;
        private bool landmarkSpawned;
        private readonly List<GameObject> spawnedChunks = new();

        public float LevelLength => levelLength;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        void Start()
        {
            if (RunnerLevelManager.Instance != null)
                phaseTheme = (PhaseTheme)Mathf.Clamp(RunnerLevelManager.Instance.currentPhaseIndex, 0, 5);

            nextSpawnX = player != null ? player.position.x : 0f;

            for (int i = 0; i < 5; i++)
                SpawnNextChunk();
        }

        void Update()
        {
            if (player == null) return;

            while (nextSpawnX < player.position.x + spawnLookAhead && totalSpawned < levelLength)
                SpawnNextChunk();

            if (!landmarkSpawned && totalSpawned >= levelLength)
                SpawnLandmark();

            CleanupBehindPlayer();
        }

        void SpawnNextChunk()
        {
            if (platformChunks == null || platformChunks.Length == 0) return;

            GameObject platformPrefab = platformChunks[Random.Range(0, platformChunks.Length)];
            Vector3 pos = new Vector3(nextSpawnX, 0f, 0f);
            var chunk = Instantiate(platformPrefab, pos, Quaternion.identity, transform);
            spawnedChunks.Add(chunk);

            SpawnObstacleInChunk(pos);
            SpawnCollectibleInChunk(pos);

            nextSpawnX += chunkLength;
            totalSpawned += chunkLength;
        }

        void SpawnObstacleInChunk(Vector3 chunkOrigin)
        {
            var obstacles = GetObstaclesForTheme();
            if (obstacles == null || obstacles.Length == 0) return;
            if (Random.value < 0.6f)
            {
                var prefab = obstacles[Random.Range(0, obstacles.Length)];
                float offsetX = Random.Range(3f, chunkLength - 3f);
                var obj = Instantiate(prefab, chunkOrigin + Vector3.right * offsetX, Quaternion.identity, transform);
                spawnedChunks.Add(obj);
            }
        }

        void SpawnCollectibleInChunk(Vector3 chunkOrigin)
        {
            if (collectiblePatterns == null || collectiblePatterns.Length == 0) return;
            if (Random.value < 0.7f)
            {
                var prefab = collectiblePatterns[Random.Range(0, collectiblePatterns.Length)];
                float offsetX = Random.Range(2f, chunkLength - 2f);
                float offsetY = Random.Range(1f, 2.5f);
                var obj = Instantiate(prefab, chunkOrigin + new Vector3(offsetX, offsetY, 0f), Quaternion.identity, transform);
                spawnedChunks.Add(obj);

                if (RunnerLevelManager.Instance != null)
                {
                    int count = obj.GetComponentsInChildren<CollectibleItem>().Length;
                    RunnerLevelManager.Instance.totalCollectibles += count;
                }
            }
        }

        void SpawnLandmark()
        {
            landmarkSpawned = true;
            int index = (int)phaseTheme;
            if (landmarkPrefabs == null || index >= landmarkPrefabs.Length || landmarkPrefabs[index] == null) return;
            Vector3 pos = new Vector3(nextSpawnX + 5f, 0f, 0f);
            Instantiate(landmarkPrefabs[index], pos, Quaternion.identity);
        }

        void CleanupBehindPlayer()
        {
            for (int i = spawnedChunks.Count - 1; i >= 0; i--)
            {
                if (spawnedChunks[i] == null)
                {
                    spawnedChunks.RemoveAt(i);
                    continue;
                }
                if (spawnedChunks[i].transform.position.x < player.position.x - destroyDistance)
                {
                    Destroy(spawnedChunks[i]);
                    spawnedChunks.RemoveAt(i);
                }
            }
        }

        GameObject[] GetObstaclesForTheme()
        {
            return phaseTheme switch
            {
                PhaseTheme.Porto       => obstaclesPorto,
                PhaseTheme.Mercado     => obstaclesMercado,
                PhaseTheme.Tribo       => obstaclesTribo,
                PhaseTheme.Curral      => obstaclesCurral,
                PhaseTheme.Praca       => obstaclesPraca,
                PhaseTheme.Bumbodromo  => obstaclesBumbodromo,
                _                      => obstaclesPorto
            };
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }
}
