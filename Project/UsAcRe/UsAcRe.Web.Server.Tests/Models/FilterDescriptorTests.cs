using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using NUnit.Framework;
using Tests.Common;
using UsAcRe.Web.Server.Data;
using UsAcRe.Web.Server.Identity;
using UsAcRe.Web.Shared.Models;

namespace UsAcRe.Web.Server.Tests.ServicesTests {
	[TestFixture]
	public class FilterDescriptorTests {
		JsonDocument jsonDocument;

		[SetUp]
		public void SetUp() {
			jsonDocument = JsonDocument.Parse("{\"Name\": \"John\",\"Grade\": 94.3}");
		}

		[TearDown]
		public void TearDown() {

		}

		[Test]
		public void Predicates_Equals_Test() {
			var predicate = FilterOperatorSpecifics.Predicates[FilterOperator.Equals];
			Assert.That(predicate(null, null), Is.True);
			Assert.That(predicate(null, 0), Is.False);
			Assert.That(predicate(0, null), Is.False);
			Assert.That(predicate(0, 0), Is.True);
			Assert.That(predicate(-1, -1), Is.True);
			Assert.That(predicate((byte)0, 0), Is.True);
			Assert.That(predicate(0, (byte)0), Is.True);
			Assert.That(predicate((float)100.0, (float)100.0), Is.True);
			Assert.That(predicate((float)-100.0, (float)-100.0), Is.True);
			Assert.That(predicate((double)100.0, (double)100.0), Is.True);
			Assert.That(predicate((double)-100.0, (double)-100.0), Is.True);
			Assert.That(predicate((decimal)100.0, (decimal)100.0), Is.True);
			Assert.That(predicate((decimal)-100.0, (decimal)-100.0), Is.True);
			Assert.That(predicate((decimal)100.0, (double)100.0), Is.True);

			Assert.That(predicate((decimal)100.0, (int)100), Is.True);

			Assert.That(predicate("A", "A"), Is.True);
			Assert.That(predicate("A", "a"), Is.False);

			Assert.That(predicate("A", 1), Is.False);
			Assert.That(predicate(1, "A"), Is.False);

			Assert.That(predicate(0, jsonDocument.RootElement.GetProperty("Grade")), Is.False);
			Assert.That(predicate(94.3, jsonDocument.RootElement.GetProperty("Grade")), Is.True);

			Assert.That(predicate(jsonDocument.RootElement.GetProperty("Grade"), 0), Is.False);
			Assert.That(predicate(jsonDocument.RootElement.GetProperty("Grade"), 94.3), Is.True);

			Assert.That(predicate("test", jsonDocument.RootElement.GetProperty("Grade")), Is.False);
			Assert.That(predicate("94.3", jsonDocument.RootElement.GetProperty("Grade")), Is.True);
			Assert.That(predicate(jsonDocument.RootElement.GetProperty("Grade"), "94.3"), Is.True);

			Assert.That(predicate("test", jsonDocument.RootElement.GetProperty("Name")), Is.False);
			Assert.That(predicate("John", jsonDocument.RootElement.GetProperty("Name")), Is.True);

			Assert.That(predicate(1, Guid.NewGuid()), Is.False);
			Assert.That(predicate(Guid.Empty, Guid.Empty), Is.True);

			Assert.That(predicate(new { Id = 15, Name = "Joe" }, new { Id = 15, Name = "Joe" }), Is.False);
			Assert.That(predicate(Tuple.Create(15, "Joe"), Tuple.Create(15, "Joe")), Is.True);
			Assert.That(predicate(Tuple.Create(15, "Joe"), Tuple.Create(42, "Joe")), Is.False);

			Assert.That(predicate(new SystemException(), new SystemException()), Is.False);
			var ex = new SystemException();
			Assert.That(predicate(ex, ex), Is.True);
		}

		[Test]
		public void Predicates_NotEquals_Test() {
			var predicate = FilterOperatorSpecifics.Predicates[FilterOperator.NotEquals];
			Assert.That(predicate(null, null), Is.False);
			Assert.That(predicate(null, 0), Is.True);
			Assert.That(predicate(0, null), Is.True);
			Assert.That(predicate(0, 0), Is.False);
			Assert.That(predicate(-1, -1), Is.False);
			Assert.That(predicate((byte)0, 1), Is.True);
			Assert.That(predicate(0, (byte)1), Is.True);
			Assert.That(predicate((float)100.0, (float)100.1), Is.True);
			Assert.That(predicate((float)-100.1, (float)-100.0), Is.True);
			Assert.That(predicate((double)100.0, (double)100.1), Is.True);
			Assert.That(predicate((double)-100.1, (double)-100.0), Is.True);
			Assert.That(predicate((decimal)100.0, (decimal)100.1), Is.True);
			Assert.That(predicate((decimal)-100.1, (decimal)-100.0), Is.True);
			Assert.That(predicate((decimal)100.0, (double)100.1), Is.True);

			Assert.That(predicate((decimal)100.0, (int)100), Is.False);

			Assert.That(predicate("A", "A"), Is.False);
			Assert.That(predicate("A", "a"), Is.True);

			Assert.That(predicate("A", 1), Is.True);
			Assert.That(predicate(1, "A"), Is.True);

			Assert.That(predicate(0, jsonDocument.RootElement.GetProperty("Grade")), Is.True);
			Assert.That(predicate(94.3, jsonDocument.RootElement.GetProperty("Grade")), Is.False);

			Assert.That(predicate(jsonDocument.RootElement.GetProperty("Grade"), 0), Is.True);
			Assert.That(predicate(jsonDocument.RootElement.GetProperty("Grade"), 94.3), Is.False);

			Assert.That(predicate("test", jsonDocument.RootElement.GetProperty("Grade")), Is.True);
			Assert.That(predicate("94.3", jsonDocument.RootElement.GetProperty("Grade")), Is.False);
			Assert.That(predicate(jsonDocument.RootElement.GetProperty("Grade"), "94.3"), Is.False);

			Assert.That(predicate("test", jsonDocument.RootElement.GetProperty("Name")), Is.True);
			Assert.That(predicate("John", jsonDocument.RootElement.GetProperty("Name")), Is.False);

			Assert.That(predicate(1, Guid.NewGuid()), Is.True);
			Assert.That(predicate(Guid.Empty, Guid.Empty), Is.False);

			Assert.That(predicate(new { Id = 15, Name = "Joe" }, new { Id = 15, Name = "Joe" }), Is.True);
			Assert.That(predicate(Tuple.Create(15, "Joe"), Tuple.Create(15, "Joe")), Is.False);
			Assert.That(predicate(Tuple.Create(15, "Joe"), Tuple.Create(42, "Joe")), Is.True);

			Assert.That(predicate(new SystemException(), new SystemException()), Is.True);
			var ex = new SystemException();
			Assert.That(predicate(ex, ex), Is.False);
		}
	}
}
