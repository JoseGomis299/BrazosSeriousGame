using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace ProjectUtils.Helpers
{
    public static class Helpers 
    {
        private static Camera _camera;
        public static Camera Camera
        {
            get
            {
                if(_camera == null) _camera = Camera.main;
                return _camera;
            }
        }
    
        private static PointerEventData _eventDataCurrentPosition;
        private static List<RaycastResult> _results;

        public static bool PointerIsOverUi()
        {
            _eventDataCurrentPosition = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
            _results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(_eventDataCurrentPosition, _results);
            return _results.Count > 0;
        }

        public static Vector2 GetWorldPositionOfCanvasElement(RectTransform element)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(element, element.position, Camera, out var result);
            return result;
        }

        public static void DeleteChildren(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                Object.Destroy(child.gameObject);
            }
        }

        /// <summary>Returns the angle in degrees between this position and the mouse position</summary>
        public static float GetAngleToPointer(this Vector3 position)
        {
            Vector3 attackDirection = Input.mousePosition;
            attackDirection = Camera.ScreenToWorldPoint(attackDirection);
            attackDirection.z = position.z;
            attackDirection = (attackDirection-position).normalized;

            float angle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;
            while (angle<0) angle += 360;

            return angle;
        }

        /// <summary>
        /// <para>Scales the object to a target scale in a determined time</para>
        /// <param name="targetScale">The target scale</param>
        /// <param name="time">The duration in seconds of the scaling effect</param>
        /// </summary>
        public static void DoScale(this Transform transform, Vector3 targetScale, float time)  =>  CoroutineController.Start(DoScaleEnumerator(transform, targetScale, time));
        private static IEnumerator DoScaleEnumerator(Transform transform, Vector3 targetScale, float time)
        {
            float timer = Time.unscaledDeltaTime;
            Vector3 initialScale = transform.localScale;
            Vector3 scaleDelta = targetScale - initialScale;

            while (timer < time)
            {
                transform.localScale = initialScale + scaleDelta * (timer/time);
                yield return null;
                timer += Time.unscaledDeltaTime;
            }

            transform.localScale = targetScale;
        }

        /// <summary>
        /// <para>Scales the object to a target scale in a determined time</para>
        /// <param name="targetScale">The target scale</param>
        /// <param name="time">The duration in seconds of the scaling effect</param>
        /// </summary>
        public static async Task DoScaleAsync(this Transform transform, Vector3 targetScale, float time)
        {
            float timer = Time.unscaledDeltaTime;
            Vector3 initialScale = transform.localScale;
            Vector3 scaleDelta = targetScale - initialScale;

            while (timer < time)
            {
                transform.localScale = initialScale + scaleDelta * (timer/time);
                await Task.Yield();
                timer += Time.unscaledDeltaTime;
            }

            transform.localScale = targetScale;
        } 
        
        /// <summary>
        /// <para>Moves the object to a target position in a determined time</para>
        /// <param name="targetPosition">The final position</param>
        /// <param name="time">The duration in seconds of the moving effect</param>
        /// </summary>
        public static void DoMove(this Transform transform, Vector3 targetPosition, float time)  =>  CoroutineController.Start(DoScaleEnumerator(transform, targetPosition, time));
        private static IEnumerator DoMoveEnumerator(Transform transform, Vector3 targetPosition, float time)
        {
            float timer = Time.unscaledDeltaTime;
            Vector3 initialPosition = transform.position;
            Vector3 moveDelta = targetPosition - initialPosition;

            while (timer < time)
            {
                transform.position = initialPosition + moveDelta * (timer/time);
                yield return null;
                timer += Time.unscaledDeltaTime;
            }

            transform.position = targetPosition;
        }

        /// <summary>
        /// <para>Moves the object to a target position in a determined time</para>
        /// <param name="targetPosition">The final position</param>
        /// <param name="time">The duration in seconds of the moving effect</param>
        /// </summary>
        public static async Task DoMoveAsync(this Transform transform, Vector3 targetPosition, float time)
        {
            float timer = Time.unscaledDeltaTime;
            Vector3 initialPosition = transform.position;
            Vector3 moveDelta = targetPosition - initialPosition;

            while (timer < time)
            {
                transform.position = initialPosition + moveDelta * (timer/time);
                await Task.Yield();
                timer += Time.unscaledDeltaTime;
            }

            transform.position = targetPosition;
        } 
        
        /// <summary>
        /// <para>Moves the object, making a shake movement with a certain magnitude in a determined time</para>
        /// <param name="magnitude">The magnitude of the movement</param>
        /// <param name="time">The duration in seconds of the shaking effect</param>
        /// <param name="moveZ">Determines if the object moves in the z axis</param>
        /// </summary>
        public static void DoShake(this Transform transform, float magnitude, float time, bool moveZ = false) => CoroutineController.Start(DoShakeEnumerator(transform, magnitude, time, moveZ));
        private static IEnumerator DoShakeEnumerator(Transform transform, float magnitude, float time, bool moveZ)
        {
            float duration = Time.unscaledTime + time;
            Vector3 initialPosition = transform.position;
            Vector3 newPosition = initialPosition;

            while (Time.unscaledTime < duration)
            {
                newPosition.x = initialPosition.x + Random.value * magnitude;
                newPosition.y = initialPosition.y + Random.value * magnitude;
                if(moveZ) newPosition.z = initialPosition.z + Random.value * magnitude;
                transform.position = newPosition;
                yield return null;
            }
            transform.position = initialPosition;
        }
        
        /// <summary>
        /// <para>Moves the object, making a shake movement with a certain magnitude in a determined time</para>
        /// <param name="magnitude">The magnitude of the movement</param>
        /// <param name="time">The duration in seconds of the shaking effect</param>
        /// <param name="moveZ">Determines if the object moves in the z axis</param>
        /// </summary>
        public static async Task DoShakeAsync(this Transform transform, float magnitude, float time, bool moveZ = false)
        {
            float duration = Time.unscaledTime + time;
            Vector3 initialPosition = transform.position;
            Vector3 newPosition = initialPosition;

            while (Time.unscaledTime < duration)
            {
                newPosition.x = initialPosition.x + Random.value * magnitude;
                newPosition.y = initialPosition.y + Random.value * magnitude;
                if(moveZ) newPosition.z = initialPosition.z + Random.value * magnitude;
                transform.position = newPosition;
                await Task.Yield();
            }
            transform.position = initialPosition;
        }

        /// <summary>
        /// <para>Rotates the object to a target rotation in a determined time</para>
        /// <param name="targetRotation">The final rotation</param>
        /// <param name="time">The duration in seconds of the rotation effect</param>
        /// </summary>
        public static void DoRotate(this Transform transform, Quaternion targetRotation, float time) => CoroutineController.Start(doRotateEnumerator(transform, targetRotation, time));
        private static IEnumerator doRotateEnumerator(Transform transform, Quaternion targetRotation, float time)
        {
            float timer = Time.unscaledDeltaTime;
            Quaternion rotation = transform.rotation;

            while (timer < time)
            {
                transform.rotation = Quaternion.Slerp(rotation, targetRotation, timer/time);
                yield return null;
                timer += Time.unscaledDeltaTime;
            }

            transform.rotation = targetRotation;
        }
        
        /// <summary>
        /// <para>Rotates the object to a target rotation in a determined time</para>
        /// <param name="targetRotation">The final rotation</param>
        /// <param name="time">The duration in seconds of the rotation effect</param>
        /// </summary>
        public static async Task DoRotateAsync(this Transform transform, Quaternion targetRotation, float time)
        {
            float timer = Time.unscaledDeltaTime;
            Quaternion rotation = transform.rotation;

            while (timer < time)
            {
                transform.rotation = Quaternion.Slerp(rotation, targetRotation, timer/time);
                await Task.Yield();
                timer += Time.unscaledDeltaTime;
            }

            transform.rotation = targetRotation;
        }

        private static Dictionary<int, bool> _blinking;
        public static bool IsBlinking(this SpriteRenderer spriteRenderer)
        {
            _blinking ??= new Dictionary<int, bool>();
        
            int id = spriteRenderer.GetHashCode();
            if (!_blinking.ContainsKey(id)) _blinking.Add(id , false);
        
            return _blinking[id];
        }
        public static bool IsBlinking(this Image spriteRenderer)
        {
            _blinking ??= new Dictionary<int, bool>();
        
            int id = spriteRenderer.GetHashCode();
            if (!_blinking.ContainsKey(id)) _blinking.Add(id , false);
        
            return _blinking[id];
        } 
    
        /// <summary>
        /// <para>Makes a blinking effect to the object</para>
        /// <param name="duration">The duration in seconds of the blinking effect</param>
        /// <param name="ticks">The number of times you want the object to blink</param>
        /// <param name="targetColor">The color to change in every blink, normally transparent or white</param>
        /// </summary>
        public static void DoBlink(this SpriteRenderer spriteRenderer, float duration, int ticks, Color targetColor) => CoroutineController.Start(DoBlinkEnumerator(spriteRenderer, duration, ticks, targetColor));
        private static IEnumerator DoBlinkEnumerator(SpriteRenderer spriteRenderer, float duration, int ticks, Color targetColor)
        {
            if (ticks <= 0) yield break;
        
            _blinking ??= new Dictionary<int, bool>();
            int id = spriteRenderer.GetHashCode();
            if(!_blinking.ContainsKey(id)) _blinking.Add( id , true);
            else _blinking[id] = true;

            float timer = 0;
            Color initialColor = spriteRenderer.color;
            WaitForSeconds waitForSeconds = new WaitForSeconds(duration / ticks/2);
        
            while (timer<duration)
            {
                initialColor = spriteRenderer.color;
                spriteRenderer.color = targetColor;
                yield return waitForSeconds;
                spriteRenderer.color = initialColor;
                yield return waitForSeconds;
                timer += duration / ticks;
            }

            spriteRenderer.color = initialColor;
            _blinking[id] = false;
        }
        
        /// <summary>
        /// <para>Makes a blinking effect to the object</para>
        /// <param name="duration">The duration in seconds of the blinking effect</param>
        /// <param name="ticks">The number of times you want the object to blink</param>
        /// <param name="targetColor">The color to change in every blink, normally transparent or white</param>
        /// </summary>
        public static async Task DoBlinkAsync(this SpriteRenderer spriteRenderer, float duration, int ticks, Color targetColor)
        {
            if (ticks <= 0) return;

            float timer = 0;
            Color initialColor = spriteRenderer.color;
            int waitForSeconds = (int)(duration/ticks/2*1000);
        
            while (timer<duration)
            {
                initialColor = spriteRenderer.color;
                spriteRenderer.color = targetColor;
                await Task.Delay(waitForSeconds);
                spriteRenderer.color = initialColor;
                await Task.Delay(waitForSeconds);
                timer += duration / ticks;
            }

            spriteRenderer.color = initialColor;
        }
    
        /// <summary>
        /// <para>Makes a blinking effect to the object</para>
        /// <param name="duration">The duration in seconds of the blinking effect</param>
        /// <param name="ticks">The number of times you want the object to blink</param>
        /// <param name="targetColor">The color to change in every blink, normally transparent or white</param>
        /// </summary>
        public static void DoBlink(this Image image, float duration, int ticks, Color targetColor) => CoroutineController.Start(DoBlinkEnumerator(image, duration, ticks, targetColor));
        private static IEnumerator DoBlinkEnumerator(Image image, float duration, int ticks, Color targetColor)
        {
            if (ticks <= 0) yield break;
        
            _blinking ??= new Dictionary<int, bool>();
            int id = image.GetHashCode();
            if(!_blinking.ContainsKey(id)) _blinking.Add( id , true);
            else _blinking[id] = true;

            float timer = 0;
            Color initialColor = image.color;
            WaitForSeconds waitForSeconds = new WaitForSeconds(duration / ticks/2);
        
            while (timer<duration)
            {
                initialColor = image.color;
                image.color = targetColor;
                yield return waitForSeconds;
                image.color = initialColor;
                yield return waitForSeconds;
                timer += duration / ticks;
            }

            image.color = initialColor;
            _blinking[id] = false;
        }
        
        /// <summary>
        /// <para>Makes a blinking effect to the object</para>
        /// <param name="duration">The duration in seconds of the blinking effect</param>
        /// <param name="ticks">The number of times you want the object to blink</param>
        /// <param name="targetColor">The color to change in every blink, normally transparent or white</param>
        /// </summary>
        public static async Task DoBlinkAsync(this Image image, float duration, int ticks, Color targetColor)
        {
            if (ticks <= 0) return;
            float timer = 0;
            Color initialColor = image.color;  
            int delay = (int)((duration/ticks/2)*1000);
        
            while (timer<duration)
            {
                initialColor = image.color;
                image.color = targetColor;
                await Task.Delay(delay);
                image.color = initialColor;
                await Task.Delay(delay);
                timer += duration / ticks;
            }

            image.color = initialColor;
        }

        
        /// <summary>
        /// <para>Makes the object transition from one color to another in a certain time</para>
        /// <param name="targetColor">The final color</param>
        /// <param name="duration">The duration in seconds of the transition</param>
        /// </summary>
        public static void DoChangeColor(this SpriteRenderer spriteRenderer, Color targetColor, float duration) => CoroutineController.Start(DoChangeColorEnumerator(spriteRenderer, targetColor, duration));
        private static IEnumerator DoChangeColorEnumerator(SpriteRenderer spriteRenderer, Color targetColor, float duration)
        {
            float timer = Time.unscaledDeltaTime;
            Color initialColor = spriteRenderer.color;
            Color colorDelta = targetColor - initialColor;
        
            while (timer<duration)
            {
                spriteRenderer.color = initialColor + colorDelta*(timer/duration);
                yield return null;
                timer += Time.unscaledDeltaTime;
            }

            spriteRenderer.color = targetColor;
        }
        
        /// <summary>
        /// <para>Makes the object transition from one color to another in a certain time</para>
        /// <param name="targetColor">The final color</param>
        /// <param name="duration">The duration in seconds of the transition</param>
        /// </summary>
        public static async Task DoChangeColorAsync(this SpriteRenderer spriteRenderer, Color targetColor, float duration)
        {
            float timer = Time.unscaledDeltaTime;
            Color initialColor = spriteRenderer.color;
            Color colorDelta = targetColor - initialColor;
        
            while (timer<duration)
            {
                spriteRenderer.color = initialColor + colorDelta*(timer/duration);
                await Task.Yield();
                timer += Time.unscaledDeltaTime;
            }

            spriteRenderer.color = targetColor;
        }
    
        /// <summary>
        /// <para>Makes the object transition from one color to another in a certain time</para>
        /// <param name="targetColor">The final color</param>
        /// <param name="duration">The duration in seconds of the transition</param>
        /// </summary>
        public static void DoChangeColor(this Image image, Color targetColor , float duration) => CoroutineController.Start(DoChangeColorEnumerator(image, targetColor, duration));
        private static IEnumerator DoChangeColorEnumerator(Image image, Color targetColor, float duration)
        {
            float timer = Time.unscaledDeltaTime;
            Color initialColor = image.color;
            Color colorDelta = targetColor - initialColor;
        
            while (timer<duration)
            {
                image.color = initialColor + colorDelta*(timer/duration);
                yield return null;
                timer += Time.unscaledDeltaTime;
            }

            image.color = targetColor;
        }
        
        /// <summary>
        /// <para>Makes the object transition from one color to another in a certain time</para>
        /// <param name="targetColor">The final color</param>
        /// <param name="duration">The duration in seconds of the transition</param>
        /// </summary>
        public static async Task DoChangeColorAsync(this Image image, Color targetColor, float duration)
        {
            float timer = Time.unscaledDeltaTime;
            Color initialColor = image.color;
            Color colorDelta = targetColor - initialColor;
        
            while (timer<duration)
            {
                image.color = initialColor + colorDelta*(timer/duration);
                await Task.Yield();
                timer += Time.unscaledDeltaTime;
            }

            image.color = targetColor;
        }

    }
}
