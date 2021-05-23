using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UsAcRe.Web.Server.Migrations {
	public partial class CreateDefaultRoles : Migration {
		protected override void Up(MigrationBuilder migrationBuilder) {
			migrationBuilder.InsertData(
				table: "AspNetRoles",
				columns: new string[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
				values: new object[] { Guid.NewGuid().ToString(), "Administrator", "Administrator", Guid.NewGuid().ToString() }
				);

			migrationBuilder.InsertData(
				table: "AspNetRoles",
				columns: new string[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
				values: new object[] { Guid.NewGuid().ToString(), "Viewer", "Viewer", Guid.NewGuid().ToString() }
				);

		}

		protected override void Down(MigrationBuilder migrationBuilder) {
			migrationBuilder.DeleteData(
				table: "AspNetRoles",
				keyColumn: "Name",
				keyValue: "Administrator"
				);

			migrationBuilder.DeleteData(
				table: "AspNetRoles",
				keyColumn: "Name",
				keyValue: "Viewer"
				);
		}
	}
}
