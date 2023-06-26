using BugsGame.ProjectUtils.Helpers;
using BugsGame.ProjectUtils.ObjectPooling;
using UnityEngine;

namespace BugsGame
{
    public class NetController : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private float radius;
        [SerializeField] private float targetSpeed;
        [SerializeField] private AudioClip netSound;
        [SerializeField] private Transform UIcanvas;

        private GameObject _model;
        private bool _catching;
        private Vector2 _bounds;

        void Start()
        {
            _model = transform.GetChild(0).gameObject;
            _model.SetActive(false);

            target.localScale = Vector3.one * radius * 2;

            _bounds.x = GameManager.instance.bounds.z;
            _bounds.y = GameManager.instance.bounds.w;
        }

        void Update()
        {
            if (_catching) return;

            if (ArmInput.GetSignalDown(ArmInput.Signal.RTriceps))
            {
                Catch();
            }

            float movement = ArmInput.GetSignal(ArmInput.Signal.LTriceps) +
                             ArmInput.GetSignal(ArmInput.Signal.LBiceps) * -1;
            if (movement != 0)
            {
                if (movement > 0 && target.position.z >= _bounds.x) movement = 0;
                if (movement < 0 && target.position.z <= _bounds.y) movement = 0;
                target.Translate(Vector3.up * (movement * (targetSpeed * Time.deltaTime)));
            }
        }

        private async void Catch()
        {
            _catching = true;
            _model.transform.position = new Vector3(target.position.x, _model.transform.position.y, target.position.z-17);
            _model.SetActive(true);
            AudioManager.Instance.PlaySound(netSound);
            await _model.transform.DoRotateAsync(Quaternion.Euler(90, 0, 0), 0.2f);

            var bugs = Physics.OverlapSphere(target.position, radius, layerMask);
            foreach (var bugCollider in bugs)
            {
                var bug = bugCollider.GetComponent<Bug>();
                GameManager.instance.AddScore(bug.score);
                if (bug.time != 0) GameManager.instance.AddTime(bug.time);
                AudioManager.Instance.PlaySound(bug.catchSound);
                bug.gameObject.SetActive(false);
                GameObject addition = ObjectPool.Instance.InstantiateFromPoolIndex(3,
                    RectTransformUtility.WorldToScreenPoint(Helpers.Camera, bug.transform.position),
                    Quaternion.identity, false);
                addition.GetComponent<ScoreAddition>().SetStats(bug.score);
                addition.transform.SetParent(UIcanvas);
            }

            await Helpers.Camera.transform.DoShakeAsync(0.1f, 0.1f, true);


            await _model.transform.DoRotateAsync(Quaternion.Euler(0, 0, 0), 0.3f);
            _model.SetActive(false);
            _catching = false;
        }
    }
}

