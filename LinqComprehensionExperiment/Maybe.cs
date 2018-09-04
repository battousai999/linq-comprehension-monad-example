using System;
using System.Collections.Generic;
using System.Text;

namespace LinqComprehensionExperiment
{
    public class Maybe<T>
    {
        private readonly T value;

        public static Maybe<T> Some(T value)
        {
            return new Maybe<T>(value);
        }

        public static Maybe<T> None()
        {
            return new Maybe<T>();
        }

        private Maybe()
        {
            HasValue = false;
        }

        private Maybe(T value)
        {
            if (value == null)
                throw new InvalidOperationException("Null value not allowed in Maybe constructor.");

            HasValue = true;
            this.value = value;
        }

        public T Value
        {
            get
            {
                if (!HasValue)
                    throw new InvalidOperationException("Cannot access value for a None Maybe.");

                return value;
            }
        }

        public bool HasValue { get; }

        public override string ToString()
        {
            return (HasValue ? $"Some({Value})" : "None");
        }

        // Monadic methods

        public static Maybe<T> Return(T value)
        {
            return Some(value);
        }

        public static Maybe<B> Bind<A, B>(Maybe<A> a, Func<A, Maybe<B>> func)
        {
            return (a.HasValue ? func(a.Value) : Maybe<B>.None());
        }

        public Maybe<B> Bind<B>(Func<T, Maybe<B>> func)
        {
            return Maybe<B>.Bind(this, func);
        }
    }

    public static class MaybeExtensions
    {
        // SelectMany implementation to allow Maybe<T> to be used with Linq query compositions
        public static Maybe<C> SelectMany<A, B, C>(this Maybe<A> a, Func<A, Maybe<B>> func, Func<A, B, C> select)
        {
            var tempValue = a.Bind(func);

            if (!tempValue.HasValue)
                return Maybe<C>.None();

            return Maybe<C>.Return(select(a.Value, tempValue.Value));
        }
    }
}
