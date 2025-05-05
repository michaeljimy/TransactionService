using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Numerics;

namespace Transaction_Service.Utils
{
    public class BigIntegerToStringConverter : ValueConverter<BigInteger, string>
    {
        public BigIntegerToStringConverter()
            : base(
                bigInt => bigInt.ToString(),
                str => BigInteger.Parse(str))
        { }
    }

}
