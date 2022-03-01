using BarbellTracker.AbstractionCode;
using System;
using System.Collections.Generic;
using Xunit;

namespace BarbellTracker.AbstractionCodeTests
{
    public class Vector2DTester
    {

        [Theory]
        [MemberData(nameof(GetTestdataForVectorAddition))]
        public void StaticAdd_TowVectors_ReturnSumOfVecors(Vector2D first, Vector2D second, Vector2D expected)
        {
            var result = Vector2D.Add(first, second);

            Assert.Equal(expected, result);
        }

        [Theory]
        [MemberData(nameof(GetTestdataForVectorAddition))]
        public void Add_TowVectors_ReturnSumOfVecors(Vector2D first, Vector2D second, Vector2D expected)
        {
            first.Add(second);

            Assert.Equal(expected, first);
        }




        [Theory]
        [MemberData(nameof(GetTestdataForVectorSubtraction))]
        public void StaticSub_OfTowVectors_ReturnDifferenceOfVecors(Vector2D first, Vector2D second, Vector2D expected)
        {
            var result = Vector2D.Sub(first, second);

            Assert.Equal(expected, result);
        }

        [Theory]
        [MemberData(nameof(GetTestdataForVectorSubtraction))]
        public void Sub_OfTowVectors_ReturnDifferenceOfVecors(Vector2D first, Vector2D second, Vector2D expected)
        {
            first.Sub(second);

            Assert.Equal(expected, first);
        }




        [Theory]
        [MemberData(nameof(GetTestdataForVectorDotProduct))]
        public void StaticDotProduct_OfTowVectors_ReturnTheirScalar(Vector2D first, Vector2D second, double expected)
        {
            var scalar = Vector2D.dotProduct(first, second);

            Assert.Equal(expected, scalar);
        }

        [Theory]
        [MemberData(nameof(GetTestdataForVectorDotProduct))]
        public void DotProduct_OfTowVectors_ReturnTheirScalar(Vector2D first, Vector2D second, double expected)
        {
            var scalar = first.dotProduct(second);

            Assert.Equal(expected, scalar);
        }


        [Theory]
        [MemberData(nameof(GetTestdataForVectorCrossProduct))]
        public void StaticCrossProduct_OfTowVectors_ReturnTheirCrossProduct(Vector2D first, Vector2D second, double expected)
        {
            var scalar = Vector2D.crossProduct(first, second);

            Assert.Equal(expected, scalar);
        }


        [Theory]
        [MemberData(nameof(GetTestdataForVectorLength))]
        public void Length_OfVectorReturnTheLength(Vector2D vector, double expectedLength)
        {
            var length = vector.length();
            int precisionOfDecimalPlaces = 3;


            Assert.Equal(expectedLength, length, precisionOfDecimalPlaces);
        }

        [Theory]
        [MemberData(nameof(GetTestdataForVectorNormalize))]
        public void Normalize_AVector_SacletheVectorToTheLengthOfOne(Vector2D vector)
        {
            vector.normalize();
            var length = vector.length();
            var precisionOfDecimalPlaces = 1;


            Assert.Equal(1, length, precisionOfDecimalPlaces);
        }

        [Fact]
        public void Normalize_AVectorTheLengthOfzero_ThrowDivideByZeroException()
        {

            var vector = new Vector2D(0, 0);

            Action normalize = () => vector.normalize();


            Assert.Throws<DivideByZeroException>(normalize);
        }


        [Fact]
        public void Copy_AVector_WillReturnAEqualVector()
        {
            var vector = new Vector2D(4,2);

            var copy = vector.Copy();

            Assert.Equal(vector, copy);
        }

        [Fact]
        public void Copy_AVector_WillReturnAEqualVectorWithANewReference()
        {
            var vector = new Vector2D(4, 2);

            var copy = vector.Copy();

            var sameReference = object.ReferenceEquals(vector, copy);

            Assert.False(sameReference);
        }


        [Theory]
        [MemberData(nameof(GetTestdataForVectorToString))]
        public void ToString_OfAVector_WillReturnTheCorrespondingString(Vector2D vector, string expected)
        {
            var actual = vector.ToString();


            Assert.Equal(expected,actual);
        }




        [Theory]
        [MemberData(nameof(GetTestdataForVectorIslinearlyIndependen))]

        public void IslinearlyIndependen_OfTowLinearlyIndependenVectors_WillReturnTheTrue(Vector2D first, Vector2D second)
        {
            var islinearlyIndependen = first.islinearlyIndependen(second);


            Assert.True(islinearlyIndependen);
        }


        public static IEnumerable<object[]> GetTestdataForVectorIslinearlyIndependen()
        {
            yield return new object[] { new Vector2D(0, 1), new Vector2D(0, 2) };
            yield return new object[] { new Vector2D(0, 1), new Vector2D(0, 1.5) };

            yield return new object[] { new Vector2D(1, 0), new Vector2D(2, 0) };
            yield return new object[] { new Vector2D(1, 0), new Vector2D(1.5, 0) };

            yield return new object[] { new Vector2D(1, 1), new Vector2D(2, 2) };
            yield return new object[] { new Vector2D(1, 1), new Vector2D(1.5, 1.5) };

            yield return new object[] { new Vector2D(1, 1), new Vector2D(int.MaxValue, int.MaxValue) };
            yield return new object[] { new Vector2D(1, 1), new Vector2D(int.MinValue, int.MinValue) };
        }

