using System;
using System.ComponentModel;

namespace CustomControls
{
    [Flags]
    [TypeConverter(typeof(EnumConverter))]
    public enum AttachPosition
    {
        Top = 1,
        Bottom = 2,
        Left = 4,
        Right = 8
    }
}