// Coloque junto ao PhaseRunner. Spawna obstáculos temáticos conforme a fase.
// Cada fase tem obstáculos diferentes (redes, barracas, mesas, etc.).

using System.Collections;
using UnityEngine;

namespace VivaParintins.Phases
{
    public class ObstacleSpawner : MonoBehaviour
    {
        [Header("Prefabs de obstáculo por fase")]
        public GameObject[] phase0Obstacles; // Porto: redes, malas
        public GameObject[] phase1Obstacles; // Feira: barracas, caixas
        public GameObject[] phase2Obstacles; // Mercado: carrinhos, pilhas
        public GameObject[] phase3Obstacles; // Orla: barcos, postes
        public GameObject[] phase4Obstacles; // Comunas: mesas, cadeiras
        public GameObject[] phase5Obstacles; // Curral: grades, bandeiras
        public GameObject[] phase6Obstacles; // Praça: torcedores opostos
        public GameObject[] phase7Obstacles; // Bumbódromo: fogos, adereços

        [Header("Spawn")]
        public Transform spawnRoot;
        public float spawnZ = 30f;
        public float laneWidth = 2.5f;
        public int   laneCount = 3;

        PhaseConfig _cfg;
        bool _spawning;
        bool _beatSync;
        Coroutine _routine;

        public void StartSpawning(PhaseConfig cfg)
        {
            _cfg = cfg;
            _spawning = true;
            _routine = StartCoroutine(SpawnLoop());
        }

        public void StopSpawning()
        {
            _spawning = false;
            if (_routine != null) StopCoroutine(_routine);
        }

        public void EnableBeatSync() => _beatSync = true;

        IEnumerator SpawnLoop()
        {
            GameObject[] pool = GetPool((int)_cfg.id);
            float interval = _beatSync ? 0.46f : Random.Range(1.2f, 2.0f);

            while (_spawning)
            {
                if (!_beatSync)
                    interval = Mathf.Lerp(2.0f, 0.8f, 1f - (_cfg.timeLimitSeconds / 90f));

                yield return new WaitForSeconds(interval);
                if (!_spawning) break;

                int lane = Random.Range(0, laneCount);
                float x  = lane * laneWidth - ((laneCount - 1) * laneWidth / 2f);
                Vector3 pos = spawnRoot.position + new Vector3(x, 0, spawnZ);

                if (pool != null && pool.Length > 0)
                {
                    var prefab = pool[Random.Range(0, pool.Length)];
                    if (prefab) Instantiate(prefab, pos, Quaternion.identity, spawnRoot);
                }
            }
        }

        GameObject[] GetPool(int phaseIndex) => phaseIndex switch
        {
            0 => phase0Obstacles,
            1 => phase1Obstacles,
            2 => phase2Obstacles,
            3 => phase3Obstacles,
            4 => phase4Obstacles,
            5 => phase5Obstacles,
            6 => phase6Obstacles,
            7 => phase7Obstacles,
            _ => phase0Obstacles
        };
    }
}