        public static IEnumerable<object[]> GetTestdataForVectorAddition()
        {
            yield return new object[] { new Vector2D(0, 0), new Vector2D(0, 0), new Vector2D(0, 0) };
            yield return new object[] { new Vector2D(1, 1), new Vector2D(0, 0), new Vector2D(1, 1) };
            yield return new object[] { new Vector2D(0, 0), new Vector2D(1, 1), new Vector2D(1, 1) };
            yield return new object[] { new Vector2D(1, 1), new Vector2D(2, 2), new Vector2D(3, 3) };
            yield return new object[] { new Vector2D(1, 1), new Vector2D(-2, -2), new Vector2D(-1, -1) };
            yield return new object[] { new Vector2D(int.MaxValue, int.MaxValue), new Vector2D(int.MinValue, int.MinValue), new Vector2D(-1, -1) };
        }
        public static IEnumerable<object[]> GetTestdataForVectorSubtraction()
        {
            yield return new object[] { new Vector2D(0, 0), new Vector2D(0, 0), new Vector2D(0, 0) };
            yield return new object[] { new Vector2D(1, 1), new Vector2D(0, 0), new Vector2D(1, 1) };
            yield return new object[] { new Vector2D(0, 0), new Vector2D(1, 1), new Vector2D(-1, -1) };
            yield return new object[] { new Vector2D(2, 2), new Vector2D(1, 1), new Vector2D(1, 1) };
            yield return new object[] { new Vector2D(1, 1), new Vector2D(-2, -2), new Vector2D(3, 3) };
            yield return new object[] { new Vector2D(-2, -2), new Vector2D(1,1), new Vector2D(-3, -3) };
        }
        public static IEnumerable<object[]> GetTestdataForVectorDotProduct()
        {
            yield return new object[] { new Vector2D(0, 0), new Vector2D(0, 0), 0 };
            yield return new object[] { new Vector2D(1, 1), new Vector2D(0, 0), 0 };
            yield return new object[] { new Vector2D(0, 0), new Vector2D(1, 1), 0 };
            yield return new object[] { new Vector2D(2, 2), new Vector2D(1, 1), 4 };
            yield return new object[] { new Vector2D(2, 3), new Vector2D(4, 5), 23 };
            yield return new object[] { new Vector2D(-2, 3), new Vector2D(4, 5), 7 };
            yield return new object[] { new Vector2D(2, -3), new Vector2D(4, 5), -7 };
            yield return new object[] { new Vector2D(-2, -3), new Vector2D(4, 5), -23 };

        }
        public static IEnumerable<object[]> GetTestdataForVectorCrossProduct()
        {
            yield return new object[] { new Vector2D(0, 0), new Vector2D(0, 0), 0 };
            yield return new object[] { new Vector2D(1, 1), new Vector2D(0, 0), 0 };
            yield return new object[] { new Vector2D(0, 0), new Vector2D(1, 1), 0 };
            yield return new object[] { new Vector2D(2, 2), new Vector2D(1, 1), 0 };

            yield return new object[] { new Vector2D(2, 3), new Vector2D(4, 5), -2 };
            yield return new object[] { new Vector2D(4, 5), new Vector2D(2, 3),  2 };

            yield return new object[] { new Vector2D(-2, 3), new Vector2D(4, 5), -22 };
            yield return new object[] { new Vector2D(4, 5), new Vector2D(-2, 3),  22 };

            yield return new object[] { new Vector2D(2, -3), new Vector2D(4, 5), 22 };
            yield return new object[] { new Vector2D(4, 5), new Vector2D(2, -3), -22 };

            yield return new object[] { new Vector2D(-2, -3), new Vector2D(4, 5), 2 };
            yield return new object[] { new Vector2D(4, 5), new Vector2D(-2, -3),  -2 };
        }
        public static IEnumerable<object[]> GetTestdataForVectorLength()
        {
            yield return new object[] { new Vector2D(0, 0), 0 };
            yield return new object[] { new Vector2D(5, 0), 5 };
            yield return new object[] { new Vector2D(0, 5), 5 };
            yield return new object[] { new Vector2D(5, 10), 11.180 };
            yield return new object[] { new Vector2D(5, -10), 11.180 };
            yield return new object[] { new Vector2D(-5, 10), 11.180 };
            yield return new object[] { new Vector2D(-5, -10), 11.180 };

        }

        public static IEnumerable<object[]> GetTestdataForVectorNormalize()
        {
            yield return new object[] { new Vector2D(5, 0)};
            yield return new object[] { new Vector2D(0, 5)};
            yield return new object[] { new Vector2D(5, 10)};
            yield return new object[] { new Vector2D(5, -10)};
            yield return new object[] { new Vector2D(-5, 10)};
            yield return new object[] { new Vector2D(-5, -10)};

        }

        public static IEnumerable<object[]> GetTestdataForVectorToString()
        {
            yield return new object[] { new Vector2D(0, 0), "(X: 0, Y: 0)" };
            yield return new object[] { new Vector2D(1, 2), "(X: 1, Y: 2)" };
            yield return new object[] { new Vector2D(-1, -2), "(X: -1, Y: -2)" };
            yield return new object[] { new Vector2D(1.2345, -1.2345), "(X: 1,235, Y: -1,235)" };

        }

       


    }
}