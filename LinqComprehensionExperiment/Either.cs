using System;
using System.Collections.Generic;
using System.Text;

namespace LinqComprehensionExperiment
{
    public class Either<L, R>
    {
        private readonly bool isLeft;
        private readonly L left;
        private readonly R right;

        private Either(bool isLeft, L left, R right)
        {
            this.isLeft = isLeft;
            this.left = left;
            this.right = right;
        }

        public static Either<L, R> Left<L, R>(L value)
        {
            return new Either<L, R>(true, value, default(R));
        }

        public static Either<L, R> Right<L, R>(R value)
        {
            return new Either<L, R>(false, default(L), value);
        }

        public bool IsLeft => isLeft;
        public bool IsRight => !isLeft;
        
        public L LeftValue
        {
            get
            {
                if (IsRight)
                    throw new InvalidOperationException("Cannot get Left value from a Right Either.");

                return left;
            }
        }

        public R RightValue
        {
            get
            {
                if (IsLeft)
                    throw new InvalidOperationException("Cannot get Right value from a Left Either.");

                return right;
            }
        }

        public override string ToString()
        {
            return (IsLeft ? $"Left({LeftValue})" : $"Right({RightValue})");
        }

        // Monadic methods

        public static Either<L, R> Return<L, R>(R value)
        {
            return Either<L, R>.Right<L, R>(value);
        }

        public static Either<L, R2> Bind<L, R, R2>(Either<L, R> a, Func<R, Either<L, R2>> func)
        {
            if (a.IsLeft)
                return Either<L, R2>.Left<L, R2>(a.LeftValue);

            return func(a.RightValue);
        }

        public Either<L, R2> Bind<R2>(Func<R, Either<L, R2>> func)
        {
            return Bind(this, func);
        }
    }

    /*
     *  Monads (and types that can participate with Linq query compositions) must be of "kind" * -> *.
     *  Since Either<L, R> is of kind * -> * -> *, one of its type parameters must be fixed in order to be of kind
     *  * -> *.
     */
    public class EitherMonad<R>
    {
        private Either<string, R> either;

        private EitherMonad(bool isLeft, string left, R right)
        {
            if (isLeft)
                either = Either<string, R>.Left<string, R>(left);
            else
                either = Either<string, R>.Right<string, R>(right);
        }

        public static EitherMonad<R> Left(string value)
        {
            return new EitherMonad<R>(true, value, default(R));
        }

        public static EitherMonad<R> Right<R>(R value)
        {
            return new EitherMonad<R>(false, null, value);
        }

        public bool IsLeft => either.IsLeft;
        public bool IsRight => either.IsRight;

        public string LeftValue => either.LeftValue;
        public R RightValue => either.RightValue;

        public override string ToString()
        {
            return either.ToString();
        }

        // Monadic methods

        public static EitherMonad<R> Return<R>(R value)
        {
            return EitherMonad<R>.Right(value);
        }

        public static EitherMonad<R2> Bind<R, R2>(EitherMonad<R> a, Func<R, EitherMonad<R2>> func)
        {
            if (a.IsLeft)
                return EitherMonad<R2>.Left(a.LeftValue);

            return func(a.RightValue);
        }

        public EitherMonad<R2> Bind<R2>(Func<R, EitherMonad<R2>> func)
        {
            return EitherMonad<R2>.Bind(this, func);
        }
    }

    public static class EitherExtensions
    {
        // SelectMany implementation to allow EitherMonad<R> to be used with Linq query compositions
        public static EitherMonad<C> SelectMany<A, B, C>(this EitherMonad<A> a, Func<A, EitherMonad<B>> func, Func<A, B, C> select)
        {
            var b = a.Bind(func);

            if (b.IsLeft)
                return EitherMonad<C>.Left(b.LeftValue);

            return EitherMonad<C>.Return(select(a.RightValue, b.RightValue));
        }
    }
}
