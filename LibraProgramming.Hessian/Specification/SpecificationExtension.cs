namespace LibraProgramming.Hessian.Specification
{
    public static class SpecificationExtension
    {
        public static ISpecification<TParam> And<TParam>(this ISpecification<TParam> left, ISpecification<TParam> right)
        {
            return new AndSpecification<TParam>(left, right);
        }

        public static ISpecification<TParam> Or<TParam>(this ISpecification<TParam> left, ISpecification<TParam> right)
        {
            return new OrSpecification<TParam>(left, right);
        }
    }
}