/*
 * using System;

namespace MPC_Part2
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
            if (!object.Equals(other.Result, this.Result))  return false;
            if (!object.Equals(other.RemainingInput, this.RemainingInput)) return false;
            return true;
        }

        public override int GetHashCode()  { return this.RemainingInput.GetHashCode(); }
    }

    // representation type for parsers
    public delegate ParseResult<T> P<T>(string input);

    public static class Parser
    {
        // core functions that know the underlying parser representation
        public static P<T> Fail<T>()
        {
            return input => null;
        }
        public static P<T> Return<T>(T x)
        {
            return input => new ParseResult<T>(x, input);
        }
        public static P<U> Then<T, U>(this P<T> p1, Func<T, P<U>> f)
        {
            return input =>
            {
                ParseResult<T> result1 = p1(input);
                if (result1 == null)
                    return null;
                else
                    return f(result1.Result)(result1.RemainingInput);
            };
        }
        public static P<T> Or<T>(this P<T> p1, P<T> p2)
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
        public static P<U> Then_<T, U>(this P<T> p1, P<U> p2)
        {
            return p1.Then(dummy => p2);
        }
    }

    // example of extension methods
    public static class StringExtensions
    {
        public static string PigLatin(this string s, int n)
        {
            return s.Substring(n) + s.Substring(0, n) + "ay";
        }
    }

    // various running examples from the blog
    class Program
    {
        static void AssertEqual<T>(T x, T y)
        {
            if (!object.Equals(x, y))
                throw new Exception("not equal");
        }

        public static P<char> Item = Parser.Item();

        public static P<char> Sat(Predicate<char> pred)
        {
            return Item.Then(c => pred(c) ? Parser.Return(c) : Parser.Fail<char>());
        }

        public static P<char> Letter = Sat(char.IsLetter);

        public static P<int> Literal(string toParse, int result)
        {
            if (toParse == "")
                return Parser.Return(result);
            else
                return Sat(c => c == toParse[0]).Then_(Literal(toParse.Substring(1), result));
        }

        public static P<int> OneToNine =
            Literal("one", 1).Or(
            Literal("two", 2).Or(
            Literal("three", 3).Or(
            Literal("four", 4).Or(
            Literal("five", 5).Or(
            Literal("six", 6).Or(
            Literal("seven", 7).Or(
            Literal("eight", 8).Or(
            Literal("nine", 9)))))))));

        public static P<int> TenToNineteen =
            Literal("ten", 10).Or(
            Literal("eleven", 11).Or(
            Literal("twelve", 12).Or(
            Literal("thirteen", 13).Or(
            Literal("fourteen", 14).Or(
            Literal("fifteen", 15).Or(
            Literal("sixteen", 16).Or(
            Literal("seventeen", 17).Or(
            Literal("eighteen", 18).Or(
            Literal("nineteen", 19))))))))));

        public static P<int> HigherTens(string tenName, int tenValue)
        {
            return Literal(tenName, tenValue).Then(tens =>
                   (
                       Sat(c => c == '-').Then_(
                       OneToNine.Then(ones =>
                       Parser.Return(tens + ones)))
                   ).Or(Parser.Return(tens)));
        }

        public static P<int> OneToNinetyNine =
            OneToNine
            .Or(TenToNineteen)
            .Or(HigherTens("twenty", 20))
            .Or(HigherTens("thirty", 30))
            .Or(HigherTens("forty", 40))
            .Or(HigherTens("fifty", 50))
            .Or(HigherTens("sixty", 60))
            .Or(HigherTens("seventy", 70))
            .Or(HigherTens("eighty", 80))
            .Or(HigherTens("ninety", 90));

        public static P<char> Space = Sat(char.IsWhiteSpace);

        public static P<int> OneHundredTo999 =
            OneToNine.Then(hundreds =>
            Space.Then_(
            Literal("hundred", 0).Then_(
            Space.Then_(
            OneToNinetyNine.Then(rest =>
            Parser.Return(hundreds * 100 + rest))))));

        public static P<int> OneTo999 = OneHundredTo999.Or(OneToNinetyNine);

        public static P<int> OneTo999999 =
                OneTo999.Then(thousands =>
                Space.Then_(
                Literal("thousand", 0).Then_(
                Space.Then_(
                OneTo999.Then(rest =>
                Parser.Return(thousands * 1000 + rest))))))
            .Or(OneTo999);

        static void Main(string[] args)
        {
            Console.WriteLine("foo".PigLatin(1));
            Console.WriteLine("star".PigLatin(2));

            Predicate<char> isDigit = char.IsDigit;
            Func<int, int, bool> less = (x, y) => x < y;

            AssertEqual(Item(""), null);
            AssertEqual(Item("foo"), new ParseResult<char>('f', "oo"));
            P<char> letter = Sat(char.IsLetter);
            P<char> digit = Sat(char.IsDigit);
            AssertEqual(letter("foo"), new ParseResult<char>('f', "oo"));
            AssertEqual(digit("foo"), null);
            P<char> letterOrDigit = letter.Or(digit);
            AssertEqual(letterOrDigit("foo"), new ParseResult<char>('f', "oo"));
            AssertEqual(letterOrDigit("123"), new ParseResult<char>('1', "23"));
            AssertEqual(letterOrDigit(";x%"), null);

            P<int> two = Sat(c => c == 't').Then_(
                         Sat(c => c == 'w').Then_(
                         Sat(c => c == 'o').Then_(
                         Parser.Return(2))));
            AssertEqual(two("two bits"), new ParseResult<int>(2, " bits"));
            AssertEqual(two("four"), null);

            P<int> three = Literal("three", 3);
            AssertEqual(three("three!"), new ParseResult<int>(3, "!"));
            P<int> thirties = HigherTens("thirty", 30);
            AssertEqual(thirties("thirty!"), new ParseResult<int>(30, "!"));
            AssertEqual(thirties("thirty-two!"), new ParseResult<int>(32, "!"));

            AssertEqual(OneToNinetyNine("twenty!"), new ParseResult<int>(20, "!"));
            AssertEqual(OneToNinetyNine("twenty-five!"), new ParseResult<int>(25, "!"));
            // Note how the next two get the 'wrong' answer.  We'll talk about
            // a combinator called NotFollowedBy, which allows for lookahead, 
            // in a future installment, rather than fix this now.
            AssertEqual(OneToNinetyNine("seventeen!"), new ParseResult<int>(7, "teen!"));
            AssertEqual(OneToNinetyNine("eighty!"), new ParseResult<int>(8, "y!"));

            P<int> englishNumber = OneTo999999;
            AssertEqual(englishNumber("one."), new ParseResult<int>(1, "."));
            AssertEqual(englishNumber("one hundred six."), new ParseResult<int>(106, "."));
            AssertEqual(englishNumber("one hundred forty-six thousand five hundred twenty-two widgets"), new ParseResult<int>(146522, " widgets"));

            Console.WriteLine("done, press a key");
            Console.ReadKey();
        }
    }
}
*/