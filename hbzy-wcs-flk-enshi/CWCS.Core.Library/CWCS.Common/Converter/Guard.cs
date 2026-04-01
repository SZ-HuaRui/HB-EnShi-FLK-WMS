using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CWCS.Common.Converter
{
    public static class Guard
    {
        public static void NullReference(object obj, string message = null)
        {
            if (obj == null)
            {
                if (string.IsNullOrEmpty(message))
                {
                    message = SR.GetString("NullReference", new object[0]);
                }
                throw new ArgumentException(message);
            }
        }

        public static void ArgumentNull(object obj, string paramName, string message = null)
        {
            if (obj == null)
            {
                if (string.IsNullOrEmpty(message))
                {
                    message = SR.GetString("ArgumentNull", new object[]
					{
						paramName
					});
                }
                throw new ArgumentNullException(message, paramName);
            }
        }

        public static void Argument(bool predicate, string paramName, string message = null)
        {
            if (!predicate)
            {
                if (string.IsNullOrEmpty(message))
                {
                    message = SR.GetString("ArgumentInvalid", new object[0]);
                }
                throw new ArgumentException(message, paramName);
            }
        }

        public static void OutOfRange(int range, int index, string message = null)
        {
            if (index < 0 || (range > -2147483648 && index > range - 1))
            {
                if (string.IsNullOrEmpty(message))
                {
                    message = SR.GetString("ArgumentOutOfRange", new object[0]);
                }
                throw new ArgumentOutOfRangeException(message);
            }
        }

        public static void Assert(bool condition, Exception exception)
        {
            if (!condition && exception != null)
            {
                throw exception;
            }
        }
    }
}
