namespace LibraProgramming.Hessian.Specification
{
    public static class Specification
    {
        public static ISpecification<TParam> Not<TParam>(ISpecification<TParam> specification)
        {
            return new NotSpecification<TParam>(specification);
        }
    }
}