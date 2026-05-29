// Spawna itens consumíveis (Guaraná, Escudo, Ímã, Tacacá) aleatoriamente.

using System.Collections;
using UnityEngine;

namespace VivaParintins.Phases
{
    public class ItemSpawner : MonoBehaviour
    {
        public GameObject guaranaPrefab;
        public GameObject escudoPrefab;
        public GameObject imaPrefab;
        public GameObject tacacaPrefab;

        public Transform spawnRoot;
        public float spawnZ    = 28f;
        public float laneWidth = 2.5f;
        public int   laneCount = 3;
        public float minInterval = 8f;
        public float maxInterval = 15f;

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
                yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
                if (!_spawning) break;

                int lane = Random.Range(0, cfg.laneCount);
                float x  = lane * laneWidth - ((cfg.laneCount - 1) * laneWidth / 2f);
                Vector3 pos = spawnRoot.position + new Vector3(x, 0.5f, spawnZ);

                GameObject prefab = PickRandomItem();
                if (prefab) Instantiate(prefab, pos, Quaternion.identity, spawnRoot);
            }
        }

        GameObject PickRandomItem()
        {
            float r = Random.value;
            if (r < 0.40f) return guaranaPrefab;
            if (r < 0.65f) return escudoPrefab;
            if (r < 0.80f) return imaPrefab;
            return tacacaPrefab;
        }
    }
}
