using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TUGraz.VectoCommon.Models
{
    [DebuggerDisplay("Axle Number: {Value}")]
    public class AxleNumber : IComparable<AxleNumber>, IComparable
    {
        private const int NOT_IN_AXLE_POWERTRAIN_CONST = -1;
        private readonly int _value;

        public AxleNumber(int i)
        {
            _value = i;
        }

        public static AxleNumber NOT_IN_AXLE_POWERTRAIN => AxleNumber.NotInAxlePowertrain();

        public static AxleNumber NotInAxlePowertrain()
        {
            return new AxleNumber(AxleNumber.NOT_IN_AXLE_POWERTRAIN_CONST);
        }

        public int Value => _value;
        
        public static bool operator ==(AxleNumber a, int b)
        {
            return (a?.Value == b);
        }

        public static bool operator !=(AxleNumber a, int b)
        {
            return !(a == b);
        }

        public static bool operator ==(AxleNumber a, AxleNumber b)
        {
            return (a == (object)b);
        }

        public static bool operator !=(AxleNumber a, AxleNumber b)
        {
            return !(a == b);
        }


        public static bool operator ==(AxleNumber a, object b)
        {
            return EqualityComparer.Equals(a, b as AxleNumber);
        }

        public static bool operator !=(AxleNumber a, object b)
        {
            // ReSharper disable once NegativeEqualityExpression
            return !(a == b);
        }


        public int CompareTo(AxleNumber other)
        {
            return RelationalComparer.Compare(this, other);
        }

        public override bool Equals(object obj)
        {
            return EqualityComparer.Equals(this, obj as AxleNumber);
        }


        public override int GetHashCode()
        {
            return _value;
        }

        public int CompareTo(object obj)
        {
            if (obj is AxleNumber otherAxleNumber)
            {
                return CompareTo(otherAxleNumber);
            }
            else
            {
                return 1;
            }
        }


        public string FormatAxleNumber()
        {
            return (Value == AxleNumber.NOT_IN_AXLE_POWERTRAIN_CONST) ? "" : $"_axle_{Value}";
        }


        // public static implicit operator AxleNumber(int i) => new AxleNumber(i);
        public static implicit operator int(AxleNumber axleNumber) => axleNumber.Value;
        
        
        
        
        
        #region Comparer

        private sealed class ValueEqualityComparer : IEqualityComparer<AxleNumber>
        {
            public bool Equals(AxleNumber x, AxleNumber y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (x is null) return false;
                if (y is null) return false;
                if (x.GetType() != y.GetType()) return false;
                return x._value == y._value;
            }

            public int GetHashCode(AxleNumber obj)
            {
                return obj._value;
            }
        }

        public static IEqualityComparer<AxleNumber> EqualityComparer { get; } = new ValueEqualityComparer();

        private sealed class ValueRelationalComparer : IComparer<AxleNumber>
        {
            public int Compare(AxleNumber x, AxleNumber y)
            {
                if (ReferenceEquals(x, y)) return 0;
                if (y is null) return 1;
                if (x is null) return -1;
                return x._value.CompareTo(y._value);
            }
        }

        public static IComparer<AxleNumber> RelationalComparer { get; } = new ValueRelationalComparer();

        #endregion

        public override string ToString()
        {
            return $"Axlenumber: {Value}";
        }
    }

}