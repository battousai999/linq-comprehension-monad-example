# linq-comprehension-monad-example
Just an example (in C#) of using Linq comprehension syntax with arbitrary monads (other than IEnumerable&lt;T>).

Note that the `SelectMany` implementations for `Maybe<T>` and `EitherMonad<R>` are not much more than a composition of their monadic `Bind()` and `Return()` methods.
