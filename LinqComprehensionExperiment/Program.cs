using System;
using System.Linq;

namespace LinqComprehensionExperiment
{
    class Program
    {
        static void Main(string[] args)
        {
            var value1 = Maybe<int>.Some(10);
            var value2 = Maybe<int>.Some(20);
            var none = Maybe<int>.None();

            /*
             * [Haskell]
             * 
             * do
             *      v1 <- value1
             *      v2 <- value2
             *      return (v1 + v2)
             */

            var tempValue1 = from v1 in value1
                             from v2 in value2
                             select v1 + v2;

            Console.WriteLine(tempValue1);

            var tempValue2 = from v1 in none
                             from v2 in value2
                             select v1 + v2;

            Console.WriteLine(tempValue2);

            var tempValue3 = from v1 in value1
                             from v2 in none
                             select v1 + v2;

            Console.WriteLine(tempValue3);

            var tempValue4 = from v1 in value1
                             from v2 in value2
                             from v3 in Maybe<int>.Some(2)
                             select (v1 + v2) * v3;

            Console.WriteLine(tempValue4);

            /*
             * [Haskell]
             * 
             * v1 <- value1
             * v2 <- value2
             * v3 <- Some(2)
             * v4 <- none
             * return (v1 + v2) * v3 / v4
             */

            var tempValue5 = from v1 in value1
                             from v2 in value2
                             from v3 in Maybe<int>.Some(2)
                             from v4 in none
                             select (v1 + v2) * v3 / v4;

            Console.WriteLine(tempValue5);

            /*
             * [Haskell]
             * 
             * value1 >>= \v1 ->
             * value2 >>= \v2 ->
             * Some(2) >>= \v3 ->
             * none >>= \v4 ->
             * return (v1 + v2) * v3 / v4
             */

            var tempValue6 = value1.Bind(v1 =>
                                value2.Bind(v2 =>
                                    Maybe<int>.Some(2).Bind(v3 =>
                                        none.Bind(v4 =>
                                            Maybe<int>.Return((v1 + v2) * v3 / v4)
                                        )
                                    )
                                )
                             );

            Console.WriteLine(tempValue6);

            var tempValue7 = from v1 in EitherMonad<int>.Right(10)
                             from v2 in EitherMonad<int>.Right(20)
                             select v1 + v2;

            Console.WriteLine(tempValue7);

            var tempValue8 = from v1 in EitherMonad<int>.Right(10)
                             from v2 in EitherMonad<int>.Left("error-1")
                             from v3 in EitherMonad<int>.Left("error-2")
                             select v1 + v2 + v3;

            Console.WriteLine(tempValue8);

            Console.WriteLine();
            Console.WriteLine("<Press enter to continue.>");
            Console.ReadLine();
        }
    }
}
