namespace CRUD_Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            //Arrange
            MyMath myMath = new MyMath();
            int a = 2, b = 3;
            int expected = 5;
            //Act
            int real = myMath.Add(a, b);

            //Assert
            Assert.Equal(expected, real);
        }
    }
}