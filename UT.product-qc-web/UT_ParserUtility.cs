using product_qc_web.Lib;
using System.Collections.Generic;
using Xunit;

namespace UT.product_qc_web
{
    public class UT_ParserUtility
    {
        [Fact]
        public void Test_ParsingMachineNum()
        {
            ParserUtility tester = new ParserUtility();
            List<int> fakeInputStr = new List<int>() { 5, 4, 3, 2, 1 };
            string fakeResultCombineStr = string.Join(ParserUtility.SEPERATOR, fakeInputStr);
            List<int> fakeResult = tester.ParsingMachineNum(fakeResultCombineStr);
            Assert.Equal(fakeInputStr, fakeResult);
        }

        [Fact]
        public void Test_ParsingMachineNum_DummyInput()
        {
            ParserUtility tester = new ParserUtility();
            List<int> fakeResult = tester.ParsingMachineNum(string.Empty);
            Assert.NotNull(fakeResult);
            Assert.Empty(fakeResult);

            fakeResult = tester.ParsingMachineNum(null);
            Assert.NotNull(fakeResult);
            Assert.Empty(fakeResult);

            fakeResult = tester.ParsingMachineNum("               ");
            Assert.NotNull(fakeResult);
            Assert.Empty(fakeResult);
        }
        
        [Fact]
        public void Test_ParsingMachineNum_SpaceHandling()
        {
            string fakeStr = "1, 2, 3, , 4,         5";
            List<int> fakeStrResult = new List<int>() { 1, 2, 3, 4, 5 };
            ParserUtility tester = new ParserUtility();
            List<int> fakeResult = tester.ParsingMachineNum(fakeStr);
            Assert.Equal(fakeStrResult, fakeResult);
        }

        [Fact]
        public void Test_ParsingMachineNum_NonNumericHandling()
        {
            string fakeStr = "1, 2, holy shit, a, b, c, 3, ooo, qqq, XDD, 4, 5";
            List<int> fakeStrResult = new List<int>() { 1, 2, 3, 4, 5 };
            ParserUtility tester = new ParserUtility();
            List<int> fakeResult = tester.ParsingMachineNum(fakeStr);
            Assert.Equal(fakeStrResult, fakeResult);
        }

        [Fact]
        public void Test_ParsingMachineNum_DuplicateMustRemainOneOnly()
        {
            string fakeStr = "1, 2, 2, 3, 4, 4, 5, 1";
            List<int> fakeStrResult = new List<int>() { 1, 2, 3, 4, 5 };
            ParserUtility tester = new ParserUtility();
            List<int> fakeResult = tester.ParsingMachineNum(fakeStr);
            Assert.Equal(fakeStrResult, fakeResult);
        }

        [Fact]
        public void Test_ParsingMachineNum_MultiDigitSupport()
        {
            string fakeStr = "1234, 567, 20";
            List<int> fakeStrResult = new List<int>() { 1234, 567, 20 };
            ParserUtility tester = new ParserUtility();
            List<int> fakeResult = tester.ParsingMachineNum(fakeStr);
            Assert.Equal(fakeStrResult, fakeResult);
        }

        [Fact]
        public void Test_ParsingMachineNum_ChineseCharacterWouldNotCrash()
        {
            string fakeStr = "1, 2, 3, ·¨¹l¥¿¡A6, 4, 5";
            List<int> fakeStrResult = new List<int>() { 1, 2, 3, 4, 5 };
            ParserUtility tester = new ParserUtility();
            List<int> fakeResult = tester.ParsingMachineNum(fakeStr);
            Assert.Equal(fakeStrResult, fakeResult);
        }
    }
}
