using System;

namespace LibraProgramming.Hessian
{
    /// <summary>
    /// 
    /// </summary>
    public interface IObjectSerializerResolver
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        IObjectSerializer GetSerializer(Type target);
    }
}