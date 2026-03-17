using System;

namespace Encounter.NightDance.Core
{
    /// <summary>
    /// 0~100까지의 범위를 갖는 바이트 기반 퍼센트 구조체
    /// </summary>
    public readonly struct Percentage: IEquatable<Percentage>, IComparable<Percentage>
    {
        private readonly byte _value;
        public Percentage(byte value)
        {
            _value = value > 100 ? (byte)100 : value;
        }
        public Percentage(int value)
        {
            _value = (byte)(value < 0 ? 0 : value > 100 ? 100 : value);
        }
        public static implicit operator byte(Percentage percentage) => percentage._value;
        public static implicit operator float(Percentage percentage) => percentage._value / 100f;
        public bool Equals(Percentage percentage) => _value == percentage._value;
        public int CompareTo(Percentage percentage) => _value.CompareTo(percentage);
        public override string ToString() => $"{_value}%";
    }
}