using System;

namespace ParserCombinator
{

    public delegate ParseResult<T> P<T>(string input);
    public delegate double del(int x, int y);

    // example of extension methods
    public static class StringExtensions
    {
        public static string PigLatin(this string s, int n)
        {
            return s.Substring(n) + s.Substring(0, n) + "ay";
        }
    }

    public static class Parser
    {
        /// <summary>
        /// Sat(pred) is a character parser that considers the first character 
        /// of the input. If that character matches predicate pred, then the 
        /// parse succeeds, yielding that character. If the character doesn’t 
        /// match the predicate (or if we’ve already reached the end of the 
        /// input string), the parse fails.
        /// </summary>
        /// <param name="pred"></param>
        /// <returns></returns>
        public static P<char> Sat(Predicate<char> pred)
        {
            return Then(Item(), c => pred(c) ? Return(c) : Fail<char>());
        }

        /// <summary>
        ///  Fail returns a parser that always fails, regardless of the input string. 
        ///  Fail("anything")  // yields null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>null</returns>
        public static P<T> Fail<T>()                             // zero
        {
            return input => null;
        }

        /// <summary>
        /// Return() returns a parser that always succeeds with a given value, ignoring the input string
        /// Return("whatever") // yields "whatever"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="x"></param>
        /// <returns>x</returns>
        public static P<T> Return<T>(T x)                        // unit
        {
            return input => new ParseResult<T>(x, input);
        }

        /// <summary>
        /// Then() is used for chaining parsers in a sequence (first parse this, then do this)
        /// </summary>
        /// <typeparam name="T">Parser that yields a result of type T.</typeparam>
        /// <typeparam name="U">Function takes a T and returns a new parser that yields a U</typeparam>
        /// <param name="p1"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        //public static P<U> Then<T, U>(this P<T> p1, Func<T, P<U>> f)  // bind
        public static P<U> Then<T, U>(P<T> p1, Func<T, P<U>> f)  // bind
        {
            return input =>
            {
                ParseResult<T> result1 = p1(input);
                if (result1 == null) return null;
                return f(result1.Result)(result1.RemainingInput);
            };
        }

        /// <summary>
        /// Or() returns a parser that tries two alternatives 
        /// (first parse this, but if that fails, parse this instead)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        //public static P<T> Or<T>(this P<T> p1, P<T> p2)               // plus
        public static P<T> Or<T>(P<T> p1, P<T> p2)               // plus
        {
            return input =>
            {
                ParseResult<T> result1 = p1(input);
                if (result1 == null)
                    return p2(input);
                else
                    return result1;
            };
        }

        /// <summary>
        /// Item() returns a parser that consumes the first character of the input string
        /// P<char> item = Item();
        /// item("foo")  // yields { 'f', "oo" }
        /// item("")     // yields null
        /// </summary>
        /// <returns></returns>
        public static P<char> Item()
        {
            return input =>
            {
                if (string.IsNullOrEmpty(input))
                    return null;
                else
                    return new ParseResult<char>(input[0], input.Substring(1));
            };
        }

        // other handy functions
        //public static P<U> Then_<T, U>(this P<T> p1, P<U> p2)
        //{
        //    return p1.Then(dummy => p2);
        //}

    }

    public class Example
    {
        public int x { get; set; }
        public int y { get; set; }

        public double TenXPlusY(int x, int y) {return 10 * x + y; }
        public double XPowerY(int x, int y) { return Math.Pow(x, y); }
    }

}
