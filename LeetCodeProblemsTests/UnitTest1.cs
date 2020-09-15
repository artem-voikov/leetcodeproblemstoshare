using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CW_A_Simplistic_TCP_Finite_State_Machine
{
    class CW_A_Simplistic_TCP_Finite_State_Machine
    {
        private Solution solution;

        [SetUp]
        public void Setup()
        {
            this.solution = new Solution();
        }

        [Test]
        public void Test1()
        {
            var result = solution.Receiving("APP_PASSIVE_OPEN", "APP_SEND", "RCV_SYN_ACK");

            Assert.AreEqual("ESTABLISHED", result);
        }

        [Test]
        public void Test2()
        {
            var result = solution.Receiving("APP_ACTIVE_OPEN");

            Assert.AreEqual("SYN_SENT", result);
        }

        [Test]
        public void Test3()
        {
            var result = solution.Receiving("APP_ACTIVE_OPEN", "RCV_SYN_ACK", "APP_CLOSE", "RCV_FIN_ACK", "RCV_ACK");

            Assert.AreEqual("ERROR", result);
        }
    }
}
