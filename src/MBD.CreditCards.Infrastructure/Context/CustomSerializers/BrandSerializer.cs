using System;
using System.Diagnostics.CodeAnalysis;
using MBD.CreditCards.Domain.Enumerations;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace MBD.CreditCards.Infrastructure.Context.CustomSerializers
{
    [ExcludeFromCodeCoverage]
    public class BrandSerializer : SerializerBase<Brand>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Brand value)
        {
            context.Writer.WriteString(value.ToString());
        }

        public override Brand Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return Enum.Parse<Brand>(context.Reader.ReadString());
        }
    }
}