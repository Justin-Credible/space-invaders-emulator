using System.Collections;
using System.Collections.Generic;

namespace JustinCredible.SIEmulator.Tests
{
    public class RegisterPermutationsClassData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object [] { RegisterID.A, RegisterID.A };
            yield return new object [] { RegisterID.A, RegisterID.B };
            yield return new object [] { RegisterID.A, RegisterID.C };
            yield return new object [] { RegisterID.A, RegisterID.D };
            yield return new object [] { RegisterID.A, RegisterID.E };
            yield return new object [] { RegisterID.A, RegisterID.H };
            yield return new object [] { RegisterID.A, RegisterID.L };
            yield return new object [] { RegisterID.B, RegisterID.A };
            yield return new object [] { RegisterID.B, RegisterID.B };
            yield return new object [] { RegisterID.B, RegisterID.C };
            yield return new object [] { RegisterID.B, RegisterID.D };
            yield return new object [] { RegisterID.B, RegisterID.E };
            yield return new object [] { RegisterID.B, RegisterID.H };
            yield return new object [] { RegisterID.B, RegisterID.L };
            yield return new object [] { RegisterID.C, RegisterID.A };
            yield return new object [] { RegisterID.C, RegisterID.B };
            yield return new object [] { RegisterID.C, RegisterID.C };
            yield return new object [] { RegisterID.C, RegisterID.D };
            yield return new object [] { RegisterID.C, RegisterID.E };
            yield return new object [] { RegisterID.C, RegisterID.H };
            yield return new object [] { RegisterID.C, RegisterID.L };
            yield return new object [] { RegisterID.D, RegisterID.A };
            yield return new object [] { RegisterID.D, RegisterID.B };
            yield return new object [] { RegisterID.D, RegisterID.C };
            yield return new object [] { RegisterID.D, RegisterID.D };
            yield return new object [] { RegisterID.D, RegisterID.E };
            yield return new object [] { RegisterID.D, RegisterID.H };
            yield return new object [] { RegisterID.D, RegisterID.L };
            yield return new object [] { RegisterID.E, RegisterID.A };
            yield return new object [] { RegisterID.E, RegisterID.B };
            yield return new object [] { RegisterID.E, RegisterID.C };
            yield return new object [] { RegisterID.E, RegisterID.D };
            yield return new object [] { RegisterID.E, RegisterID.E };
            yield return new object [] { RegisterID.E, RegisterID.H };
            yield return new object [] { RegisterID.E, RegisterID.L };
            yield return new object [] { RegisterID.H, RegisterID.A };
            yield return new object [] { RegisterID.H, RegisterID.B };
            yield return new object [] { RegisterID.H, RegisterID.C };
            yield return new object [] { RegisterID.H, RegisterID.D };
            yield return new object [] { RegisterID.H, RegisterID.E };
            yield return new object [] { RegisterID.H, RegisterID.H };
            yield return new object [] { RegisterID.H, RegisterID.L };
            yield return new object [] { RegisterID.L, RegisterID.A };
            yield return new object [] { RegisterID.L, RegisterID.B };
            yield return new object [] { RegisterID.L, RegisterID.C };
            yield return new object [] { RegisterID.L, RegisterID.D };
            yield return new object [] { RegisterID.L, RegisterID.E };
            yield return new object [] { RegisterID.L, RegisterID.H };
            yield return new object [] { RegisterID.L, RegisterID.L };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
