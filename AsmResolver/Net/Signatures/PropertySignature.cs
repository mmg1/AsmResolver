﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsmResolver.Net.Metadata;

namespace AsmResolver.Net.Signatures
{
    public class PropertySignature : BlobSignature, IHasTypeSignature
    {
        public static PropertySignature FromReader(MetadataHeader header, IBinaryStreamReader reader)
        {
            var signature = new PropertySignature
            {
                Attributes = (PropertySignatureAttributes)reader.ReadByte(),
            };

            var paramCount = reader.ReadCompressedUInt32();
            signature.PropertyType = TypeSignature.FromReader(header, reader);

            for (int i = 0; i < paramCount; i++)
                signature.Parameters.Add(ParameterSignature.FromReader(header, reader));

            return signature;
        }

        public PropertySignature()
        {
            Parameters = new List<ParameterSignature>();
        }

        public PropertySignatureAttributes Attributes
        {
            get;
            set;
        }

        public TypeSignature PropertyType
        {
            get;
            set;
        }

        TypeSignature IHasTypeSignature.TypeSignature
        {
            get { return PropertyType; }
        }

        public IList<ParameterSignature> Parameters
        {
            get;
            set;
        }

        public override uint GetPhysicalLength()
        {
            return (uint)(sizeof (byte) +
                          Parameters.Count.GetCompressedSize() +
                          PropertyType.GetPhysicalLength() +
                          Parameters.Sum(x => x.GetPhysicalLength()));
        }

        public override void Write(WritingContext context)
        {
            var writer = context.Writer;
            writer.WriteByte((byte)Attributes);
            writer.WriteCompressedUInt32((uint)Parameters.Count);
            PropertyType.Write(context);
            foreach (var parameter in Parameters)
                parameter.Write(context);
        }
    }

    [Flags]
    public enum PropertySignatureAttributes
    {
        None = 0,
        HasThis = 0x20,
    }
}