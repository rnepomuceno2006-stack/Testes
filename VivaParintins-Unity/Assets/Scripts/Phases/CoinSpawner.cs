// Spawna miçangas (moedas) e itens coletáveis durante o runner.

using System.Collections;
using UnityEngine;

namespace VivaParintins.Phases
{
    public class CoinSpawner : MonoBehaviour
    {
        public GameObject coinPrefab;
        public Transform  spawnRoot;
        public float spawnZ    = 25f;
        public float laneWidth = 2.5f;
        public int   laneCount = 3;
        public float interval  = 0.8f;

        bool _spawning;
        Coroutine _routine;

        public void StartSpawning(PhaseConfig cfg)
        {
            _spawning = true;
            _routine  = StartCoroutine(SpawnLoop(cfg));
        }

        public void StopSpawning()
        {
            _spawning = false;
            if (_routine != null) StopCoroutine(_routine);
        }

        IEnumerator SpawnLoop(PhaseConfig cfg)
        {
            while (_spawning)
            {
                yield return new WaitForSeconds(interval);
                if (!_spawning) break;

                // Gera linha de 1–3 moedas na mesma pista
                int lane  = Random.Range(0, cfg.laneCount);
                float x   = lane * laneWidth - ((cfg.laneCount - 1) * laneWidth / 2f);
                int count = Random.Range(1, 4);

                for (int i = 0; i < count; i++)
                {
                    Vector3 pos = spawnRoot.position + new Vector3(x, 0.5f, spawnZ + i * 1.5f);
                    if (coinPrefab) Instantiate(coinPrefab, pos, Quaternion.identity, spawnRoot);
                }
            }
        }
    }
}
