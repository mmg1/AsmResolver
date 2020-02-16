using System.Collections.Generic;

namespace AsmResolver.DotNet.Signatures
{
    public partial class SignatureComparer :
        IEqualityComparer<IMethodDescriptor>,
        IEqualityComparer<IFieldDescriptor>
    {
        /// <inheritdoc />
        public bool Equals(IMethodDescriptor x, IMethodDescriptor y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            return x.Name == y.Name
                   && Equals(x.DeclaringType, y.DeclaringType)
                   && Equals(x.Signature, y.Signature);
        }

        /// <inheritdoc />
        public int GetHashCode(IMethodDescriptor obj)
        {
            unchecked
            {
                int hashCode = obj.Name == null ? 0 : obj.Name.GetHashCode();
                hashCode = (hashCode * 397) ^ GetHashCode(obj.DeclaringType);
                hashCode = (hashCode * 397) ^ GetHashCode(obj.Signature);
                return hashCode;
            }
        }

        /// <inheritdoc />
        public bool Equals(IFieldDescriptor x, IFieldDescriptor y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            return x.Name == y.Name
                   && Equals(x.DeclaringType, y.DeclaringType)
                   && Equals(x.Signature, y.Signature);
        }

        /// <inheritdoc />
        public int GetHashCode(IFieldDescriptor obj)
        {
            unchecked
            {
                int hashCode = obj.Name == null ? 0 : obj.Name.GetHashCode();
                hashCode = (hashCode * 397) ^ GetHashCode(obj.DeclaringType);
                hashCode = (hashCode * 397) ^ GetHashCode(obj.Signature);
                return hashCode;
            }
        }
    }
}