using System;
using System.Runtime.InteropServices;
using System.Text;

namespace LedCSharp
{
    public enum KeyboardName
    {
        ESC = 0x01,
        F1 = 0x3b,
        F2 = 0x3c,
        F3 = 0x3d,
        F4 = 0x3e,
        F5 = 0x3f,
        F6 = 0x40,
        F7 = 0x41,
        F8 = 0x42,
        F9 = 0x43,
        F10 = 0x44,
        F11 = 0x57,
        F12 = 0x58,
        PrintScreen = 0x137,
        ScrollLock = 0x46,
        PauseBreak = 0x145,
        TILDE = 0x29,
        ONE = 0x02,
        TWO = 0x03,
        THREE = 0x04,
        FOUR = 0x05,
        FIVE = 0x06,
        SIX = 0x07,
        SEVEN = 0x08,
        EIGHT = 0x09,
        NINE = 0x0A,
        ZERO = 0x0B,
        MINUS = 0x0C,
        EQUALS = 0x0D,
        BACKSPACE = 0x0E,
        INSERT = 0x152,
        HOME = 0x147,
        PageUp = 0x149,
        NumLock = 0x45,
        NumSlash = 0x135,
        NumAsterix = 0x37,
        NumMinus = 0x4A,
        TAB = 0x0F,
        Q = 0x10,
        W = 0x11,
        E = 0x12,
        R = 0x13,
        T = 0x14,
        Y = 0x15,
        U = 0x16,
        I = 0x17,
        O = 0x18,
        P = 0x19,
        OpenBracket = 0x1A,
        CloseBracket = 0x1B,
        BACKSLASH = 0x2B,
        KeyboardDelete = 0x153,
        END = 0x14F,
        PageDown = 0x151,
        NumSeven = 0x47,
        NumEight = 0x48,
        NumNine = 0x49,
        NumPlus = 0x4E,
        CapsLock = 0x3A,
        A = 0x1E,
        S = 0x1F,
        D = 0x20,
        F = 0x21,
        G = 0x22,
        H = 0x23,
        J = 0x24,
        K = 0x25,
        L = 0x26,
        SEMICOLON = 0x27,
        APOSTROPHE = 0x28,
        ENTER = 0x1C,
        NumFour = 0x4B,
        NumFive = 0x4C,
        NumSix = 0x4D,
        LeftShift = 0x2A,
        Z = 0x2C,
        X = 0x2D,
        C = 0x2E,
        V = 0x2F,
        B = 0x30,
        N = 0x31,
        M = 0x32,
        COMMA = 0x33,
        PERIOD = 0x34,
        ForwardSlash = 0x35,
        RightShift = 0x36,
        ArrowUp = 0x148,
        NumOne = 0x4F,
        NumTwo = 0x50,
        NumThree = 0x51,
        NumEnter = 0x11C,
        LeftControl = 0x1D,
        LeftWindows = 0x15B,
        LeftAlt = 0x38,
        SPACE = 0x39,
        RightAlt = 0x138,
        RightWindows = 0x15C,
        ApplicationSelect = 0x15D,
        RightControl = 0x11D,
        ArrowLeft = 0x14B,
        ArrowDown = 0x150,
        ArrowRight = 0x14D,
        NumZero = 0x52,
        NumPeriod = 0x53,
        G1 = 0xFFF1,
        G2 = 0xFFF2,
        G3 = 0xFFF3,
        G4 = 0xFFF4,
        G5 = 0xFFF5,
        G6 = 0xFFF6,
        G7 = 0xFFF7,
        G8 = 0xFFF8,
        G9 = 0xFFF9,
        GLogo = 0xFFFF1,
        GBadge = 0xFFFF2
    };

    public enum DeviceType
    {
        Keyboard = 0x0,
        Mouse = 0x3,
        Mousemat = 0x4,
        Headset = 0x8,
        Speaker = 0xe
    }
    public static class LogitechSDKConstants
    {
        private const int LogiDevicetypeMonochromeOrd = 0;
        private const int LogiDevicetypeRGBOrd = 1;
        private const int LogiDevicetypePerKeyRGBOrd = 2;

        public const int LogiDevicetypeMonochrome = (1 << LogiDevicetypeMonochromeOrd);
        public const int LogiDevicetypeRGB = (1 << LogiDevicetypeRGBOrd);
        public const int LogiDevicetypePerKeyRGB = (1 << LogiDevicetypePerKeyRGBOrd);
        public const int LogiDevicetypeAll = (LogiDevicetypeMonochrome | LogiDevicetypeRGB | LogiDevicetypePerKeyRGB);

        public const int LogiLEDBitmapWidth = 21;
        public const int LogiLEDBitmapHeight = 6;
        public const int LogiLEDBitmapBytesPerKey = 4;

