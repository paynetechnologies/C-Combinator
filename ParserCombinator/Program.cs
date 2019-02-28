using System;
using ParserCombinator;
using System.Collections.Generic;
using System.Text;

namespace ParserCombinator
{
    class Program
    {
        // public delegate <return type< >delegate-name> <arg list>


        static void Main(string[] args)
        {

            var example = new Example();

            del Nat = new del(example.TenXPlusY);
            del Exp = new del(example.XPowerY);

            // lambda
            del lNat = (x, y) => 10 * x + y;
            del lPow = (x, y) => Math.Pow(x, y);

            Console.WriteLine(Nat(10, 5));
            Console.WriteLine(Exp(10, 5));
            Console.WriteLine(lNat(10, 5));
            Console.WriteLine(lPow(10, 5));


            // FAIL
            P<int> failure = Parser.Fail<int>();
            ParseResult<int> f = failure("whatever");
            Console.WriteLine("Fail {0}",  f ?? null );  // yields { null, null }


            // RETURN
            P<string> returns = Parser.Return<string>("hello world");
            ParseResult<string> r = returns("whatever");
            Console.WriteLine("Return {0}", r.Result);  // yields { 'w', "hatever" }

            // Item
            P<char> item = Parser.Item();
            ParseResult<char> i = item("foo");  // yields { 'f', "oo" }
            Console.WriteLine("Return {0}", i.Result);  // yields { 'w', "hatever" }

            ParseResult<char> j = item("");     // yields null
            Console.WriteLine("Return {0}", null);  // yields { 'w', "hatever" }


            //Sat
            P<char> letter = Parser.Sat(char.IsLetter);
            ParseResult<char> l = letter("foo");
            Console.WriteLine("Sat {0}",l.Result);  // yields { 'f', "oo" }

            letter("123");  // yields null
            letter("");     // yields null/

            //P<char> digit = Parser.Sat(char.IsDigit);

            //P<char> letterOrDigit = Parser.Or(letter, digit);
            //letterOrDigit("foo");  // yields { 'f', "oo" }
            //letterOrDigit("123");  // yields { '1', "23" }
            //letterOrDigit(";x%");  // yields null

        }
    }
}