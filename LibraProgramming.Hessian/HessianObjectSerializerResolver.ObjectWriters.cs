using System;
using System.Reflection;

namespace LibraProgramming.Hessian
{
    public partial class HessianObjectSerializerResolver
    {
        /// <summary>
        /// 
        /// </summary>
        private class BooleanSerializer : IObjectSerializer
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            public bool CanHandle(Type type)
            {
                return typeof (bool) == type;
            }

            public void Serialize(HessianOutputWriter writer, object graph)
            {
                writer.WriteBoolean((bool) graph);
            }

            public object Deserialize(HessianInputReader reader)
            {
                return reader.ReadBoolean();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private class Int32Serializer : IObjectSerializer
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            public bool CanHandle(Type type)
            {
                var ti = type.GetTypeInfo();
                return typeof (int) == type || ti.IsEnum;
            }

            public void Serialize(HessianOutputWriter writer, object graph)
            {
                writer.WriteInt32((int) graph);
            }

            public object Deserialize(HessianInputReader reader)
            {
                return reader.ReadInt32();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private class Int64Serializer : IObjectSerializer
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            public bool CanHandle(Type type)
            {
                return typeof (long) == type;
            }

            public void Serialize(HessianOutputWriter writer, object graph)
            {
                writer.WriteInt64((long) graph);
            }

            public object Deserialize(HessianInputReader reader)
            {
                return reader.ReadInt64();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private class StringSerializer : IObjectSerializer
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            public bool CanHandle(Type type)
            {
                return typeof (string) == type;
            }

            public void Serialize(HessianOutputWriter writer, object graph)
            {
                writer.WriteString((string) graph);
            }

            public object Deserialize(HessianInputReader reader)
            {
                return reader.ReadString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private class DateTimeSerializer : IObjectSerializer
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            public bool CanHandle(Type type)
            {
                return typeof (DateTime) == type;
            }

            public void Serialize(HessianOutputWriter writer, object graph)
            {
                writer.WriteDateTime((DateTime) graph);
            }

            public object Deserialize(HessianInputReader reader)
            {
                return reader.ReadDateTime();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private class DoubleSerializer : IObjectSerializer
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            public bool CanHandle(Type type)
            {
                return typeof (double) == type;
            }

            public void Serialize(HessianOutputWriter writer, object graph)
            {
                writer.WriteDouble((double) graph);
            }

            public object Deserialize(HessianInputReader reader)
            {
                return reader.ReadDouble();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private class GuidSerializer : IObjectSerializer
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            public bool CanHandle(Type type)
            {
                return typeof (Guid) == type;
            }

            public void Serialize(HessianOutputWriter writer, object graph)
            {
                var bytes = ((Guid) graph).ToByteArray();
                writer.WriteBytes(bytes);
            }

            public object Deserialize(HessianInputReader reader)
            {
                var bytes = reader.ReadBytes();
                return new Guid(bytes);
            }
        }
    }
}