using UnityEngine;
using System.IO.Ports; //Si da errores ir a Project Settings > Player > Api Compatibility Leve > cambiar a .NET Framework

    public class ArmInput
    {

        //Inputs
        public static KeyCode inputForSignal1 = KeyCode.F;
        public static KeyCode inputForSignal2 = KeyCode.J;
        public static KeyCode inputForSignal3 = KeyCode.D;
        public static KeyCode inputForSignal4 = KeyCode.K;

        //Serial port
        private static float[] valuesSerialPorts = new float[4];

        private static SerialPort serialPort = new SerialPort("COM3", 9600);
        //en funci�n de Serial.begin(n); en el arduino

        //Señales para detectar las señales por flanco de subida y bajada
        private static bool[] upSignals = new bool[4];
        private static bool[] downSignals = new bool[4];

        //Cada se�al es equivalente a un eje
        public enum Signal
        {
            S1,
            S2,
            S3,
            S4,
            LBiceps,
            RBiceps,
            LTriceps,
            RTriceps
        }

        public static float GetSignal(Signal signal)
        {
            portCheck();

            switch (signal)
            {
                case Signal.S1:
                case Signal.LBiceps:
                    return Input.GetKey(inputForSignal1) ||
                           valuesSerialPorts[0] == 1
                        ? 1
                        : 0;
                case Signal.S2:
                case Signal.RBiceps:
                    return Input.GetKey(inputForSignal2) ||
                           valuesSerialPorts[1] == 1
                        ? 1
                        : 0;
                case Signal.S3:
                case Signal.LTriceps:
                    return Input.GetKey(inputForSignal3) ||
                           valuesSerialPorts[2] == 1
                        ? 1
                        : 0;
                case Signal.S4:
                case Signal.RTriceps:
                    return Input.GetKey(inputForSignal4) ||
                           valuesSerialPorts[3] == 1
                        ? 1
                        : 0;
            }

            return 0;
        }

        public static float GetSignal(string SignalName) => GetSignal(GetSignalByString(SignalName));

        private static void portCheck()
        {
            try
            {
                if (!serialPort.IsOpen)
                {
                    serialPort.Open();
                    serialPort.ReadTimeout = 1;
                }

                string[] s = serialPort.ReadLine().Split(',');
                for (int i = 0; i < s.Length; i++)
                    valuesSerialPorts[i] = float.Parse(s[i]);
            }
            catch
            {

            }
        }

        public static Vector2 GetSignalLeftArm()
        {
            return new Vector2(GetSignal(Signal.LBiceps), GetSignal(Signal.LTriceps));
        }

        public static Vector2 GetSignalRightArm()
        {
            return new Vector2(GetSignal(Signal.RBiceps), GetSignal(Signal.RTriceps));
        }

        public static Vector4 GetAllSignals()
        {
            return new Vector4(GetSignal(Signal.S1), GetSignal(Signal.S2), GetSignal(Signal.S3), GetSignal(Signal.S4));
        }

        public static bool GetSignalDown(Signal signal)
        {
            bool res = false;
            bool signalValue = GetSignal(signal) == 1;

            switch (signal)
            {
                case Signal.S1:
                case Signal.LBiceps:
                    res = signalValue && !downSignals[0];
                    downSignals[0] = signalValue;
                    break;
                case Signal.S2:
                case Signal.RBiceps:
                    res = signalValue && !downSignals[1];
                    downSignals[1] = signalValue;
                    break;
                case Signal.S3:
                case Signal.LTriceps:
                    res = signalValue && !downSignals[2];
                    downSignals[2] = signalValue;
                    break;
                case Signal.S4:
                case Signal.RTriceps:
                    res = signalValue && !downSignals[3];
                    downSignals[3] = signalValue;
                    break;
            }

            return res;
        }

        public static bool GetSignalDown(string signalName) => GetSignalDown(GetSignalByString(signalName));

        public static bool GetSignalUp(Signal signal)
        {
            bool res = false;
            bool signalValue = GetSignal(signal) == 1;

            switch (signal)
            {
                case Signal.S1:
                case Signal.LBiceps:
                    res = !signalValue && upSignals[0];
                    upSignals[0] = signalValue;
                    break;
                case Signal.S2:
                case Signal.RBiceps:
                    res = !signalValue && upSignals[1];
                    upSignals[1] = signalValue;
                    break;
                case Signal.S3:
                case Signal.LTriceps:
                    res = !signalValue && upSignals[2];
                    upSignals[2] = signalValue;
                    break;
                case Signal.S4:
                case Signal.RTriceps:
                    res = !signalValue && upSignals[3];
                    upSignals[3] = signalValue;
                    break;
            }

            return res;
        }

        public static bool GetSignalUp(string signalName) => GetSignalUp(GetSignalByString(signalName));

        private static Signal GetSignalByString(string SignalName)
        {
            switch (SignalName)
            {
                case "S1":
                case "LBiceps":
                    return Signal.S1;
                case "S2":
                case "RBiceps":
                    return Signal.S2;
                case "S3":
                case "LTriceps":
                    return Signal.S3;
                case "S4":
                case "RTriceps":
                    return Signal.S4;
            }
#if UNITY_EDITOR
            Debug.LogError($"Signal {SignalName} does not exist!");
#endif
            return (Signal)(-1);
        }

    }
