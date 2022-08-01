using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator_WinUI3_MVVMToolkit7.Models
{
    internal class NumberBuilder
    {
        private readonly StringBuilder sb = new();

        public bool HasDot { get; private set; } = false;

        public bool IsNegative { get; private set; } = false;

        public void AddDigit(int digit)
        {
            if (0 <= digit && digit <= 9)
            {
                sb.Append(digit);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(digit), "must be between 0-9");
            }
        }

        public void Clear()
        {
            sb.Clear();
            HasDot = false;
            IsNegative = false;
        }

        public void Clear(decimal init)
        {
            Clear();
            if (init >= 0)
            {
                sb.Append(init);
            }
            else
            {
                sb.Append(-init);
                IsNegative = true;
            }
            HasDot = init.ToString().Contains('.');
        }

        public void AddDot()
        {
            if (!HasDot)
            {
                HasDot = true;
                sb.Append('.');
            }
        }

        public void Invert() => IsNegative = !IsNegative;

        public bool IsEmpty() => sb.Length == 0 && !IsNegative;

        public decimal? Build() => IsEmpty() ? null : decimal.Parse(ToString());

        public override string ToString()
        {
            if (IsEmpty())
            {
                return "(Empty)";
            }
            var str = $"{(IsNegative ? "-" : "")}{(sb.Length == 0 ? "0" : sb.ToString())}".Replace("-.", "-0.");
            if (str.StartsWith("."))
            {
                str = "0" + str;
            }
            if (str.EndsWith("."))
            {
                str += "0";
            }
            return str;
        }
    }

}
