using System.Collections;
using System.Collections.Generic;

namespace JustinCredible.SIEmulator.Tests
{
    public class RegistersClassData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object [] { RegisterID.A };
            yield return new object [] { RegisterID.B };
            yield return new object [] { RegisterID.C };
            yield return new object [] { RegisterID.D };
            yield return new object [] { RegisterID.E };
            yield return new object [] { RegisterID.H };
            yield return new object [] { RegisterID.L };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
