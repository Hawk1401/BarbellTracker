using BarbellTracker.AbstractionCode;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace BarbellTracker.AbstractionCodeTests
{
    public class Vector2DTester
    {

        [Theory]
        [MemberData(nameof(VectorAdditionWithResultEnumerable))]
        public void StaticAdd_TowVectors_ReturnSumOfVecors(Vector2D first, Vector2D second, Vector2D expected)
        {
            //Act
            var result = Vector2D.Add(first, second);

            //Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [MemberData(nameof(VectorAdditionWithResultEnumerable))]
        public void Add_TowVectors_ReturnSumOfVecors(Vector2D first, Vector2D second, Vector2D expected)
        {

            //Act
            var result = first.Add(second);

            //Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [MemberData(nameof(VectorSubtractionResultEnumerable))]
        public void StaticSub_OfTowVectors_ReturnDifferenceOfVecors(Vector2D first, Vector2D second, Vector2D expected)
        {
            //Act
            var result = Vector2D.Sub(first, second);

            //Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [MemberData(nameof(VectorSubtractionResultEnumerable))]
        public void Sub_OfTowVectors_ReturnDifferenceOfVecors(Vector2D first, Vector2D second, Vector2D expected)
        {
            //Act
            var result = first.Sub(second);

            //Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [MemberData(nameof(VectorDotProductResultEnumerable))]
        public void StaticDotProduct_OfTowVectors_ReturnTheirScalar(Vector2D first, Vector2D second, double expected)
        {
            //Act
            var scalar = Vector2D.DotProduct(first, second);

            //Assert
            Assert.Equal(expected, scalar);
        }

        [Theory]
        [MemberData(nameof(VectorDotProductResultEnumerable))]
        public void DotProduct_OfTowVectors_ReturnTheirScalar(Vector2D first, Vector2D second, double expected)
        {
            //Act
            var scalar = first.DotProduct(second);

            //Assert
            Assert.Equal(expected, scalar);
        }


        [Theory]
        [MemberData(nameof(VectorCrossProductResultEnumerable))]
        public void CrossProduct_OfTowVectors_ReturnTheirCrossProduct(Vector2D first, Vector2D second, double expected)
        {
            //Act
            var scalar = first.CrossProduct(second);

            //Assert
            Assert.Equal(expected, scalar);
        }


        [Theory]
        [MemberData(nameof(VectorCrossProductResultEnumerable))]
        public void StaticCrossProduct_OfTowVectors_ReturnTheirCrossProduct(Vector2D first, Vector2D second, double expected)
        {
            //Act
            var scalar = Vector2D.CrossProduct(first, second);

            //Assert
            Assert.Equal(expected, scalar);
        }

        [Theory]
        [MemberData(nameof(VectorWithLengthEnumerable))]
        public void Length_OfVectorReturnTheLength(Vector2D vector, double expectedLength)
        {
            //Act
            var length = vector.Length();

            //Assert
            int precisionOfDecimalPlaces = 3;
            Assert.Equal(expectedLength, length, precisionOfDecimalPlaces);
        }

        [Theory]
        [MemberData(nameof(VectorEnumerable))]
        public void Normalize_AVector_SacletheVectorToTheLengthOfOne(Vector2D vector)
        {
            //Act
            var result = vector.Normalize();
            var length = result.Length();

            //Assert
            var precisionOfDecimalPlaces = 1;
            Assert.Equal(1, length, precisionOfDecimalPlaces);
        }

        [Fact]
        public void Normalize_AVectorTheLengthOfzero_ThrowDivideByZeroException()
        {
            //Arrange
            var vector = new Vector2D(0, 0);

            //Act
            Action normalize = () => vector.Normalize();

            //Assert
            Assert.Throws<DivideByZeroException>(normalize);
        }


        [Fact]
        public void Copy_AVector_WillReturnAEqualVector()
        {
            //Arrange
            var vector = new Vector2D(4,2);

            //Act
            var copy = vector.Copy();

            //Assert
            Assert.Equal(vector, copy);
        }

        [Fact]
        public void Copy_AVector_WillReturnAVectorWithANewReference()
        {
            // Arrange
            var vector = new Vector2D(4, 2);

            //Act
            var copy = vector.Copy();

            // Assert
            var sameReference = object.ReferenceEquals(vector, copy);
            Assert.False(sameReference);
        }


        [Fact]
        public void Copy_AVectorUsingConstructor_WillReturnAEqualVector()
        {
            // Arrange
            var vector = new Vector2D(4, 2);

            //Act
            var copy = new Vector2D(vector);

            // Assert
            Assert.Equal(vector, copy);
        }

        [Fact]
        public void Copy_AVectorUsingConstructor_WillReturnAEqualVectorWithANewReference()
        {
            // Arrange
            var vector = new Vector2D(4, 2);

            //Act
            var copy = new Vector2D(vector);

            // Assert
            Assert.StrictEqual(vector, copy);
        }


        [Theory]
        [MemberData(nameof(VectorWithStringEnumerable))]
        public void ToString_OfAVector_WillReturnTheCorrespondingString(Vector2D vector, string expected)
        {
            //Act
            var actual = vector.ToString();

            // Assert
            Assert.Equal(expected,actual);
        }



        [Theory]
        [MemberData(nameof(TwolinearlyIndependenVectorsEnumerable))]
        public void IslinearlyIndependen_OfTowNotLinearlyIndependenVectors_WillReturnTheFalse(Vector2D first, Vector2D second)
        {
            //Act
            var islinearlyIndependen = first.IslinearlyIndependen(second);

            // Assert
            Assert.False(islinearlyIndependen);
        }

        [Theory]
        [MemberData(nameof(TwolinearlyIndependenVectorsEnumerable))]
        public void IslinearlyIndependen_OfTowNotLinearlyIndependenVectorsWithCustomEpsilon_WillReturnTheFalse(Vector2D first, Vector2D second)
        {
            //Act
            var Epsilon = 0.5d;
            var islinearlyIndependen = first.IslinearlyIndependen(second, Epsilon);

            // Assert
            Assert.False(islinearlyIndependen);
        }

        [Theory]
        [MemberData(nameof(TwolinearlyIndependenVectorsEnumerable))]
        public void StaticIslinearlyIndependen_OfTowNotLinearlyIndependenVectors_WillReturnTheFalse(Vector2D first, Vector2D second)
        {
            //Act
            var islinearlyIndependen = Vector2D.IslinearlyIndependen(first, second);

            // Assert
            Assert.False(islinearlyIndependen);
        }

        [Theory]
        [MemberData(nameof(TwolinearlyIndependenVectorsEnumerable))]
        public void StaticIslinearlyIndependen_OfTowNotLinearlyIndependenVectorsWithCustomEpsilon_WillReturnTheFalse(Vector2D first, Vector2D second)
        {
            //Act
            var Epsilon = 0.5d;
            var islinearlyIndependen = Vector2D.IslinearlyIndependen(first, second, Epsilon);

            // Assert
            Assert.False(islinearlyIndependen);
        }


        [Fact]
        public void Equals_OfNullAndAVector_willReturnFalse()
        {
            //Arrange
            var vector = new Vector2D();

            //Act
            var Result = vector.Equals(null);

            //Assert
            Assert.False(Result);
        }

        [Fact]
        public void Equals_OfaObjectAndAVector_willReturnFalse()
        {
            //Arrange
            var vector = new Vector2D();
            
            //Act
            var Result = vector.Equals(new object());
            
            //Assert
            Assert.False(Result);
        }

        [Theory]
        [InlineData(1,0)]
        [InlineData(0, 1)]
        [InlineData(1, 1)]
        [InlineData(-1, 0)]
        [InlineData(0, -1)]
        [InlineData(-1, -1)]
        public void Equals_OfTwoDiffrentVectors_willReturnFalse(int x, int y)
        {
            //Arrange
            var vectorBase = new Vector2D();
            var DiffrentVector = new Vector2D(x, y);

            //Act
            var Result = vectorBase.Equals(DiffrentVector);

            //Assert
            Assert.False(Result);
        }

        public static IEnumerable<object[]> TwolinearlyIndependenVectorsEnumerable()
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
        public static IEnumerable<object[]> VectorAdditionWithResultEnumerable()
        {
            yield return new object[] { new Vector2D(0, 0), new Vector2D(0, 0), new Vector2D(0, 0) };
            yield return new object[] { new Vector2D(1, 1), new Vector2D(0, 0), new Vector2D(1, 1) };
            yield return new object[] { new Vector2D(0, 0), new Vector2D(1, 1), new Vector2D(1, 1) };
            yield return new object[] { new Vector2D(1, 1), new Vector2D(2, 2), new Vector2D(3, 3) };
            yield return new object[] { new Vector2D(1, 1), new Vector2D(-2, -2), new Vector2D(-1, -1) };
            yield return new object[] { new Vector2D(int.MaxValue, int.MaxValue), new Vector2D(int.MinValue, int.MinValue), new Vector2D(-1, -1) };
        }
        public static IEnumerable<object[]> VectorSubtractionResultEnumerable()
        {
            yield return new object[] { new Vector2D(0, 0), new Vector2D(0, 0), new Vector2D(0, 0) };
            yield return new object[] { new Vector2D(1, 1), new Vector2D(0, 0), new Vector2D(1, 1) };
            yield return new object[] { new Vector2D(0, 0), new Vector2D(1, 1), new Vector2D(-1, -1) };
            yield return new object[] { new Vector2D(2, 2), new Vector2D(1, 1), new Vector2D(1, 1) };
            yield return new object[] { new Vector2D(1, 1), new Vector2D(-2, -2), new Vector2D(3, 3) };
            yield return new object[] { new Vector2D(-2, -2), new Vector2D(1,1), new Vector2D(-3, -3) };
        }
        public static IEnumerable<object[]> VectorDotProductResultEnumerable()
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
        public static IEnumerable<object[]> VectorCrossProductResultEnumerable()
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
        public static IEnumerable<object[]> VectorWithLengthEnumerable()
        {
            yield return new object[] { new Vector2D(0, 0), 0 };
            yield return new object[] { new Vector2D(5, 0), 5 };
            yield return new object[] { new Vector2D(0, 5), 5 };
            yield return new object[] { new Vector2D(5, 10), 11.180 };
            yield return new object[] { new Vector2D(5, -10), 11.180 };
            yield return new object[] { new Vector2D(-5, 10), 11.180 };
            yield return new object[] { new Vector2D(-5, -10), 11.180 };

        }
        public static IEnumerable<object[]> VectorEnumerable()
        {
            yield return new object[] { new Vector2D(5, 0)};
            yield return new object[] { new Vector2D(0, 5)};
            yield return new object[] { new Vector2D(5, 10)};
            yield return new object[] { new Vector2D(5, -10)};
            yield return new object[] { new Vector2D(-5, 10)};
            yield return new object[] { new Vector2D(-5, -10)};

        }
        public static IEnumerable<object[]> VectorWithStringEnumerable()
        {
            yield return new object[] { new Vector2D(0, 0), "(X: 0, Y: 0)" };
            yield return new object[] { new Vector2D(1, 2), "(X: 1, Y: 2)" };
            yield return new object[] { new Vector2D(-1, -2), "(X: -1, Y: -2)" };

            if(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator == ",")
            {
                yield return new object[] { new Vector2D(1.2345, -1.2345), "(X: 1,235, Y: -1,235)" };
            }
            
            if (Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator == ".")
            {
                yield return new object[] { new Vector2D(1.2345, -1.2345), "(X: 1.235, Y: -1.235)" };
            }

        }

       


    }
}