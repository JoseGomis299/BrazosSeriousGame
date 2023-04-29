using UnityEngine;
using Random = UnityEngine.Random;

namespace BugsGame
{
    public class Bug : MonoBehaviour
    {
        [field: SerializeReference] public int score { get; private set; }
        [field: SerializeReference] public int time { get; private set; }
        [SerializeField] private float initialSpeed;
        private float _speed;
        private Vector3 _moveDirection;
        [field: SerializeReference] public bool badBug { get; private set; }
        private Vector4 _bounds;

        public void SetParameters()
        {
            _moveDirection = new Vector3(Random.Range(-5, 5), transform.position.y, Random.Range(-10, 10)) -
                             transform.position;
            _moveDirection.Normalize();
            Transform child = transform.GetChild(0);
            child.rotation = Quaternion.LookRotation(_moveDirection, Vector3.up);
            child.localScale = Vector3.one * Random.Range(0.8f, 1.2f);
            _speed = initialSpeed + Random.Range(-1f, 3f);
            _bounds = GameManager.instance.bounds * 1.2f;
        }

        private void Update()
        {
            transform.Translate(_moveDirection * (_speed * Time.deltaTime));
            Vector3 pos = transform.position;
            if (pos.x > _bounds.x || pos.x < _bounds.y || pos.z > _bounds.z || pos.z < _bounds.w)
                gameObject.SetActive(false);
        }

    }
}
