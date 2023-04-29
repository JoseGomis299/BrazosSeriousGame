using BugsGame.ProjectUtils.ObjectPooling;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BugsGame
{
    public class BugSpawner : MonoBehaviour
    {
        [Header("Prefabs")] [SerializeField] private GameObject bugPrefab;
        [SerializeField] private GameObject badBugPrefab;
        [SerializeField] private GameObject goldenBugPrefab;

        [Header("Spawner stats")] [SerializeField]
        private float spawnRate;

        private float _timer;
        private Vector4 _bounds;

        private void Start()
        {
            _bounds = GameManager.instance.bounds;
        }

        void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= spawnRate)
            {
                GameObject bugGameObject =
                    ObjectPool.Instance.InstantiateFromPool(GetPrefab(), GetSpawnPoint(), Quaternion.identity, false);
                Bug bug = bugGameObject.GetComponent<Bug>();
                bug.SetParameters();
                _timer = 0;
            }
        }

        private Vector3 GetSpawnPoint()
        {
            float randomValue = Random.value;

            Vector3 position = Vector3.zero;
            if (randomValue < 0.25 && randomValue >= 0f) position.z = _bounds.z + transform.localScale.z * 2;
            else if (randomValue < 0.5 && randomValue >= 0.25f) position.z = _bounds.w - transform.localScale.z * 2;
            else if (randomValue < 0.75 && randomValue >= 0.5f) position.x = _bounds.y - transform.localScale.x * 2;
            else if (randomValue >= 0.75f) position.x = _bounds.x + transform.localScale.x * 2;

            Vector3 position2 = Vector3.zero;
            if (position.x != 0) position2.z = Random.Range(_bounds.w, _bounds.z);
            if (position.z != 0) position2.x = Random.Range(_bounds.y, _bounds.x);

            position += position2;
            return position;
        }

        private GameObject GetPrefab()
        {
            float randomValue = Random.value;
            if (randomValue < 0.1 && randomValue >= 0f) return goldenBugPrefab;
            if (randomValue < 0.85 && randomValue >= 0.1f) return bugPrefab;
            return badBugPrefab;
        }
    }
}
