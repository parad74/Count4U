using System;
using System.Collections.Generic;
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.SelectionParams;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Count4U.TestProject.Count4U.IIturRepository
{
    /// <summary>
    ///This is a test class for InventProductEFRepositoryTest and is intended
    ///to contain all InventProductEFRepositoryTest Unit Tests
    ///</summary>
    [TestClass()]
    public class IturEFRepositoryTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        /// Можем получить все записи.
        ///</summary>
        [TestMethod()]
        public void EF_CanGetIturs()
        {
			var target = new IturEFRepository(new ConnectionDB(null), null, null, null, null,null,null);
			var result = target.GetIturs("");
            Assert.IsTrue(result.Count > 0, "Количество записей должно быть больше нуля");
        }

        /// <summary>
        /// Можем получить страницу записей.
        ///</summary>
        [TestMethod()]
        public void EF_CanGetItursWithPaging()
        {
			var target = new IturEFRepository(new ConnectionDB(null), null, null, null, null,null);
            var selectParams = new SelectParams()
            {
                IsEnablePaging = true,
                CountOfRecordsOnPage = 5,
                CurrentPage = 2
            };
            var result = target.GetIturs(selectParams, "");
            Assert.IsTrue(result.Count == selectParams.CountOfRecordsOnPage, "Количество записей должно быть равно размеру запрашиваемой страницы");
            Assert.IsTrue(result.TotalCount > 0, "Общее количество записей должно быть больше нуля");
        }

        /// <summary>
        /// Можем получить записи с сортировкой по возрастанию.
        /// </summary>
        [TestMethod()]
        public void EF_CanGetItursWithSortAsc()
        {
			var target = new IturEFRepository(new ConnectionDB(null), null, null, null, null);
            var selectParams = new SelectParams()
            {
                IsEnablePaging = false,
                SortParams = "Name ASC"
            };
            var result = target.GetIturs(selectParams, "");
            Assert.IsTrue(result.Count > 0, "Количество записей должно быть больше нуля");
            Assert.IsTrue(result.TotalCount > 0, "Общее количество записей должно быть больше нуля");
        }

        /// <summary>
        /// Можем получить записи с сортировкой по убыванию.
        /// </summary>
        [TestMethod()]
        public void EF_CanGetItursWithSortDesc()
        {
			var target = new IturEFRepository(new ConnectionDB(null), null, null, null, null);
            var selectParams = new SelectParams()
            {
                IsEnablePaging = false,
                SortParams = "Name DESC"
            };
			var result = target.GetIturs(selectParams, "");
            Assert.IsTrue(result.Count > 0, "Количество записей должно быть больше нуля");
            Assert.IsTrue(result.TotalCount > 0, "Общее количество записей должно быть больше нуля");
        }

        /// <summary>
        /// Можем получить записи с фильтром.
        /// </summary>
        [TestMethod()]
        public void EF_CanGetItursWithFilter()
        {
			var target = new IturEFRepository(new ConnectionDB(null), null, null, null, null);
            var selectParams = new SelectParams()
            {
                IsEnablePaging = false,
            };
			selectParams.FilterParams.Add("IturCode", new FilterParam() { Operator = FilterOperator.Contains, Value = "Itur1" });
            selectParams.FilterParams.Add("Name", new FilterParam() { Operator = FilterOperator.Contains, Value = "Itur1" });
            selectParams.FilterListParams.Add("LocationID", new FilterListParam() { Values = new List<long>() { 5, 6 } });
            selectParams.FilterListParams.Add("StatusIturID", new FilterListParam() { Values = new List<long>() { 1, 2 } });
			var result = target.GetIturs(selectParams, "");
            Assert.IsTrue(result.Count > 0, "Количество записей должно быть больше нуля");
            Assert.IsTrue(result.TotalCount > 0, "Общее количество записей должно быть больше нуля");
        }

        /// <summary>
        /// Можем получить записи с фильтром.
        /// </summary>
        [TestMethod()]
        public void EF_CanGetItursWithPagingAndSortAndFilter()
        {
			var target = new IturEFRepository(new ConnectionDB(null), null, null, null, null);
            var selectParams = new SelectParams()
            {
                IsEnablePaging = true,
                CountOfRecordsOnPage = 2,
                CurrentPage = 1,
                SortParams = "Name DESC"
                //CountOfRecordsOnPage = 5,
                //CurrentPage = 2,
                //SortParams = "LocationID DESC"
            };
			selectParams.FilterParams.Add("IturCode", new FilterParam() { Operator = FilterOperator.Contains, Value = "Itur1" });
            selectParams.FilterParams.Add("Name", new FilterParam() { Operator = FilterOperator.Contains, Value = "Itur1" });
            selectParams.FilterListParams.Add("LocationID", new FilterListParam() { Values = new List<long>() { 5, 6 } });
            selectParams.FilterListParams.Add("StatusIturID", new FilterListParam() { Values = new List<long>() { 1, 2 } });
            var result = target.GetIturs(selectParams, "");
            Assert.IsTrue(result.Count == selectParams.CountOfRecordsOnPage, "Количество записей должно быть равно размеру запрашиваемой страницы");
            Assert.IsTrue(result.TotalCount > 0, "Общее количество записей должно быть больше нуля");
        }
    }
}
