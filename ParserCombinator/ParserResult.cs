using System;
using System.Collections.Generic;
using System.Text;

namespace ParserCombinator
{
    public class ParseResult<T>
    {
        public readonly T Result;
        public readonly string RemainingInput;
        public ParseResult(T r, string ri) { this.Result = r; this.RemainingInput = ri; }

        public override bool Equals(object obj)
        {
            ParseResult<T> other = obj as ParseResult<T>;

            if (other == null) return false;
            if (!object.Equals(other.Result, this.Result)) return false;
            if (!object.Equals(other.RemainingInput, this.RemainingInput)) return false;
            return true;
        }

        public override int GetHashCode() { return this.RemainingInput.GetHashCode(); }
    }
}
