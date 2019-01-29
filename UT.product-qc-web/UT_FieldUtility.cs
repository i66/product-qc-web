using product_qc_web.Lib;
using product_qc_web.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace UT.product_qc_web
{
    public class UT_FieldUtility
    {
        [Fact]
        public void Test_GetFieldNameShouldReturnNullifInputNullOrEmpty()
        {
            FieldUtility fakeFieldUtility = new FieldUtility();
            string result;

            List<Type> fakeNullObject = null;
            result = fakeFieldUtility.getFieldName(fakeNullObject);
            Assert.Null(result);

            List<Type> fakeEmptyObject = new List<Type>();
            result = fakeFieldUtility.getFieldName(fakeEmptyObject);
            Assert.Null(result);
        }

        [Fact]
        public void Test_GetFieldNameShoudlNotEmptyIfInputRight()
        {
            FieldUtility fakeFieldUtility = new FieldUtility();
            List<Type> fakeObject = new List<Type>() { typeof(MetadataTProduct), typeof(MetadataTDelivery)};
            string result;

            result = fakeFieldUtility.getFieldName(fakeObject);

            Assert.False(string.IsNullOrWhiteSpace(result));
        }
    }
}
