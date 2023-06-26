using System;
using System.Threading.Tasks;
using ProjectUtils.Helpers;
using UnityEngine;

public class ClawController : MonoBehaviour
{
    [SerializeField] private Transform moverTransform;
    [SerializeField] private GameObject button;
    [SerializeField] private Transform joystickTransform;
    [SerializeField] private float moverSpeed = 3;
    private Transform _toyTransform;
    private ClawFinger[] _clawFingers;

    private bool _transitioning;
    private Task[] _tasks;

    private float _timer;
    
    void Start()
    {
        _clawFingers = GetComponentsInChildren<ClawFinger>();
        _tasks = new Task[3];
    }

    // Update is called once per frame
     void Update()
    {
        transform.position = new Vector3(moverTransform.position.x, transform.position.y, moverTransform.position.z);

        if (_transitioning)
        {
            joystickTransform.localRotation = Quaternion.Slerp(joystickTransform.localRotation, Quaternion.Euler(new Vector3(270, 0)), Time.deltaTime);
            return;
        }

        DetectInput();
        
        Vector3 movement = new Vector3( ArmInput.GetSignal(ArmInput.Signal.LTriceps) + ArmInput.GetSignal(ArmInput.Signal.LBiceps) * -1, ArmInput.GetSignal(ArmInput.Signal.RTriceps) + ArmInput.GetSignal(ArmInput.Signal.RBiceps) * -1);
        var rotation = Quaternion.Euler(movement != Vector3.zero ? new Vector3(280, Mathf.Atan2(movement.x, -movement.y)*Mathf.Rad2Deg) : new Vector3(270, 0));
        joystickTransform.localRotation = Quaternion.Lerp(joystickTransform.localRotation, rotation, Time.deltaTime*7);
        if (movement.x > 0 && moverTransform.localPosition.x >= 1.2f) movement.x = 0;
        if (movement.x < 0 && moverTransform.localPosition.x <= -1.2f) movement.x = 0;   
        if (movement.y < 0 && moverTransform.localPosition.z >= 1.2f) movement.y = 0;
        if (movement.y > 0 && moverTransform.localPosition.z <= -1.2f) movement.y = 0;
        moverTransform.Translate(movement * (moverSpeed * Time.deltaTime));
    }

     private void DetectInput()
     {
         if (ArmInput.GetSignal(ArmInput.Signal.LTriceps) == 1)
         {
             if (_timer < 0.1f && ArmInput.GetSignal(ArmInput.Signal.RTriceps) == 1)
             {
                 button.GetComponent<Animation>().Play();
                 GoDown(1.5f);
             }
             else _timer += Time.deltaTime;
         }
         else if (ArmInput.GetSignal(ArmInput.Signal.RTriceps) == 1)
         {
             if (_timer < 0.1f && ArmInput.GetSignal(ArmInput.Signal.LTriceps) == 1)
             {
                 button.GetComponent<Animation>().Play();
                 GoDown(2);
             }
             else _timer += Time.deltaTime;
         }
         else _timer = 0;
     }

     private async void GoDown(float time)
    {
        _transitioning = true;
        _toyTransform = null;
       

        await RotateFingers(time/2, ClawFinger.RotationType.open, false);

        Vector3 targetPosition = transform.localPosition;
        targetPosition.y = -0.1f;
        float timer = Time.deltaTime;
        Vector3 initialPosition = transform.localPosition;
        Vector3 moveDelta = targetPosition - initialPosition;
        while (!_toyTransform && timer < time)
        {
            transform.localPosition = initialPosition + moveDelta * (timer / time);
            await Task.Yield();
            timer += Time.deltaTime;
        }

        await RotateFingers(time/2, ClawFinger.RotationType.final, true);

        await Catch(time);
        _transitioning = false;
    }

    private async Task Catch(float time)
    {
        if (_toyTransform)
        {
            _toyTransform.SetParent(transform);
            _toyTransform.GetComponent<Rigidbody>().useGravity = false;
            //_toyTransform.localPosition = new Vector3(0,0,-0.025f);
            _toyTransform.GetComponent<Collider>().enabled = false;
        }
        else {
            foreach (var clawFinger in _clawFingers)
            {
                clawFinger.DoRotateAsync(clawFinger.initialRotation, time, false);
            }
        }
        
        Vector3 targetPosition = transform.localPosition;
        targetPosition.y = 1.5f;
        await MoveLocalPosition(transform, time, targetPosition);

        if (_toyTransform)
        {
            await Drop(time);
        }
    }

    private async Task Drop(float time)
    {
        var targetPosition = new Vector3(1.15f, moverTransform.localPosition.y, 1.15f);
        await MoveLocalPosition(moverTransform, time, targetPosition);

        _toyTransform.GetComponent<Collider>().enabled = true;
        _toyTransform.SetParent(null);
        _toyTransform.GetComponent<Rigidbody>().useGravity = true;
        
        await RotateFingers(time / 2, ClawFinger.RotationType.open, false);
        foreach (var clawFinger in _clawFingers)
        {
            clawFinger.DoRotateAsync(clawFinger.initialRotation, time, false);
        }

        targetPosition = new Vector3(0, moverTransform.localPosition.y, 0);
        await MoveLocalPosition(moverTransform, time, targetPosition);
    }

    private async Task RotateFingers(float time, ClawFinger.RotationType rotationType, bool stopWhenHit)
    {
        for (int i = 0; i < _clawFingers.Length; i++)
        {
            _tasks[i] = _clawFingers[i].DoRotateAsync(_clawFingers[i].GetRotation(rotationType), time, stopWhenHit);
        }

        await Task.WhenAll(_tasks);
    }

    private async Task MoveLocalPosition(Transform transform, float time, Vector3 targetPosition)
    {
        float timer = Time.deltaTime;
        Vector3 initialPosition = transform.localPosition;
        Vector3 moveDelta = targetPosition - initialPosition;
        while (timer < time)
        {
            transform.localPosition = initialPosition + moveDelta * (timer / time);
            await Task.Yield();
            timer += Time.deltaTime;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Toy"))
        {
            _toyTransform = other.transform;
            foreach (var clawFinger in _clawFingers)
            {
                clawFinger.grabbed = _toyTransform;
            }
        }
    }
}
