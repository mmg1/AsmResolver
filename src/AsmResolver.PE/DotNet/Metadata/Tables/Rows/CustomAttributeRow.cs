// AsmResolver - Executable file format inspection library 
// Copyright (C) 2016-2019 Washi
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 3.0 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA

namespace AsmResolver.PE.DotNet.Metadata.Tables.Rows
{
    /// <summary>
    /// Represents a single row in the custom attribute metadata table.
    /// </summary>
    public readonly struct CustomAttributeRow : IMetadataRow
    {
        /// <summary>
        /// Reads a single custom attribute row from an input stream.
        /// </summary>
        /// <param name="reader">The input stream.</param>
        /// <param name="layout">The layout of the custom attribute table.</param>
        /// <returns>The row.</returns>
        public static CustomAttributeRow FromReader(IBinaryStreamReader reader, TableLayout layout)
        {
            return new CustomAttributeRow(
                reader.ReadIndex((IndexSize) layout.Columns[0].Size),
                reader.ReadIndex((IndexSize) layout.Columns[1].Size),
                reader.ReadIndex((IndexSize) layout.Columns[2].Size));
        }

        public CustomAttributeRow(uint parent, uint type, uint value)
        {
            Parent = parent;
            Type = type;
            Value = value;
        }

        /// <inheritdoc />
        public TableIndex TableIndex => TableIndex.CustomAttribute;

        /// <summary>
        /// Gets a HasCustomAttribute index (an index into either the Method, Field, TypeRef, TypeDef,
        /// Param, InterfaceImpl, MemberRef, Module, DeclSecurity, Property, Event, StandAloneSig, ModuleRef,
        /// TypeSpec, Assembly, AssemblyRef, File, ExportedType, ManifestResource, GenericParam, GenericParamConstraint,
        /// or MethodSpec table) that this attribute is assigned to.
        /// </summary>
        public uint Parent
        {
            get;
        }

        /// <summary>
        /// Gets and CustomAttributeType index (an index into either the Method or MemberRef table) defining the
        /// constructor to call when initializing the custom attribute.
        /// </summary>
        public uint Type
        {
            get;
        }

        /// <summary>
        /// Gets an index into the #Blob stream containing the arguments of the constructor call.
        /// </summary>
        public uint Value
        {
            get;
        }

        /// <summary>
        /// Determines whether this row is considered equal to the provided custom attribute row.
        /// </summary>
        /// <param name="other">The other row.</param>
        /// <returns><c>true</c> if the rows are equal, <c>false</c> otherwise.</returns>
        public bool Equals(CustomAttributeRow other)
        {
            return Parent == other.Parent && Type == other.Type && Value == other.Value;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is CustomAttributeRow other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int) Parent;
                hashCode = (hashCode * 397) ^ (int) Type;
                hashCode = (hashCode * 397) ^ (int) Value;
                return hashCode;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"({Parent:X8}, {Type:X8}, {Value:X8})";
        }
        
    }
}