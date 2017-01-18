namespace CoolMonads
{
    using System;

    public abstract partial class Option<T>
    {
        public static readonly Option<T> None = new ChoiceTypes.None();
        public static Option<T> NewSome(T t) => new ChoiceTypes.Some(t);
        private Option()
        {
        }

        public abstract TResult Match<TResult>(Func<TResult> noneFunc, Func<T, TResult> someFunc);
        private static class ChoiceTypes
        {
            public class None : Option<T>
            {
                public override TResult Match<TResult>(Func<TResult> noneFunc, Func<T, TResult> someFunc) => noneFunc();
            }

            public class Some : Option<T>
            {
                public Some(T item)
                {
                    Item = item;
                }

                private T Item
                {
                    get;
                }

                public override TResult Match<TResult>(Func<TResult> noneFunc, Func<T, TResult> someFunc) => someFunc(Item);
            }
        }
    }
}