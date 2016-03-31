namespace LibraProgramming.Hessian.Specification
{
    public interface ISpecification<in TParam>
    {
        bool IsSatisfied(TParam arg);
    }
}