using CWCS.DataAccess.UserException;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace CWCS.DataAccess.DataProvider
{
    internal class ExceptionHelper
    {
        internal static string ThrowSyntaxCreteException(DbType dbType)
        {
            return string.Empty;
        }

        internal static string ThrowSyntaxConvertException(DbType dbType)
        {
            return string.Empty;
        }

        internal static string ThrowSyntaxParseException(string methodName)
        {
            throw new Exception(methodName);
        }
    }
}