        public const int LogiLEDBitmapSize = LogiLEDBitmapWidth * LogiLEDBitmapHeight * LogiLEDBitmapBytesPerKey;
        public const int LogiLEDDurationInfinite = 0;
    }


    internal static class NativeMethods
    {
        [DllImport("LogitechLedEnginesWrapper ", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool LogiLedInit();

        [DllImport("LogitechLedEnginesWrapper ", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        internal static extern bool LogiLedInitWithName(String name);

        //Config option functions
        [DllImport("LogitechLedEnginesWrapper ", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool LogiLedGetConfigOptionNumber([MarshalAs(UnmanagedType.LPWStr)]String configPath, ref double defaultNumber);

        [DllImport("LogitechLedEnginesWrapper ", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool LogiLedGetConfigOptionBool([MarshalAs(UnmanagedType.LPWStr)]String configPath, ref bool defaultRed);

        [DllImport("LogitechLedEnginesWrapper ", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool LogiLedGetConfigOptionColor([MarshalAs(UnmanagedType.LPWStr)]String configPath, ref int defaultRed, ref int defaultGreen, ref int defaultBlue);

        [DllImport("LogitechLedEnginesWrapper ", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        internal static extern bool LogiLedGetConfigOptionKeyInput([MarshalAs(UnmanagedType.LPWStr)]String configPath, StringBuilder buffer, int bufsize);
        /////////////////////

        [DllImport("LogitechLedEnginesWrapper ", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool LogiLedSetTargetDevice(int targetDevice);

        [DllImport("LogitechLedEnginesWrapper ", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool LogiLedGetSdkVersion(ref int majorNum, ref int minorNum, ref int buildNum);

        [DllImport("LogitechLedEnginesWrapper ", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool LogiLedSaveCurrentLighting();

        [DllImport("LogitechLedEnginesWrapper ", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool LogiLedSetLighting(int redPercentage, int greenPercentage, int bluePercentage);

        [DllImport("LogitechLedEnginesWrapper ", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool LogiLedRestoreLighting();

        [DllImport("LogitechLedEnginesWrapper ", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool LogiLedFlashLighting(int redPercentage, int greenPercentage, int bluePercentage, int milliSecondsDuration, int milliSecondsInterval);

        [DllImport("LogitechLedEnginesWrapper ", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool LogiLedPulseLighting(int redPercentage, int greenPercentage, int bluePercentage, int milliSecondsDuration, int milliSecondsInterval);

        [DllImport("LogitechLedEnginesWrapper ", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool LogiLedStopEffects();

        [DllImport("LogitechLedEnginesWrapper ", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool LogiLedExcludeKeysFromBitmap(KeyboardName[] keyList, int listCount);

        [DllImport("LogitechLedEnginesWrapper ", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool LogiLedSetLightingFromBitmap(byte[] bitmap);

        [DllImport("LogitechLedEnginesWrapper ", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool LogiLedSetLightingForKeyWithScanCode(int keyCode, int redPercentage, int greenPercentage, int bluePercentage);

        [DllImport("LogitechLedEnginesWrapper ", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool LogiLedSetLightingForKeyWithHidCode(int keyCode, int redPercentage, int greenPercentage, int bluePercentage);

        [DllImport("LogitechLedEnginesWrapper ", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool LogiLedSetLightingForKeyWithQuartzCode(int keyCode, int redPercentage, int greenPercentage, int bluePercentage);

        [DllImport("LogitechLedEnginesWrapper ", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool LogiLedSetLightingForKeyWithKeyName(KeyboardName keyCode, int redPercentage, int greenPercentage, int bluePercentage);

        [DllImport("LogitechLedEnginesWrapper ", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool LogiLedSaveLightingForKey(KeyboardName keyName);

        [DllImport("LogitechLedEnginesWrapper ", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool LogiLedRestoreLightingForKey(KeyboardName keyName);

        [DllImport("LogitechLedEnginesWrapper ", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool LogiLedFlashSingleKey(KeyboardName keyName, int redPercentage, int greenPercentage, int bluePercentage, int msDuration, int msInterval);

        [DllImport("LogitechLedEnginesWrapper ", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool LogiLedPulseSingleKey(KeyboardName keyName, int startRedPercentage, int startGreenPercentage, int startBluePercentage, int finishRedPercentage, int finishGreenPercentage, int finishBluePercentage, int msDuration, bool isInfinite);

        [DllImport("LogitechLedEnginesWrapper ", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool LogiLedStopEffectsOnKey(KeyboardName keyName);

        [DllImport("LogitechLedEnginesWrapper ", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool LogiLedSetLightingForTargetZone(DeviceType deviceType, int zone, int redPercentage, int greenPercentage, int bluePercentage);

        [DllImport("LogitechLedEnginesWrapper ", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void LogiLedShutdown();
    }

}
