namespace LibraProgramming.Hessian.Specification
{
    public class OrSpecification<TParam> : BinarySpecification<TParam>
    {
        public OrSpecification(ISpecification<TParam> left, ISpecification<TParam> right)
            : base(left, right)
        {
        }

        public override bool IsSatisfied(TParam arg)
        {
            return Left.IsSatisfied(arg) || Right.IsSatisfied(arg);
        }
    }
}