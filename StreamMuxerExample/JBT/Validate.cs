using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;

namespace RevolutionaryStuff.JBT
{
    public static class Validate
    {
        public static void SingleCall(ref bool alreadyCalled)
        {
            if (alreadyCalled) throw new SingleCallException();
            alreadyCalled = true;
        }

        public static void SingleCall(ref int incrementer)
        {
            if (1 != System.Threading.Interlocked.Increment(ref incrementer)) throw new SingleCallException();
        }

        public static void NonNegative(double arg, string argName)
        {
            if (arg < 0) throw new ArgumentOutOfRangeException("Must be >= 0", argName);
        }

        public static void NonNegative(long arg, string argName)
        {
            if (arg < 0) throw new ArgumentOutOfRangeException("Must be >= 0", argName);
        }

        public static void NonNullArg(object arg, string argName)
        {
            if (null == arg) throw new ArgumentNullException(argName);
        }

        public static void Between(double arg, string argName, double min, double max)
        {
            if (arg < min || arg > max)
            {
                throw new ArgumentOutOfRangeException(
                    argName,
                    arg,
                    string.Format("{0} must be between {1} and {2}", argName, min, max)
                    );
            }
        }

        public static void StringArg(string arg, string argName)
        {
            StringArg(arg, argName, false, 1, int.MaxValue);
        }

        public static void ObjIsType(object testObj, string objName, Type isType)
        {
            Validate.NonNullArg(testObj, objName);
            IsType(testObj.GetType(), isType);
        }

        public static void IsType(Type testType, Type isType)
        {
            Validate.NonNullArg(testType, "testType");
            Validate.NonNullArg(isType, "isType");
            if (isType.IsAssignableFrom(testType)) return;
            throw new ArgumentException(string.Format("{0} is not a {1}", testType, isType));
        }

        public static void StringArg(string arg, string argName, bool nullable, int minLen, int maxLen)
        {
            if (minLen > maxLen) throw new ArgumentException("minLen cannot be > than maxLen");
            if (!nullable && null == arg) throw new ArgumentNullException(argName);
            if (arg.Length < minLen) throw new ArgumentException(string.Format("{0} must be at least {1} chars", argName, minLen), argName);
            if (arg.Length > maxLen) throw new ArgumentException(string.Format("{0} must be lenn than {1} chars", argName, maxLen), argName);
        }

        public static void ArrayArg(IList arg, int offset, int size)
        {
            ArrayArg(arg, offset, size, 0, null, false);
        }

        public static void CollectionHasData(ICollection arg, string argName)
        {
            Validate.NonNullArg(arg, argName);
            if (arg.Count == 0) throw new ArgumentException(string.Format("{0} must have at least 1 member", argName), argName);
        }

        public static void ArrayArg(IList arg, int offset, int size, int minSize, string argName, bool nullable)
        {
            if (argName == null) argName = "";
            if (!nullable && null == arg) throw new ArgumentNullException(argName);
            if (size < minSize) throw new ArgumentException(string.Format("size must be >= {0}", minSize));
            if (offset < 0) throw new ArgumentException("offset must be >= 0");
            if (size + offset > arg.Count) throw new ArgumentException(string.Format("size+offset must be <= {0}.Count", argName));
        }

        public static void FileExists(string arg, string argName)
        {
            Validate.StringArg(arg, argName);
            if (!File.Exists(arg)) throw new ArgumentException(string.Format("File=[{0}] does not exist", arg), argName);
        }

        public static void DirectoryExists(string arg, string argName)
        {
            Validate.StringArg(arg, argName);
            if (!Directory.Exists(arg)) throw new ArgumentException(string.Format("Directory=[{0}] does not exist", arg), argName);
        }

        public static void ConnectionIsOpen(IDbConnection conn, string argName)
        {
            Validate.NonNullArg(conn, argName);
            if (conn.State != ConnectionState.Open) throw new InvalidOperationException("The connection must be open");
        }

        public static void TransactionHasBegun(IDbTransaction trans, string argName)
        {
            Validate.NonNullArg(trans, argName);
            Validate.ConnectionIsOpen(trans.Connection, argName + ".Connection");
        }

        public static void StreamArg(Stream stream, string argName, bool mustBeReadable, bool mustBeWriteable, bool mustBeSeekable)
        {
            Validate.NonNullArg(stream, argName);
            if (mustBeReadable && !stream.CanRead) throw new ArgumentException("Cannot read from this stream", argName);
            if (mustBeWriteable && !stream.CanWrite) throw new ArgumentException("Cannot write to this stream", argName);
            if (mustBeSeekable && !stream.CanSeek) throw new ArgumentException("Cannot seek in this stream", argName);
        }

        public static void WriteableStreamArg(Stream stream, string argName)
        {
            StreamArg(stream, argName, false, true, false);
        }

        public static void WriteableStreamArg(Stream stream, string argName, bool mustBeSeekable)
        {
            StreamArg(stream, argName, false, true, mustBeSeekable);
        }

        public static void ReadableStreamArg(Stream stream, string argName)
        {
            StreamArg(stream, argName, true, false, false);
        }

        public static void ReadableStreamArg(Stream stream, string argName, bool mustBeSeekable)
        {
            StreamArg(stream, argName, true, false, mustBeSeekable);
        }

        public static void NotReadonly(bool isReadonly)
        {
            if (isReadonly) throw new JBT.ReadOnlyException();
        }

        public static void Handle(IntPtr ip, string argName)
        {
            Handle(ip, argName, false);
        }
        public static void Handle(IntPtr ip, string argName, bool neg1Ok)
        {
            if (ip == IntPtr.Zero || !neg1Ok && (int)ip == -1) throw new ArgumentException("Handle is not valid", argName);
        }
    }
}
