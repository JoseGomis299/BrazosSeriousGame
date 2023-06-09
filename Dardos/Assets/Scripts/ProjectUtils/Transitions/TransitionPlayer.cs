using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Transitions
{
    [ExecuteAlways, Icon("Assets/Scripts/ProjectUtils/Transitions/Editor/Icons/TransitionPlayerIcon.png"), AddComponentMenu("Transitions/Transition Player")]
    public class TransitionPlayer : MonoBehaviour
    {
        private enum TimeScale
        {
            Scaled,
            Unscaled
        }
        [Header("TransitionPlayer Parameters")]
        [SerializeField] private TimeScale timeScale;
        private SpriteRenderer _spriteRenderer;
        private Image _image;

        [Header("Transition List")]
        [SerializeField] private List<Transition> transitions = new();
    
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _image = GetComponent<Image>();
            if (_spriteRenderer == null) _spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
            transitions ??= new List<Transition>();
        }

        private void Start()
        {
            if(!Application.IsPlaying(gameObject)) return;

            foreach (var transition in transitions.Where(transition => transition.playOnStart))
            {
                PlayTransition(transition);
            }
        }


#if UNITY_EDITOR
        private void Update()
        {
            if(Application.IsPlaying(gameObject)) return;
            System.Random random = new System.Random();
            for (int i = 1; i < transitions.Count; i++)
            {
                if(transitions[i-1].id == transitions[i].id) {transitions[i].id = random.Next();}
            }
        }
        
        private void AddKeyFrame(int index)
        {
            KeyFrame keyFrame = new KeyFrame();
            keyFrame.SetValues(transform, _spriteRenderer, _image);
            transitions[index].keyFrames.Add(keyFrame);
        }
    
        public void AddKeyFrame(Transition transition)
        {
            AddKeyFrame(GetTransitionIndex(transition));
        }
    
        private int GetTransitionIndex(Transition transition)
        {
            for (int i = 0; i < transitions.Count; i++)
            {
                if (transitions[i] == transition) return i;
            }
            return -1;
        }
    
#endif
    
        private void SetTransformToFrame(KeyFrame targetFrame)
        {
            var myTransform = transform;
            myTransform.position = targetFrame.position;
            myTransform.eulerAngles = targetFrame.rotation;
            myTransform.localScale = targetFrame.scale;

            if (_spriteRenderer != null) _spriteRenderer.color = targetFrame.color;
            else if (_image != null) _image.color = targetFrame.color;
        }
        public async Task PlayTransitionAsync(int index)
        { 
            await PlayTransitionAsync(transitions[index]);
        }
    
        public async Task PlayTransitionAsync(Transition transition)
        {
#if UNITY_EDITOR
            if (!Application.IsPlaying(gameObject))
            {
                await PlayTransitionFixedAsync(transition);
                return;
            }
#endif
            switch (timeScale)
            {
                case TimeScale.Scaled: 
                    await PlayTransitionScaledAsync(transition);
                    break;
                case TimeScale.Unscaled:
                    await PlayTransitionUnscaledAsync(transition);
                    break;
            }
        }
   
        public void PlayTransition(int index)
        { 
            PlayTransition(transitions[index]);
        }
    
        public void PlayTransition(Transition transition)
        {
            switch (timeScale)
            {
                case TimeScale.Scaled: 
                    PlayTransitionScaled(transition);
                    break;
                case TimeScale.Unscaled:
                    PlayTransitionUnscaled(transition);
                    break;
            }
        }

        private async Task PlayTransitionUnscaledAsync(Transition transition)
        {
            for (int i = 0; i < transition.keyFrames.Count-1; i++)
            {
                await PlayTransitionUnscaledAsync(transition.keyFrames[i], transition.keyFrames[i+1], transition.animationCurve, transition.duration);
            }
        }
    
        private async Task PlayTransitionUnscaledAsync(KeyFrame initialFrame, KeyFrame targetFrame, AnimationCurve animationCurve, float duration)
        {
            float timer = Time.unscaledDeltaTime;
            KeyFrame scaleDelta = targetFrame - initialFrame;

            while (timer < duration)
            {
                SetTransformToFrame(initialFrame + scaleDelta * (animationCurve.Evaluate(timer/duration)));
                await Task.Yield();
                timer += Time.unscaledDeltaTime;
            }

            SetTransformToFrame(initialFrame + scaleDelta);
        } 
    
        private async Task PlayTransitionScaledAsync(Transition transition)
        {
            for (int i = 0; i < transition.keyFrames.Count-1; i++)
            {
                await PlayTransitionScaledAsync(transition.keyFrames[i], transition.keyFrames[i+1], transition.animationCurve, transition.duration/(transition.keyFrames.Count-1));
            }
        }
    
        private async Task PlayTransitionScaledAsync(KeyFrame initialFrame, KeyFrame targetFrame, AnimationCurve animationCurve, float duration)
        {
            float timer = Time.deltaTime;
            KeyFrame scaleDelta = targetFrame - initialFrame;

            while (timer < duration)
            {
                SetTransformToFrame(initialFrame + scaleDelta * (animationCurve.Evaluate(timer/duration)));
                await Task.Yield();
                timer += Time.deltaTime;
            }

            SetTransformToFrame(initialFrame + scaleDelta);
        } 
    
        private async Task PlayTransitionFixedAsync(Transition transition)
        {
            for (int i = 0; i < transition.keyFrames.Count-1; i++)
            {
                await PlayTransitionFixedAsync(transition.keyFrames[i], transition.keyFrames[i+1], transition.animationCurve, transition.duration);
            }
        }
    
        private async Task PlayTransitionFixedAsync(KeyFrame initialFrame, KeyFrame targetFrame, AnimationCurve animationCurve, float duration)
        {
            float timer = Time.fixedDeltaTime;
            KeyFrame scaleDelta = targetFrame - initialFrame;
        

            while (timer < duration)
            {
                SetTransformToFrame(initialFrame + scaleDelta * (animationCurve.Evaluate(timer/duration)));
                await Task.Yield();
                timer += Time.fixedDeltaTime;
            }

            SetTransformToFrame(initialFrame + scaleDelta);
        }
    
        private void PlayTransitionUnscaled(Transition transition)
        {
            StartCoroutine(PlayTransitionUnscaledEnumerator(transition));
        }
        
        private IEnumerator PlayTransitionUnscaledEnumerator(Transition transition)
        {
            for (int i = 0; i < transition.keyFrames.Count-1; i++)
            {
                yield return StartCoroutine(PlayTransitionUnscaled(transition.keyFrames[i], transition.keyFrames[i+1], transition.animationCurve, transition.duration));
            }
        }
    
        private IEnumerator PlayTransitionUnscaled(KeyFrame initialFrame, KeyFrame targetFrame, AnimationCurve animationCurve, float duration)
        {
            float timer = Time.unscaledDeltaTime;
            KeyFrame scaleDelta = targetFrame - initialFrame;

            while (timer < duration)
            {
                SetTransformToFrame(initialFrame + scaleDelta * (animationCurve.Evaluate(timer/duration)));
                yield return null;
                timer += Time.unscaledDeltaTime;
            }

            SetTransformToFrame(initialFrame + scaleDelta);
        } 
    
        private void PlayTransitionScaled(Transition transition)
        {
            StartCoroutine(PlayTransitionScaledEnumerator(transition));
        }
        
        private IEnumerator PlayTransitionScaledEnumerator(Transition transition)
        {
            for (int i = 0; i < transition.keyFrames.Count-1; i++)
            {
                yield return StartCoroutine(PlayTransitionScaled(transition.keyFrames[i], transition.keyFrames[i+1], transition.animationCurve, transition.duration));
            }
        }
    
        private IEnumerator PlayTransitionScaled(KeyFrame initialFrame, KeyFrame targetFrame, AnimationCurve animationCurve, float duration)
        {
            float timer = Time.deltaTime;
            KeyFrame scaleDelta = targetFrame - initialFrame;

            while (timer < duration)
            {
                SetTransformToFrame(initialFrame + scaleDelta * (animationCurve.Evaluate(timer/duration)));
                yield return null;
                timer += Time.deltaTime;
            }

            SetTransformToFrame(initialFrame + scaleDelta);
        } 
    
    }
}
