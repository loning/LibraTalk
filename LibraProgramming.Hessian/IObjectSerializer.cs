using System;

namespace LibraProgramming.Hessian
{
    /// <summary>
    /// 
    /// </summary>
    public interface IObjectSerializer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool CanHandle(Type type);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="graph"></param>
        void Serialize(HessianOutputWriter writer, object graph);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        object Deserialize(HessianInputReader reader);
    }
}